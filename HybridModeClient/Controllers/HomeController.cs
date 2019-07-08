using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HybridModeClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;

namespace HybridModeClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 将清除本地cookie，然后重定向到IdentityServer。IdentityServer将清除其cookie，然后为用户提供返回MVC应用程序的链接。
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public async Task<IActionResult> CallApi()
        {
            
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var oldIdToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

          
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("http://localhost:5001/api/values");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View();
        }

        public async Task<string> RefreshToken()

        {
            var discoveryResponse = await DiscoveryClient.GetAsync("http://localhost:5002");
            if (discoveryResponse.IsError)
            {
                throw new Exception(discoveryResponse.Error);
            }

            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "mvc", "secret");
            // This will request a new access_token and a new refresh token.
            var tokenResponse = await tokenClient.RequestRefreshTokenAsync(await HttpContext.GetTokenAsync("refresh_token"));

            if (tokenResponse.IsError)
            {
                // Handle error.
            }

            var oldIdToken = await HttpContext.GetTokenAsync("id_token");

            var tokens = new List<AuthenticationToken>
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = oldIdToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResponse.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResponse.RefreshToken
                }
            };
            
            var expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            tokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
            });

            // Sign in the user with a new refresh_token and new access_token.
            //var info = await HttpContext.Authentication.GetAuthenticateInfoAsync("Cookies");
            //info.Properties.StoreTokens(tokens);
            //await httpContext.Authentication.SignInAsync("Cookies", info.Principal, info.Properties);

            return tokenResponse.RefreshToken;

            var client = new HttpClient();

            var disco = client.GetDiscoveryDocumentAsync("http://localhost:5002").Result;
            
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
            }

            //var tokenResponse = client.RequestRefreshTokenAsync(new RefreshTokenRequest
            //{
            //    Address = disco.TokenEndpoint,
            //    GrantType = "hybrid",

            //    ClientId = "mvc",
            //    ClientSecret = "secret",

            //    //RefreshToken = refresh_token,

            //    Parameters =
            //    {
            //        { "refresh_token", refresh_token},
            //        { "scope", "api1" }
            //    }
            //}).Result;

            //var tokenResponse = client.RequestRefreshTokenAsync(new RefreshTokenRequest
            //{
            //    Address = disco.TokenEndpoint,
            //    GrantType = "refresh_token",

            //    ClientId = "mvc",
            //    ClientSecret = "secret",

            //    RefreshToken = HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken).Result
            //}).Result;

            //if (tokenResponse.IsError)
            //{
            //    Console.WriteLine(tokenResponse.Error);
            //}
            //else
            //{
            //    Console.WriteLine(tokenResponse.Json);
            //}

            //return tokenResponse.RefreshToken;

            //client.SetBearerToken(tokenResponse.RefreshToken);

            //var response = client.GetAsync("http://localhost:5001/api/values").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            //}

            

         

        
        }
    }
}
