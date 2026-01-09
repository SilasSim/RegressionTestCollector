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
        public void WhenConstructorIsCalled_ThenQualifierIsSetCorrectly()
        {
            Assert.That(mSut.Qualifier, Is.EqualTo("TEST"));
        }

        [Test]
        public void GivenDataStringWithQualifier_WhenParseIsCalled_ThenReturnContainsKeyValuePairs()
        {
            var data = "TEST arg1=val1 arg2=val2";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["arg1"], Is.EqualTo("val1"));
            Assert.That(result["arg2"], Is.EqualTo("val2"));
        }

        [Test]
        public void GivenDataStringWithWrongQualifier_WhenParseIsCalled_ThenReturnsEmptyDictionary()
        {
            var data = "NoTest arg1=val1 arg2=val2";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void GivenDataStringWithQualifierInDifferentCase_WhenParseIsCalled_ThenReturnContainsKeyValuePairs()
        {
            var data = "teST arg1=val1 arg2=val2";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["arg1"], Is.EqualTo("val1"));
            Assert.That(result["arg2"], Is.EqualTo("val2"));
        }

        [Test]
        public void GivenDataStringWithQuotedValues_WhenParseIsCalled_ThenReturnContainsValuesWithoutQuotes()
        {
            var data = "TEST arg1=\"val1 with quote\" arg2=\'val2 with single quote\'";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result["arg1"], Is.EqualTo("val1 with quote"));
            Assert.That(result["arg2"], Is.EqualTo("val2 with single quote"));
        }


        [Test]
        public void GivenDataStringWithOnlyQualifier_WhenParseIsCalled_ThenReturnsEmptyDictionary()
        {
            var data = "TEST";

            var result = mSut.Parse(data);

            Assert.That(result.Count, Is.EqualTo(0));
        }


    }
}