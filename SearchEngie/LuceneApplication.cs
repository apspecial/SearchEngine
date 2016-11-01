using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Lucene.Net.Analysis;			 	// for Analyser
using Lucene.Net.Analysis.Standard; 	// for standard analyser
using Lucene.Net.Documents; 			// for Documents
using Lucene.Net.Index; 				// for Index Writer
using Lucene.Net.QueryParsers; 			// for query parser
using Lucene.Net.Search;
using Lucene.Net.Analysis.Snowball;
using System.IO; 						// for File
using System.Text.RegularExpressions;


namespace SearchEngie
{
	class LuceneApplication
	{
		Lucene.Net.Store.Directory luceneIndexDirectory;
		Analyzer analyzer;
		IndexWriter writer;
		IndexSearcher searcher;
		QueryParser parser;
		const Lucene.Net.Util.Version VERSION = Lucene.Net.Util.Version.LUCENE_30;
		const string DOCID_FN = "DocID";
		const string TITLE_FN = "Title";
		const string AUTHOR_FN = "Author";
		const string BIB_FN = "Bibliography";
		const string WORDS_FN = "Abstract";

        //setting the  directory of the source files
        string filesPath;
        //setting the query text entered by user
        string queryText;
        //setting the spliitters 
        public char[] splitters =  { ' ', '\t', '\'', '"', '-', '(', ')', ',', '’', '\n', ':', ';', '?', '.', '!' };
        //setting the stopwords
		public string[] stopWords = { "a", "an", "and", "are", "as", "at", "be", "but", "by", "for", "if", "in", "into", "is", "it", "no", "not", "of", "on", "or", "such", "that", "the", "their", "then", "there", "these", "they", "this", "to", "was", "will", "with" }; 

		//setting properties
		public string SubmittedQuery { get; set; }
		public string QueryText{ get; set; }
        public int HitDocNum { get; set; }

		//setting coustomized similarity
		Similarity newSimilarity;


		public LuceneApplication(string pathF, string pathI)
		{
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(pathI); // set lucene Index Directory
			filesPath = pathF;
			newSimilarity = new NewSimilarity();

		}

		public LuceneApplication(string pathF, string pathI,string askQuery)
		{
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(pathI); // set lucene Index Directory
			filesPath = pathF;
			queryText = askQuery;   //the information need of the user
			newSimilarity = new NewSimilarity();

		}


		/// Creates the analyser
		public void CreateAnalyser()
		{
			//analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(VERSION);
			//analyzer = new Lucene.Net.Analysis.StopAnalyzer(VERSION);
			analyzer = new SnowballAnalyzer(VERSION,"English");
		
        }


		/// Creates the index writer
		public void CreateWriter()
		{
			IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
			writer = new IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
			//using customized similarity
			writer.SetSimilarity(newSimilarity);
		}

		/// Flushes buffers and closes the index
		public void CleanUpIndexer()
		{
			writer.Optimize();
			writer.Flush(true, true, true);
			writer.Dispose();
		}

		/// closes the searcher
		public void CleanUpSearch()
		{
			searcher.Dispose();
		}


	
		public void IndexBook(Book book)
		{
			//define fields
			Field docField = new Field(DOCID_FN, book.DocId, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Field titleField = new Field(TITLE_FN, book.Title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Field authorField = new Field(AUTHOR_FN, book.Author, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Field bibField = new Field(BIB_FN, book.Bibliography, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Field wordsField = new Field(WORDS_FN, book.Words, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
            Document doc = new Document();
			//setting boost
			titleField.Boost = 2;
			//adding field to doc
			doc.Add(docField);
			doc.Add(titleField);
			doc.Add(authorField);
			doc.Add(bibField);
			doc.Add(wordsField);
			writer.AddDocument(doc);

		}


		/// Reads the content from one file, and split it to different fields.
		/// reture the structed data, as Book format.
        public Book ReadOneFile(string content)
        {
            string delims = @".[ITABW]";
            Book outbook = new Book();    
			//split the whole text in file
            string[] sections = Regex.Split(content, delims);
            outbook.DocId = sections[1].Replace("\"", " ").Replace("\n", " ");
            outbook.Title = sections[2].Replace("\"", " ").Replace("\n", " ");
            outbook.Author = sections[3].Replace("\"", " ").Replace("\n", " ");
            outbook.Bibliography = sections[4].Replace("\"", " ").Replace("\n", " ");
            //delete first sentence.
            outbook.Words = sections[5].Remove(0, sections[2].Length).Replace("\"", " ").Replace("\n", " ");
            return outbook;
        }


		/// Creates the index.
        public void CreateIndex()
		{
            //Using multi-thread,  faster than one thread
            Parallel.ForEach(Directory.EnumerateFiles(filesPath), file =>       //  string file in System.IO.Directory.EnumerateFiles(filesPath, "*.txt"))
            {
                Book genBook = ReadOneFile(File.ReadAllText(file));
                IndexBook(genBook);
            });


        }


		///setup lucene searcher 
		public void SetupSearch()
		{
			searcher = new IndexSearcher(luceneIndexDirectory);
			searcher.Similarity = newSimilarity;
            //setting boost of the fields
            Dictionary<string, float> boosts = new Dictionary<string, float>();
            boosts.Add(TITLE_FN, 1);
            boosts.Add(WORDS_FN, 5);
			//setting the analyzer for query parser
			Analyzer queryAnalyzer = new StandardAnalyzer(VERSION);
			//using snowball analyzer
			queryAnalyzer = analyzer;
			//create the query parser
            parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { TITLE_FN,WORDS_FN  }, queryAnalyzer, boosts);
            parser.DefaultOperator = QueryParser.OR_OPERATOR;


		}

		///get first sentence of a text
		public string GetFirstSentence(string inText)
		{
			string[] segs = inText.Split('.');
			return segs[0];
		}

		///searching the input text accoring to the created index
		public DataTable SearchText(string queryText, bool isprecess)
		{
			DataTable table = new DataTable();
			//create the data table columns
			table.Columns.Add("Rank", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("Author", typeof(string));
			table.Columns.Add("Bibliography", typeof(string));
			table.Columns.Add("Abstract", typeof(string));
			table.Columns.Add("Abstract_all", typeof(string));
            table.Columns.Add("DocID", typeof(string));
            table.Columns.Add("Score", typeof(string));

            queryText = queryText.ToLower();
			TopDocs results;
			if (isprecess)
			{
				BooleanQuery finalQ = GenFianlQuery(queryText);
				SubmittedQuery = finalQ.ToString();
				results = searcher.Search(finalQ, 1400);
			}
			else
			{	// "as it" function, just seperate the words and search
				QueryParser parser_simple = new MultiFieldQueryParser(VERSION, new[] { TITLE_FN, WORDS_FN }, new WhitespaceAnalyzer());
				Query finalQa = parser_simple.Parse(queryText);
                SubmittedQuery = finalQa.ToString();
				results = searcher.Search(finalQa, 1400);
			}
			// the total hit doucuments 
            HitDocNum = results.TotalHits;
			int rank = 0;
			if (results.TotalHits != 0)
			{
				foreach (ScoreDoc scoreDoc in results.ScoreDocs)
				{
					rank++;
					Document doc = searcher.Doc(scoreDoc.Doc);
                    //Output everything into the table
                    table.Rows.Add(rank, doc.Get(TITLE_FN), doc.Get(AUTHOR_FN), doc.Get(BIB_FN),
                       GetFirstSentence(doc.Get(WORDS_FN)),doc.Get(WORDS_FN), doc.Get(DOCID_FN), (scoreDoc.Score).ToString("F7"));

                }

            }
			return table;
		}


  

        public string StopWordFilter(string inpText)
        {

            string[] tokens = inpText.Split(' ');
            int numTokens = tokens.Count();
            //List<string> filteredTokens = new List<string>();
            string filteredTokens = "";
            for (int i = 0; i < numTokens; i++)
            {
                string token = tokens[i];
                if (!stopWords.Contains(token) && (token.Length > 2))
                    filteredTokens+= token + " ";
            }
            return filteredTokens;
        }


		///transmit the string to the final query
		public BooleanQuery GenFianlQuery(string inputText)
		{
			string delims = "\"(.*)\"";
			//get the matched phrase
			string getPhrase = Regex.Match(inputText, delims).Value;
			//filter the words using stopword list
			string filterWords = StopWordFilter(inputText.Replace("\"", " ").Replace("\n", " "));
			//expanding the queries using thesaurus
			string expandwrods = GetWeightedExpandedQuery(CreateThesaurus(), filterWords);
			//mask the expanding function
			expandwrods = filterWords;
			BooleanQuery finalQuery = new BooleanQuery();

			if (getPhrase != "")
			{
				//drag out the content of the phrase 
				string remPhrase = getPhrase.Remove(getPhrase.Length - 1, 1).Remove(0, 1);
				PhraseQuery phraseQuery = new PhraseQuery();
				phraseQuery.Add(new Term(remPhrase));
				//setting boost
				phraseQuery.Boost = 10;
				//adding phraseQuery into boolean query
				finalQuery.Add(phraseQuery, Occur.SHOULD);
			}


			finalQuery.Add(parser.Parse(expandwrods), Occur.SHOULD);
			return finalQuery;

		}

 
		///create dictionary for expanding the query
        public Dictionary<string, string[]> CreateThesaurus()
        {
            Dictionary<string, string[]> thesaurus = new Dictionary<string, string[]>();

            thesaurus.Add("similarity", new[] { "perceived" });
            thesaurus.Add("models", new[] { "framework", "entity" });
			thesaurus.Add("analysis", new[] { "investigation", "study", "techniques" });
			thesaurus.Add("behaviour", new[] { "action", "reaction", "demeanor"," conduct" });
			thesaurus.Add("experimental", new[] { "empirical"});
			thesaurus.Add("noise", new[] { "dissonance", "randomness" });
			thesaurus.Add("deflection", new[] { "diversion", "digression"});
            return thesaurus;
        }
		    

		/// Gets the weighted expanded query.
        public string GetWeightedExpandedQuery(Dictionary<string, string[]> thesausus, string intext)
        {
            string expandedQuery = "";
			string[] querys = intext.Split(' ');
			foreach (string text in querys)
			{
				if (thesausus.ContainsKey(text))
				{
					bool first = true;
					string[] array = thesausus[text];
					foreach (string a in array)
					{
						expandedQuery += " " + a;
						if (first)
						{
							expandedQuery += "^5";    //the first query has moer weight
							first = false;
						}
					}
				}
				else
				{
					expandedQuery += " " +text;
				}
			}
            return expandedQuery;
        }



		/// <summary>
		/// Excuting the operation of generating index from the files in a specific directory
		/// </summary>
		/// <returns>Text of time consuming</returns>
		public string GenIndex()
		{
			DateTime start = DateTime.Now;
			CreateAnalyser();
			CreateWriter();
			CreateIndex();
			CleanUpIndexer();
			DateTime indexEnd = DateTime.Now;
			return "Create Index Successfully! \n" + "The Total Time to Index:" + (indexEnd - start);
		}

		/// <summary>
		///  Excuting the operation of searching the input text and returns the result.
		/// </summary>
		/// <returns>The table contains the information of all fields.</returns>
        public DataTable GenSearch(bool Process,out int hitNum)
		{
 
            CreateAnalyser();
			SetupSearch();
            DataTable outputTable = SearchText(queryText, Process);
			//output how many docs are hitted
            hitNum = HitDocNum;
            CleanUpSearch();
            return outputTable;
		}

    }
}