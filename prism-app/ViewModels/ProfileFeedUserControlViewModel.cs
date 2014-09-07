using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class ProfileFeedUserControlViewModel : LoadableViewModel
    {
        private IncrementalLoadingCollection<ActivityViewModel> activities;

        public IncrementalLoadingCollection<ActivityViewModel> Activities
        {
            get { return activities; }
            private set { SetProperty(ref activities, value); }
        }

        public void Load(ProfileStatisticViewModel profile)
        {
            Activities = new IncrementalLoadingCollection<ActivityViewModel>((i, s) => LoadMoreActivities(profile.ProfileId, i, s));
        }

        private String FormatMessage(ActivityType activityType, String message)
        {
            return string.Format(CultureInfo.CurrentCulture,
                                             ResourceLoader.GetString("ActivityMessage_" + activityType),
                                             message);
        }

        private async Task<IEnumerable<ActivityViewModel>> LoadMoreActivities(Guid profileId, int pageIndex, int pageSize)
        {
            try
            {
                var basePictureUri = new Uri(ResourceDictionary["BaseThumbnailUrl"] as string, UriKind.Absolute);
                var items = await ExecuteAsync(new GetProfileActivity { ProfileId = profileId, PageNumber = pageIndex, PageSize = pageSize });
                var result = new List<ActivityViewModel>(items.Content.Count());
                foreach (var item in items.Content)
                {
                    result.Add(new ActivityViewModel(basePictureUri, item, FormatMessage(item.ActivityType, item.Message)));
                }
                return result;
            }
            catch (Exception)
            {
                return Enumerable.Empty<ActivityViewModel>();
            }
        }
    }
}
