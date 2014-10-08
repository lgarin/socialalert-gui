using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Socialalert.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Services
{
    public class TextItem
    {
        private readonly string key;
        private readonly IResourceLoader resourceLoader;

        public TextItem(string key, IResourceLoader resourceLoader)
        {
            this.key = key;
            this.resourceLoader = resourceLoader;
        }

        public string Key { get { return key; } }
        public string Value { get { return resourceLoader.GetString(Key); } }

        public override bool Equals(object obj)
        {
            return Equals(obj as TextItem);
        }

        public bool Equals(TextItem other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public static bool operator ==(TextItem a, TextItem b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            else if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }
            return a.Key == b.Key;
        }

        public static bool operator !=(TextItem a, TextItem b)
        {
            return !(a == b);
        }

    }

    public interface IMasterDataService
    {
        List<TextItem> CountryList { get; }
        List<TextItem> AbuseReasonList { get; }

        TextItem LastCountry { get; }
    }

    public sealed class MasterDataService : IMasterDataService
    {
        private readonly IApplicationStateService applicationStateService;
        private readonly IResourceLoader resourceLoader;
        private readonly List<TextItem> countryList = new List<TextItem>();
        private readonly List<TextItem> reasonList = new List<TextItem>();

        public MasterDataService(IResourceLoader resourceLoader, IApplicationStateService applicationStateService)
        {
            this.resourceLoader = resourceLoader;
            this.applicationStateService = applicationStateService;

            countryList.Add(new TextItem("CH", resourceLoader));
            countryList.Add(new TextItem("DE", resourceLoader));
            countryList.Add(new TextItem("FR", resourceLoader));
            countryList.Add(new TextItem("IT", resourceLoader));
            countryList.Add(new TextItem("ES", resourceLoader));
            countryList.Add(new TextItem("UK", resourceLoader));
            countryList.Add(new TextItem("US", resourceLoader));

            reasonList.Add(new TextItem("VIOLENCE", resourceLoader));
            reasonList.Add(new TextItem("SEX", resourceLoader));
            reasonList.Add(new TextItem("BAD_LANGUAGE", resourceLoader));
            reasonList.Add(new TextItem("DRUGS", resourceLoader));
            reasonList.Add(new TextItem("DISCRIMINATION", resourceLoader));
        }

        public List<TextItem> CountryList
        {
            get { return countryList; }
        }

        public List<TextItem> AbuseReasonList
        {
            get { return reasonList; }
        }

        public TextItem LastCountry
        {
            get
            {
                if (applicationStateService.LastCountry == null)
                {
                    return null;
                }
                return new TextItem(applicationStateService.LastCountry, resourceLoader);
            }
        }
    }
}
