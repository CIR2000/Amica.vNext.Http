using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amica.vNext.Http;

namespace Playground
{
	class Person {
		[JsonProperty("lastname")]
		public string LastName { get; set; }
		[JsonProperty("firstname")]
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
			var rc = new RestClient ("http://127.0.0.1:5000/");

			Person person;

//			person = await rc.GetAsync<Person> ("people", "54450894d71ddf000237ae8d");
//			Console.WriteLine (person.LastName);
//			Console.WriteLine (person.FirstName);

//			rc.ResourceName = "people";
//			person = await rc.GetAsync<Person> ("54450894d71ddf000237ae8c");
//			Console.WriteLine (person.LastName);
//			Console.WriteLine (person.FirstName);

//			rc.DocumentId = "54450894d71ddf000237ae8e";
//			person = await rc.GetAsync<Person> ();
//			Console.WriteLine (person.LastName);
//			Console.WriteLine (person.FirstName);

			person = new Person {LastName = "Serena"};

//			rc.ResourceName = "people";
//			HttpResponseMessage r = await rc.PostAsync (person);

			HttpResponseMessage r = await rc.PostAsync ("people", person);
			Console.WriteLine (r.StatusCode);

			var s = await r.Content.ReadAsStringAsync ();
			Console.WriteLine (s);


	  }
	}
}
