using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    [JsonObject]
    public sealed class AppState : SimpleModel
    {
        public string DefaultUsername
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        private Lazy<JsonSerializer> Serializer
        {
            get { return new Lazy<JsonSerializer>(JsonSerializer.CreateDefault); }
        }

        public string Serialize()
        {
            using (StringWriter writer = new StringWriter())
            {
                Serializer.Value.Serialize(writer, this, typeof(AppState));
                return writer.ToString();
            }
        }

        public bool Populate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            StringReader reader = new StringReader(value);
            try
            {
                Serializer.Value.Populate(reader, this);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
