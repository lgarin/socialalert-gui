﻿using Bravson.Socialalert.Portable.Data;
using Newtonsoft.Json;
using System.IO;

namespace Bravson.Socialalert.Portable.Model
{
    [JsonObject]
    public sealed class AppState : SimpleModel
    {
        private readonly JsonSerializer serializer;

        public AppState(string serializedState)
        {
            serializer = JsonSerializer.CreateDefault();
            Populate(serializedState);
        }

        [JsonIgnore]
        public UserInfo UserInfo
        {
            get { return Get<UserInfo>(); }
            set { Set(value); }
        }

        public string DefaultUsername
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public string Serialize()
        {
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, this, typeof(AppState));
                return writer.ToString();
            }
        }

        private bool Populate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            StringReader reader = new StringReader(value);
            try
            {
                serializer.Populate(reader, this);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
