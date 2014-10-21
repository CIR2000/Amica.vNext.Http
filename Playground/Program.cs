using System;
using System.Threading.Tasks;
using Amica.vNext.Http;

namespace Playground
{
	class Person {
		public string LastName { get; set; }
		public string FirstName { get; set; }
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			RunAsync ().Wait ();
		}
        static async Task RunAsync()
        {
			var rc = new RestClient ("http://eve-demo.herokuapp.com/");

			Person person;

//			person = await rc.GetAsync<Person> ("people", "54450894d71ddf000237ae8d");
//			Console.WriteLine (person.LastName);
//			Console.WriteLine (person.FirstName);

//			rc.ResourceName = "people";
			person = await rc.GetAsync<Person> ("54450894d71ddf000237ae8c");
			Console.WriteLine (person.LastName);
			Console.WriteLine (person.FirstName);

//			rc.DocumentId = "54450894d71ddf000237ae8e";
//			person = await rc.GetAsync<Person> ();
//			Console.WriteLine (person.LastName);
//			Console.WriteLine (person.FirstName);


	  }
	}
}
