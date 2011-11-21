using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NQUnit;
using Moq;
using com.gargoylesoftware.htmlunit.html;
using com.gargoylesoftware.htmlunit;
using NQUnit.Interfaces;

namespace NQunit.Tests
{
    [TestFixture]
    public class HtmlUnitQUnitParserTests
    {
        [Test]
        public void WhenAPageWithResultsIsReturnedTheResultsShouldBeExtracted()
        {
            // ARRANGE
            List<QUnitTest> expectedResult = new List<QUnitTest>();
            expectedResult.Add(new QUnitTest { TestName = "IE browser should call IE branch", Result = "pass", Message = "", FileName = "" });
            expectedResult.Add(new QUnitTest { TestName = "Firefox browser should call its branch", Result = "fail", Message = "Register Css For Firefox is called Once", FileName = "" });
            var mockWebResponse = new Mock<WebResponse>();

            var embeddedResourceHelper = new EmbeddedResourceHelper(
                Assembly.GetExecutingAssembly(),
                FullDataResource("QUnitResults.htm"),
                "QUnitResults.htm"
                );

            var testUrl = String.Format("file://{0}", embeddedResourceHelper.FullPath);

            Moq.Mock<IBrowser> mockBrowser = new Moq.Mock<IBrowser>();

            // ACT
            var sut = new QUnitParserHtmlUnit(5000,new HtmlUnitBrowser());
            var result = sut.GetQUnitTestResults(testUrl);

            // ASSERT
            var resultEnumerator = result.GetEnumerator();
            var expectedItem = 0;
            while (resultEnumerator.MoveNext())
            {
                Assert.That(resultEnumerator.Current.Result, Is.EqualTo(expectedResult[expectedItem].Result));
                Assert.That(resultEnumerator.Current.Message, Is.EqualTo(expectedResult[expectedItem].Message));
                Assert.That(resultEnumerator.Current.FileName, Is.EqualTo(expectedResult[expectedItem].FileName));
                Assert.That(resultEnumerator.Current.InitializationException, Is.EqualTo(expectedResult[expectedItem].InitializationException));
                Assert.That(resultEnumerator.Current.TestName, Is.EqualTo(expectedResult[expectedItem].TestName));
                expectedItem++;
            }

        }


        private static string FullDataResource(string resourceName)
        {
            return String.Format("NQUnit.Tests.{0}", resourceName);
        }

    }

}
