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
        private string _targetProfessorUrl;
        public Dictionary<string, string> ProfessorListingUrlByUniversity = new Dictionary<string, string>();
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

        public double GetOverallQualityRating()
        {
            string firstProfessorUrlExtension = _targetProfessorUrl;
            string totalProfessorListingUrl = _baseUrl + firstProfessorUrlExtension;

            var rootNode = Utilities.getRootNodeOfHtmlDocument(totalProfessorListingUrl);
            var parentNode = getParentNodeOfOverallQualityRating(rootNode);

            var overallRating = parentNode.ChildNodes[1].InnerHtml;

            return Double.Parse(overallRating);
        }

        private void setStateOfProfessorUrls(string firstName, string lastName)
        {
            var listingHandler = new ProfessorListingHandler(firstName, lastName);

            this.ProfessorUrls = listingHandler.ProfessorUrls;
            this.ProfessorUniversities = listingHandler.ProfessorUniversities;

            this.ProfessorListingUrlByUniversity = this.joinUniversityNamesAndProfessorUrlsToDict(this.ProfessorUniversities, this.ProfessorUrls);
        }

        private void handleMultipleProfessors()
        {
            if (this.ProfessorUrls.Count == 1)
            {
                _targetProfessorUrl = this.ProfessorUrls[0];
            }
            else if (this.ProfessorUrls.Count > 1 && _targetProfessorUrl == null)
            {
                throw new Exception("Multiple professors found; narrow your search by entering the university name into the constructor");
            }
        }


        private void filterProfessorsByUniversity(string universityNameFilter)
        {
            string universityNameKey;
            foreach(string universityName in this.ProfessorUniversities)
            {
                if (universityName.Contains(universityNameFilter))
                {
                    universityNameKey = universityName;
                    _targetProfessorUrl = this.ProfessorListingUrlByUniversity[universityNameKey];
                }
            }

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
    }
}
