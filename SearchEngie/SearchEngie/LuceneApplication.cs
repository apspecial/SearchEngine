using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis; // for Analyser
using Lucene.Net.Documents; // for Socument
using Lucene.Net.Index; //for Index Writer
using Lucene.Net.Store; //for Director
using System.IO; //for File

namespace SearchEngie
{
    class LuceneApplication
    {
        Lucene.Net.Store.Directory luceneIndexDirectory;
        Lucene.Net.Analysis.Analyzer analyzer;
        Lucene.Net.Index.IndexWriter writer;
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

        public LuceneApplication()
        {
            luceneIndexDirectory = null; // Is set in Create Index
            analyzer = null;  // Is set in CreateAnalyser
            writer = null; // Is set in CreateWriter
			docID = "";
			author= "";
			title= "";
			bibliography= "";
			words= "";

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

		public void IndexText(string files_directory)
		{
			
			File dir = new File(files_directory);
			File[] files = dir.listFiles();
			for (File file : files)
			{
				Document document = new Document();

				String path = file.getCanonicalPath();
				document.add(new Field(FIELD_PATH, path, Field.Store.YES, Field.Index.UN_TOKENIZED));
				Reader sfa = new
				Reader reader = new FileReader(file);

				////read text from file.
				//try
				//{   // Open the text file using a stream reader.
				//	using (StreamReader sr = new StreamReader("TestFile.txt"))
				//	{
				//		// Read the stream to a string, and write the string to the console.
				//		String line = sr.ReadToEnd();
				//		Console.WriteLine(line);
				//	}
				//}
				//catch (Exception e)
				//{
				//	Console.WriteLine("The file could not be read:");
				//	Console.WriteLine(e.Message);
				//}

				document.add(new Field(FIELD_CONTENTS, reader));

				indexWriter.addDocument(document);
			}
			indexWriter.optimize();
			indexWriter.close();
		}

		public void ReadFile(string FilePath)
		{
			char[] delims = { '.I', '.T', '.A', '.B', '.W' };
			try
			{
				// Create an instance of StreamReader to read from a file.
				// The using statement also closes the StreamReader.
				using (StreamReader sr = new StreamReader(FilePath))
				{
					string line;
					// Read and display lines from the file until the end of 
					// the file is reached.

					//define a array
					//int length = 0;

					while ((line = sr.ReadLine()) != null)
					{
						string[] sections = line.Split(delims);
						docID = sections[0];
						author = sections[1];
						title = sections[2];
						bibliography = sections[3];
						words = sections[4];
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


		public void IndexBook(string docID,string author, string title, string bibliography,string words)
		{
			Lucene.Net.Documents.Field docIDField = new Field(DOCID_FN, docID, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Field authorField = new Field(AUTHOR_FN, author, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Field titleField = new Field(TITLE_FN, text, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			//            Lucene.Net.Documents.Field publisherField = new Field(PUBLISHER_FN, publisher, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Field publisherField = new Field(PUBLISHER_FN, publisher, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
			Lucene.Net.Documents.Document doc = new Document();
			authorField.Boost = 2; // activity 9
			doc.Add(authorField);
			doc.Add(titleField);
			doc.Add(publisherField);
			writer.AddDocument(doc);
		}

        /// Flushes buffers and closes the index
        public void CleanUp()
        {
            writer.Optimize();
            writer.Flush(true, true, true);
            writer.Dispose();
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