
using IdentityModel.Client;
using System;
using System.Net.Http;

namespace ThirdPartyClient
{
    class Program
    {
        static void Main(string[] args)
        {

			var client = new HttpClient();

			var disco = client.GetDiscoveryDocumentAsync("http://localhost:5000").Result;
			// var disco = DiscoveryClient.GetAsync("http://localhost:5000").Result; //DiscoveryClient 已经过时

			if (disco.IsError)
			{
				Console.WriteLine(disco.Error);
			}

			//var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret"); //ToKenClient已经过时
			//var tokenResponse = tokenClient.RequestClientCredentialsAsync("api").Result;
			var tokenResponse = client.RequestTokenAsync(new TokenRequest
			{
				Address = disco.TokenEndpoint,
				GrantType = "client_credentials",

				ClientId = "client",
				ClientSecret = "secret",

				//Parameters =
				//{
				//	{ "custom_parameter", "custom value"},
				//	{ "scope", "api1" }
				//}
			}).Result;


			if (tokenResponse.IsError)
			{
				Console.WriteLine(tokenResponse.Error);
			}
			else
			{
				Console.WriteLine(tokenResponse.Json);
			}

			client.SetBearerToken(tokenResponse.AccessToken);

			var response = client.GetAsync("http://localhost:5001/api/values").Result;
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine(response.Content.ReadAsStringAsync().Result);
			}

            
        }
    }
}
