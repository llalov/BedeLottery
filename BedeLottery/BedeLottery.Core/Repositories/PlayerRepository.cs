using BedeLottery.Common.Models;
using System.Collections.Concurrent;

namespace BedeLottery.Core.Repositories;
public class PlayerRepository : IPlayerRepository
{
	private readonly ConcurrentDictionary<int, Player> playersRepository = new();

	public List<Player> GetAllPlayers()
	{
		return playersRepository.Values.ToList();
	}

	public Player GetPlayer(int playerId)
	{
		if (playersRepository.TryGetValue(playerId, out var result))
		{
			return result;
		}

		return null;
	}

	public bool AddPlayer(int playerId, decimal balance, int ticketsCount)
	{
		var player = new Player()
		{
			Id = playerId,
			Balance = balance,
			TicketsCount = ticketsCount
		};

		return playersRepository.TryAdd(playerId, player);
	}

	public bool UpdatePlayer(int playerId, decimal balance, int ticketsCount)
	{
		if (!playersRepository.TryGetValue(playerId, out var existingPlayer))
			return false;

		existingPlayer.Balance = balance;
		existingPlayer.TicketsCount = ticketsCount;

		return true;
	}
}
