using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class CommentListUserControlViewModel : LoadableViewModel
    {
        private Uri mediaUri;
        private PictureCommentViewModel newComment;
        private ReportContentUserControlViewModel reportCommentContent;
        private ConfirmActionUserControlViewModel confirmCommentRepost;

        private IncrementalLoadingCollection<PictureCommentViewModel> comments;

        [Dependency]
        public IApplicationStateService ApplicationStateService { get; set; }

        [Dependency]
        public IMasterDataService MasterDataService { get; set; }

        public CommentListUserControlViewModel()
        {
            NewCommentCommand = new DelegateCommand(CreateComment, CanCreateComment);
            PostCommentCommand = new DelegateCommand(PostComment, CanPostComment);
            CancelCommentCommand = new DelegateCommand(CancelComment, CanCancelComment);
            InitConfirmCommentRepostCommand = new DelegateCommand<Guid>(InitConfirmCommentRepost, CanRepostComment);
            InitReportCommentContentCommand = new DelegateCommand<Guid>(InitReportCommentContent, CanReportComment);
        }

        public DelegateCommand<Guid> InitConfirmCommentRepostCommand { get; private set; }

        public DelegateCommand<Guid> InitReportCommentContentCommand { get; private set; }

        private void InitConfirmCommentRepost(Guid commentId)
        {
            var comment = Comments.SingleOrDefault(c => c.CommentId == commentId);
            if (comment != null)
            {
                ConfirmCommentRepost = new ConfirmActionUserControlViewModel(ResourceLoader.GetString("confirmRepostComment"), new DelegateCommand(() => RepostComment(commentId)));
            }
        }

        private void InitReportCommentContent(Guid commentId)
        {
            var comment = Comments.SingleOrDefault(c => c.CommentId == commentId);
            if (comment != null)
            {
                ReportCommentContent = new ReportContentUserControlViewModel(MasterDataService, new DelegateCommand<ReportContentData>((d) => DoReportComment(commentId, d)));
            }
        }

        private bool CanReportComment(Guid commentId)
        {
            return ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private async void DoReportComment(Guid commentId, ReportContentData data)
        {
            ApplicationStateService.LastCountry = data.Country;
            await ExecuteAsync(new ReportAbusiveComment() { CommentId = commentId, Country = data.Country, Reason = data.Reason });
        }


        public ReportContentUserControlViewModel ReportCommentContent
        {
            get
            {
                return reportCommentContent;
            }
            set
            {
                SetProperty(ref reportCommentContent, value);
            }
        }

        public PictureCommentViewModel NewComment
        {
            get
            {
                return newComment;
            }
            set
            {
                SetProperty(ref newComment, value);
            }
        }

        public DelegateCommand NewCommentCommand { get; private set; }

        private bool CanCreateComment()
        {
            return mediaUri != null && ApplicationStateService.HasUserRole(UserRole.USER);
        }
        
        private void CreateComment()
        {
            CommentInfo info = new CommentInfo()
            {
                ProfileId = ApplicationStateService.CurrentUser.ProfileId.Value,
                Creator = ApplicationStateService.CurrentUser.Nickname,
                Online = true,
                MediaUri = mediaUri,
                Creation = DateTime.Now
            };
            NewComment = new PictureCommentViewModel(ProfileUriPattern, info);

            NewCommentCommand.RaiseCanExecuteChanged();
            PostCommentCommand.RaiseCanExecuteChanged();
            CancelCommentCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand PostCommentCommand { get; private set; }

        private bool CanPostComment()
        {
            return NewComment != null && ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private async void PostComment()
        {
            if (!String.IsNullOrWhiteSpace(NewComment.Comment))
            {
                var comment = await ExecuteAsync(new AddCommentRequest() { Comment = NewComment.Comment, MediaUri = NewComment.MediaUri });
                Comments.Insert(0, new PictureCommentViewModel(ProfileUriPattern, comment));
                CancelComment();
            }
        }

        public DelegateCommand CancelCommentCommand { get; private set; }

        private bool CanCancelComment()
        {
            return NewComment != null;
        }

        private void CancelComment()
        {
            NewComment = null;
            NewCommentCommand.RaiseCanExecuteChanged();
            PostCommentCommand.RaiseCanExecuteChanged();
            CancelCommentCommand.RaiseCanExecuteChanged();
        }

        public ConfirmActionUserControlViewModel ConfirmCommentRepost
        {
            get { return confirmCommentRepost; }
            set
            {
                SetProperty(ref confirmCommentRepost, value);
            }
        }

        private bool CanRepostComment(Guid commentId)
        {
            return ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private async void RepostComment(Guid commentId)
        {
            await ExecuteAsync(new RepostCommentRequest() { CommentId = commentId });
        }

        public IncrementalLoadingCollection<PictureCommentViewModel> Comments
        {
            get { return comments; }
            private set { SetProperty(ref comments, value); }
        }

        public void Load(Uri mediaUri)
        {
            this.mediaUri = mediaUri;

            Comments = new IncrementalLoadingCollection<PictureCommentViewModel>((i, s) => LoadMoreComments(mediaUri, i, s));
            RefreshState();
            ApplicationStateService.PropertyChanged += (s, e) => RefreshState();
        }

        private void RefreshState()
        {
            CancelComment();

            ReportCommentContent = null;
            ConfirmCommentRepost = null;
        }

       
        private string ProfileUriPattern
        {
            get
            {
                var baseProfileUri = ResourceDictionary["BaseThumbnailUrl"] as string;
                var profileUriPattern = ResourceDictionary["ProfilePictureUriPattern"] as string;
                return baseProfileUri + profileUriPattern;
            }
        }

        private async Task<IEnumerable<PictureCommentViewModel>> LoadMoreComments(Uri pictureUri, int pageIndex, int pageSize)
        {
            try
            {
                var items = await ExecuteAsync(new ListCommentsRequest { MediaUri = pictureUri, PageNumber = pageIndex, PageSize = pageSize });
                var result = new List<PictureCommentViewModel>(items.Content.Count());
                foreach (var item in items.Content)
                {
                    result.Add(new PictureCommentViewModel(ProfileUriPattern, item));
                }
                return result;
            }
            catch (Exception)
            {
                return Enumerable.Empty<PictureCommentViewModel>();
            }
        }
    }
}
