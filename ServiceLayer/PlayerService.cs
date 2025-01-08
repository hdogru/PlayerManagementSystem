using DomainLayer;
using DataLayer;

namespace ServiceLayer
{
    public class PlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _playerRepository.GetAllAsync();
        }

        public async Task AddPlayerAsync(Player player)
        {
            if (player.Skills == null || !player.Skills.Any())
            {
                throw new ArgumentException("Player must have at least one skill.");
            }

            await _playerRepository.AddAsync(player);

        }

        public async Task UpdatePlayerAsync(Player player)
        {
            ValidatePlayer(player);
            await _playerRepository.UpdateAsync(player);
        }

        private void ValidatePlayer(Player player)
        {
            if (player.Skills == null || !player.Skills.Any())
            {
                throw new ArgumentException("A player must have at least one skill.");
            }
        }

        public async Task DeletePlayerAsync(string name)
        {
            await _playerRepository.DeleteAsync(p => p.Name == name);
        }

        public async Task<List<TeamSelectionResponse>> SelectBestTeamAsync(TeamSelectionRequest request)
        {
            var players = await _playerRepository.GetAllAsync();
            var responses = new List<TeamSelectionResponse>();

            foreach (var requirement in request.PositionSkillRequirements)
            {
                // Validate the position and skill
                if (!PlayerRepository.AvailablePositions.Contains(requirement.Position.ToLower()))
                {
                    throw new ArgumentException($"Invalid position: {requirement.Position}. Valid positions are: {string.Join(", ", PlayerRepository.AvailablePositions)}.");
                }

                if (!PlayerRepository.AvailableSkills.Contains(requirement.Skill.ToLower()))
                {
                    throw new ArgumentException($"Invalid skill: {requirement.Skill}. Valid skills are: {string.Join(", ", PlayerRepository.AvailableSkills)}.");
                }

                // Find the best player for the given position and skill
                var bestPlayer = players
                    .Where(p => p.Position.Equals(requirement.Position, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(p => p.Skills.FirstOrDefault(s => s.SkillName.Equals(requirement.Skill, StringComparison.OrdinalIgnoreCase))?.Value ?? 0)
                    .FirstOrDefault();

                responses.Add(new TeamSelectionResponse
                {
                    Position = requirement.Position,
                    Skill = requirement.Skill,
                    Player = bestPlayer
                });
            }

            return responses;
        }

    }
}
