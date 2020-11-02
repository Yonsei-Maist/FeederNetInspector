using System.ComponentModel;

namespace FeederNetInspcetor.Model
{
    public class ResponseSessionModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region index
        private string reponse;
        public string Response
        {
            get { return reponse; }
            set
            {
                if (reponse != value)
                {
                    reponse = value;
                    NotifyPropertyChanged("ResponseBody");
                }
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
