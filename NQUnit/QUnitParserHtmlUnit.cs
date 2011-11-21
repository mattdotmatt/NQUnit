using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using com.gargoylesoftware.htmlunit;
using com.gargoylesoftware.htmlunit.html;
using System.Xml;
using System.Xml.XPath;
using NQUnit.Interfaces;

namespace NQUnit
{
    public class QUnitParserHtmlUnit : IQUnitParser
    {
        private IBrowser Browser { get; set; }
        private int MsWait { get; set; }

        public QUnitParserHtmlUnit(int msWait,IBrowser browser)
        {
            Browser = browser;
            MsWait = msWait;
        }

        public string ParserType
        {
            get
            {
                return "htmlunit";
            }
        }

        /// <summary>
        /// Returns an array of QUnitTest objects given a test page URI.
        /// </summary>
        /// <param name="testPage">The URI of the test page; either a URL or a file path</param>
        /// <returns>An array of QUnitTest objects</returns>
        public IEnumerable<QUnitTest> GetQUnitTestResults(string testPage)
        {
            HtmlPage page = Browser.GetPage(testPage);

            System.Threading.Thread.Sleep(MsWait);

            return GrabTestResultsFromWebPage(page);
        }

        private IEnumerable<QUnitTest> GrabTestResultsFromWebPage(HtmlPage testPage)
        {
            var documentRoot = testPage.getElementById("qunit-tests");

            XmlDocument xmlResults = new XmlDocument();
            xmlResults.LoadXml(documentRoot.asXml());

            var results = new List<QUnitTest>();

            foreach (XmlNode result in xmlResults.FirstChild.ChildNodes)
            {

                var resultClass = result.Attributes["class"].Value;

                var testName = result.SelectSingleNode("strong/span[@class='test-name']/text()").Value.Trim();

                var message = "";
                var failMessageItem = result.SelectSingleNode("ol/li[@class='fail']/text()");
                if (failMessageItem != null)
                {

                    message = failMessageItem.Value.Trim();
                }

                yield return new QUnitTest
                {
                    FileName = "",
                    TestName = testName.ToString(),
                    Result = resultClass,
                    Message = message
                };

            }
        }

        public void Dispose()
        {

        }
    }
}
