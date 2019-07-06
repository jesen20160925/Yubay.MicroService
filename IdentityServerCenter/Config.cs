using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace IdentityServerCenter
{
	/// <summary>
	/// Resource API和Client配置
	/// </summary>
	public class Config
	{
		/// <summary>
		/// 指定Resource API 的名称，在ResourceApi的startup中需要配置哪个名称
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<ApiResource> GetResource()
		{
			return new List<ApiResource> {
				new ApiResource("api","My api")
			};
		}

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
					ClientId = "client", //客户端Id
					AllowedGrantTypes = { GrantType.ClientCredentials }, //授权类型
					ClientSecrets = { new Secret("secret".Sha256()) }, //密钥
					AllowedScopes = { "api" } //指定客户端可以访问Resource为api的资源
				},

				new Client()
				{
					ClientId = "pwdClient", //客户端Id
					AllowedGrantTypes = { GrantType.ResourceOwnerPassword }, //授权类型
					ClientSecrets = { new Secret("secret".Sha256()) }, //密钥

					RequireClientSecret = false, //如果将RequireClientSecret设为false，则客户端访问不需要传递secret

					AllowedScopes = { "api" } //指定客户端可以访问Resource为api的资源
				}

			};
		}


		/// <summary>
		/// 密码模式需要用户
		/// </summary>
		/// <returns></returns>
		public static List<TestUser> GetTestUsers()
		{
			return new List<TestUser> { new TestUser()
			{
				 SubjectId = "1",
				  Username="jesen",
				   Password = "123456"
			}
			};
		}

	}
}
