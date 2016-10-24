using System;
namespace SearchEngie
{
	public class Book
	{
		public string DocId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Bibliography { get; set; }
		public string Words { get; set; }

		public Book()
		{
		}

		public Book(string docId, string title,string author, string bibliography, string words)
		{
			DocId = docId;
			Title = title;
			Author = author;
			Bibliography = bibliography;
			Words = words;
		}
	}
}

