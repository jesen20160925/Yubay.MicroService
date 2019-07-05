using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace IdentityServerCenter
{
	/// <summary>
	/// Authentication Center认证中心
	/// Author:Jesen
	/// Date:2019-07-05
	/// </summary>
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).UseUrls("http://localhost:5000").Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
