using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreMvcAuthSample
{
	public class Config
	{
		/// <summary>
		/// 给客户端的信息，客户端访问需要携带这些信息
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Client> GetClients()
		{
            return new List<Client>
            {
                new Client()
                {
                    ClientId = "mvc", //客户端Id
					ClientName = "Jesen Mvc Client",
                    Description = "This is Jesen Info",
                    ClientUri = "http://localhost:5001",
                    LogoUri = "http://localhost:5000/img/logo.jpg",
                    AllowRememberConsent = true,

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,// { GrantType.Implicit }, //授权类型 : 隐式模式
					ClientSecrets = { new Secret("secret".Sha256()) }, //密钥

					RequireConsent = true, //是不是同意授权的页面
					RedirectUris = { "http://localhost:5001/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc"},

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1"
						//IdentityServerConstants.StandardScopes.Email,
					} ,

                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true, //直接在Id Token中携带Claim
				}
			};
		}

		/// <summary>
		/// 指定Resource API 的名称，在ResourceApi的startup中需要配置哪个名称
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<ApiResource> GetResource()
		{
			return new List<ApiResource> {
				new ApiResource("api1","API Application")
			};
		}


		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource> {
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
				new IdentityResources.Email()
			};
		}

		public static List<TestUser> GetTestUsers()
		{
			return new List<TestUser> { new TestUser()
			{
				 SubjectId = "10000",
				 Username="jesen",
				 Password = "123456",
				 Claims = new List<Claim>{
					 new Claim("name","Lin"),
					 new Claim("website","www.yubay.cn")
				 }
			}
			};
		}
	}
}
