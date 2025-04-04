using BedeLottery.Core.Repositories;
using NUnit.Framework;

namespace BedeLottery.Tests.Core.Repositories
{
	[TestFixture]
	public class PlayerRepositoryTests
	{
		private PlayerRepository _repo;

		[SetUp]
		public void Setup()
		{
			_repo = new PlayerRepository();
		}

		[Test]
		public void AddPlayer_ShouldReturnTrue_WhenNewPlayerAdded()
		{
			var result = _repo.AddPlayer(1, 10, 5);
			Assert.That(result, Is.True);
		}

		[Test]
		public void GetPlayer_ShouldReturnCorrectPlayer()
		{
			_repo.AddPlayer(2, 10, 3);
			var player = _repo.GetPlayer(2);

			Assert.That(player, Is.Not.Null);
			Assert.That(player.Id, Is.EqualTo(2));
			Assert.That(player.Balance, Is.EqualTo(10));
			Assert.That(player.TicketsCount, Is.EqualTo(3));
		}

		[Test]
		public void UpdatePlayer_ShouldReturnFalse_IfPlayerDoesNotExist()
		{
			var result = _repo.UpdatePlayer(99, 5, 1);
			Assert.That(result, Is.False);
		}

		[Test]
		public void UpdatePlayer_ShouldUpdatePlayer_WhenExists()
		{
			_repo.AddPlayer(3, 10, 2);
			var result = _repo.UpdatePlayer(3, 6, 4);
			var player = _repo.GetPlayer(3);

			Assert.That(result, Is.Not.Null);
			Assert.That(player.Balance, Is.EqualTo(6));
			Assert.That(player.TicketsCount, Is.EqualTo(4));
		}
	}
}
