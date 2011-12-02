using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;

namespace NQUnit.MSTest
{
    [TestClass]
    public class QUnitTests
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize()]
        public static void Initialize(TestContext context)
        {
            var testRunner = new QUnitParserFactory().CreateParser(5000, "htmlunit");

            var tests = global::NQUnit.NQUnit.GetTests(testRunner, @"file:///M:/Product/01%20-%20RED%20DEVELOPMENT/Test_Web/JavaScript%20Tests/run.html?test=js/tests/Salamander.Css.js");

            MSTestHelpers.WriteResultsToXmlFile(context, tests);

        }  

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\jsTests.xml", "Test", DataAccessMethod.Sequential), TestMethod]
        public void Test()
        {
            var test = new QUnitTest { TestName = TestContext.DataRow["TestName"].ToString(), Result = TestContext.DataRow["Result"].ToString() };
            test.ShouldPass();
        }
    }
}