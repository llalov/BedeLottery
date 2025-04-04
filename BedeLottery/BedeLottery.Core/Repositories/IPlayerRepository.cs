using BedeLottery.Common.Models;

namespace BedeLottery.Core.Repositories;
public interface IPlayerRepository
{
	bool AddPlayer(int playerId, decimal balance, int ticketsCount);

	Player GetPlayer(int playerId);

	bool UpdatePlayer(int playerId, decimal balance, int ticketsCount);

	List<Player> GetAllPlayers();
}
