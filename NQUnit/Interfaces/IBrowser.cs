using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.gargoylesoftware.htmlunit;
using com.gargoylesoftware.htmlunit.html;

namespace NQUnit.Interfaces
{
   public interface IBrowser:IDisposable
   {
       HtmlPage GetPage(string url);
    }
}
