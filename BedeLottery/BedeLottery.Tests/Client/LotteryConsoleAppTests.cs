using BedeLottery.Client;
using BedeLottery.Client.Services;
using BedeLottery.Common.Models;
using BedeLottery.Core.Services;
using Moq;
using NUnit.Framework;

namespace BedeLottery.Tests.Client
{
    [TestFixture]
    public class LotteryConsoleAppTests
    {
        private Mock<IGameService> _gameServiceMock;
        private Mock<IConsoleService> _consoleServiceMock;
        private LotteryConsoleApp _app;

        [SetUp]
        public void Setup()
        {
            _gameServiceMock = new Mock<IGameService>();
            _consoleServiceMock = new Mock<IConsoleService>();
            _app = new LotteryConsoleApp(_gameServiceMock.Object, _consoleServiceMock.Object);
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenGameServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new LotteryConsoleApp(null, _consoleServiceMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenConsoleServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new LotteryConsoleApp(_gameServiceMock.Object, null));
        }

        [Test]
        public void Run_ShouldExecuteGameFlow_WhenValidInputProvided()
        {
            // Arrange
            var player = new Player { Id = 1, Balance = 10m };
            var cpuPlayers = new List<Player> { new Player(), new Player() };
            var drawResult = new DrawResult
            {
                GrandPrizeWinner = player,
                GrandPrizeAmount = 5m,
                SecondTierWinners = new List<Player> { new Player { Id = 2 } },
                SecondTierPrizeAmount = 2m,
                ThirdTierWinners = new List<Player> { new Player { Id = 3 } },
                ThirdTierPrizeAmount = 1m
            };

            _gameServiceMock.Setup(g => g.CreateConsolePlayer()).Returns(player);
            _gameServiceMock.Setup(g => g.GetTicketPrice()).Returns(1m);
            _gameServiceMock.Setup(g => g.GenerateCpuPlayers()).Returns(cpuPlayers);
            _gameServiceMock.Setup(g => g.DrawTickets()).Returns(drawResult);
            _gameServiceMock.Setup(g => g.GetHouseRevenue()).Returns(2m);

            _consoleServiceMock.SetupSequence(c => c.ReadLine())
                .Returns("3")   // Ticket input
                .Returns("");   // Pause

            // Act
            _app.Run();

            // Assert
            _gameServiceMock.Verify(g => g.PurchaseTickets(player.Id, 3), Times.Once);
            _gameServiceMock.Verify(g => g.GenerateCpuPlayers(), Times.Once);
            _gameServiceMock.Verify(g => g.DrawTickets(), Times.Once);
            _gameServiceMock.Verify(g => g.GetHouseRevenue(), Times.Once);
            _consoleServiceMock.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("Welcome to the Bede Lottery"))), Times.Once);
        }
    }
}
