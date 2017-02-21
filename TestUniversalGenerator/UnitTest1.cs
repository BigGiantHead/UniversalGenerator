using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversalGenerator;

namespace TestUniversalGenerator
{
    public class TestClass1
    {
        public string StringTest { get; set; }

        public TestClass2 TestInstance { get; set; }
    }

    public class TestClass2
    {
        public string[] StringArrayTest { get; set; }
    }

    public class TestClass3
    {
        public int? IntNullableTest { get; set; }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSimple()
        {
            TestClass1 instance = UniversalGenerator.UniversalGenerator.Instance.CreateInstance<TestClass1>();
            Assert.IsTrue(!string.IsNullOrEmpty(instance.StringTest));
        }

        [TestMethod]
        public void TestInternal()
        {
            TestClass1 instance = UniversalGenerator.UniversalGenerator.Instance.CreateInstance<TestClass1>();
            Assert.IsTrue(instance.TestInstance != null);
        }

        [TestMethod]
        public void TestArray()
        {
            TestClass2 instance = UniversalGenerator.UniversalGenerator.Instance.CreateInstance<TestClass2>();
            Assert.IsTrue(instance.StringArrayTest.Length > 0);
        }

        [TestMethod]
        public void TestNullableValue()
        {
            TestClass3 instance = UniversalGenerator.UniversalGenerator.Instance.CreateInstance<TestClass3>();
            Assert.IsTrue(instance.IntNullableTest.HasValue);
        }
    }
}
