﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// TODO Support for Queries
// TODO: native supprot for request headers like If-Modified-Since.

namespace Amica.vNext.Http
{
	public class RestClient
	{
		#region "I N I T"

		private HttpResponseMessage _httpResponse;

	    public RestClient ()
		{
			// don't serialize null values.
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings { 
				NullValueHandling = NullValueHandling.Ignore,
			};

		    LastUpdatedField = "_updated";
		}

		public RestClient (Uri baseAddress) : this ()
		{
			BaseAddress = baseAddress;
		}


		public RestClient (string baseAddress) : this (new Uri (baseAddress))
		{
		}

		public RestClient (string baseAddress, BasicAuthenticator authenticator) : this (baseAddress)
		{
			BasicAuthenticator = authenticator;
		}

		public RestClient (Uri baseAddress, BasicAuthenticator authenticator) : this (baseAddress)
		{
			BasicAuthenticator = authenticator;
		}

		#endregion

		#region "G E T"

	    /// <summary>
	    /// Performs an asynchronous GET request on an arbitrary endpoint.
	    /// </summary>
	    /// <param name="uri">Endpoint URI.</param>
	    /// <param name="etag">ETag</param>
	    /// <param name="ifModifiedSince">Return only documents that changed since this date.</param>
	    public async Task<HttpResponseMessage> GetAsync(string uri, string etag, DateTime? ifModifiedSince)
	    {
	        
	        if (uri == null) {
	            throw new ArgumentNullException("uri");
	        }
			ValidateBaseAddress ();

			using (var client = new HttpClient ()) {
				Settings (client);
                var query = new System.Text.StringBuilder(uri);
			    if (etag != null) {
                    client.DefaultRequestHeaders.TryAddWithoutValidation ("If-None-Match", etag);
			    }
			    if (ifModifiedSince != null)
			    {
			        query.Append(string.Format(@"?where={{""{0}"": ""{1}""}}", LastUpdatedField,  ((DateTime) ifModifiedSince).ToString("r")));
                    //client.DefaultRequestHeaders.TryAddWithoutValidation ("If-Modified-Since", ((DateTime)ifModifiedSince).ToString("r"));
			    }
			    _httpResponse = await client.GetAsync(query.ToString());
				return _httpResponse;
			}
	    }

	    /// <summary>
	    /// Performs an asynchronous GET request on an arbitrary endpoint.
	    /// </summary>
	    /// <param name="uri">Endpoint URI.</param>
	    /// <param name="etag">ETag</param>
	    public async Task<HttpResponseMessage> GetAsync(string uri, string etag)
	    {
	        return await GetAsync(uri, etag, null);

	    }

	    /// <summary>
	    /// Performs an asynchronous GET request on an arbitrary endpoint.
	    /// </summary>
	    /// <param name="uri">Endpoint URI.</param>
	    /// <param name="ifModifiedSince">Return only documents that changed since this date.</param>
	    public async Task<HttpResponseMessage> GetAsync(string uri, DateTime? ifModifiedSince)
	    {
	        return await GetAsync(uri, null, ifModifiedSince);
	    }

        /// <summary>
        /// Performs an asynchronous GET request on an arbitrary endpoint.
        /// </summary>
        /// <param name="uri">Endpoint URI.</param>
	    public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            return await GetAsync(uri, etag:null);
        }

	    /// <summary>
	    /// Performs an asynchronous GET request on a document endpoint.
	    /// </summary>
	    /// <returns> An istance of the requested document, or null if document was not found or some other issue arised.</returns>
	    /// <param name="resourceName">Resource name.</param>
	    /// <param name="documentId">Document identifier.</param>
	    /// <param name="etag">Document ETag.</param>
	    /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
	    public async Task<T> GetAsync<T> (string resourceName, string documentId, string etag)
		{

			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
			if (resourceName == string.Empty) {
				throw new ArgumentException ("resourceName cannot be empty.");
			}
			if (documentId == null) {
				throw new ArgumentNullException ("documentId");
			}
			_httpResponse = await GetAsync (string.Format ("{0}/{1}", resourceName, documentId), etag);

			if (_httpResponse.StatusCode != HttpStatusCode.OK)
				return default(T);
			var json = await _httpResponse.Content.ReadAsStringAsync ();
			var obj = JsonConvert.DeserializeObject<T> (json);
			return obj;
		}

		/// <summary>
		/// Performs an asynchronous GET request on a document endpoint.
		/// </summary>
		/// <returns> An istance of the requested document, or null if document was not found or some other issue arised.</returns>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="documentId">Document identifier.</param>
		/// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
		public async Task<T> GetAsync<T> (string resourceName, string documentId)
		{
		    return await GetAsync<T>(resourceName, documentId, null);
		}

		/// <summary>
		/// Performs an asynchronous GET request of a document.
		/// </summary>
		/// <returns> An istance of the requested document, or null if document was not found or some other issue arised.</returns>
		/// <param name="obj">The instance of the document to be retrieved.</param>
		/// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
		public async Task<T> GetAsync<T> (object obj)
		{
			ValidateResourceName ();
		    return await GetAsync<T>(ResourceName, obj);
		}

		/// <summary>
		/// Performs an asynchronous GET request on a document endpoint.
		/// </summary>
		/// <returns> An istance of the requested document, or null if document was not found or some other issue arised.</returns>
		/// <param name="resourceName">The resource name.</param>
		/// <param name="obj">The instance of the document to be retrieved.</param>
		/// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
		public async Task<T> GetAsync<T> (string resourceName, object obj)
		{
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}

			var retObj = await GetAsync<T> (resourceName, GetDocumentId (obj), GetETag(obj));

		    return _httpResponse.StatusCode == HttpStatusCode.NotModified ? (T)obj : retObj;
		}

		/// <summary>
		/// Performs an asynchronous GET request for a specific document.
		/// </summary>
		/// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
		/// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
		public async Task<List<T>> GetAsync<T> ()
		{
			ValidateResourceName ();
			return await GetAsync<T> (ResourceName);
		}

		/// <summary>
		/// Performs an asynchronous GET request for a specific document.
		/// </summary>
		/// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
		/// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
		public async Task<List<T>> GetAsync<T> (DateTime? ifModifiedSince)
		{
			ValidateResourceName ();
			return await GetAsync<T> (ResourceName, ifModifiedSince);
		}

	    /// <summary>
	    /// Performs an asynchronous GET request on a resource endpoint.
	    /// </summary>
	    /// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
	    /// <param name="resourceName">Resource endpoint.</param>
	    /// <param name="ifModifiedSince">Return only documents that changed since this date. </param>
	    /// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
	    public async Task<List<T>> GetAsync<T>(string resourceName, DateTime? ifModifiedSince)
	    {
			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
			if (resourceName == string.Empty) {
				throw new ArgumentException ("resourceName cannot be empty.");
			}

			_httpResponse = await GetAsync (resourceName, ifModifiedSince);

		    if (_httpResponse.StatusCode != HttpStatusCode.OK)
		        return default(List<T>);
			var json = await _httpResponse.Content.ReadAsStringAsync ();

		    var jo = JObject.Parse(json);
		    return JsonConvert.DeserializeObject<List<T>>(jo.Property("_items").Value.ToString(Formatting.None));
	    }

		/// <summary>
		/// Performs an asynchronous GET request on a resource endpoint.
		/// </summary>
		/// <returns>A list of objects of the requested type, or null if the response from the remote service was something other than 200 OK.</returns>
		/// <param name="resourceName">Resource endpoint.</param>
		/// <typeparam name="T">The type to which the retrieved JSON should be casted.</typeparam>
		public async Task<List<T>> GetAsync<T> (string resourceName)
		{
		    return await GetAsync<T>(resourceName, ifModifiedSince:null);
		}

		#endregion

		#region "P O S T"

		/// <summary>
		/// Performs an asynchronous POST request on a resource endpoint.
		/// </summary>
		/// <returns>The raw response returned by the service.</returns>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="obj">Object to be stored.</param>
		public async Task<HttpResponseMessage> PostAsync (string resourceName, object obj)
		{
			ValidateBaseAddress ();
			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
		    if (resourceName == string.Empty) {
		        throw new ArgumentException("resourceName");
		    }
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}

			using (var client = new HttpClient ()) {
				Settings (client);
				_httpResponse = await client.PostAsync (resourceName, SerializeObject (obj));
				return _httpResponse;
			}
		}

		/// <summary>
		/// Performs an asynchronous POST request on a resource endpoint.
		/// </summary>
		/// <returns>The raw response returned by the service.</returns>
		/// <param name="obj">Object to be stored.</param>
		public async Task<HttpResponseMessage> PostAsync (object obj)
		{
			ValidateResourceName ();
			return await PostAsync (ResourceName, obj);
		}

		/// <summary>
		/// Performs an asynchronous POST request on a resource endpoint.
		/// </summary>
		/// <returns>An instance of the document.</returns>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="obj">Object to be stored on the service.</param>
		/// <typeparam name="T">Type of the document.</typeparam>
		public async Task<T> PostAsync<T> (string resourceName, object obj)
		{
			_httpResponse = await PostAsync (resourceName, obj);

			switch (_httpResponse.StatusCode) {
			case HttpStatusCode.Created:
				var s = await _httpResponse.Content.ReadAsStringAsync ();
				T instance = JsonConvert.DeserializeObject<T> (s);
				return instance;
			default:
				return default(T);
			}
		}

		/// <summary>
		/// Performs an asynchronous POST request on a resource endpoint.
		/// </summary>
		/// <returns>An instance of the document.</returns>
		/// <param name="obj">Object to be stored on the service.</param>
		/// <typeparam name="T">Type of the document.</typeparam>
		public async Task<T> PostAsync<T> (object obj)
		{
			ValidateResourceName ();
			return await PostAsync<T> (ResourceName, obj);
		}

		#endregion

		#region "P U T"

		/// <summary>
		/// Performs an asynchronous PUT request on a document endpoint.
		/// </summary>
		/// <returns>The raw response returned by the the servce.</returns>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="obj">Object to be stored on the service.</param>
		public async Task<HttpResponseMessage> PutAsync (string resourceName, object obj)
		{

			ValidateBaseAddress ();
			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
			if (resourceName == string.Empty) {
				throw new ArgumentException ("resourceName");
			}
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}

			using (var client = new HttpClient ()) {
				SettingsForEditing (client, obj);
				_httpResponse = await client.PutAsync (string.Format ("{0}/{1}", resourceName, GetDocumentId (obj)), SerializeObject (obj));
				return _httpResponse;
			}
		}

		/// <summary>
		/// Performs an asynchronous PUT request on a document endpoint.
		/// </summary>
		/// <returns>The raw response returned by the service.</returns>
		/// <param name="obj">Object to be stored on the service.</param>
		public async Task<HttpResponseMessage> PutAsync (object obj)
		{
			return await PutAsync (ResourceName, obj);
		}

		/// <summary>
		/// Performs an asynchronous PUT request on a document endpoint.
		/// </summary>
		/// <returns>The instance of the document.</returns>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="obj">Object to be stored on the service.</param>
		/// <typeparam name="T">Type of the document.</typeparam>
		public async Task<T> PutAsync<T> (string resourceName, object obj)
		{
			_httpResponse = await PutAsync (resourceName, obj);

			switch (_httpResponse.StatusCode) {
			case HttpStatusCode.OK:
				var s = await _httpResponse.Content.ReadAsStringAsync ();
				var instance = JsonConvert.DeserializeObject<T> (s);
				return instance;
			default:
				return default(T);
			}
		}

		/// <summary>
		/// Performs an asynchronous PUT request on a document endpoint.
		/// </summary>
		/// <returns>An instance of the document.</returns>
		/// <param name="obj">Object to be stored on the service.</param>
		/// <typeparam name="T">Type of the document.</typeparam>
		public async Task<T> PutAsync<T> (object obj)
		{
			ValidateResourceName ();
			return await PutAsync<T> (ResourceName, obj);
		}

		#endregion

		#region "D E L E T E"

		/// <summary>
		/// Performs an asynchronous DELETE request on a document endpoint.
		/// </summary>
		/// <returns>The raw response returned by the service.</returns>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="obj">Object to be deleted on the service.</param>
		public async Task<HttpResponseMessage> DeleteAsync (string resourceName, object obj)
		{

			ValidateBaseAddress ();
			if (resourceName == null) {
				throw new ArgumentNullException ("resourceName");
			}
		    if (resourceName == string.Empty) {
		        throw new ArgumentException("resourceName");
		    }
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}

			using (var client = new HttpClient ()) {
				SettingsForEditing (client, obj);
				_httpResponse = await client.DeleteAsync (string.Format ("{0}/{1}", resourceName, GetDocumentId (obj)));
				return _httpResponse;
			}
		}

		/// <summary>
		/// Performs an asynchronous DELETE request on a document endpoint
		/// </summary>
		/// <returns>The raw response returned by the service.</returns>
		/// <param name="obj">Object to be deleted from the service.</param>
		public async Task<HttpResponseMessage> DeleteAsync (object obj)
		{
			ValidateResourceName ();
			_httpResponse = await DeleteAsync (ResourceName, obj);
			return _httpResponse;
		}
		#endregion

		#region "P R O P R I E R T I E S"

		/// <summary>
		/// Gets or sets the remote service base address.
		/// </summary>
		/// <value>The remote service base address.</value>
		public Uri BaseAddress { get; set; }

		/// <summary>
		/// Gets or sets the name of the resource endpoint.
		/// </summary>
		/// <value>The name of the resource endpoint.</value>
		public string ResourceName { get; set; }

		/// <summary>
		/// Gets or sets the document identifier.
		/// </summary>
		/// <value>The document identifier.</value>
		/// <remarks>Used in conjuction with BaseAddress and ResourceName to construct the document endpoint.</remarks>
		public string DocumentId { get; set; }

		/// <summary>
		/// Represents a HTTP response message.
		/// </summary>
		/// <value>The http response.</value>
		public HttpResponseMessage HttpResponse{ get { return _httpResponse; } }

	    /// <summary>
	    /// Gets or sets the authenticator.
	    /// </summary>
	    /// <value>The authenticator.</value>
	    public BasicAuthenticator BasicAuthenticator { get; set; }

        /// <summary>
        /// Gets or sets the name of the LastUpdated field.
        /// </summary>
	    public string LastUpdatedField { get; set; }

	    #endregion

		#region "S U P P O R T"

		/// <summary>
		/// Sets the default client settings needed by GET and POST request methods.
		/// </summary>
		/// <param name="client">HttpClient instance.</param>
		/// <remarks>>Does not handle the If-Match header.</remarks>
		private void Settings (HttpClient client)
		{
			client.BaseAddress = BaseAddress;
			client.DefaultRequestHeaders.Accept.Clear ();
			client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
			if (BasicAuthenticator != null) {
				client.DefaultRequestHeaders.Authorization = BasicAuthenticator.AuthenticationHeader ();
			}

		}

		/// <summary>
		/// Sets the default client settings needed by PUT and DELETE request methods.
		/// </summary>
		/// <param name="client">HttpClient instance.</param>
		/// <param name="obj">Object to be edited.</param>
		/// <remarks>>Adds the object ETag to the request headers so edit operations can perform successfully.</remarks>
		private void SettingsForEditing (HttpClient client, object obj)
		{
			Settings (client);
			client.DefaultRequestHeaders.TryAddWithoutValidation ("If-Match", GetETag (obj));
		}

		/// <summary>
		/// Serializes an object to JSON and provides it as a StringContent object which can be used by any request method.
		/// </summary>
		/// <returns>A StringContent instance.</returns>
		/// <param name="obj">The object to be serialized.</param>
		private static StringContent SerializeObject (object obj)
		{
			var settings = new JsonSerializerSettings { ContractResolver = new EveContractResolver () };
			var s = JsonConvert.SerializeObject (obj, settings);
			var content = new StringContent (s);
			content.Headers.ContentType = new MediaTypeHeaderValue ("application/json");
			return content;
		}

		/// <summary>
		/// Returns the document identifier by which the document is known on the service.
		/// </summary>
		/// <returns>The document identifier.</returns>
		/// <param name="obj">The object to be sent to the service.</param>
		private static string GetDocumentId (object obj)
		{
			return GetRemoteMetaFieldValue (obj, Meta.DocumentId);
		}

		/// <summary>
		/// Returns the document ETag which is needed by edit operations on the service.
		/// </summary>
		/// <returns>The document Etag.</returns>
		/// <param name="obj">The object to be sent to the sent to the service.</param>
		private static string GetETag (object obj)
		{
			return GetRemoteMetaFieldValue (obj, Meta.ETag);
		}

		/// <summary>
		/// Returns the value of an object meta field.
		/// </summary>
		/// <returns>The remote meta field value.</returns>
		/// <param name="obj">The object.</param>
		/// <param name="metaField">Meta field to be returned.</param>
		private static string GetRemoteMetaFieldValue (object obj, Meta metaField)
		{
			var pInfo = obj.GetType ().GetProperties ().Where (
				            p => p.IsDefined (typeof(RemoteAttribute), true)).ToList ();

			foreach (var p in pInfo) {
				var attr = (RemoteAttribute)p.GetCustomAttributes (typeof(RemoteAttribute), true).FirstOrDefault ();
				if (attr != null && attr.Field == metaField) {
					var v = p.GetValue (obj, null);
				    return (v == null) ? null : v.ToString();
				}
			}
		    return null;
		}

		/// <summary>
		/// Validates the name of the resource.
		/// </summary>
		private void ValidateResourceName ()
		{
			if (ResourceName == null) {
                // ReSharper disable once NotResolvedInText
				throw new ArgumentNullException ("ResourceName");
			}
			if (ResourceName == string.Empty) {
				throw new ArgumentException ("ResourceName cannot be empty.");
			}
		}

	    /// <summary>
		/// Validates the base address.
		/// </summary>
		private void ValidateBaseAddress ()
		{
			if (BaseAddress == null) {
                // ReSharper disable once NotResolvedInText
				throw new ArgumentNullException ("BaseAddress");
			}
		}

		#endregion
	}
}
