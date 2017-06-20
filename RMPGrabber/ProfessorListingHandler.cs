using HtmlAgilityPack;
using HtmlInteractionUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMPGrabber
{
    class ProfessorListingHandler
    {
        private string _baseUrl = "http://www.ratemyprofessors.com/search.jsp?query=";
        private string _queryUrl;
        private HtmlNode _rootNode;
        public HtmlNodeCollection ParentNodesOfProfUrl;
        public List<string> ProfessorUrls = new List<string>();
        public List<string> ProfessorUniversities = new List<string>();

        public ProfessorListingHandler(string firstName, string lastName)
        {
            _queryUrl = getQueryUrlFromSearchName(firstName, lastName);
            _rootNode = Utilities.getRootNodeOfHtmlDocument(_queryUrl);
            this.ParentNodesOfProfUrl = selectProfessorListingNodes(_rootNode);
            this.ProfessorUrls = this.getProfessorUrlsFromNodes(this.ParentNodesOfProfUrl);
            this.ProfessorUniversities = this.getProfessorUniversitiesFromNodes(this.ParentNodesOfProfUrl);
        }

        private HtmlNodeCollection selectProfessorListingNodes(HtmlNode rootNode)
        {
            return rootNode.SelectNodes("//li[@class= 'listing PROFESSOR']");
        }
        private string getQueryUrlFromSearchName(string firstName, string lastName)
        {
            string queryUrl = _baseUrl + firstName + "+" + lastName;
            return queryUrl;
        }

        private List<string> getProfessorUrlsFromNodes(HtmlNodeCollection nodes)
        {
            var urlList = new List<string> { };
            foreach (var childNode in nodes)
            {
                var grandchildren = childNode.SelectNodes("a")[0].Attributes;
                var professorUrl = grandchildren[0].Value;
                urlList.Add(professorUrl);
            }
            return urlList;
        }

        private List<string> getProfessorUniversitiesFromNodes(HtmlNodeCollection nodes)
        {
            var ProfessorUniversities = new List<string> { };
            foreach (var childNode in nodes)
            {
                var universityName = childNode.ChildNodes[1].ChildNodes[3].ChildNodes[3].InnerHtml;
                ProfessorUniversities.Add(universityName);
            }
            return ProfessorUniversities;
        }
    }
}
