using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.gargoylesoftware.htmlunit;
using com.gargoylesoftware.htmlunit.html;
using NQUnit.Interfaces;

namespace NQUnit
{
    public class HtmlUnitBrowser:IBrowser
    {
        #region IBrowser Members

        private WebClient webClient;

        public HtmlPage GetPage(string url)
        {
            webClient = new WebClient();
            webClient.setJavaScriptEnabled(true);
            webClient.waitForBackgroundJavaScriptStartingBefore(10000);
            HtmlPage page = (HtmlPage)webClient.getPage(url);
            return page;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            webClient = null;
        }

        #endregion
    }
}
