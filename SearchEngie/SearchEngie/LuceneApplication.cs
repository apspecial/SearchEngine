using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis; // for Analyser
using Lucene.Net.Analysis.Standard; // for standard analyser
using Lucene.Net.Documents; // for Socument
using Lucene.Net.Index; //for Index Writer
using Lucene.Net.Store; //for Directory
using Lucene.Net.QueryParsers; // for query parser
using Lucene.Net.Search;
using System.IO; //for File
using System.Text.RegularExpressions;


namespace SearchEngie
{
	class LuceneApplication
	{
		Lucene.Net.Store.Directory luceneIndexDirectory;
		Lucene.Net.Analysis.Analyzer analyzer;
		Lucene.Net.Index.IndexWriter writer;
		IndexSearcher searcher;
		QueryParser parser;
		// public static Lucene.Net.Util.Version VERSION = Lucene.Net.Util.Version.LUCENE_30;
		const Lucene.Net.Util.Version VERSION = Lucene.Net.Util.Version.LUCENE_30;
		const string DOCID_FN = "DocID";
		const string TITLE_FN = "Title";
		const string AUTHOR_FN = "Author";
		const string BIB_FN = "Bibliography";
		const string WORDS_FN = "Abstract";
		//string docID;   //file information segment
		//string author;
		//string title;
		//string bibliography;
		//string words;
		string currentFilename;
		//setting the  directory of the source files
		string filesPath;
        //setting the query text 
        string queryText;
		//Dictionary<int,string> queryDic;
        //
        string submittedQuery { get; set; }
        //
        public char[] splitters = new char[] { ' ', '\t', '\'', '"', '-', '(', ')', ',', '’', '\n', ':', ';', '?', '.', '!' };
        public string[] stopWords = { "a", "an", "and", "are", "as", "at", "be", "but", "by", "for", "if", "in", "into", "is", "it", "no", "not", "of", "on", "or", "such", "that", "the", "their", "then", "there", "these", "they", "this", "to", "was", "will", "with" }; // for challange activity

		//
		public string SubmittedQuery { get; set; }
		public string QueryText{ get; set; }

        public LuceneApplication()
		{
			luceneIndexDirectory = null; // Is set in Create Index
			analyzer = null;  // Is set in CreateAnalyser
			writer = null; // Is set in CreateWriter
			//docID = "";
			//author = "";
			//title = "";
			//bibliography = "";
			//words = "";
			filesPath = "";

		}

		public LuceneApplication(string pathF, string pathI)
		{
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(pathI); // Is set in Create Index
			analyzer = null;  // Is set in CreateAnalyser
			writer = null; // Is set in CreateWriter
			//docID = "";
			//author = "";
			//title = "";
			//bibliography = "";
			//words = "";
			filesPath = pathF;

		}

		public LuceneApplication(string pathF, string pathI,string askQuery)
		{
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(pathI); // Is set in Create Index
			analyzer = null;  // Is set in CreateAnalyser
			writer = null; // Is set in CreateWriter
			//docID = "";
			//author = "";
			//title = "";
			//bibliography = "";
			//words = "";
			filesPath = pathF;
			queryText = askQuery;   //the information need of the user
		}

  


    

        /// Directory to store the index
        public void OpenIndex(string indexPath)
		{
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);

		}



		/// Creates the analyser
		public void CreateAnalyser()
		{
			analyzer = new Lucene.Net.Analysis.SimpleAnalyzer();
		}


		/// Creates the index writer
		public void CreateWriter()
		{

			IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
			writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
		}

		/// Creates the index writer
		public void CreateWriter(string indexPath)
		{

			IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);
			writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
		}

	
		public void IndexBook(Book book)
		{
			
			Lucene.Net.Documents.Field docField = new Field(DOCID_FN, book.DocId, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Field titleField = new Field(TITLE_FN, book.Title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Field authorField = new Field(AUTHOR_FN, book.Author, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Field bibField = new Field(BIB_FN, book.Bibliography, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Field wordsField = new Field(WORDS_FN, book.Words, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);

			Lucene.Net.Documents.Document doc = new Document();
			doc.Add(docField);
			//setting boost
			titleField.Boost = 2;
			doc.Add(titleField);
			doc.Add(authorField);
			doc.Add(bibField);
			doc.Add(wordsField);
			writer.AddDocument(doc);
		}


		public Book ReadOneFile(string filePath)
		{
			string delims = @".[ITABW]";
			Book outbook = new Book();

			try
			{
				// Create an instance of StreamReader to read from a file.
				// The using statement also closes the StreamReader.
				using (StreamReader sr = new StreamReader(filePath))
				{
					string line;
					while ((line = sr.ReadToEnd()) != null)
					{
						string[] sections = Regex.Split(line, delims);
						outbook.DocId = sections[1].Replace("\"", " ").Replace("\n", " ");
						outbook.Title = sections[2].Replace("\"", " ").Replace("\n", " ");
						outbook.Author = sections[3].Replace("\"", " ").Replace("\n", " ");
						outbook.Bibliography = sections[4].Replace("\"", " ").Replace("\n", " ");
						//delete first sentence.
						outbook.Words = sections[5].Remove(0, sections[2].Length).Replace("\"", " ").Replace("\n", " ");
					}
					
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("The file could not be read:");
				Console.WriteLine(e.Message);
			}
			return outbook;
		}

		public void CreateIndex()
		{
			//read files from directory
			DirectoryInfo di = new DirectoryInfo(filesPath);
			FileInfo[] listfile = di.GetFiles();
			int numberofFiles = listfile.Count();
			string fileName;
			//
			foreach (FileInfo name in listfile)
			{

				//fileName = filesPath + "\\" + name;
				Book genBook = ReadOneFile(name.ToString());
				IndexBook(genBook);

			}

		}


		public void ReadFile(string FilePath)
		{
			//char[] delims = { '.I', '.T', '.A', '.B', '.W' };
			//string delims = @"\s\.[ITABW]";
			string delims = @"\.[ITABW]";
            Lucene.Net.Documents.Field docField;
            Lucene.Net.Documents.Field titleField;
            Lucene.Net.Documents.Field authorField;
            Lucene.Net.Documents.Field bibField;
            Lucene.Net.Documents.Field wordsField;
            
            //
            DirectoryInfo di = new DirectoryInfo(FilePath);
			FileInfo[] listfile = di.GetFiles();
			int numberofFiles = listfile.Count();

            //
			foreach (FileInfo name in listfile)
			{
                currentFilename = FilePath + "\\" + name;
                //currentFilename = FilePath + "\\" + listfile[0];
                try
				{
					// Create an instance of StreamReader to read from a file.
					// The using statement also closes the StreamReader.
					using (StreamReader sr = new StreamReader(currentFilename))
					{
						string line;
						// Read and display lines from the file until the end of 
						// the file is reached.

						//define a array
						//int length = 0;


						// string[] sectionsa = Regex.Split(line, delims);
						//  int a = 1;
						while ((line = sr.ReadToEnd()) != null)
						{
							//creat

							//string[] sections = line.Split(delims);
							string[] sections = Regex.Split(line, delims);
							Regex regex_id = new Regex(@"\d+");
							Match match = regex_id.Match(sections[0]);
							string docID = match.Value;
							string title = sections[1];
							string author = sections[2];
							string bibliography = sections[3];
                            //delete the first line in abrstact
							string words = sections[4].Remove(0, sections[1].Length);

						    docField = new Field(DOCID_FN, docID, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							titleField = new Field(TITLE_FN, title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							authorField = new Field(AUTHOR_FN, author, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							bibField = new Field(BIB_FN, bibliography, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							wordsField = new Field(WORDS_FN, words, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							//            Lucene.Net.Documents.Field publisherField = new Field(PUBLISHER_FN, publisher, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							//		Lucene.Net.Documents.Field publisherField = new Field(PUBLISHER_FN, publisher, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);

							
                            //setting boost
                            titleField.Boost = 2.0f;  
							Lucene.Net.Documents.Document doc = new Document();
                            doc.Add(docField);
							doc.Add(titleField);
							doc.Add(authorField);
							doc.Add(bibField);
							doc.Add(wordsField);

							writer.AddDocument(doc);



							//string delims = @"\s\.[ITABW]\n";
							//char[] delimsa = { '\n' };
							//string[] sectionsa = Regex.Split(line, delims);
							//Regex regex = new Regex(@"\d+");
							//Match match = regex.Match(sectionsa[0]);
							//string ax = match.Value;
							//string[] b = sectionsa[1].Split(delimsa);
							//int a = 1;
							//a = a++;

							//----
							//String[] substrings = line.Split(' ');
							////Console.WriteLine(substrings.GetLength(0));
							//length += substrings.GetLength(0);
							//Console.WriteLine(line.ToUpper());
							//// Console.WriteLine(length);
							////foreach (String substring in substrings)
							//// Console.WriteLine("{0}", substring.ToUpper());
							////nsole.WriteLine("{0} \n", substring.ToUpper());
						}
						//Console.WriteLine("\nThe number of words is {0}", length);
					}

				}

				catch (Exception e)
				{
					// Let the user know what went wrong.
					Console.WriteLine("The file could not be read:");
					Console.WriteLine(e.Message);
				}
			}


            

			//// ACTIVITY 7 - FILL IN CODE HERE
			//char[] delims = { ' ', '\n' };
			//System.IO.StreamReader reader = new System.IO.StreamReader(name);
			//string line = "";
			//int numWords = 0;
			//while ((line = reader.ReadLine()) != null)
			//{
			//	numWords += line.Split(delims, StringSplitOptions.RemoveEmptyEntries).Count();
			//}
			//reader.Close();

			//Console.WriteLine("Fileame " + name + " Word Count " + numWords);
		}


	

		/// Flushes buffers and closes the index
		public void CleanUpIndexer()
		{
			writer.Optimize();
			writer.Flush(true, true, true);
			writer.Dispose();
		}

		public void CleanUpSearch()
		{
			searcher.Dispose();
		}

		public void GenIndex(string setPath)
		{
			CreateAnalyser();
			CreateWriter(setPath);
			ReadFile(setPath);
			////CreateIndex(Indpath);
			//ReadFile(string filepath);
			//CleanUpIndexer;

		}

		public string GenIndex()
		{
			DateTime start = System.DateTime.Now;
			CreateAnalyser();
			CreateWriter();
			CreateIndex();
			//ReadFile(filesPath);
            CleanUpIndexer();
            DateTime indexEnd = DateTime.Now;
            return "Create Index Successfully! \n"+"The Total Time to Index:" + (indexEnd - start);
		}


		//setup lucene searcher 
		public void SetupSearch()
		{
			searcher = new IndexSearcher(luceneIndexDirectory);

            //         parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, TITLE_FN, analyzer);
            //            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, AUTHOR_FN, analyzer); 
            //            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, PUBLISHER_FN, analyzer);  

            //setting boost of the fields
            //HashMap<String, Float> boosts = new HashMap<String, Float>();
            Dictionary<string, float> boosts = new Dictionary<string, float>();
            boosts.Add(TITLE_FN, 1);
            boosts.Add(WORDS_FN, 5);
            parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { TITLE_FN,WORDS_FN  }, analyzer, boosts);
            parser.DefaultOperator = QueryParser.OR_OPERATOR;
		}

		public List<string> SearchAndSaveResults(string querytext)
		{
           // List<QueryParser> parserlist = new List<QueryParser>;

            //create the correct parser
            //foreach (KeyValuePair<int, string> pair in queryDicIn)
            //{
            //    switch (pair.Key )
            //    {
            //        case 0:
            //            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, TITLE_FN, analyzer);
            //            break;
            //        case 1:
            //            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, AUTHOR_FN, analyzer);
            //            break;
            //        case 2:
            //            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, BIB_FN, analyzer);
            //            break;
            //        case 3:
            //            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, WORDS_FN, analyzer);
            //            break;
            //        case 4:
            //            parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { AUTHOR_FN, TITLE_FN, BIB_FN , WORDS_FN }, analyzer);
            //            break;
            //    }
                
            //}




            queryText = querytext.ToLower();
            Query query = parser.Parse(queryText);
            submittedQuery = query.ToString();
            //searching top 100 results
            TopDocs results = searcher.Search(query, 100);
            string outputResult = "Found" + results.TotalHits + " documents.\n\n";
			//System.Console.WriteLine("Found " + results.TotalHits + " documents.");
			List<string> resultofSearch = new List<string>();

			int rank = 0;

			foreach (ScoreDoc scoreDoc in results.ScoreDocs)
			{
				rank++;
				Lucene.Net.Documents.Document doc = searcher.Doc(scoreDoc.Doc);

				//output the score of doc
				float outScore = scoreDoc.Score;

				// output the information for each result
				string titleValue = doc.Get(TITLE_FN).ToString();
				string authorValue = doc.Get(AUTHOR_FN).ToString();
				string bibValue = doc.Get(BIB_FN).ToString();
				string wordsValue = doc.Get(WORDS_FN).ToString();
				// activity 5
				//                string publisherValue = doc.Get(PUBLISHER_FN).ToString(); // activity 5, 7
				//                Console.WriteLine("Rank " + rank + " title " + titleValue);
				//                Console.WriteLine("Rank " + rank + " title " + titleValue + " author " + authorValue + " Publisher " + publisherValue); // Activity 5
				//Console.WriteLine("Rank " + rank + " title " + titleValue + " author " + authorValue); // Activity 7

				//save the result in the list
				 outputResult += "Rank: " + rank + " Title: " + titleValue + "\n Author: " + authorValue + "\n Bibliography:" + bibValue + "\n Abrstract:" + wordsValue +"\n Score:"+outScore;
				resultofSearch.Add(outputResult);
			}

			return resultofSearch;
		}

		//get first sentence
		public string GetFirstSentence(string inText)
		{
			string[] segs = inText.Split('.');
			return segs[0];
		}

		//
		public DataTable SearchText(string queryText, bool isprecess)
		{
			DataTable table = new DataTable();
			table.Columns.Add("Rank", typeof(int));
			table.Columns.Add("Title", typeof(string));
			table.Columns.Add("Author", typeof(string));
			table.Columns.Add("Bibliography", typeof(string));
			table.Columns.Add("Abstract", typeof(string));
			table.Columns.Add("Abstract_all", typeof(string));

			queryText = queryText.ToLower();
			TopDocs results;
			if (isprecess)
			{
				BooleanQuery finalQ = GenFianlQuery(queryText);
				submittedQuery = finalQ.ToString();
				results = searcher.Search(finalQ, 100);
			}
			else
			{
				results = searcher.Search(parser.Parse(queryText), 100);
			}

			int rank = 0;
			if (results.TotalHits != 0)
			{
				foreach (ScoreDoc scoreDoc in results.ScoreDocs)
				{
					rank++;
					Lucene.Net.Documents.Document doc = searcher.Doc(scoreDoc.Doc);

					////output the score of doc
					//outScore = scoreDoc.Score;

					//// output the information for each result
					//titleValue = doc.Get(TITLE_FN).ToString();
					//authorValue = doc.Get(AUTHOR_FN).ToString();
					//bibValue = doc.Get(BIB_FN).ToString();
					//wordsValue = doc.Get(WORDS_FN).ToString();
					//docid = doc.Get(DOCID_FN).ToString();
					//linetoWrite = pair.Key + "\t" + "Q0" + "\t" + docid + "\t" + rank.ToString() + "\t" + outScore.ToString() + "\t" + "n9391576_dreamers";

					//writetext.WriteLine(linetoWrite);




					// Here we add five DataRows.
					table.Rows.Add(rank, doc.Get(TITLE_FN).ToString(), doc.Get(AUTHOR_FN).ToString(), doc.Get(BIB_FN).ToString(),GetFirstSentence(doc.Get(WORDS_FN).ToString()),doc.Get(WORDS_FN).ToString());
				

				}

			}
			return table;
		}

		//inputQes refers to the information need in txt file, filepath is the path which the result is saved
		public void SearchAndSaveResults(Dictionary<string,string> inputQes,string filepath)
        {
            BooleanQuery finalQuery;
            string titleValue;
            string authorValue;
            string bibValue;
            string wordsValue;
            string docid;
            string linetoWrite;
            float outScore;
            List<string> resultofSearch = new List<string>();
            TopDocs results;
            int rank;
			using (StreamWriter writetext = new StreamWriter(filepath))
            {
				foreach (var pair in inputQes)
				{
					finalQuery = GenFianlQuery(pair.Value);
					//searching top 100 results
					//results = searcher.Search(finalQuery, 100);
					results = searcher.Search(finalQuery, 100);
					//Query query = parser.Parse("what \"similarity laws\" must be obeyed when constructing aeroelastic models\nof heated high speed aircraft");
					//results = searcher.Search(query, 100);
					rank = 0;
					if (results.TotalHits != 0)
					{
						foreach (ScoreDoc scoreDoc in results.ScoreDocs)
						{
							rank++;
							Lucene.Net.Documents.Document doc = searcher.Doc(scoreDoc.Doc);

							//output the score of doc
							outScore = scoreDoc.Score;

							// output the information for each result
							titleValue = doc.Get(TITLE_FN).ToString();
							authorValue = doc.Get(AUTHOR_FN).ToString();
							bibValue = doc.Get(BIB_FN).ToString();
							wordsValue = doc.Get(WORDS_FN).ToString();
							docid = doc.Get(DOCID_FN).ToString();
							linetoWrite = pair.Key + "\t" + "Q0" + "\t" + docid + "\t" + rank.ToString() + "\t" + outScore.ToString() + "\t" + "n9391576_dreamers";

							writetext.WriteLine(linetoWrite);
						}
						//save the result in the list
						// outputResult += "Rank: " + rank + " Title: " + titleValue + "\n Author: " + authorValue + "\n Bibliography:" + bibValue + "\n Abrstract:" + wordsValue + "\n Score:" + outScore;
						//resultofSearch.Add(outputResult);
					}
				}
            }
        }


       


        public string[] TokeniseString(string text)
        { 
           return text.ToLower().Split(splitters, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] StopWordFilter(string[] tokens)
        {

            int numTokens = tokens.Count();
            List<string> filteredTokens = new List<string>();
            for (int i = 0; i < numTokens; i++)
            {
                string token = tokens[i];
                if (!stopWords.Contains(token) && (token.Length > 2)) filteredTokens.Add(token);
            }
            return filteredTokens.ToArray<string>();
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


        //transmit the string to the final query
        public BooleanQuery GenFianlQuery(string inputText)
        {
            string delims = "\"(.*)\"";
			//inputText = "I love you haha what is that \"file ahde\" what kind";
			//inputText = "I love you haha what is that ";
            //inputText = "\"similarity laws\" must be obeyed when constructing aeroelastic models of heated high speed aircraft";
            //get the matched phrase
            string getPhrase = Regex.Match(inputText, delims).Value;
            //the remaining words excluding the phase
            string remainWords = inputText.Remove(Regex.Match(inputText, delims).Index, Regex.Match(inputText, delims).Value.Length);

			//tokenlize the remaining words
			//string[] filterTokens = StopWordFilter(TokeniseString(remainWords));
			//delete stopwords from remaining words
			//string filterWords = StopWordFilter(remainWords);
			string filterWords = StopWordFilter(inputText.Replace("\"", " ").Replace("\n"," "));

            BooleanQuery finalQuery = new BooleanQuery();

			if (getPhrase != "")
			{
				//drag out the content of the phrase without qoatation
				string remPhrase = getPhrase.Remove(getPhrase.Length - 1, 1).Remove(0, 1);
				PhraseQuery phraseQuery = new PhraseQuery();
				phraseQuery.Add(new Term(remPhrase));
				//setting boost
				phraseQuery.Boost = 10;
				//finalQuery.Add(phraseQuery, Occur.MUST);
				finalQuery.Add(phraseQuery, Occur.SHOULD);
			}
			//if query need tokens, should change the method
			//finalQuery.Add(parser.Parse(remainWords), Occur.SHOULD);
			finalQuery.Add(parser.Parse(filterWords), Occur.SHOULD);
            string result = finalQuery.ToString();
            return finalQuery;
            
        }

        public Dictionary<string, string> ReadInfoNeed()
        {

			//System.IO.Directory filePath = System.IO.Directory.GetParent(filesPath);
			//var gparent = System.IO.Directory.GetParent(filesPath).ToString();
			//string filepath = gparent + "\\" + "cran_information_needs.txt";

			//string filepath = "H:\\IFN647\\Assignment\\collection(2)\\" + "cran_information_needs.txt";
			string filepath = "cran_information_needs.txt";
			//string delims = @"\s\.[ID]";
			string delims = ".[ID]";
            //char[] splitters = new char[] { ' ', '\t', '\'', '"', '-', '(', ')', ',', '’', '\n', ':', ';', '?', '.', '!' };
            Dictionary<string, string> outInfoNeed = new Dictionary<string, string>();
            //string topicID;
            //string topicCon;
            //int startP;
            //string topicContend;
            
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(filepath))
                {
                    string line;
                    if ((line = sr.ReadToEnd()) != null)
                    {
                        DateTime startTime = DateTime.Now;
                        //using Regex to split
                        string[] sections = Regex.Split(line, delims);
                        //another method to split
                        //string[] sections = line.Split('.');
                        //startP = line.IndexOf(".I");
                        //topicID = line. 
                        //line.
                        //int halfsize = sections.Length / 2;
                        for (int i = 1; i < sections.Length; i=i+2)
                        {
                            //if (i % 2 == 0)
                            //{

                            //    topicID = sections[i].Substring(sections[i].Length - 3, 3);
                          
                            //}
                            //else
                            //{
                            //    topicCon = sections[i];
                            //}
                            //adding to dictionary
                            //outInfoNeed.Add(sections[i].Substring(sections[i].Length - 3, 3), sections[i+1]);
							outInfoNeed.Add(sections[i].Replace("\n", " "), sections[i + 1].Replace("\n", " "));
                        }


                        DateTime endTime = DateTime.Now;
                        //
                        TimeSpan totaltime = endTime-startTime;
                        string showtime = totaltime.ToString();
                        
                        //int  topicID = match.Value;
                       // title = sections[1];
                        //author = sections[2];
                        //bibliography = sections[3];
                        ////delete the first line in abrstact
                        //words = sections[4].Remove(0, sections[1].Length);

                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return outInfoNeed;
        }

		public DataTable GenSearch()
		{
            //test
            //GenFianlQuery(queryText);
            ///
            CreateAnalyser();
			SetupSearch();
			//GenFianlQuery(queryText);
			//return SearchAndSaveResults(queryText);
			return SearchText(queryText,true);
		}

		public void TestFun(string filepath)
        {
            //test
            //GenFianlQuery("test");
            ///
            CreateAnalyser();
            SetupSearch();

			SearchAndSaveResults(ReadInfoNeed(),filepath);
			CleanUpSearch();
        }
        /// <summary>
        /// The main program
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// 
        /*
        static void Main(string[] args)
        {

            System.Console.WriteLine("Hello Lucene.Net");

            LuceneApplication myLuceneApp = new LuceneApplication();

            string indexPath = @"c:\temp\Week4Index";

            // Activity 7
            myLuceneApp.OpenIndex(indexPath);

            // Activity 8
            myLuceneApp.CreateAnalyser();
            myLuceneApp.CreateWriter();

            // Acivity 9
            // do some indexing
            //            myLuceneApp.IndexText("The Daily Star");

            // Acivity 11
            List<string> l = new List<string>();
            l.Add("The Daily Star");
            l.Add("The Daily Planet");
            l.Add("Daily News");
            l.Add("News of the Day");
            l.Add("New New York News");

            foreach (string s in l)
            {
                myLuceneApp.IndexText(s);
            }


            // clean up
            myLuceneApp.CleanUp();


            System.Console.ReadLine();
        }
        */
    }
}