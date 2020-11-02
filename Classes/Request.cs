namespace FeederNetInspector.Classes
{
    public class Request
    {
        public Request(string requestHeaders, string requestBody)
        {
            this.requestHeaders = requestHeaders;
            this.requestBody = requestBody;
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
    }
}
