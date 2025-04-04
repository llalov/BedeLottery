using BedeLottery.Common.Configration;
using BedeLottery.Common.Models;
using BedeLottery.Core.Repositories;
using BedeLottery.Core.Services;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace BedeLottery.Tests.Core.Services
{
	[TestFixture]
	public class PlayerServiceTests
	{
		private Mock<IPlayerRepository> _repoMock;
		private Configuration _config;
		private PlayerService _service;

		[SetUp]
		public void Setup()
		{
			_repoMock = new Mock<IPlayerRepository>();
			_config = new Configuration
			{
				StartBalanceMoney = 10,
				PlayersMinCount = 10,
				PlayersMaxCount = 15,
				MaxTicketsPerPlayer = 10,
				TicketPrice = 1
			};

			_service = new PlayerService(Options.Create(_config), _repoMock.Object);
		}

		[Test]
		public void CreateConsolePlayer_ShouldCreatePlayerWithId1()
		{
			_repoMock.Setup(r => r.AddPlayer(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<int>())).Returns(true);

			var player = _service.CreateConsolePlayer();

			Assert.That(player.Id, Is.EqualTo(1));
			Assert.That(player.Balance, Is.EqualTo(10));
			Assert.That(player.TicketsCount, Is.EqualTo(0));
		}

		[Test]
		public void GenerateCpuPlayers_ShouldReturnPlayersWithinConfiguredRange()
		{
			_repoMock.Setup(r => r.AddPlayer(It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<int>())).Returns(true);

			// Force call to CreateConsolePlayer first to avoid ID collision
			_service.CreateConsolePlayer();

			var players = _service.GenerateCpuPlayers();
			var count = new List<Player>(players).Count;

			Assert.That(count, Is.GreaterThanOrEqualTo(9));
			Assert.That(count, Is.LessThanOrEqualTo(14));
		}

		[Test]
		public void PurchaseTickets_ShouldUpdatePlayer_WhenSufficientBalance()
		{
			var player = new Player { Id = 2, Balance = 10, TicketsCount = 0 };
			_repoMock.Setup(r => r.GetPlayer(2)).Returns(player);
			_repoMock.Setup(r => r.UpdatePlayer(2, It.IsAny<decimal>(), It.IsAny<int>())).Returns(true);

			_service.PurchaseTickets(2, 5);

			Assert.That(player.TicketsCount, Is.EqualTo(5));
			Assert.That(player.Balance, Is.EqualTo(5));
		}

		[Test]
		public void PurchaseTickets_ShouldNotPurchase_WhenInsufficientBalance()
		{
			var player = new Player { Id = 3, Balance = 0, TicketsCount = 0 };
			_repoMock.Setup(r => r.GetPlayer(3)).Returns(player);

			_service.PurchaseTickets(3, 5);

			Assert.That(player.TicketsCount, Is.EqualTo(0));
			Assert.That(player.Balance, Is.EqualTo(0));
		}

		[Test]
		public void PurchaseTickets_ShouldThrow_WhenPlayerNotFound()
		{
			_repoMock.Setup(r => r.GetPlayer(99)).Returns((Player)null);

			Assert.Throws<InvalidOperationException>(() => _service.PurchaseTickets(99, 1));
		}
	}
}
