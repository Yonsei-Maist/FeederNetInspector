namespace FeederNetInspector.Classes
{
    class Response
    {
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
    }
}
