using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Amica.vNext.Http;

namespace Playground
{
	class Company
	{


		[JsonProperty ("_id")][Remote (Meta.DocumentId)]
		public string RemoteId { get; set; }

		[JsonProperty ("_etag")][Remote (Meta.ETag)]
		public string ETag { get; set; }

		[JsonProperty ("_updated")][Remote (Meta.LastUpdated)]
		public DateTime? Updated { get; set; }

		[JsonProperty ("_created")][Remote (Meta.DateCreated)]
		public DateTime? Created { get; set; }

		[JsonProperty ("n")]
		public string Name { get; set; }

		[JsonProperty ("p")]
		public string Password { get; set; }

		[JsonProperty ("c")]
		public int CompanyId { get; set; }

	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			RunAsync ().Wait ();
		}

		static async Task RunAsync ()
		{
			BasicAuthenticator auth = new BasicAuthenticator ("token1", "");
			var rc = new RestClient ("http://127.0.0.1:5000/", auth);

			Company company;

			company = await rc.GetAsync<Company> ("companies", "5200f31a38345bd591e06842");
			if (company != null)
				Console.WriteLine (company.Name);
			else
				Console.WriteLine (rc.HttpResponse.StatusCode);
//			rc.ResourceName = "companies";
//			company.Name = "exmynewname";
//			await rc.PutAsync (company);
//			Console.WriteLine (rc.HttpResponse.StatusCode);
//			Company c = await rc.GetAsync<Company> ("companies", company);
//			Console.WriteLine (c.Name);

//			HttpResponseMessage r = await rc.PostAsync ("people", person);
//			Console.WriteLine (r.StatusCode);
//			var s = await r.Content.ReadAsStringAsync ();
//			Console.WriteLine (s);

//			company = new Company { Name = "nome", Password = "Password", CompanyId = 98, RemoteId = "113"};
//			Company c = await rc.PostAsync<Company> ("companies", company);
//			if (c != null)
//				Console.WriteLine (c.Name);
//			else {
//				Console.WriteLine (rc.HttpResponse.StatusCode);
//			}


		}
	}
}
