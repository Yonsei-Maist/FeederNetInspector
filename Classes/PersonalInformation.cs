using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeederNetInspector.Classes
{
    public class PersonalInformation
    {
        public PersonalInformation(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        private string key;
        public string Key
        {
            get { return key; }
            set
            {
                if (key != value)
                {
                    key = value;
                }
            }
        }

        private string value;
        public string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                }
            }
        }
    }
}
