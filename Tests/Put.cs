using System;
using System.Threading.Tasks;
using Amica.vNext.Objects;
using NUnit.Framework;
using System.Net;

namespace Amica.vNext.Http.Tests
{
    [TestFixture]
    class Put : MethodsBase
    {
         [SetUp]
        public void DerivedInit()
        {
            Init();

            // POST in order to get a valid ETag
            Original = RestClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);

            Original.Name = "Another Name";
        }

        [Test]
        public void AcceptEndpointAndObject()
        {
            var result = RestClient.PutAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [Test]
        public void AcceptObject()
        {
            RestClient.ResourceName = Endpoint;
            var result = RestClient.PutAsync<Company>(Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [Test]
        public void AcceptEndpointAndObjectReturnHttpResponse()
        {
            var message = RestClient.PutAsync(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            ValidateReturnedHttpResponse(message, Original);
        }

        [Test]
        public void AcceptObjectReturnHttpResponse()
        {
            RestClient.ResourceName = Endpoint;
            var message = RestClient.PutAsync(Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            ValidateReturnedHttpResponse(message, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressPropertyNullException()
        {
            RestClient.BaseAddress = null;
            await RestClient.PutAsync("resource", Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentNullException()
        {
            await RestClient.PutAsync(null, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await RestClient.PutAsync(string.Empty, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj",MatchType= MessageMatch.Contains)]
        public async Task ObjArgumentNullException()
        {
            await RestClient.PutAsync("resource", null);
        }
    }
}
