using Lucene.Net.Search;
using FieldInvertState = Lucene.Net.Index.FieldInvertState;
namespace SearchEngie 
{
	public class NewSimilarity : DefaultSimilarity
	{
		//modify the Tf parameters
		public override float Tf(float freq)
		{
			//return (float)System.Math.Sqrt(freq);
			return 1.0f;
		}

	

	}
}
