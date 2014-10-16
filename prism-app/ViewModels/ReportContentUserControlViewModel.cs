using Microsoft.Practices.Prism.Commands;
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
    public class ReportContentData
    {
        public string Reason;
        public string Country;

        public ReportContentData(string reason, string country)
        {
            Reason = reason;
            Country = country;
        }
    }

    public class ReportContentUserControlViewModel : FlyoutViewModel
    {
        private readonly IMasterDataService masterData;
        private readonly DelegateCommand<ReportContentData> command;


        public ReportContentUserControlViewModel(IMasterDataService masterData, DelegateCommand<ReportContentData> command)
        {
            this.masterData = masterData;
            this.command = command;
            ReportCommand = new DelegateCommand(DoReport, CanReport);
            ShowCommand = new DelegateCommand(Reset, CanShow);
            command.CanExecuteChanged += (s, e) => ShowCommand.RaiseCanExecuteChanged();
            PropertyChanged += (s, e) => ReportCommand.RaiseCanExecuteChanged();
        }

        private bool CanReport()
        {
            return SelectedCountry != null && SelectedReason != null;
        }

        private ReportContentData CreateReportContentData()
        {
            return new ReportContentData(SelectedReason != null ? SelectedReason.Key : null, SelectedCountry != null ? SelectedCountry.Key : null);
        }

        private bool CanShow()
        {
            return command.CanExecute(CreateReportContentData());
        }

        private void DoReport()
        {
            Hide();
            command.Execute(CreateReportContentData());
        }

        public override void Reset()
        {
            SelectedReason = null;
            SelectedCountry = masterData.LastCountry;
        }

        public DelegateCommand ShowCommand { get; private set; }

        public DelegateCommand ReportCommand { get; private set; }

        public List<TextItem> CountryList { get { return masterData.CountryList; } }
        public List<TextItem> ReasonList { get { return masterData.AbuseReasonList; } }

        [Required]
        public TextItem SelectedCountry { get { return Get<TextItem>(); } set { Set<TextItem>(value); } }

        [Required]
        public TextItem SelectedReason { get { return Get<TextItem>(); } set { Set<TextItem>(value); } }
    }
}

