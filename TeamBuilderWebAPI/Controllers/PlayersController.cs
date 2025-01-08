using DataLayer;
using DomainLayer;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer;

namespace TeamBuilderWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly PlayerService _playerService;

        public PlayersController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        //[HttpGet]
        // GET: api/Players/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _playerService.GetAllPlayersAsync();
            return Ok(players);
        }

        //[HttpPost]
        // POST: api/Players/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddPlayer([FromBody] Player player)
        {
            //await _playerService.AddPlayerAsync(player);
            //return Ok("Player added.");

            try
            {
                await _playerService.AddPlayerAsync(player);
                return Ok("Player added successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return BadRequest with exception message
            }

        }

        //[HttpDelete("{name}")]
        // DELETE: api/Players/DeleteByName/{name}
        [HttpDelete("DeleteByName/{name}")]
        public async Task<IActionResult> DeletePlayer(string name)
        {
            await _playerService.DeletePlayerAsync(name);
            return Ok("Player deleted.");
        }

        [HttpPost("SelectBestTeam")]
        public async Task<IActionResult> SelectBestTeam([FromBody] TeamSelectionRequest request)
        {
            try
            {
                var result = await _playerService.SelectBestTeamAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
