using BedeLottery.Client;
using BedeLottery.Client.Services;
using BedeLottery.Common.Configration;
using BedeLottery.Core.Repositories;
using BedeLottery.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace BedeLottery.Tests.HostTests
{
	[TestFixture]
	public class ProgramTests
	{
		private IHost _host;

		[SetUp]
		public void Setup()
		{
			var configDictionary = new Dictionary<string, string>
			{
				{ "GameSettings:PlayersMinCount", "10" },
				{ "GameSettings:PlayersMaxCount", "15" },
				{ "GameSettings:MaxTicketsPerPlayer", "10" },
				{ "GameSettings:StartBalanceMoney", "10" },
				{ "GameSettings:TicketPrice", "1" }
			};

			_host = Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((context, config) =>
				{
					config.AddInMemoryCollection(configDictionary);
				})
				.ConfigureServices((context, services) =>
				{
					services.Configure<Configuration>(context.Configuration.GetSection("GameSettings"));
					services.AddSingleton<IConsoleService, ConsoleService>();
					services.AddSingleton<IPlayerRepository, PlayerRepository>();
					services.AddSingleton<IPlayerService, PlayerService>();
					services.AddSingleton<IGameService, GameService>();
					services.AddSingleton<LotteryConsoleApp>();
				})
				.Build();
		}

		[Test]
		public void Host_ShouldBuildSuccessfully()
		{
			Assert.That(_host, Is.Not.Null);
		}

		[Test]
		public void ShouldResolve_LotteryConsoleApp()
		{
			using var scope = _host.Services.CreateScope();
			var app = scope.ServiceProvider.GetService<LotteryConsoleApp>();

			Assert.That(app, Is.Not.Null);
		}

		[Test]
		public void ShouldResolve_GameService_WithDependencies()
		{
			using var scope = _host.Services.CreateScope();
			var gameService = scope.ServiceProvider.GetService<IGameService>();

			Assert.That(gameService, Is.Not.Null);
			Assert.That(gameService.GetTicketPrice(), Is.EqualTo(1));
		}
	}
}
