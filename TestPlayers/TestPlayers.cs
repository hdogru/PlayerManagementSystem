

using Moq;
using Xunit;
using ServiceLayer;
using DomainLayer;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace TestPlayers
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private readonly PlayerService _playerService;

        public PlayerServiceTests()
        {
            _mockPlayerRepository = new Mock<IPlayerRepository>();
            _playerService = new PlayerService(_mockPlayerRepository.Object);
        }

        // Test: Add Player
        [Fact]
        public async Task AddPlayerAsync_ShouldAddPlayer()
        {
            // Arrange
            var player = new Player { Name = "John", Position = "defender", Skills = new List<Skill> { new Skill { SkillName = "defense", Value = 90 } } };

            // Act
            await _playerService.AddPlayerAsync(player);

            // Assert
            _mockPlayerRepository.Verify(r => r.AddAsync(It.IsAny<Player>()), Times.Once);
        }

        // Test: Get All Players
        [Fact]
        public async Task GetAllPlayersAsync_ShouldReturnAllPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Name = "John", Position = "defender", Skills = new List<Skill> { new Skill { SkillName = "defense", Value = 90 } } },
                new Player { Name = "Jane", Position = "midfielder", Skills = new List<Skill> { new Skill { SkillName = "speed", Value = 85 } } }
            };
            _mockPlayerRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(players);

            // Act
            var result = await _playerService.GetAllPlayersAsync();

            // Assert
            result.Should().BeEquivalentTo(players);
        }

        // Test: Update Player
        [Fact]
        public async Task UpdatePlayerAsync_ShouldUpdatePlayer()
        {
            // Arrange
            var player = new Player { Name = "John", Position = "defender", Skills = new List<Skill> { new Skill { SkillName = "defense", Value = 90 } } };

            // Act
            await _playerService.UpdatePlayerAsync(player);

            // Assert
            _mockPlayerRepository.Verify(r => r.UpdateAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task DeletePlayerAsync_ShouldDeletePlayer()
        {
            // Arrange
            var playerName = "John";

            // Act
            await _playerService.DeletePlayerAsync(playerName);

            // Assert
            _mockPlayerRepository.Verify(
                r => r.DeleteAsync(It.Is<Expression<Func<Player, bool>>>(expr => TestExpression(expr, playerName))),
                Times.Once);
        }

        // Helper Method to Test Expression Behavior
        private bool TestExpression(Expression<Func<Player, bool>> expression, string expectedName)
        {
            // Compile the expression into a delegate
            var compiledExpression = expression.Compile();

            // Test the expression with a mock player
            var testPlayer = new Player { Name = expectedName };
            return compiledExpression(testPlayer); // Should return true for the correct player
        }

        // Test: Select Best Team
        [Fact]
        public async Task SelectBestTeamAsync_ShouldSelectBestTeamBasedOnSkillAndPosition()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player { Name = "John", Position = "defender", Skills = new List<Skill> { new Skill { SkillName = "defense", Value = 90 } } },
                new Player { Name = "Jane", Position = "midfielder", Skills = new List<Skill> { new Skill { SkillName = "speed", Value = 85 } } },
                new Player { Name = "Mike", Position = "defender", Skills = new List<Skill> { new Skill { SkillName = "defense", Value = 95 } } }
            };

            var request = new TeamSelectionRequest
            {
                PositionSkillRequirements = new List<PositionSkillRequest>
                {
                    new PositionSkillRequest { Position = "defender", Skill = "defense" },
                    new PositionSkillRequest { Position = "midfielder", Skill = "speed" }
                }
            };

            _mockPlayerRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(players);

            // Act
            var result = await _playerService.SelectBestTeamAsync(request);

            // Assert: We expect the best defender for "defense" and the best midfielder for "speed"
            result.Should().HaveCount(2);
            result[0].Player.Name.Should().Be("Mike"); // Best defender based on "defense"
            result[1].Player.Name.Should().Be("Jane"); // Best midfielder based on "speed"
        }
    }
}
