using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NQUnit.NUnit.JavaScriptTests.NQUnit;
using NQunit.Tests;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
using NQUnit.NUnit;

namespace NQUnit.MSTest.JavaScriptTests.NQUnit
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

            WriteResultsToXmlFile(context, tests);

        }

        private static void WriteResultsToXmlFile(TestContext context, IEnumerable<QUnitTest> tests)
        {
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(context.TestDeploymentDir, "jsTests.xml")))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Rows");

                foreach (var testCase in tests)
                {
                    writer.WriteStartElement("Test");

                    writer.WriteElementString("TestName", testCase.TestName);
                    writer.WriteElementString("Result", testCase.Result);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\jsTests.xml", "Test", DataAccessMethod.Sequential), TestMethod]
        public void Test()
        {
            var test = new QUnitTest { TestName = TestContext.DataRow["TestName"].ToString(), Result = TestContext.DataRow["Result"].ToString() };
            test.ShouldPass();
        }
    }
}