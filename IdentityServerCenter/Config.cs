using IdentityServer4.Models;
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
				}
			};
		}
	}
}
