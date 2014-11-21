using System;
using System.Threading.Tasks;
using Amica.vNext.Objects;
using NUnit.Framework;
using System.Net;

namespace Amica.vNext.Http.Tests
{
    [TestFixture]
    class Get : MethodsBase
    {

        [SetUp]
        public void DerivedInit()
        {
            Init();

            RestClient = new RestClient(Service);

            // POST in order to get a valid ETag
            Original = RestClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);
        }

        [Test]
        public void AcceptIdReturnHttpResponse()
        {
            var result = RestClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId)).Result;
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void AcceptIdReturnHttpResponseConsiderETag()
        {
            // Get returns NotModified as the etag matches the one on the service.
            var result = RestClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId), Original.ETag).Result;
            Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode);

            // GET returns OK since the etag does not match the one on the service and all object is retrieved.
            result = RestClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId), "not really").Result;
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void AcceptIdReturnObject()
        {
            var result = RestClient.GetAsync<Company>(Endpoint, Original.UniqueId).Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            ValidateAreEquals(Original, result);
        }

        [Test]
        public void AcceptIdReturnObjectConsiderETag()
        {
            // GET will return NotModified and null object since etag still matches the one on the service.
            var result = RestClient.GetAsync<Company>(Endpoint, Original.UniqueId, Original.ETag).Result;
            Assert.AreEqual(HttpStatusCode.NotModified, RestClient.HttpResponse.StatusCode);
            Assert.IsNull(result);

            // GET will return OK and equal object since etag does not match the one on the service.
            result = RestClient.GetAsync<Company>(Endpoint, Original.UniqueId, "not really").Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            Assert.NotNull(result);
            Assert.AreEqual(Original.UniqueId, result.UniqueId);
        }

        [Test]
        public void AcceptObjectReturnObjectConsiderETag()
        {
            // GET will return NotModified and identical object since etag still matches the one on the service.
            var result = RestClient.GetAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.NotModified, RestClient.HttpResponse.StatusCode);
            ValidateAreEquals(Original, result);

            // GET will return OK and different object since etag does not match the one on the service.
            Original.ETag = "not really";
            result = RestClient.GetAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            Assert.NotNull(result);
            Assert.AreEqual(Original.UniqueId, result.UniqueId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressNullException()
        {
            RestClient.BaseAddress = null;
            await RestClient.GetAsync<Company>("123", "123");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameNullException()
        {
            await RestClient.GetAsync<Company>(null, "123");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="documentId", MatchType = MessageMatch.Contains)]
        public async Task DocumentIdNullException()
        {
            await RestClient.GetAsync<Company>("123", documentId: null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName", MatchType = MessageMatch.Contains)]
        public async Task ResourceNameArgumentNullException()
        {
            await RestClient.GetAsync<Company>(null, new Company());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName", MatchType = MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await RestClient.GetAsync<Company>(string.Empty, new Company());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj", MatchType = MessageMatch.Contains)]
        public async Task ObjArgumentNullException()
        {
            var rc = new RestClient(Service);
            await rc.GetAsync<Company>("123", obj: null);
        }
    }
}
