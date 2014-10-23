using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Amica.vNext.Http;

namespace Playground
{
	class Company {

		[JsonProperty("_id")]
		public string RemoteId { get; set; }

		[JsonProperty("_etag")]
		public string ETag { get; set; }

		[JsonProperty("_updated")]
		public DateTime? Updated { get; set; }

		[JsonProperty("n")]
		public string Name {get; set;}

		[JsonProperty("p")]
		public string Password { get; set; }

		[JsonProperty("c")]
		public int CompanyId { get; set; }

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

			company = await rc.GetAsync<Company> ("companies", "544767cb38345b4a6047ba41");
			if (company != null)
				Console.WriteLine (company.Name);
			else
				Console.WriteLine (rc.HttpResponse.StatusCode);
			rc.ResourceName = "companies";
//			company.Name = "mynewname";
			await rc.DeleteAsync(company);
			Console.WriteLine (rc.HttpResponse.StatusCode);
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

//			company = new Company { Name = "nome", Password = "Password", CompanyId = 113};
//			Company c = await rc.PostAsync<Company> ("companies", company);
//			if (c != null)
//				Console.WriteLine (c.Name);
//			else {
//				Console.WriteLine (rc.HttpResponse.StatusCode);
//			}


	  }
	}
}
