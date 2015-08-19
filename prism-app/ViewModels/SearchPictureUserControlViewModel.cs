using Microsoft.Practices.Prism.StoreApps;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Socialalert.ViewModels
{
    public class SearchPictureUserControlViewModel : LoadableViewModel
    {
        public SearchPictureUserControlViewModel() 
        {
            SearchSuggestionsCommand = new DelegateCommand<SearchBoxSuggestionsRequestedEventArgs>(SearchSuggestion);
            SearchCommand = new DelegateCommand<SearchBoxQuerySubmittedEventArgs>(Search);
        }

        public Action<string> SearchAction { get; set; }

        private async void SearchSuggestion(SearchBoxSuggestionsRequestedEventArgs args)
        {
            if (args.QueryText.Trim().Count() < 3)
            {
                return;
            }
            var deferral = args.Request.GetDeferral();
            try
            {
                var result = await ExecuteAsync(new FindKeywordSuggestionsRequest(args.QueryText));
                args.Request.SearchSuggestionCollection.AppendQuerySuggestions(result);
            }
            catch (Exception)
            {
                args.Request.SearchSuggestionCollection.AppendQuerySuggestion(args.QueryText.Trim());
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void Search(SearchBoxQuerySubmittedEventArgs args)
        {
            var query = args.QueryText.Trim();
            if (query.Length == 0)
            {
                query = null;
            }
            if (SearchAction != null)
            {
                SearchAction(query);
            }
        }

        public DelegateCommand<SearchBoxSuggestionsRequestedEventArgs> SearchSuggestionsCommand { get; private set; }
        public DelegateCommand<SearchBoxQuerySubmittedEventArgs> SearchCommand { get; private set; }
    }
}
