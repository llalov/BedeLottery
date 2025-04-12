using BedeLottery.Client;
using BedeLottery.Client.Services;
using BedeLottery.Common.Configration;
using BedeLottery.Core.Repositories;
using BedeLottery.Core.Services;

var host = Host.CreateDefaultBuilder(args)
	.ConfigureAppConfiguration((context, config) =>
	{
		config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
	})
	.ConfigureServices((context, services) =>
	{
		var config = context.Configuration;

		services.Configure<Configuration>(config.GetSection("GameSettings"));

		// Register core services
		services.AddSingleton<IConsoleService, ConsoleService>();
		services.AddSingleton<IPlayerRepository, PlayerRepository>();
		services.AddSingleton<IPlayerService, PlayerService>();
		services.AddSingleton<IGameService, GameService>();
		services.AddSingleton<LotteryConsoleApp>();
	})
	.Build();

using var scope = host.Services.CreateScope();
var app = scope.ServiceProvider.GetRequiredService<LotteryConsoleApp>();
app.Run();
