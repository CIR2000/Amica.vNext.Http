using System;
using System.Threading.Tasks;
using Amica.vNext.Objects;
using NUnit.Framework;
using System.Net;

namespace Amica.vNext.Http.Tests
{
    [TestFixture]
    class Delete : MethodsBase
    {
         [SetUp]
        public void DerivedInit()
        {
            Init();

            // POST in order to get a valid ETag
            Original = RestClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, RestClient.HttpResponse.StatusCode);
        }

        [Test]
        public void AcceptEndpointAndObject()
        {
            var message = RestClient.DeleteAsync(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);

            // confirm that item has been deleted on remote
            message = RestClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId)).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Test]
        public void AcceptObject()
        {
            RestClient.ResourceName = Endpoint;
            var message = RestClient.DeleteAsync(Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);

            // confirm that item has been deleted on remote
            message = RestClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId)).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressPropertyNullException()
        {
            RestClient.BaseAddress = null;
            await RestClient.DeleteAsync("resource", Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressPropertyNullExceptionAlt()
        {
            RestClient.BaseAddress = null;
            RestClient.ResourceName = "resource";
            await RestClient.DeleteAsync(Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceArgumentNameNullException()
        {
            await RestClient.DeleteAsync(null, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await RestClient.DeleteAsync(string.Empty, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj",MatchType= MessageMatch.Contains)]
        public async Task ObjArgumentNullException()
        {
            await RestClient.DeleteAsync("resource", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="ResourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNamePropertyNullException()
        {
            await RestClient.DeleteAsync(Original);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj",MatchType= MessageMatch.Contains)]
        public async Task ObjArgumentNullExceptionAlt()
        {
            RestClient.ResourceName = "resource";
            await RestClient.DeleteAsync(null);
        }

    }
}
