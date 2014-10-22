using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Amica.vNext.Http
{
    public class RestClient
    {
		#region "I N I T"

		private HttpResponseMessage _httpResponse;

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

				_httpResponse = await client.GetAsync(string.Format("{0}/{1}", resourceName, documentId));
				if (_httpResponse.StatusCode == HttpStatusCode.OK) {
					var json = await _httpResponse.Content.ReadAsStringAsync ();
					var obj = JsonConvert.DeserializeObject<T> (json);
					return obj;
				}
				else {
					return default(T);
				}
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
				_httpResponse = await client.PostAsync (resourceName, content);
				return _httpResponse;
			}
		}
		public async Task<HttpResponseMessage> PostAsync(object value) {
			return await PostAsync (ResourceName, value);
		}

		public async Task<T> PostAsync<T>(string resourceName, object value) {
			_httpResponse = await PostAsync (resourceName, value);

			switch (_httpResponse.StatusCode) {
			case HttpStatusCode.Created:
				var s = await _httpResponse.Content.ReadAsStringAsync ();
				T obj = JsonConvert.DeserializeObject<T> (s);
				return obj;
			default:
				return default(T);
			}
		}

		public async Task<T> PostAsync<T>(object value) {
			return await PostAsync<T> (ResourceName, value);
		}
		#endregion

		#region "P R O P R I E R T I E S"

		public Uri BaseAddress { get; set; }
		public string ResourceName { get; set; }
		public string DocumentId { get; set; }
		public HttpResponseMessage HttpResponse{ get { return _httpResponse; } }

		#endregion
    }


}
