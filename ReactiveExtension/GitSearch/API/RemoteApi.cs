using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace GitSearch.API
{
    public class RemoteApi
    {
        private const string BaseUrl = "https://api.github.com/";

        public async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            var client = new RestClient(BaseUrl)
            {
                Authenticator =
                    new OAuth2AuthorizationRequestHeaderAuthenticator("b9e826a936007fc1a636ade00ae16d13d5d2ab39")
            };

            var response = await client.ExecuteTaskAsync<T>(request);
            
            if (response.ErrorException != null)
            {
                throw new Exception("Error in retriving the response", response.ErrorException);
            }

            var data = JsonConvert.DeserializeObject<T>(response.Content);

            return data;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient(BaseUrl)
            {
                Authenticator =
                    new OAuth2AuthorizationRequestHeaderAuthenticator("b9e826a936007fc1a636ade00ae16d13d5d2ab39")
            };

            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                throw new Exception("Error in retriving the response", response.ErrorException);
            }


            var data = JsonConvert.DeserializeObject<T>(response.Content);

            return data;
        }
    }
}
