/**
 * @file
 * @author Vicheka Phor, Yonsei Univ. Researcher, since 2020.10
 * @date 2020.11.02
 */
using FeederNetInspector.Classes;
using System.Collections.Generic;
using System.ComponentModel;

namespace FeederNetInspcetor.Model
{
    public class ResponseSessionModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ResponseSessionModel(List<Response> responseList)
        {
            this.responseList = responseList;
        }


        #region index
        private List<Response> responseList = new List<Response>();
        public List<Response> ResponseList
        {
            get
            {
                if (responseList == null) responseList = new List<Response>();
                return responseList;
            }

            set
            {
                responseList = value;
                NotifyPropertyChanged("ResponseList");
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
