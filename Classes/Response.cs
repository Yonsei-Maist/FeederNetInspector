/**
 * @file
 * @author Vicheka Phor, Yonsei Univ. Researcher, since 2020.10
 * @date 2020.11.02
 */
using System.Collections.Generic;

namespace FeederNetInspector.Classes
{
    public class Response
    {
        public Response(string responseHeaders, string responseBody, List<PersonalInformation> personalInformationList)
        {
            this.responseHeaders = responseHeaders;
            this.responseBody = responseBody;
            this.personalInformationList = personalInformationList;
        }

        private string responseHeaders;
        public string ResponseHeaders
        {
            get { return responseHeaders; }
            set
            {
                if (responseHeaders != value)
                {
                    responseHeaders = value;
                }
            }
        }

        private string responseBody;
        public string ResponseBody
        {
            get { return responseBody; }
            set
            {
                if (responseBody != value)
                {
                    responseBody = value;
                }
            }
        }

        private List<PersonalInformation> personalInformationList = new List<PersonalInformation>();
        public List<PersonalInformation> PersonalInformationList
        {
            get
            {
                if (personalInformationList == null) personalInformationList = new List<PersonalInformation>();
                return personalInformationList;
            }

            set
            {
                personalInformationList = value;
            }
        }
    }
}
