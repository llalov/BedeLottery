namespace BedeLottery.Client;

using BedeLottery.Core.Services;
public class LotteryConsoleApp
{
	private readonly IGameService gameService;

	public LotteryConsoleApp(IGameService gameService)
	{
		this.gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
	}

	public void Run()
	{
		var player = gameService.CreateConsolePlayer();

		Console.WriteLine($"Welcome to the Bede Lottery, Player {player.Id}!");
		Console.WriteLine();
		Console.WriteLine($"* Your digital balance: ${player.Balance:F2}");
		Console.WriteLine($"* Ticket Price: ${gameService.GetTicketPrice():F2} each");
		Console.WriteLine();
		while (true)
		{
			Console.WriteLine($"How many tickets do you want to buy, Player {player.Id}?");
			var input = Console.ReadLine();

			if (int.TryParse(input, out var ticketsCount) && ticketsCount > 0)
			{
				gameService.PurchaseTickets(player.Id, ticketsCount);
				break;
			}
			Console.WriteLine("Invalid input. Please enter a valid positive number.");
		}
		Console.WriteLine();
		var cpuPlayers = gameService.GenerateCpuPlayers();
		Console.WriteLine($"{cpuPlayers.Count()} other CPU players also have purchased tickets.");
		Console.WriteLine();
		Console.WriteLine("Ticket Draw Results:");
		Console.WriteLine();
		var drawResults = gameService.DrawTickets();
		Console.WriteLine($"* Grand Prize: Player {drawResults.GrandPrizeWinner.Id} wins ${drawResults.GrandPrizeAmount:F2}!");
		var secondTierIds = string.Join(", ", drawResults.SecondTierWinners.Select(p => p.Id));
		Console.WriteLine($"* Second Tier: Players {secondTierIds} win ${drawResults.SecondTierPrizeAmount:F2} each!");
		var thirdTierIds = string.Join(", ", drawResults.ThirdTierWinners.Select(p => p.Id));
		Console.WriteLine($"* Third Tier: Players {thirdTierIds} win ${drawResults.ThirdTierPrizeAmount:F2} each!");
		Console.WriteLine();
		Console.WriteLine("Congratulations to the winners!");
		Console.WriteLine();
		var houseRevenue = gameService.GetHouseRevenue();
		Console.WriteLine($"House Revenue: ${houseRevenue:F2}");
		Console.ReadLine();
	}
}
