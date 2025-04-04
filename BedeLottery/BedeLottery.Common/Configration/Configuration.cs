namespace BedeLottery.Common.Configration;
public class Configuration
{
	public int PlayersMinCount { get; set; }

	public int PlayersMaxCount { get; set; }

	public int MaxTicketsPerPlayer { get; set; }

	public decimal StartBalanceMoney { get; set; }

	public int TicketPrice { get; set; }
}