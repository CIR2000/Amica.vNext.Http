using System;
using Amica.vNext.Http;

namespace Playground
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var rc = new RestClient ();
			Console.WriteLine (rc.ToString());
		    Console.ReadKey();
		}
	}
}
