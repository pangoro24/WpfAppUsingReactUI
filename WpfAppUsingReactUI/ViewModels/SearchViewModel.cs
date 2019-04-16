using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfAppUsingReactUI.Services;

namespace WpfAppUsingReactUI
{
    public class SearchViewModel : ReactiveObject
    {
        private readonly DatabaseService SearchService;

        [Reactive]
        public string SearchQuery { get; set; }//property you want to notify of changes
        [Reactive]
        public bool IsSearching { get; set; }

        // public ReactiveList<string> Search { [ObservableAsProperty]get; }
        //public ReactiveList<string> SearchResults { get; set; }
        private readonly SourceList<string> SearchResults;
        public IObservableCollection<string> ResultsListBindable { get; }

        public ReactiveCommand<Unit, Unit> ClickCommand { get; private set; }

        private IObservable<Unit> ActionAsync() => Observable
            .Return(new Random().Next(0, 2) == 1)
            .Delay(TimeSpan.FromSeconds(1))
            .Do(
                success =>
                {
                    if (!success)
                    {
                        Debug.WriteLine("Failed to login");
                        MessageBox.Show("Failed to login");
                    }
                }
            )
            .Select(_ => Unit.Default);

        public SearchViewModel()
        {
            SearchService = new DatabaseService();
            // SearchResults = new ReactiveList<string>();
            SearchResults = new SourceList<string>();
            ResultsListBindable = new ObservableCollectionExtended<string>();
            this.SearchResults.Connect().Bind(this.ResultsListBindable).Subscribe();
            SearchQuery = "audio";

            //User update
            var canClick = this.WhenAnyValue(x => x.SearchQuery, query => !string.IsNullOrWhiteSpace(query));
            ClickCommand = ReactiveCommand.CreateFromObservable(this.ActionAsync, canClick);// Set the command source and its conditions


            //Auto update 
            this.WhenAnyValue(x => x.SearchQuery)
                .Do(x =>
                {
                    if (string.IsNullOrEmpty(x))
                    {
                        //clean list when text from searchbar is deleted
                        SearchResults.Clear();
                        IsSearching = false;
                    }
                })
                .Where(x => !string.IsNullOrEmpty(x))
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
                .Subscribe(async searchTerm =>
                {
                    SearchResults.Clear();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        IsSearching = true;

                        Debug.WriteLine($"Searching for: {searchTerm}");
                        var results = await SearchService.FetchArtists(searchTerm);
                        if (results?.Item1 == SearchQuery)
                        {
                            //there is no method in sqlite pcl to fetch strings so I have to loop here
                            List<string> resultsString = new List<string>();
                            for (int i = 0; i < results.Item2.Count; i++)
                            {
                                resultsString.Add(results.Item2[i].Name);

                            }
                            SearchResults.AddRange(resultsString);
                            IsSearching = false;
                        }
                    }
                    IsSearching = false;
                });
        }
    }
}
