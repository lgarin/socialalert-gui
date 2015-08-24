using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public class PictureCommentUserControlViewModel : LoadableViewModel
    {
        private bool likeChecked;
        private bool dislikeChecked;
        private PictureViewModel picture;
        private ReportContentUserControlViewModel reportPictureContent;
        private ConfirmActionUserControlViewModel confirmPictureRepost;


        [Dependency]
        public IApplicationStateService ApplicationStateService { get; set; }

        [Dependency]
        public IMasterDataService MasterDataService { get; set; }

        [Dependency]
        public CommentListUserControlViewModel CommentList { get; set; }

        public PictureCommentUserControlViewModel()
        {
            LikeCommand = new DelegateCommand(() => SetPictureApproval(UserApprovalModifier.LIKE), () => CanSetPictureApproval(UserApprovalModifier.LIKE));
            DislikeCommand = new DelegateCommand(() => SetPictureApproval(UserApprovalModifier.DISLIKE), () => CanSetPictureApproval(UserApprovalModifier.DISLIKE));
            RepostPictureCommand = new DelegateCommand(RepostPicture, CanRepostPicture);
            ReportPictureCommand = new DelegateCommand<ReportContentData>(DoReportPicture, CanReportPicture);
        }

        public DelegateCommand<ReportContentData> ReportPictureCommand { get; private set; }

        public DelegateCommand<Guid> InitConfirmCommentRepostCommand { get; private set; }

        public DelegateCommand<Guid> InitReportCommentContentCommand { get; private set; }

        private bool CanReportPicture(ReportContentData data)
        {
            return picture != null && ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private async void DoReportPicture(ReportContentData data)
        {
            ApplicationStateService.LastCountry = data.Country;
            await ExecuteAsync(new ReportAbusiveMedia() { MediaId = picture.PictureUri, Country = data.Country, Reason = data.Reason });
        }

        public ReportContentUserControlViewModel ReportPictureContent
        {
            get
            {
                return reportPictureContent;
            }
            set
            {
                SetProperty(ref reportPictureContent, value);
            }
        }
        
        public ConfirmActionUserControlViewModel ConfirmPictureRepost
        {
            get { return confirmPictureRepost; }
            set
            {
                SetProperty(ref confirmPictureRepost, value);
            }
        }

        public DelegateCommand RepostPictureCommand { get; private set; }

        private bool CanRepostPicture()
        {
            return picture != null && ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private async void RepostPicture()
        {
            await ExecuteAsync(new RepostMediaRequest() { MediaUri = picture.PictureUri });
        }

        public bool IsLikeChecked
        {
            get
            {
                return likeChecked;
            }
            set
            {
                SetProperty(ref likeChecked, value);
            }
        }

        public DelegateCommand LikeCommand { get; private set; }
        public DelegateCommand DislikeCommand { get; private set; }

        public bool IsDislikeChecked
        {
            get
            {
                return dislikeChecked;
            }
            set
            {
                SetProperty(ref dislikeChecked, value);
            }
        }

       
        public void Load(PictureViewModel picture)
        {
            this.picture = picture;
            CommentList.Load(picture?.PictureUri);
            RefreshState();
            ApplicationStateService.PropertyChanged += (s, e) => RefreshState();
        }

        private void RefreshState()
        {
            ReportPictureContent = new ReportContentUserControlViewModel(MasterDataService, ReportPictureCommand);
            ConfirmPictureRepost = new ConfirmActionUserControlViewModel(ResourceLoader.GetString("confirmRepostPicture"), RepostPictureCommand);

            IsLikeChecked = picture.UserApprovalModifier == UserApprovalModifier.LIKE;
            IsDislikeChecked = picture.UserApprovalModifier == UserApprovalModifier.DISLIKE;
            
            LikeCommand.RaiseCanExecuteChanged();
            DislikeCommand.RaiseCanExecuteChanged();

            RepostPictureCommand.RaiseCanExecuteChanged();
            ReportPictureCommand.RaiseCanExecuteChanged();
        }

        private bool CanSetPictureApproval(UserApprovalModifier modifier)
        {
            return picture != null && ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private async void SetPictureApproval(UserApprovalModifier? modifier)
        {
            if (modifier == picture.UserApprovalModifier)
            {
                modifier = null;
            }
            try
            {
                var info = await ExecuteAsync(new SetMediaApprovalRequest() { MediaUri = picture.PictureUri, Modifier = modifier });
                picture.UpdateWith(info);
                RefreshState();
            }
            catch (Exception)
            {
                // ignore
            }
            
        }
    }
}