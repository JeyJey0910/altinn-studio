﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AltinnCore.RepositoryClient.CustomApi
{
    public class UserApiClient
    {
        private string giteaCoookieId = "i_like_gitea";

        public UserApiClient()
        {
        }

        public async Task<AltinnCore.RepositoryClient.Model.User> GetCurrentUser(string giteaSession)
        {
            AltinnCore.RepositoryClient.Model.User user;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AltinnCore.RepositoryClient.Model.User));

            Uri giteaUrl = new Uri("http://altinn3.no:3000/api/v1/user");
            Cookie cookie = new Cookie(giteaCoookieId, giteaSession,"/","altinn3.no");
            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(cookie);
            HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            using (HttpClient client = new HttpClient(handler))
            {
                var streamTask = client.GetStreamAsync(giteaUrl);
                user = serializer.ReadObject(await streamTask) as AltinnCore.RepositoryClient.Model.User;
            }

            return user;
        }
    }
}
