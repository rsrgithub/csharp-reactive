using System.Collections.Generic;
using Newtonsoft.Json;

namespace GitSearch.Models
{
    public class GitHubUser
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }        
    }

    public class SearchResult
    {
        [JsonProperty(PropertyName = "incomplete_results")]
        public bool IncompleteResults { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<GitHubUser> FoundUsers { get; set; }

        [JsonProperty(PropertyName = "total_count")]
        public string TotalCount { get; set; }       
    }
}
