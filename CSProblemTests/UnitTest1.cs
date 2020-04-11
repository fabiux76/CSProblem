using System.Linq;
using CSProblem;
using NUnit.Framework;


namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleTest()
        {
            CSVariable v1 = new CSVariable("A", Enumerable.Range(1,2).ToList());
            CSVariable v2 = new CSVariable("B", Enumerable.Range(1,2).ToList());
            
            Assert.Pass();
        }
    }
}