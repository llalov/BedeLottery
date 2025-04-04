using BedeLottery.Client;
using BedeLottery.Core.Services;
using Moq;
using NUnit.Framework;

namespace BedeLottery.Tests.Client
{
	[TestFixture]
	public class LotteryConsoleAppTests
	{
		private Mock<IGameService> _gameServiceMock;
		private LotteryConsoleApp _app;

		[SetUp]
		public void Setup()
		{
			_gameServiceMock = new Mock<IGameService>();
			_app = new LotteryConsoleApp(_gameServiceMock.Object);
		}

		[Test]
		public void Constructor_ShouldThrowException_WhenGameServiceIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new LotteryConsoleApp(null));
		}
	}
}