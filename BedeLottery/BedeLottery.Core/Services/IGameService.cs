using BedeLottery.Common.Models;

namespace BedeLottery.Core.Services;
public interface IGameService
{
	Player CreateConsolePlayer();

	IEnumerable<Player> GenerateCpuPlayers();

	void PurchaseTickets(int playerId, int ticketsCount);

	DrawResult DrawTickets();

	decimal GetHouseRevenue();

	decimal GetTicketPrice();

}
