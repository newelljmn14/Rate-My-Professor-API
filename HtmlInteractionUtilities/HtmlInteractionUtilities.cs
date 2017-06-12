using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlInteractionUtilities
{
    public class Utilities
    {
        public static HtmlNode getRootNodeOfHtmlDocument(string documentUrl)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(documentUrl);
            HtmlNode rootNode = document.DocumentNode;

            return rootNode;
        }
    }
}
