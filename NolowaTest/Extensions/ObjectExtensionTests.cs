using NolowaBackendDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolowaTest.Extensions
{
    class ObjectExtensionTests
    {
        [Test]
        [TestCase(null)]
        public void IsNull_성공케이드(object src)
        {
            var actual = src.IsNull();

            Assert.That(actual, Is.EqualTo(true));
        }
    }
}
