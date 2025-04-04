using BedeLottery.Common.Configration;
using BedeLottery.Common.Models;
using BedeLottery.Core.Repositories;
using Microsoft.Extensions.Options;

namespace BedeLottery.Core.Services;

public class PlayerService : IPlayerService
{
	private readonly Configuration configuration;
	private readonly IPlayerRepository playerRepository;
	private int nextPlayerId = 1;

	public PlayerService(IOptions<Configuration> configuration, IPlayerRepository playerRepository)
	{
		this.configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
		this.playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
	}

	public IEnumerable<Player> GetAllPlayers()
		=> playerRepository.GetAllPlayers();

	public Player CreateConsolePlayer()
	{
		if (nextPlayerId != 1)
			throw new InvalidOperationException("CreateConsolePlayer must be called before generating other players.");

		var userId = nextPlayerId;
		nextPlayerId++;

		var player = new Player
		{
			Id = userId,
			Balance = configuration.StartBalanceMoney,
			TicketsCount = 0
		};

		var isCreated = playerRepository.AddPlayer(player.Id, player.Balance, player.TicketsCount);
		//error log can be added if isCreated is not true;

		return isCreated ? player : new Player();
	}

	public IEnumerable<Player> GenerateCpuPlayers()
	{
		var random = new Random();
		var totalPlayers = random.Next(configuration.PlayersMinCount, configuration.PlayersMaxCount + 1);
		var cpuPlayers = new List<Player>();

		for (var i = 0; i < totalPlayers - 1; i++)
		{
			var player = new Player
			{
				Id = nextPlayerId++,
				Balance = configuration.StartBalanceMoney
			};

			var ticketsToBuy = random.Next(1, configuration.MaxTicketsPerPlayer + 1);
			var affordableTickets = Math.Min(ticketsToBuy, (int)(player.Balance / configuration.TicketPrice));
			player.TicketsCount = affordableTickets;
			player.Balance -= affordableTickets * configuration.TicketPrice;

			playerRepository.AddPlayer(player.Id, player.Balance, player.TicketsCount);
			cpuPlayers.Add(player);
		}

		return cpuPlayers;
	}

	public void PurchaseTickets(int playerId, int ticketsCount)
	{
		var playerInfo = GetPlayerInfo(playerId);
		if (playerInfo == null)
		{
			throw new InvalidOperationException($"No information for Player with id {playerId}");
		}

		//calculate how many tickets the player can afford
		var maxAffordableTickets = (int)(playerInfo.Balance / configuration.TicketPrice);
		var actualTicketsToBuy = Math.Min(ticketsCount, maxAffordableTickets);

		if (actualTicketsToBuy == 0)
		{
			//log or notify that player couldn't afford any tickets
			return;
		}

		playerInfo.TicketsCount += actualTicketsToBuy;
		playerInfo.Balance -= actualTicketsToBuy * configuration.TicketPrice;

		playerRepository.UpdatePlayer(playerInfo.Id, playerInfo.Balance, playerInfo.TicketsCount);
	}

	private Player GetPlayerInfo(int playerId)
		=> playerRepository.GetPlayer(playerId);
}
