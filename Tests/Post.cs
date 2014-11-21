using System;
using System.Threading.Tasks;
using Amica.vNext.Objects;
using NUnit.Framework;
using System.Net;

namespace Amica.vNext.Http.Tests
{
    [TestFixture]
    class Post : MethodsBase
    {
         [SetUp]
        public void DerivedInit()
        {
            Init();
            Original = new Company {Name = "Name"};
        }

        [Test]
        public void AcceptEndpointAndObject()
        {
            var result = RestClient.PostAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [Test]
        public void AcceptObject()
        {
            RestClient.ResourceName = Endpoint;
            var result = RestClient.PostAsync<Company>(Original).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [Test]
        public void AcceptEndpointAndObjectReturnHttpResponse()
        {
            var message = RestClient.PostAsync(Endpoint, Original).Result;
            ValidateReturnedHttpResponse(message, Original);
        }

        [Test]
        public void AcceptObjectReturnHttpResponse()
        {
            RestClient.ResourceName = Endpoint;
            var message = RestClient.PostAsync(Original).Result;
            ValidateReturnedHttpResponse(message, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressPropertyNullException()
        {
            RestClient.BaseAddress = null;
            await RestClient.PostAsync("resource", Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentNullException()
        {
            await RestClient.PostAsync(null, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await RestClient.PostAsync(string.Empty, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj",MatchType= MessageMatch.Contains)]
        public async Task ObjArgumentNullException()
        {
            await RestClient.PostAsync("resource", null);
        }
    }
}
