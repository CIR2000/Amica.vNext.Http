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
    }
}
