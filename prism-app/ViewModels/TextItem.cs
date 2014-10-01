using Microsoft.Practices.Prism.StoreApps.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class TextItem : SimpleViewModel
    {
        public TextItem(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public TextItem(string key, IResourceLoader resourceLoader)
        {
            Key = key;
            Value = resourceLoader.GetString(key);
        }

        public string Key { get { return Get<string>(); } set { Set(value); } }
        public string Value { get { return Get<string>(); } set { Set(value); } }
    }
}
