using FeederNetInspector.Classes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace FeederNetInspcetor.Model
{
    public class RequestSessionModel : INotifyPropertyChanged
    {
        public RequestSessionModel(List<Request> requestList)
        {
            this.requestList = requestList;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region index
        private List<Request> requestList = new List<Request>();
        public List<Request> RequestList
        {
            get
            {
                if (requestList == null) requestList = new List<Request>();
                return requestList;
            }

            set
            {
                requestList = value;
                NotifyPropertyChanged("RequestList");
            }
        }
        #endregion

        #region UI
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
