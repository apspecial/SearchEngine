using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis; // for Analyser
using Lucene.Net.Documents; // for Socument
using Lucene.Net.Index; //for Index Writer
using Lucene.Net.Store; //for Directory

namespace SearchEngie
{
    class LuceneApplication
    {
        Lucene.Net.Store.Directory luceneIndexDirectory;
        Lucene.Net.Analysis.Analyzer analyzer;
        Lucene.Net.Index.IndexWriter writer;
        public static Lucene.Net.Util.Version VERSION = Lucene.Net.Util.Version.LUCENE_30;

        public LuceneApplication()
        {
            luceneIndexDirectory = null; // Is set in Create Index
            analyzer = null;  // Is set in CreateAnalyser
            writer = null; // Is set in CreateWriter
        }

       
        /// <param name="indexPath">Directory to store the index</param>
        public void OpenIndex(string indexPath)
        {
            // TODO: Enter code to create the Lucene Index 
            luceneIndexDirectory = Lucene.Net.Store.FSDirectory.Open(indexPath);
        }


        /// <summary>
        /// Creates the analyser
        /// </summary>
        public void CreateAnalyser()
        {
            // TODO: Enter code to create the Lucene Analyser 
            analyzer = new Lucene.Net.Analysis.SimpleAnalyzer();

        }

        /// <summary>
        /// Creates the index writer
        /// </summary>
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