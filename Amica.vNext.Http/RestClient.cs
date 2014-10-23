using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace Amica.vNext.Http
{
    public class RestClient
    {
		#region "I N I T"

		private HttpResponseMessage _httpResponse;
		private BasicAuthenticator _basicAuthenticator;

		public RestClient () {
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings { 
				NullValueHandling = NullValueHandling.Ignore,
			};
		}

		public RestClient(Uri baseAddress) :this () {
			BaseAddress = baseAddress;
		}


		public RestClient (string baseAddress) : this (new Uri (baseAddress)) { }

		public RestClient(string baseAddress, BasicAuthenticator authenticator) : this(baseAddress) {
			BasicAuthenticator = authenticator;
		}

		public RestClient(Uri baseAddress, BasicAuthenticator authenticator) : this(baseAddress) {
			BasicAuthenticator = authenticator;
		}

		#endregion

		#region "G E T"

		public async Task<T> GetAsync<T>(string resourceName, string documentId) {

			ValidateBaseAddress ();
			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
			if (documentId == null) {
				throw new ArgumentNullException ("documentId");
			}

			using (var client = new HttpClient ()) {
				SetSettings (client);
				_httpResponse = await client.GetAsync(string.Format("{0}/{1}", resourceName, documentId));
			    if (_httpResponse.StatusCode != HttpStatusCode.OK) return default(T);
			    var json = await _httpResponse.Content.ReadAsStringAsync ();
			    var obj = JsonConvert.DeserializeObject<T> (json);
			    return obj;
			}
		}

		public async Task<T> GetAsync<T>() {
			ValidateResourceName ();
			ValidateDocumentId ();
			return await GetAsync<T> (ResourceName, DocumentId);
		}

		public async Task<T> GetAsync<T>(string documentId) {
			ValidateDocumentId ();
			return await GetAsync<T> (ResourceName, documentId);
		}

		public async Task<T> GetAsync<T>(object value) {
			ValidateResourceName ();
			if (value == null) {
				throw new ArgumentNullException ("value");
			}

			var documentId = GetRemoteId (value);
			return await GetAsync<T> (ResourceName, documentId);
		}

		public async Task<T> GetAsync<T>(string resourceName, object value) {
			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
			if (resourceName == string.Empty) {
				throw new ArgumentException ("resourceName cannot be empty.");
			}
			if (value == null) {
				throw new ArgumentNullException ("value");
			}

			var documentId = GetRemoteId (value);
			return await GetAsync<T> (ResourceName, documentId);
		}

		#endregion

		#region "P O S T"

		public async Task<HttpResponseMessage> PostAsync(string resourceName, object value) {
			ValidateBaseAddress ();
			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
			if (value == null) {
				throw new ArgumentNullException ("value");
			}

			using (var client = new HttpClient ()) {
				SetSettings (client);
				_httpResponse = await client.PostAsync (resourceName, GetContent(value));
				return _httpResponse;
			}
		}

		public async Task<HttpResponseMessage> PostAsync(object value) {
			ValidateResourceName ();
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
			ValidateResourceName ();
			return await PostAsync<T> (ResourceName, value);
		}
		#endregion

		#region "P U T"
		public async Task<HttpResponseMessage> PutAsync(string resourceName, object value) {

			ValidateBaseAddress ();
			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
			if (value == null) {
				throw new ArgumentNullException ("value");
			}

			using (var client = new HttpClient ()) {
				SetSettings (client, value);
				_httpResponse = await client.PutAsync(string.Format("{0}/{1}", resourceName, GetRemoteId(value)), GetContent(value));
				return _httpResponse;
			}
		}

		public async Task<HttpResponseMessage> PutAsync(object value) {
			return await PutAsync (ResourceName, value);
		}

		public async Task<T> PutAsync<T>(string resourceName, object value) {
			_httpResponse = await PutAsync (resourceName, value);

			switch (_httpResponse.StatusCode) {
			case HttpStatusCode.OK:
				var s = await _httpResponse.Content.ReadAsStringAsync ();
				T obj = JsonConvert.DeserializeObject<T> (s);
				return obj;
			default:
				return default(T);
			}
		}

		public async Task<T> PutAsync<T>(object value) {
			ValidateResourceName ();
			return await PutAsync<T> (ResourceName, value);
		}

		#endregion

		#region "D E L E T E"

		public async Task<HttpResponseMessage> DeleteAsync(string resourceName, object value) {

			ValidateBaseAddress ();
			if (resourceName == null) {
				throw new ArgumentNullException ("ResourceName");
			}
			if (value == null) {
				throw new ArgumentNullException ("value");
			}

			using (var client = new HttpClient ()) {
				SetSettings (client, value);
				_httpResponse = await client.DeleteAsync (string.Format ("{0}/{1}", resourceName, GetRemoteId (value)));
				return _httpResponse;
			}
		}

		public async Task<HttpResponseMessage> DeleteAsync(object value) {
			ValidateResourceName ();
			_httpResponse = await DeleteAsync (ResourceName, value);
			return _httpResponse;
		}
			
		#endregion

		#region "P R O P R I E R T I E S"

		public Uri BaseAddress { get; set; }
		public string ResourceName { get; set; }
		public string DocumentId { get; set; }
		public HttpResponseMessage HttpResponse{ get { return _httpResponse; } }

		/// <summary>
		/// Gets or sets the authenticator.
		/// </summary>
		/// <value>The authenticator.</value>
		public BasicAuthenticator BasicAuthenticator {
			get { return _basicAuthenticator; }
			set { _basicAuthenticator = value; }
		}

		#endregion

		#region "S U P P O R T"

		private void SetSettings(HttpClient client) {
			client.BaseAddress = BaseAddress;
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			if (BasicAuthenticator != null) {
				client.DefaultRequestHeaders.Authorization = BasicAuthenticator.AuthenticationHeader ();
			}

		}

		private void SetSettings (HttpClient client, object value) {
			SetSettings (client);

			// TODO use maybe reflection or custom attribute to know which property stands for etag field
			// instead of hardcoding it.
			PropertyInfo info = value.GetType ().GetProperty ("ETag");
			if (info == null) {
				// TODO explicit exception, also see TODO above.
				throw new Exception ("Etag property not found.");
			}
			var etag = info.GetValue (value, null);
			if (etag == null) {
				// TODO explicit exception, also see TODO above.
				throw new Exception ("Etag value cannot be nulla when doing an edit operation.");
			}
			client.DefaultRequestHeaders.TryAddWithoutValidation ("If-Match", etag.ToString());
		}

		private StringContent GetContent(object value) {
			var settings = new JsonSerializerSettings { ContractResolver = new EveContractResolver () };
			var content = new StringContent (JsonConvert.SerializeObject (value, settings));
			content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");
			return content;
		}

		private string GetRemoteId(object value) {
			// TODO use maybe reflection or custom attribute to know which property stands for API unique id
			// instead of hardcoding it.
			PropertyInfo info = value.GetType ().GetProperty ("RemoteId");
			if (info == null) {
				// TODO explicit exception, also see TODO above.
				throw new Exception("RemoteId property not found.");
			}
			var v = info.GetValue (value, null);
			if (v == null) {
				// TODO explicit exception, also see TODO above.
				throw new Exception ("RemoteId value cannot be null when doing an edit operation.");
			}
			return v.ToString ();
		}

		private void ValidateResourceName() {
			if (ResourceName == null) {
				throw new ArgumentNullException ("ResourceName");
			}
			if (ResourceName == string.Empty) {
				throw new ArgumentException ("ResourceName cannot be empty.");
			}
		}

		private void ValidateDocumentId() {
			if (DocumentId == null) {
				throw new ArgumentNullException ("DocumentId");
			}
			if (DocumentId == string.Empty) {
				throw new ArgumentException ("DocumentId cannot be empty.");
			}
		}

		private void ValidateBaseAddress() {
			if (BaseAddress == null) {
				throw new ArgumentNullException ("BaseAddress");
			}
		}
		#endregion


    }


}
