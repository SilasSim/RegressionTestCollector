using RegressionTestCollector.Parser;

namespace RegressionTestCollectorTests.Parser
{
    [TestFixture]
    public class BatParserTests
    {
        private BatParser mSut = new BatParser("TEST");

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Constructor_SetQualifier()
        {
            Assert.That(mSut.Qualifier, Is.EqualTo("TEST"));
        }

        [Test]
        public void Parse_WithMatchingQualifier_ExtractsKeyValuePairs()
        {
            var data = "TEST arg1=val1 arg2=val2";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["arg1"], Is.EqualTo("val1"));
            Assert.That(result["arg2"], Is.EqualTo("val2"));
        }

        [Test]
        public void Parse_WithNonMatchingQualifier_ReturnsEmptyDictionary()
        {
            var data = "NoTest arg1=val1 arg2=val2";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void Parse_QualifierIgnoresCapitalization_ReturnsEmptyDictionary()
        {
            var data = "teST arg1=val1 arg2=val2";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["arg1"], Is.EqualTo("val1"));
            Assert.That(result["arg2"], Is.EqualTo("val2"));
        }

        [Test]
        public void Parse_WithQuotedValues_RemovesQuotes()
        {
            var data = "TEST arg1=\"val1 with quote\" arg2=\'val2 with single quote\'";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["arg1"], Is.EqualTo("val1 with quote"));
            Assert.That(result["arg2"], Is.EqualTo("val2 with single quote"));
        }


        [Test]
        public void Parse_WithEmptyData_ReturnsEmptyDictionary()
        {
            var data = "TEST";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(0));
        }


    }
}