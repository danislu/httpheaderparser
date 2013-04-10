namespace Tests
{
    using HttpHeaderParser;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class AcceptHeaderPartTests
    {
        [Test]
        public void TestParse()
        {
            var part = new AcceptHeaderPart("*/*");
            Assert.AreEqual("*", part.Type, "type");
            Assert.AreEqual("*", part.SubType, "subtype");
            Assert.False(part.Parameters.Any(), "parameter");
            Assert.AreEqual(1, part.Quality, "quality");
        }

        [Test]
        public void TestParseTextPlain()
        {
            var part = new AcceptHeaderPart("text/plain");
            Assert.AreEqual("text", part.Type, "type");
            Assert.AreEqual("plain", part.SubType, "subtype");
            Assert.False(part.Parameters.Any(), "parameter");
            Assert.AreEqual(1, part.Quality, "quality");
        }

        [Test]
        public void TestParseTextPlainParameter()
        {
            var part = new AcceptHeaderPart("text/plain;parameter");
            Assert.AreEqual("text", part.Type, "type");
            Assert.AreEqual("plain", part.SubType, "subtype");
            Assert.AreEqual(1, part.Parameters.Count(), "parameter");
            Assert.AreEqual(string.Empty, part.Parameters["parameter"], "parameter");
            Assert.AreEqual(1, part.Quality, "quality");
        }
        
        [Test]
        public void TestParseTextPlainq1()
        {
            var part = new AcceptHeaderPart("text/plain;q=0.3;blabla;ost=jarlsberg");
            Assert.AreEqual("text", part.Type, "type");
            Assert.AreEqual("plain", part.SubType, "subtype");
            Assert.AreEqual(2, part.Parameters.Count(), "parameter");
            Assert.AreEqual("jarlsberg", part.Parameters["ost"]);
            Assert.AreEqual(string.Empty, part.Parameters["blabla"]);
            Assert.AreEqual(0.3, part.Quality, "quality");
        }

        [Test]
        public void TestIsExactFullTypeWithParametersMatch1()
        {
            var header = new AcceptHeaderPart("text/html;level=2");
            var candidate = new AcceptHeaderPart("text/html;level=2");
            Assert.That(header.IsExactFullTypeWithParametersMatch(candidate));
        }

        [Test]
        public void TestIsExactFullTypeWithParametersMatch2()
        {
            var header = new AcceptHeaderPart("text/html;q=0.7");
            var candidate = new AcceptHeaderPart("text/html;level=2");
            Assert.False(header.IsExactFullTypeWithParametersMatch(candidate));
        } 
        
        [Test]
        public void TestIsExactFullTypeWithParametersMatch3()
        {
            var header = new AcceptHeaderPart("text/html;level=2");
            var candidate = new AcceptHeaderPart("text/html");
            Assert.False(header.IsExactFullTypeWithParametersMatch(candidate));
        }

        [Test]
        public void TestIsExactTypeSubTypeMatch1()
        {
            var header = new AcceptHeaderPart("text/html");
            var candidate = new AcceptHeaderPart("text/html");
            Assert.That(header.IsExactTypeSubTypeMatch(candidate));
        }
        
        [Test]
        public void TestIsExactTypeSubTypeMatch2()
        {
            var header = new AcceptHeaderPart("text/html");
            var candidate = new AcceptHeaderPart("text/plain");
            Assert.False(header.IsExactTypeSubTypeMatch(candidate));
        }
        
        [Test]
        public void TestIsExactTypeSubTypeMatch3()
        {
            var header = new AcceptHeaderPart("text/html;level=1");
            var candidate = new AcceptHeaderPart("text/html;level=3");
            Assert.False(header.IsExactTypeSubTypeMatch(candidate));
        }
        
        [Test]
        public void TestIsExactTypeSubTypeMatch4()
        {
            var header = new AcceptHeaderPart("text/html");
            var candidate = new AcceptHeaderPart("text/html;level=3");
            Assert.True(header.IsExactTypeSubTypeMatch(candidate));
        }
    }
}
