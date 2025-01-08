
using Moq;
using ServiceLayer;
using DomainLayer;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestPlayers
{
    public class ServiceLayerTests
    {
        private readonly PlayerService _playerService;
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;

        public ServiceLayerTests()
        {
            // Mock the PlayerRepository
            _mockPlayerRepository = new Mock<IPlayerRepository>();

            // Initialize the PlayerService with the mocked PlayerRepository
            _playerService = new PlayerService(_mockPlayerRepository.Object);
        }


        [Fact]
        public async Task AddPlayerAsync_ShouldThrowException_WhenNoSkills()
        {
            // Arrange
            var player = new Player
            {
                Name = "John",
                Position = "forward",
                Skills = new List<Skill>() // No skills
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _playerService.AddPlayerAsync(player));
        }
      


        [Fact]
        public async Task GetAllPlayersAsync_ShouldReturnAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Name = "John", Position = "forward", Skills = new List<Skill> { new Skill { SkillName = "speed", Value = 85 } } },
                new Player { Name = "Doe", Position = "defender", Skills = new List<Skill> { new Skill { SkillName = "defense", Value = 90 } } }
            };
            _mockPlayerRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(players);

            // Act
            var result = await _playerService.GetAllPlayersAsync();

            // Assert
            Assert.Equal(players, result);
        }

        [Fact]
        public async Task AddPlayerAsync_ShouldAddPlayer()
        {
            // Arrange
            var player = new Player
            {
                Name = "Jane",
                Position = "midfielder",
                Skills = new List<Skill> { new Skill { SkillName = "attack", Value = 80 } }
            };

            // Act
            await _playerService.AddPlayerAsync(player);

            // Assert
            _mockPlayerRepository.Verify(r => r.AddAsync(player), Times.Once);
        }

        [Fact]
        public async Task DeletePlayerAsync_ShouldDeletePlayer()
        {
            // Arrange
            var playerName = "John";

            // Act
            await _playerService.DeletePlayerAsync(playerName);

            // Assert
            _mockPlayerRepository.Verify(r => r.DeleteAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Player, bool>>>()), Times.Once);
        }
    }
}
