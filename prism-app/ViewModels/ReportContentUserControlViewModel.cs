using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Socialalert.Services;
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
    public class ReportContentUserControlViewModel : SimpleViewModel
    {
        private bool isFlyoutClosed;

        private IMasterDataService masterData;
        private ReportDelegate reportDelegate;

        public delegate void ReportDelegate(string reason, string country);

        public ReportContentUserControlViewModel(IMasterDataService masterData, ReportDelegate reportDelegate)
        {
            this.masterData = masterData;
            this.reportDelegate = reportDelegate;
            PropertyChanged += (s, e) => ReportCommand.RaiseCanExecuteChanged();
            CancelCommand = new DelegateCommand(DoCancel);
            ReportCommand = new DelegateCommand(DoReport, CanReport);
        }

        private void DoCancel()
        {
            IsFlyoutClosed = true;
            Reset();
        }

        private bool CanReport()
        {
            return SelectedCountry != null && SelectedReason != null;
        }

        private void DoReport()
        {
            reportDelegate(SelectedReason.Key, SelectedCountry.Key);
            DoCancel();
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
            }
        }

        private void Reset()
        {
            IsFlyoutClosed = false;
            SelectedReason = null;
            ReportCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand ReportCommand { get; private set; }

        public List<TextItem> CountryList { get { return masterData.CountryList; } }
        public List<TextItem> ReasonList { get { return masterData.AbuseReasonList; } }

        [Required]
        public TextItem SelectedCountry { get { return Get<TextItem>(); } set { Set<TextItem>(value); } }

        [Required]
        public TextItem SelectedReason { get { return Get<TextItem>(); } set { Set<TextItem>(value); } }
    }
}

