using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NQUnit.MSTest
{
    public static class MSTestHelpers
    {
        public static void WriteResultsToXmlFile(TestContext context, IEnumerable<QUnitTest> tests)
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

        public static void ShouldPass(this QUnitTest theTest)
        {
            if (theTest.InitializationException != null)
                throw new Exception("The QUnit initialization failed.", theTest.InitializationException);

            Assert.AreEqual("pass", theTest.Result, String.Format("Test: {0}", theTest.TestName + Environment.NewLine + theTest.Message));
        }
    }
}
