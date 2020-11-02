using System.Collections.Generic;

namespace FeederNetInspector.Classes
{
    public class Request
    {
        public Request(string requestHeaders, string requestBody, List<PersonalInformation> personalInformationList)
        {
            this.requestHeaders = requestHeaders;
            this.requestBody = requestBody;
            this.personalInformationList = personalInformationList;
        }

        private string requestHeaders;
        public string RequestHeaders
        {
            get { return requestHeaders; }
            set
            {
                if (requestHeaders != value)
                {
                    requestHeaders = value;
                }
            }
        }

        private string requestBody;
        public string RequestBody
        {
            get { return requestBody; }
            set
            {
                if (requestBody != value)
                {
                    requestBody = value;
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
