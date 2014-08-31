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
        private PictureCommentViewModel newComment;
        private IncrementalLoadingCollection<PictureCommentViewModel> comments;

        [Dependency]
        public IApplicationStateService ApplicationStateService { get; set; }

        public PictureCommentUserControlViewModel()
        {
            LikeCommand = new DelegateCommand(() => SetPictureApproval(UserApprovalModifier.LIKE), () => CanSetPictureApproval(UserApprovalModifier.LIKE));
            DislikeCommand = new DelegateCommand(() => SetPictureApproval(UserApprovalModifier.DISLIKE), () => CanSetPictureApproval(UserApprovalModifier.DISLIKE));
            NewCommentCommand = new DelegateCommand(CreateComment, CanCreateComment);
            PostCommentCommand = new DelegateCommand(PostComment, CanPostComment);
            CancelCommentCommand = new DelegateCommand(CancelComment, CanCancelComment);
            RepostCommentCommand = new DelegateCommand<Guid?>(RepostComment, CanRepostComment);
            RepostPictureCommand = new DelegateCommand(RepostPicture, CanRepostPicture);
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
            return picture != null && ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private void CreateComment()
        {
            CommentInfo info = new CommentInfo()
            {
                ProfileId = ApplicationStateService.CurrentUser.ProfileId.Value,
                Creator = ApplicationStateService.CurrentUser.Nickname,
                Online = true,
                MediaUri = picture.PictureUri,
                Creation = DateTime.Now
            };
            NewComment = new PictureCommentViewModel(ProfileUriPattern, info, RepostCommentCommand);

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
                var comment = await ExecuteAsync(new AddCommentRequest() { Comment = NewComment.Comment, PictureUri = NewComment.MediaUri });
                Comments.Insert(0, new PictureCommentViewModel(ProfileUriPattern, comment, RepostCommentCommand));
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

        public DelegateCommand<Guid?> RepostCommentCommand { get; private set; }

        private bool CanRepostComment(Guid? commentId)
        {
            return commentId != null && ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private async void RepostComment(Guid? commentId)
        {
            await ExecuteAsync(new RepostCommentRequest() { CommentId = commentId.Value });
        }

        public DelegateCommand RepostPictureCommand { get; private set; }

        private bool CanRepostPicture()
        {
            return picture != null && ApplicationStateService.HasUserRole(UserRole.USER);
        }

        private async void RepostPicture()
        {
            await ExecuteAsync(new RepostPictureRequest() { PictureId = picture.PictureUri });
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

        public IncrementalLoadingCollection<PictureCommentViewModel> Comments
        {
            get { return comments; }
            private set { SetProperty(ref comments, value); }
        }

        public void Load(PictureViewModel picture)
        {
            this.picture = picture;
            Comments = new IncrementalLoadingCollection<PictureCommentViewModel>((i, s) => LoadMoreComments(picture.PictureUri, i, s));
            RefreshState();
        }

        private void RefreshState()
        {
            CancelComment();

            IsLikeChecked = picture.UserApprovalModifier == UserApprovalModifier.LIKE;
            IsDislikeChecked = picture.UserApprovalModifier == UserApprovalModifier.DISLIKE;
            
            LikeCommand.RaiseCanExecuteChanged();
            DislikeCommand.RaiseCanExecuteChanged();

            RepostPictureCommand.RaiseCanExecuteChanged();
            RepostCommentCommand.RaiseCanExecuteChanged();
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
                var info = await ExecuteAsync(new SetPictureApprovalRequest() { PictureUri = picture.PictureUri, Modifier = modifier });
                picture.UpdateWith(info);
                RefreshState();
            }
            catch (Exception)
            {
                // ignore
            }
            
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
                var items = await ExecuteAsync(new ListCommentsRequest { PictureUri = pictureUri, PageNumber = pageIndex, PageSize = pageSize });
                var result = new List<PictureCommentViewModel>(items.Content.Count());
                foreach (var item in items.Content)
                {
                    result.Add(new PictureCommentViewModel(ProfileUriPattern, item, RepostCommentCommand));
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