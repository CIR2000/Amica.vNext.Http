using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Amica.vNext.Http
{
    public class RestClient
    {
		#region "I N I T"

		public RestClient(Uri baseAddress)
		{
			BaseAddress = baseAddress;
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings { 
				NullValueHandling = NullValueHandling.Ignore,
			};
		}

		public RestClient (string baseAddress) : this (new Uri (baseAddress)) { }

		#endregion

		#region "G E T"

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
		#endregion

		#region "P O S T"

		public async Task<HttpResponseMessage> PostAsync(string resourceName, object value) {

			if (BaseAddress == null) {
				throw new ArgumentNullException ("BaseAddress");
			}
			if (resourceName == null) {
				throw new ArgumentNullException ("ResourceName");
			}

			var content = new StringContent(JsonConvert.SerializeObject(value));
			content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");

			using (var client = new HttpClient ()) {
				client.BaseAddress = BaseAddress;
				var response = await client.PostAsync (resourceName, content);
				return response;
			}
		}
		public async Task<HttpResponseMessage> PostAsync(object value) {
			return await PostAsync (ResourceName, value);
		}

		#endregion

		#region "P R O P R I E R T I E S"

		public Uri BaseAddress { get; set; }
		public string ResourceName { get; set; }
		public string DocumentId { get; set; }

		#endregion
    }


}
