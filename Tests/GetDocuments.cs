using System;
using System.Threading.Tasks;
using Amica.vNext.Objects;
using NUnit.Framework;
using System.Net;

namespace Amica.vNext.Http.Tests
{
    [TestFixture]
    class GetDocuments : MethodsBase
    {
        internal Company Original2;

        [SetUp]
        public void DerivedInit()
        {
            Init();

            // POST in order to get a valid ETag
            Original = RestClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);
            Original2 = RestClient.PostAsync<Company>(Endpoint, new Company { Name = "Name2" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);
        }

        [Test]
        public void UseResourceName()
        {
            RestClient.ResourceName = Endpoint;
            var result = RestClient.GetAsync<Company>().Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 2);
            ValidateAreEquals(Original, result[0]);
            ValidateAreEquals(Original2, result[1]);
        }

        [Test]
        public void UseResourceNameConsiderIms()
        {
            System.Threading.Thread.Sleep(1000);

            // POST in order to get a valid ETag
            var original3 = RestClient.PostAsync<Company>(Endpoint, new Company { Name = "Name3" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);

            RestClient.ResourceName = Endpoint;
            var result = RestClient.GetAsync<Company>(original3.Updated).Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 1);
            ValidateAreEquals(original3, result[0]);
        }

        [Test]
        public void AcceptEndpoint()
        {
            var result = RestClient.GetAsync<Company>(Endpoint).Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 2);
            ValidateAreEquals(Original, result[0]);
            ValidateAreEquals(Original2, result[1]);
        }

        [Test]
        public void AcceptEndpointConsiderIms()
        {
            System.Threading.Thread.Sleep(1000);

            // POST in order to get a valid ETag
            var original3 = RestClient.PostAsync<Company>(Endpoint, new Company { Name = "Name3" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);

            var result = RestClient.GetAsync<Company>(Endpoint, original3.Updated).Result;
            Assert.AreEqual(HttpStatusCode.OK, RestClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 1);
            ValidateAreEquals(original3, result[0]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task BaseAddessNullException()
        {
            RestClient.BaseAddress = null;
            await RestClient.GetAsync<Company>(Endpoint);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName", MatchType = MessageMatch.Contains)]
        public async Task ResourceNameNullException()
        {
            await RestClient.GetAsync<Company>(resourceName:null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName", MatchType = MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await RestClient.GetAsync<Company>(string.Empty);
        }
    }
}
