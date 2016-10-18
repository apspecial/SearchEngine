using System;
using System.Collections.Generic;
using System.Linq;
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
		const string WORDS_FN = "Words";
		string docID;   //file information segment
		string author;
		string title;
		string bibliography;
		string words;
		string currentFilename;
		//setting the  directory of the source files
		string filesPath;
		//setting the query text 
		string queryText;

		public LuceneApplication()
		{
			luceneIndexDirectory = null; // Is set in Create Index
			analyzer = null;  // Is set in CreateAnalyser
			writer = null; // Is set in CreateWriter
			docID = "";
			author = "";
			title = "";
			bibliography = "";
			words = "";
			filesPath = "";

		}

		public LuceneApplication(string pathF, string pathI)
		{
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(pathI); // Is set in Create Index
			analyzer = null;  // Is set in CreateAnalyser
			writer = null; // Is set in CreateWriter
			docID = "";
			author = "";
			title = "";
			bibliography = "";
			words = "";
			filesPath = pathF;

		}

		public LuceneApplication(string pathF, string pathI,string askQuery)
		{
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(pathI); // Is set in Create Index
			analyzer = null;  // Is set in CreateAnalyser
			writer = null; // Is set in CreateWriter
			docID = "";
			author = "";
			title = "";
			bibliography = "";
			words = "";
			filesPath = pathF;
			queryText = askQuery;   //the information need of the user
		}

        public string QueryText
        {
            get
            {
                return queryText;
            }
            set
            {
                queryText = value;
            }
        }

        //public string Name
        //{
        //	get
        //	{
        //		return name;
        //	}
        //	set
        //	{
        //		name = value;
        //	}
        //}

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
		public void CreateIndex(string indexPath)
		{

			IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
			luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);
			writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
		}

		public void CreateIndex()
		{

			IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
			//luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(filesPath);
			writer = new Lucene.Net.Index.IndexWriter(luceneIndexDirectory, analyzer, true, mfl);
		}
		// Activity 9
		/// <summary>
		/// Add the text to the index
		/// </summary>
		/// <param name="text">The text tio index</param>
		public void IndexText(string text)
		{

			// TODO: Enter code to index text
			System.Console.WriteLine("Indexing " + text);
			Lucene.Net.Documents.Field field = new Field("Text", text, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Document doc = new Document();
			doc.Add(field);
			writer.AddDocument(doc);
		}

		//public void indextexta(string files_directory)
		//{

		//	file dir = new file(files_directory);
		//	file[] files = dir.listfiles();
		//	foreach (file file in files)
		//	{
		//		document document = new document();

		//		string path = file.getcanonicalpath();
		//		document.add(new field(field_path, path, field.store.yes, field.index.un_tokenized));
		//		reader sfa = new
		//		reader reader = new filereader(file);

		//		////read text from file.
		//		//try
		//		//{   // open the text file using a stream reader.
		//		//	using (streamreader sr = new streamreader("testfile.txt"))
		//		//	{
		//		//		// read the stream to a string, and write the string to the console.
		//		//		string line = sr.readtoend();
		//		//		console.writeline(line);
		//		//	}
		//		//}
		//		//catch (exception e)
		//		//{
		//		//	console.writeline("the file could not be read:");
		//		//	console.writeline(e.message);
		//		//}

		//		document.add(new field(field_contents, reader));

		//		indexwriter.adddocument(document);
		//	}
		//	indexwriter.optimize();
		//	indexwriter.close();
		//}

		public void ReadFile(string FilePath)
		{
			//char[] delims = { '.I', '.T', '.A', '.B', '.W' };
			string delims = @"\s\.[ITABW]";

			//
			DirectoryInfo di = new DirectoryInfo(FilePath);
			FileInfo[] listfile = di.GetFiles();
			int numberofFiles = listfile.Count();
			foreach (FileInfo name in listfile)
			{
				currentFilename = FilePath + "\\" + name;
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
							docID = match.Value;
							title = sections[1];
							author = sections[2];
							bibliography = sections[3];
							words = sections[4].Remove(0, sections[1].Length);

							Lucene.Net.Documents.Field docField = new Field(DOCID_FN, docID, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							Lucene.Net.Documents.Field titleField = new Field(TITLE_FN, title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							Lucene.Net.Documents.Field authorField = new Field(AUTHOR_FN, author, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							Lucene.Net.Documents.Field bibField = new Field(BIB_FN, bibliography, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							Lucene.Net.Documents.Field wordsField = new Field(WORDS_FN, words, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							//            Lucene.Net.Documents.Field publisherField = new Field(PUBLISHER_FN, publisher, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
							//		Lucene.Net.Documents.Field publisherField = new Field(PUBLISHER_FN, publisher, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);

							Lucene.Net.Documents.Document doc = new Document();
							//authorField.Boost = 2; // activity 9
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


		public void IndexBook(string docID, string author, string title, string bibliography, string words)
		{
			Lucene.Net.Documents.Field docIDField = new Field(DOCID_FN, docID, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Field authorField = new Field(AUTHOR_FN, author, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			//Lucene.Net.Documents.Field titleField = new Field(TITLE_FN, text, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			//            Lucene.Net.Documents.Field publisherField = new Field(PUBLISHER_FN, publisher, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			//		Lucene.Net.Documents.Field publisherField = new Field(PUBLISHER_FN, publisher, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Document doc = new Document();
			authorField.Boost = 2; // activity 9
			doc.Add(authorField);
			//	doc.Add(titleField);
			//	doc.Add(publisherField);
			writer.AddDocument(doc);
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
			CreateIndex(setPath);
			ReadFile(setPath);
			////CreateIndex(Indpath);
			//ReadFile(string filepath);
			//CleanUpIndexer;

		}

		public string GenIndex()
		{
			DateTime start = System.DateTime.Now;
			CreateAnalyser();
			CreateIndex();
			ReadFile(filesPath);
            CleanUpIndexer();
            DateTime indexEnd = DateTime.Now;
            return "Create Index Successfully! \n"+"The Total Time to Index:" + (indexEnd - start);
		}


		//setup lucene searcher 
		public void SetupSearch()
		{
			searcher = new IndexSearcher(luceneIndexDirectory);

			           parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, TITLE_FN, analyzer);
			//            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, AUTHOR_FN, analyzer); 
			//            parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, PUBLISHER_FN, analyzer);  

			//parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { AUTHOR_FN, TITLE_FN }, analyzer);
		}

		public List<string> SearchAndSaveResults(string querytext)
		{
			querytext = querytext.ToLower();
         
			Query query = parser.Parse(querytext);
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

		public List<string> GenSearch()
		{
            CreateAnalyser();
			SetupSearch();
			return SearchAndSaveResults(queryText);
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