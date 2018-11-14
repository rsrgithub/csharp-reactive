using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using GitSearch.Models;
using RestSharp;

namespace GitSearch.API
{
    public class GitHubApi : RemoteApi
    {
        public IObservable<List<GitHubUser>> SearchGitHubUsers(string strToSearch)
        {           
            return Observable.Create<List<GitHubUser>>(observer =>
            {                                
                var request = new RestRequest { Resource = "search/users" };

                request.AddQueryParameter("q", strToSearch);
                var result = Execute<SearchResult>(request);                
                observer.OnNext(result.FoundUsers);
                return Disposable.Empty;
            });
            
        }

        public IObservable<List<GitHubUser>> SearchGitHubUsersAsync(string strToSearch)
        {
            return Observable.FromAsync(() =>
            {
                var request = new RestRequest { Resource = "search/users" };

                request.AddQueryParameter("q", strToSearch);
                var result = ExecuteAsync<SearchResult>(request);

                return result;
            }).Select(x => x.FoundUsers);

        }
    }
}
