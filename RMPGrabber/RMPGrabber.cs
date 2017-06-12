using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlInteractionUtilities;

namespace RMPGrabber
{
    public class RMPGrabber
    {
        private string _baseUrl = "http://www.ratemyprofessors.com";
        private string _queryUrl { get; set; }
        private string _targetProfessorUrl { get; set; }
        private HtmlNodeCollection _parentNodesOfProfUrl { get; set; }
        private HtmlNode _rootNode { get; set; }
        public Dictionary<string, string> ProfessorListingUrlByUniversity = new Dictionary<string, string> { };
        public List<string> ProfessorUrls { get; set; }
        public List<string> ProfessorUniversities { get; set; }
        

        public RMPGrabber(string firstName, string lastName)
        {
            setStateOfProfessorUrls(firstName, lastName);
            handleMultipleProfessors();
        }
        public RMPGrabber(string firstName, string lastName, string universityName)
        {
            setStateOfProfessorUrls(firstName, lastName);
            filterProfessorsByUniversity(universityName);
            handleMultipleProfessors();
        }

        public double GetOverallQualityRatingOfFirstProfessorInList()
        {
            string firstProfessorUrlExtension = this._targetProfessorUrl;
            string totalProfessorListingUrl = this._baseUrl + firstProfessorUrlExtension;

            var rootNode = Utilities.getRootNodeOfHtmlDocument(totalProfessorListingUrl);
            var parentNode = getParentNodeOfOverallQualityRating(rootNode);

            var overallRating = parentNode.ChildNodes[1].InnerHtml;
            return Double.Parse(overallRating);
        }

        private void setStateOfProfessorUrls(string firstName, string lastName)
        {
            this._queryUrl = getQueryUrlFromSearchName(firstName, lastName);
            this._rootNode = Utilities.getRootNodeOfHtmlDocument(_queryUrl);
            this._parentNodesOfProfUrl = selectProfessorListingNodes(_rootNode);
            this.ProfessorUrls = getProfessorUrlsFromNodes(_parentNodesOfProfUrl);
            this.ProfessorUniversities = getProfessorUniversitiesFromNodes(_parentNodesOfProfUrl);

            this.ProfessorListingUrlByUniversity = this.joinUniversityNamesAndProfessorUrlsToDict(this.ProfessorUniversities, this.ProfessorUrls);
        }

        private void handleMultipleProfessors()
        {
            if (this.ProfessorUrls.Count == 1)
            {
                this._targetProfessorUrl = this.ProfessorUrls[0];
            }
            else if (this.ProfessorUrls.Count > 1 && this._targetProfessorUrl == null)
            {
                throw new Exception("Multiple professors found; narrow your search by entering the university name into the constructor");
            }
        }

        private List<string> getProfessorUrlsFromNodes(HtmlNodeCollection nodes)
        {
            var urlList = new List<string> { };
            foreach (var childNode in nodes)
            {
                var grandc = childNode.SelectNodes("a")[0].Attributes;
                var professorUrl = grandc[0].Value;
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

        private void filterProfessorsByUniversity(string universityNameFilter)
        {
            string universityNameKey;
            foreach(string universityName in this.ProfessorUniversities)
            {
                if (universityName.Contains(universityNameFilter))
                {
                    universityNameKey = universityName;
                    this._targetProfessorUrl = ProfessorListingUrlByUniversity[universityNameKey];
                }
            }

        }

        private HtmlNodeCollection selectProfessorListingNodes(HtmlNode rootNode)
        {
            return rootNode.SelectNodes("//li[@class= 'listing PROFESSOR']");
        }

        private HtmlNode getParentNodeOfOverallQualityRating(HtmlNode rootNode)
        {
            var parentNode = rootNode.SelectSingleNode("//div[contains(.,'Overall Quality') and ./div[@class='grade']]");
            return parentNode;
        }

        private Dictionary<string, string> joinUniversityNamesAndProfessorUrlsToDict(List<string> universityNames, List<string> professorUrls)
        {
            var professorUrlsByUniversityName = new Dictionary<string, string> { };
            for (int i = 0; i < professorUrls.Count; i++)
            {
                professorUrlsByUniversityName.Add(universityNames[i], professorUrls[i]);
            }

            return professorUrlsByUniversityName;
        }

        private string getQueryUrlFromSearchName(string firstName, string lastName)
        {
            string baseUrl = "http://www.ratemyprofessors.com/search.jsp?query=";

            string queryUrl = baseUrl + firstName + "+" + lastName;
            return queryUrl;
        }
    }
}
