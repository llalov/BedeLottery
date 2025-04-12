namespace BedeLottery.Client;

using BedeLottery.Client.Services;
using BedeLottery.Core.Services;
public class LotteryConsoleApp
{
	private readonly IGameService gameService;
    private readonly IConsoleService console;

    public LotteryConsoleApp(IGameService gameService, IConsoleService console)
	{
		this.gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        this.console = console ?? throw new ArgumentNullException(nameof(console));
    }

	public void Run()
	{
		var player = gameService.CreateConsolePlayer();

		console.WriteLine($"Welcome to the Bede Lottery, Player {player.Id}!");
		console.WriteLine();
		console.WriteLine($"* Your digital balance: ${player.Balance:F2}");
		console.WriteLine($"* Ticket Price: ${gameService.GetTicketPrice():F2} each");
		console.WriteLine();
		while (true)
		{
			console.WriteLine($"How many tickets do you want to buy, Player {player.Id}?");
			var input = console.ReadLine();

			if (int.TryParse(input, out var ticketsCount) && ticketsCount > 0)
			{
				gameService.PurchaseTickets(player.Id, ticketsCount);
				break;
			}
			console.WriteLine("Invalid input. Please enter a valid positive number.");
		}
		console.WriteLine();
		var cpuPlayers = gameService.GenerateCpuPlayers();
		console.WriteLine($"{cpuPlayers.Count()} other CPU players also have purchased tickets.");
		console.WriteLine();
		console.WriteLine("Ticket Draw Results:");
		console.WriteLine();
		var drawResults = gameService.DrawTickets();
		console.WriteLine($"* Grand Prize: Player {drawResults.GrandPrizeWinner.Id} wins ${drawResults.GrandPrizeAmount:F2}!");
		var secondTierIds = string.Join(", ", drawResults.SecondTierWinners.Select(p => p.Id));
		console.WriteLine($"* Second Tier: Players {secondTierIds} win ${drawResults.SecondTierPrizeAmount:F2} each!");
		var thirdTierIds = string.Join(", ", drawResults.ThirdTierWinners.Select(p => p.Id));
		console.WriteLine($"* Third Tier: Players {thirdTierIds} win ${drawResults.ThirdTierPrizeAmount:F2} each!");
		console.WriteLine();
		console.WriteLine("Congratulations to the winners!");
		console.WriteLine();
		var houseRevenue = gameService.GetHouseRevenue();
		console.WriteLine($"House Revenue: ${houseRevenue:F2}");
		console.ReadLine();
	}
}
