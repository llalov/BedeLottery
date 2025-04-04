using BedeLottery.Common.Configration;
using BedeLottery.Common.Models;
using Microsoft.Extensions.Options;

namespace BedeLottery.Core.Services;
public class GameService : IGameService
{
	private readonly IPlayerService playerService;
	private readonly Configuration configuration;
	private decimal houseRevenue = 0;
	public GameService(IPlayerService playerService, IOptions<Configuration> configuration)
	{
		this.playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
		this.configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
	}

	public Player CreateConsolePlayer()
		=> playerService.CreateConsolePlayer();

	public IEnumerable<Player> GenerateCpuPlayers()
		=> playerService.GenerateCpuPlayers();

	public void PurchaseTickets(int playerId, int ticketsCount)
		=> playerService.PurchaseTickets(playerId, ticketsCount);

	public DrawResult DrawTickets()
	{
		var allPlayers = playerService.GetAllPlayers().ToList();
		var drawResult = new DrawResult();
		var ticketPool = new List<(int PlayerId, int TicketId)>();

		var ticketIdCounter = 0;
		foreach (var player in allPlayers)
		{
			for (var i = 0; i < player.TicketsCount; i++)
			{
				ticketPool.Add((player.Id, ticketIdCounter++));
			}
		}

		if (!ticketPool.Any())
			throw new InvalidOperationException("No tickets were purchased.");

		var totalTickets = ticketPool.Count;
		decimal totalRevenue = totalTickets * configuration.TicketPrice;
		var rng = new Random();

		//Shuffle all ticket entries
		var shuffledTickets = ticketPool.OrderBy(_ => rng.Next()).ToList();

		//Track used ticket IDs to avoid reusing them across prize tiers
		var usedTickets = new HashSet<int>();

		//Grand Prize (50%)
		var grandPrizeTicket = shuffledTickets[0];
		usedTickets.Add(grandPrizeTicket.TicketId);

		drawResult.GrandPrizeWinner = allPlayers.First(p => p.Id == grandPrizeTicket.PlayerId);
		drawResult.GrandPrizeAmount = Math.Floor(totalRevenue * 0.5m * 100) / 100;

		//Second Tier (30%)
		var secondTierWinnersCount = (int)Math.Round(totalTickets * 0.10);
		var secondTierTickets = shuffledTickets
			.Where(t => !usedTickets.Contains(t.TicketId))
			.Take(secondTierWinnersCount)
			.ToList();

		foreach (var ticket in secondTierTickets)
			usedTickets.Add(ticket.TicketId);

		var secondTierTotal = Math.Floor(totalRevenue * 0.3m * 100) / 100;
		var secondTierPrizePerTicket = secondTierTickets.Any()
			? Math.Floor((secondTierTotal / secondTierTickets.Count) * 100) / 100
			: 0;

		drawResult.SecondTierWinners = secondTierTickets
			.Select(t => allPlayers.First(p => p.Id == t.PlayerId))
			.Distinct()
			.ToList();

		drawResult.SecondTierPrizeAmount = secondTierPrizePerTicket;

		//Third Tier (10%)
		var thirdTierWinnersCount = (int)Math.Round(totalTickets * 0.20);
		var thirdTierTickets = shuffledTickets
			.Where(t => !usedTickets.Contains(t.TicketId))
			.Take(thirdTierWinnersCount)
			.ToList();

		foreach (var ticket in thirdTierTickets)
			usedTickets.Add(ticket.TicketId);

		var thirdTierTotal = Math.Floor(totalRevenue * 0.1m * 100) / 100;
		var thirdTierPrizePerTicket = thirdTierTickets.Any()
			? Math.Floor((thirdTierTotal / thirdTierTickets.Count) * 100) / 100
			: 0;

		drawResult.ThirdTierWinners = thirdTierTickets
			.Select(t => allPlayers.First(p => p.Id == t.PlayerId))
			.Distinct()
			.ToList();

		drawResult.ThirdTierPrizeAmount = thirdTierPrizePerTicket;

		//Distribute Prizes to Players (per ticket)
		drawResult.GrandPrizeWinner.Balance += drawResult.GrandPrizeAmount;

		foreach (var ticket in secondTierTickets)
		{
			var player = allPlayers.First(p => p.Id == ticket.PlayerId);
			player.Balance += secondTierPrizePerTicket;
		}

		foreach (var ticket in thirdTierTickets)
		{
			var player = allPlayers.First(p => p.Id == ticket.PlayerId);
			player.Balance += thirdTierPrizePerTicket;
		}

		//House Profit
		var paidOut = drawResult.GrandPrizeAmount +
					  (secondTierPrizePerTicket * secondTierTickets.Count) +
					  (thirdTierPrizePerTicket * thirdTierTickets.Count);

		houseRevenue = totalRevenue - paidOut;
		houseRevenue = Math.Floor(houseRevenue * 100) / 100;

		return drawResult;
	}

	public decimal GetHouseRevenue()
		=> houseRevenue;

	public decimal GetTicketPrice()
		=> configuration.TicketPrice;
}
