using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Amica.vNext.Http
{
    public class RestClient
    {
		public RestClient(Uri baseAddress)
		{
			BaseAddress = baseAddress;
		}
		public RestClient (string baseAddress) : this (new Uri (baseAddress)) { }

		public async Task<T> GetAsync<T>(string resourceName, string documentId) {

			if (BaseAddress == null) {
				throw new ArgumentNullException ("BaseAddress");
			}
			if (resourceName == null) {
				throw new ArgumentNullException ("ResourceName");
			}
			if (documentId == null) {
				throw new ArgumentNullException ("DocumentId");
			}

			using (var client = new HttpClient ()) {
				client.BaseAddress = BaseAddress;
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var response = await client.GetAsync(string.Format("{0}/{1}", resourceName, documentId));
				response.EnsureSuccessStatusCode ();

			    var json = await response.Content.ReadAsStringAsync ();
			    var obj = JsonConvert.DeserializeObject<T>(json);
			    return obj;
			}
		}

		public async Task<T> GetAsync<T>() {
			return await GetAsync<T> (ResourceName, DocumentId);
		}

		public async Task<T> GetAsync<T>(string documentId) {
			return await GetAsync<T> (ResourceName, documentId);
		}

		public Uri BaseAddress { get; set; }
		public string ResourceName { get; set; }
		public string DocumentId { get; set; }
    }


}
