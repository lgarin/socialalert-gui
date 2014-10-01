using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;

namespace Socialalert.ViewModels
{
    public class ReportContentUserControlViewModel : LoadableViewModel
    {
        private bool isFlyoutClosed;

        private readonly List<TextItem> countryList = new List<TextItem>();
        private readonly List<TextItem> reasonList = new List<TextItem>();

        private TextItem selectedCountry;

        private TextItem selectedReason;

        public ReportContentUserControlViewModel(IResourceLoader resourceLoader)
        {
            CancelCommand = new DelegateCommand(() => IsFlyoutClosed = true);
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

        public bool IsFlyoutClosed
        {
            get
            {
                return isFlyoutClosed;
            }
            set
            {
                SetProperty(ref isFlyoutClosed, value);
                if (value)
                {
                    Reset();
                }
            }
        }

        private void Reset()
        {
            IsFlyoutClosed = false;
        }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand ReportCommand { get; private set; }

        public List<TextItem> CountryList { get { return countryList; } }
        public List<TextItem> ReasonList { get { return reasonList; } }

        public TextItem SelectedCountry { get { return selectedCountry; } set { SetProperty(ref selectedCountry, value); } }
        public TextItem SelectedReason { get { return selectedReason; } set { SetProperty(ref selectedReason, value); } }
    }
}

