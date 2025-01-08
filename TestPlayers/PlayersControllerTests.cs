using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceLayer;
using DomainLayer;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBuilderWebAPI.Controllers;

public class PlayersControllerTests
{
    private readonly Mock<IPlayerRepository> _mockPlayerRepository;
    private readonly PlayerService _playerService;
    private readonly PlayersController _controller;


    public PlayersControllerTests()
    {
        // Mocking PlayerRepository (dependency of PlayerService)
        _mockPlayerRepository = new Mock<IPlayerRepository>();

        // Inject the mocked repository into PlayerService
        _playerService = new PlayerService(_mockPlayerRepository.Object);

        // Inject the PlayerService into the PlayersController
        _controller = new PlayersController(_playerService);
    }

    [Fact]
    public async Task AddPlayer_ShouldReturnBadRequest_WhenNoSkills()
    {
        // Arrange
        var playerWithoutSkills = new Player
        {
            Name = "NoSkillPlayer",
            Position = "forward",
            Skills = new List<Skill>() // Empty skills
        };

        // Set up the PlayerService to throw an exception when adding a player with no skills
        _mockPlayerRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Player>()))
            .ThrowsAsync(new ArgumentException("Player must have at least one skill."));

        // Act
        var result = await _controller.AddPlayer(playerWithoutSkills);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Player must have at least one skill.", badRequestResult.Value);
    }
}
