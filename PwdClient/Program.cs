using IdentityModel.Client;
using System;
using System.Net.Http;

namespace PwdClient
{
	/// <summary>
	/// oauth 2.0密码模式 客户带
	/// </summary>
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
			//var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("jesen", "123456");

			var tokenResponse = client.RequestTokenAsync(new TokenRequest
			{
				Address = disco.TokenEndpoint,
				GrantType = "password",
				
				ClientId = "pwdClient",
				ClientSecret = "secret",

				Parameters =
				{
					{ "username","jesen" },
					{"password","123456" }
					//{ "custom_parameter", "custom value"},
					//{ "scope", "api1" }
				}
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
