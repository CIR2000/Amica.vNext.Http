using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amica.vNext.Http;

namespace Playground
{
	class Company {
		[JsonProperty("n")]
		public string Name {get; set;}
		[JsonProperty("p")]
		public string Password { get; set; }
		[JsonProperty("c")]
		public int CompanyId { get; set; }
	}

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
			BasicAuthenticator auth = new BasicAuthenticator ("token1", "");
			var rc = new RestClient ("http://127.0.0.1:5000/", auth);

			Company company;

//			company = await rc.GetAsync<Company> ("companies", "544767cb38345b4a6047ba41");
//			if (company != null)
//				Console.WriteLine (company.Name);
//			else
//				Console.WriteLine (rc.HttpResponse.StatusCode);

//			rc.ResourceName = "people";
//			person = await rc.GetAsync<Person> ("54450894d71ddf000237ae8c");
//			Console.WriteLine (person.LastName);
//			Console.WriteLine (person.FirstName);

//			rc.DocumentId = "54450894d71ddf000237ae8e";
//			person = await rc.GetAsync<Person> ();
//			Console.WriteLine (person.LastName);
//			Console.WriteLine (person.FirstName);

//			person = new Person {LastName = "Serena"};

//			rc.ResourceName = "people";
//			HttpResponseMessage r = await rc.PostAsync (person);

//			HttpResponseMessage r = await rc.PostAsync ("people", person);
//			Console.WriteLine (r.StatusCode);
//			var s = await r.Content.ReadAsStringAsync ();
//			Console.WriteLine (s);

			company = new Company { Name = "nome", Password = "Password", CompanyId = 100 };
			Company c = await rc.PostAsync<Company> ("companies", company);
			if (c != null)
				Console.WriteLine (c.Name);
			else {
				Console.WriteLine (rc.HttpResponse.StatusCode);
			}


	  }
	}
}
