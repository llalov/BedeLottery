using BedeLottery.Common.Configration;
using BedeLottery.Core.Services;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace BedeLottery.Tests.Core.Services
{
	[TestFixture]
	public class GameServiceTests
	{
		private Mock<IPlayerService> _playerServiceMock;
		private GameService _gameService;
		private Configuration _config;

		[SetUp]
		public void Setup()
		{
			_playerServiceMock = new Mock<IPlayerService>();
			_config = new Configuration
			{
				TicketPrice = 1
			};
			var options = Options.Create(_config);

			_gameService = new GameService(_playerServiceMock.Object, options);
		}

		[Test]
		public void GetTicketPrice_ShouldReturnConfiguredPrice()
		{
			Assert.That(_gameService.GetTicketPrice(), Is.EqualTo(1));
		}

		[Test]
		public void GetHouseRevenue_ShouldReturnZero_Initially()
		{
			Assert.That(_gameService.GetHouseRevenue(), Is.EqualTo(0));
		}
	}
}