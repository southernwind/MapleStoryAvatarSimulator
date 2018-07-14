using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MapleStoryAvatarSimulator {
	public class Program {
		public static void Main(string[] args) {
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) {
			return WebHost.CreateDefaultBuilder(args)
				.UseUrls("http://*:54197")
				.UseStartup<Startup>()
				.Build();
		}
	}
}
