namespace Tests
{
    using HttpHeaderParser;
    using NUnit.Framework;

    [TestFixture]
    class AcceptHeaderParserTests
    {
        [Test]
        public void TestExactMatch()
        {
            const string acceptHeader = "text/*, text/html, text/html;level=1, */*";
            var bestMatch = AcceptHeaderParser.GetPreferedMediaType(new[] { "text/*", "text/html", "text/html;level=1", "*/*" }, acceptHeader);
            Assert.AreEqual("text/html;level=1", bestMatch);

            bestMatch = AcceptHeaderParser.GetPreferedMediaType(new[] { "text/*", "text/html", "*/*" }, acceptHeader);
            Assert.AreEqual("text/html", bestMatch);

            bestMatch = AcceptHeaderParser.GetPreferedMediaType(new[] { "text/*", "*/*" }, acceptHeader);
            Assert.AreEqual("text/*", bestMatch);
        }

        [Test]
        public void TestSimple()
        {
            const string acceptHeader = "text/*, text/html";
            Assert.AreEqual("text/html", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html", "text/plain" }, acceptHeader));
            Assert.AreEqual("text/html", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/plain", "text/html" }, acceptHeader));
        }

        [Test]
        public void TestSimple2()
        {
            const string acceptHeader = "text/*, text/html, text/html;level=1, */*";
            var bestMatch = AcceptHeaderParser.GetPreferedMediaType(new[] { "image/png", "text/plain" }, acceptHeader);
            Assert.AreEqual("text/plain", bestMatch);

            bestMatch = AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;he=ho", "text/plain" }, acceptHeader);
            Assert.AreEqual("text/html;he=ho", bestMatch);

            bestMatch = AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html", "text/plain;he=ho" }, acceptHeader);
            Assert.AreEqual("text/html", bestMatch);

            bestMatch = AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;he=ho", "text/plain" }, acceptHeader);
            Assert.AreEqual("text/html;he=ho", bestMatch);
        }
        
        // The media type quality factor associated with a given type 
        // is determined by finding the media range with the highest 
        // precedence which matches that type. For example,
        //
        //       Accept: text/*;q=0.3, text/html;q=0.7, text/html;level=1,
        //               text/html;level=2;q=0.4, */*;q=0.5
        //
        // would cause the following values to be associated:
        //
        //       text/html;level=1         = 1
        //       text/html                 = 0.7
        //       text/html;level=3         = 0.7
        //       image/jpeg                = 0.5
        //       text/html;level=2         = 0.4
        //       text/plain                = 0.3
        [Test]
        public void TestRfc2616Example()
        {
            const string acceptHeader = "text/*;q=0.3, text/html;q=0.7, text/html;level=1, text/html;level=2;q=0.4, */*;q=0.5";
            Assert.AreEqual("text/html;level=1", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;level=1", "text/html" }, acceptHeader));
            Assert.AreEqual("text/html;level=1", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;level=1", "image/jpeg" }, acceptHeader));
            Assert.AreEqual("text/html", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/plain", "text/html" }, acceptHeader));
            Assert.AreEqual("text/html;level=3", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;level=3", "image/jpeg" }, acceptHeader));
            Assert.AreEqual("image/jpeg", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;level=2", "image/jpeg" }, acceptHeader));
            Assert.AreEqual("text/html", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;level=2", "text/html" }, acceptHeader));
            Assert.AreEqual("text/html;level=1", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;level=2", "text/html", "text/html;level=1" }, acceptHeader));
        }

        [Test]
        public void TestWithSubTypeWildcard()
        {
            const string acceptHeader = "text/*;q=0.7, text/html;q=0.5";
            Assert.AreEqual("text/plain", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/plain", "text/html" }, acceptHeader));
            Assert.AreEqual("text/plain", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html", "text/plain" }, acceptHeader));
        }

        [Test]
        public void TestWithDoubleWildcard()
        {
            const string acceptHeader = "text/*;q=0.3, text/html;q=0.7, text/html;level=1, text/html;level=2;q=0.4, */*;q=0.5";
            Assert.AreEqual("image/jpeg", AcceptHeaderParser.GetPreferedMediaType(new[] { "text/html;level=2", "image/jpeg" }, acceptHeader)); 
        }

        [Test]
        public void TestSortByPrecedence1()
        {
            var sorted = AcceptHeaderParser.SortByPrecedence("text/*, text/html, text/html;level=1, */* ");
            Assert.AreEqual(4, sorted.Length);
            Assert.AreEqual("text/html;level=1", sorted[0]);
            Assert.AreEqual("text/html", sorted[1]);
            Assert.AreEqual("text/*", sorted[2]);
            Assert.AreEqual("*/*", sorted[3]);
        }

        [Test]
        public void TestSortByPrecedence2()
        {
            var sorted = AcceptHeaderParser.SortByPrecedence("text/plain; q=0.5, text/html, text/x-dvi; q=0.8, text/x-c");
            Assert.AreEqual(4, sorted.Length);
            Assert.AreEqual("text/html", sorted[0]);
            Assert.AreEqual("text/x-c", sorted[1]);
            Assert.AreEqual("text/x-dvi; q=0.8", sorted[2]);
            Assert.AreEqual("text/plain; q=0.5", sorted[3]);
        }

        [Test]
        public void TestSortByPrecedence3()
        {
            const string acceptHeader = "text/*;q=0.3, text/html;q=0.7, text/html;level=1, text/html;level=2;q=0.4, */*;q=0.5";
            var sorted = AcceptHeaderParser.SortByPrecedence(acceptHeader);
            Assert.AreEqual(5, sorted.Length);
            Assert.AreEqual("text/html;level=1", sorted[0]);
            Assert.AreEqual("text/html;q=0.7", sorted[1]);
            Assert.AreEqual("*/*;q=0.5", sorted[2]);
            Assert.AreEqual("text/html;level=2;q=0.4", sorted[3]);
            Assert.AreEqual("text/*;q=0.3", sorted[4]);
        }
    }
}
