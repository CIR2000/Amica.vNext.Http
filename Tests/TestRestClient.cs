using Amica.vNext.Objects;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;

namespace Amica.vNext.Http.Tests
{
    [TestFixture]
    public class TestRestClient
    {
        // We are running Windows in a VirtualBox VM so in order to access the OSX Host 'localhost'
        // where a local instance of the REST API is running, we use standard 10.0.2.2:5000
        private const string Service = "http://10.0.2.2:5000/";
        private const string Endpoint = "companies";

        [SetUp]
        public void Init()
        {
            // make sure remote remote endpoint is completely empty
            var hc = new HttpClient { BaseAddress = new Uri(Service) };
            Assert.IsTrue(hc.DeleteAsync(string.Format("/{0}", Endpoint)).Result.StatusCode == HttpStatusCode.OK);
        }

        [Test] 
        public void BasicAuthenticatorDefaults() { 
            var ba = new BasicAuthenticator("user", "pw");
            Assert.AreEqual(ba.UserName, "user"); 
            Assert.AreEqual(ba.Password, "pw");
        }

        [Test]
        public void PostAsyncT()
        {
            var rc = new RestClient(Service);
            var original = new Company {Name = "Name"};

            var result = rc.PostAsync<Company>(Endpoint, original).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            ValidateReturnedObject(result, original);
        }

        [Test]
        public void PostAsyncTAlt()
        {
            var rc = new RestClient(Service);
            var original = new Company {Name = "Name"};

            rc.ResourceName = Endpoint;
            var result = rc.PostAsync<Company>(original).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            ValidateReturnedObject(result, original);
        }

        [Test]
        public void PostAsync()
        {
            var rc = new RestClient(Service);
            var original = new Company {Name = "Name"};

            var message = rc.PostAsync(Endpoint, original).Result;
            ValidateReturnedHttpResponse(message, original);
        }


        [Test]
        public void PostAsyncAlt()
        {
            var rc = new RestClient(Service);
            var original = new Company {Name = "Name"};

            var message = rc.PostAsync(Endpoint, original).Result;
            ValidateReturnedHttpResponse(message, original);
        }

        [Test]
        public void PutAsyncT()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            
            original.Name = "Another Name";

            var result = rc.PutAsync<Company>(Endpoint, original).Result;
            Assert.AreEqual(HttpStatusCode.OK, rc.HttpResponse.StatusCode);
            ValidateReturnedObject(result, original);
        }

        [Test]
        public void PutAsyncTAlt()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            
            original.Name = "Another Name";

            rc.ResourceName = Endpoint;
            var result = rc.PutAsync<Company>(original).Result;
            Assert.AreEqual(HttpStatusCode.OK, rc.HttpResponse.StatusCode);
            ValidateReturnedObject(result, original);
        }

        [Test]
        public void PutAsync()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            
            original.Name = "Another Name";

            var message = rc.PutAsync(Endpoint, original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            ValidateReturnedHttpResponse(message, original);
        }

        [Test]
        public void PutAsyncAlt()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            
            original.Name = "Another Name";

            rc.ResourceName = Endpoint;
            var message = rc.PutAsync(original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            ValidateReturnedHttpResponse(message, original);
        }


        [Test]
        public void DeleteAsync()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            
            original.Name = "Another Name";

            var message = rc.DeleteAsync(Endpoint, original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);

            // confirm that item has been deleted on remote
            message = rc.GetAsync(Endpoint, original.UniqueId).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Test]
        public void DeleteAsyncAlt()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            
            original.Name = "Another Name";

            rc.ResourceName = Endpoint;
            var message = rc.DeleteAsync(original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);

            // confirm that item has been deleted on remote
            message = rc.GetAsync(Endpoint, original.UniqueId).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Test]
        public void GetAsyncListOfT()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original1 = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name1"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            var original2 = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name2"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);

            rc.ResourceName = Endpoint;
            var result = rc.GetAsync<Company>().Result;
            Assert.AreEqual(HttpStatusCode.OK, rc.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count,2);
            ValidateAreEquals(original1, result[0]);
            ValidateAreEquals(original2, result[1]);
        }

        [Test]
        public void GetAsyncListOfTAlt()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original1 = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name1"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);
            var original2 = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name2"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);

            //rc.ResourceName = Endpoint;
            var result = rc.GetAsync<Company>(Endpoint).Result;
            Assert.AreEqual(HttpStatusCode.OK, rc.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count,2);
            ValidateAreEquals(original1, result[0]);
            ValidateAreEquals(original2, result[1]);
        }

        [Test]
        public void GetAsyncTAlt1()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);

            var result = rc.GetAsync<Company>(Endpoint, original.UniqueId).Result;
            Assert.AreEqual(HttpStatusCode.OK, rc.HttpResponse.StatusCode);
            ValidateAreEquals(original, result);
        }

        [Test]
        public void GetAsyncTAlt2()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);

            rc.ResourceName = Endpoint;
            var result = rc.GetAsync<Company>(original).Result;
            Assert.AreEqual(HttpStatusCode.OK, rc.HttpResponse.StatusCode);
            ValidateAreEquals(original, result);
        }

        [Test]
        public void GetAsyncTAlt3()
        {
            var rc = new RestClient(Service);

            // POST in order to get a valid ETag
            var original = rc.PostAsync<Company>(Endpoint, new Company {Name = "Name"}).Result;
            Assert.AreEqual(HttpStatusCode.Created, rc.HttpResponse.StatusCode);

            var result = rc.GetAsync<Company>(Endpoint, original).Result;
            Assert.AreEqual(HttpStatusCode.OK, rc.HttpResponse.StatusCode);
            ValidateAreEquals(original, result);
        }

        /// <summary>
        /// Validate that two Company instances are equal (properties have same values)
        /// </summary>
        /// <param name="original">First instance.</param>
        /// <param name="result">Second instance.</param>
        public void ValidateAreEquals(Company original, Company result)
        {
            Assert.AreEqual(original.UniqueId, result.UniqueId);
            Assert.AreEqual(original.ETag, result.ETag);
            Assert.AreEqual(original.Created, result.Created);
            Assert.AreEqual(original.Updated, result.Updated);
            Assert.AreEqual(original.Name, result.Name);
            Assert.AreEqual(original.Password, result.Password);
        }

        /// <summary>
        /// Validate that the HttpResponseMessage can be casted to a Company and that its values match the original instance.
        /// </summary>
        /// <param name="responseMessage">HttoResponseMessage.</param>
        /// <param name="original">Original Company instance.</param>
        public void ValidateReturnedHttpResponse(HttpResponseMessage responseMessage, Company original)
        {
            var s = responseMessage.Content.ReadAsStringAsync ().Result;
            var c = JsonConvert.DeserializeObject<Company> (s);
            ValidateReturnedObject(c, original);
        }

        /// <summary>
        /// Validate that a Company instance is valid (has all remote service meta field values) and similar to original instance.
        /// </summary>
        /// <param name="obj">Company instance to be tested.</param>
        /// <param name="original">Similar instance.</param>
        /// <remarks>Since we are only changing Name property values, we only make sure that Name values match.</remarks>
        public void ValidateReturnedObject(Company obj, Company original)
        {
            Assert.IsNotNull(obj);
            Assert.IsInstanceOf<DateTime>(obj.Created);
            Assert.IsInstanceOf<DateTime>(obj.Updated);
            Assert.IsNotNullOrEmpty(obj.UniqueId);
            Assert.IsNotNullOrEmpty(obj.ETag);
            Assert.AreEqual(obj.Name, original.Name);
        }
    }
}
