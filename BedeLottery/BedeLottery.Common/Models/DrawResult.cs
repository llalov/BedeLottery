namespace BedeLottery.Common.Models;
public class DrawResult
{
	public Player GrandPrizeWinner { get; set; }

	public decimal GrandPrizeAmount { get; set; }

	public IEnumerable<Player> SecondTierWinners { get; set; }

	public decimal SecondTierPrizeAmount { get; set; }

	public IEnumerable<Player> ThirdTierWinners { get; set; }

	public decimal ThirdTierPrizeAmount { get; set; }
}
