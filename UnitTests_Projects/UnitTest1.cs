using NUnit.Framework;
using Project;
namespace UnitTests_Projects
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGoodLogin()
        {
            Assert.AreEqual(true, false,"admin","");
        }
    }
}