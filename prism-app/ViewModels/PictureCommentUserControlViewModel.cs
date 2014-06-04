using Microsoft.Practices.Prism.StoreApps;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public class PictureCommentUserControlViewModel : LoadableViewModel
    {
        private IncrementalLoadingCollection<PictureCommentViewModel> comments;

        public IncrementalLoadingCollection<PictureCommentViewModel> Comments
        {
            get { return comments; }
            private set { SetProperty(ref comments, value); }
        }

        public void LoadComments(Uri pictureUri)
        {
            Comments = new IncrementalLoadingCollection<PictureCommentViewModel>((i, s) => LoadMoreComments(pictureUri, i, s));
            Comments.CollectionChanged += Comments_CollectionChanged;
        }

        void Comments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string json = SerializeToJson();
            Debug.WriteLine(GetType().Name);
            Debug.WriteLine(json);
            Debug.WriteLine("----------------------------------");
        }

        private async Task<IEnumerable<PictureCommentViewModel>> LoadMoreComments(Uri pictureUri, int pageIndex, int pageSize)
        {
            try
            {
                var items = await ExecuteAsync(new ListCommentsRequest { PictureUri = pictureUri, PageNumber = pageIndex, PageSize = pageSize });
                var baseProfileUri = ResourceDictionary["BaseThumbnailUrl"] as string;
                var profileUriPattern = ResourceDictionary["ProfilePictureUriPattern"] as string;
                var result = new List<PictureCommentViewModel>(items.Content.Count());
                foreach (var item in items.Content)
                {
                    result.Add(new PictureCommentViewModel(baseProfileUri + profileUriPattern, item));
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