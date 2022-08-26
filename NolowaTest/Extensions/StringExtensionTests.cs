using NolowaBackendDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolowaTest.Extensions
{
    public class StringExtensionTests
    {
        [Test]
        [TestCase("a")]
        [TestCase(" a")]
        [TestCase(".")]
        [TestCase("          .          ")]
        public void IsValid_성공케이스(string src)
        {
            var actual = src.IsValid();

            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("        ")]
        public void IsValid_실패케이스(string src)
        {
            var actual = src.IsValid();

            Assert.That(actual, Is.EqualTo(false));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("        ")]
        public void IsNotValid_성공케이스(string src)
        {
            var actual = src.IsNotVaild();

            Assert.That(actual, Is.EqualTo(true));
        }

        [Test]
        [TestCase("a")]
        [TestCase(" a")]
        [TestCase(".")]
        [TestCase("          .          ")]
        public void IsNotValid_실패케이스(string src)
        {
            var actual = src.IsNotVaild();

            Assert.That(actual, Is.EqualTo(false));
        }
    }
}
