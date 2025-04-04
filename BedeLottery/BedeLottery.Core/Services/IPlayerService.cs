using BedeLottery.Common.Models;

namespace BedeLottery.Core.Services;
public interface IPlayerService
{
	Player CreateConsolePlayer();

	IEnumerable<Player> GenerateCpuPlayers();

	void PurchaseTickets(int playerId, int ticketsCount);

	IEnumerable<Player> GetAllPlayers();
}
