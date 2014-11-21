using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Amica.vNext.Http.Tests
{
    [TestFixture]
    class BasicAuthenticator
    {
        [Test] 
        public void BasicAuthenticatorDefaults() { 
            var ba = new Http.BasicAuthenticator("user", "pw");
            Assert.AreEqual(ba.UserName, "user"); 
            Assert.AreEqual(ba.Password, "pw");
        }
    }
}
