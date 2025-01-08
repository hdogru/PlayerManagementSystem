using DataLayer;
using DomainLayer;
using System.Linq.Expressions;



namespace DataLayer
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IDataProvider _dataProvider;

        // Available positions
        public static readonly List<string> AvailablePositions = new List<string>
        {
            "defender",
            "midfielder",
            "forward"
        };

        // Available skills
        public static readonly List<string> AvailableSkills = new List<string>
        {
            "defense",
            "attack",
            "speed",
            "strength",
            "stamina"
        };

        public PlayerRepository(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _dataProvider.GetAllAsync<Player>();
        }

        public async Task<Player> GetAsync(Expression<Func<Player, bool>> predicate)
        {
            var players = await _dataProvider.GetAllAsync<Player>();
            return players.AsQueryable().FirstOrDefault(predicate);
        }

        public async Task AddAsync(Player player)
        {
            //if (!AvailablePositions.Contains(player.Position.ToLower()))
            //{
            //    throw new ArgumentException($"Invalid position: {player.Position}. Valid positions are: {string.Join(", ", AvailablePositions)}.");
            //}
            ValidatePosition(player.Position);
            ValidateSkills(player.Skills);
            await _dataProvider.AddAsync(player);
        }


        public async Task UpdateAsync(Player player)
        {
            //if (!AvailablePositions.Contains(player.Position.ToLower()))
            //{
            //    throw new ArgumentException($"Invalid position: {player.Position}. Valid positions are: {string.Join(", ", AvailablePositions)}.");
            //}
            ValidatePosition(player.Position);
            ValidateSkills(player.Skills);
            await _dataProvider.UpdateAsync(player);
        }

        //public async Task DeleteAsync(Expression<Func<Player, bool>> predicate)
        //{
        //    var players = await _dataProvider.GetAllAsync<Player>();
        //    var player = players.AsQueryable().FirstOrDefault(predicate);
        //    if (player != null)
        //    {
        //        // Simplified logic; typically, you'd have a unique ID for deletion
        //        await _dataProvider.DeleteAsync<Player>(player.Name);
        //    }
        //}

        public async Task DeleteAsync(Expression<Func<Player, bool>> predicate)
        {
            await _dataProvider.DeleteAsync(predicate);

        }

        // Helper Methods
        private void ValidatePosition(string position)
        {
            if (!AvailablePositions.Contains(position.ToLower()))
            {
                throw new ArgumentException($"Invalid position: {position}. Valid positions are: {string.Join(", ", AvailablePositions)}.");
            }
        }

        private void ValidateSkills(IEnumerable<Skill> skills)
        {
            foreach (var skill in skills)
            {
                if (!AvailableSkills.Contains(skill.SkillName.ToLower()))
                {
                    throw new ArgumentException($"Invalid skill: {skill.SkillName}. Valid skills are: {string.Join(", ", AvailableSkills)}.");
                }

                if (skill.Value < 0 || skill.Value > 100)
                {
                    throw new ArgumentException($"Invalid value for skill '{skill.SkillName}': {skill.Value}. Skill values must be between 0 and 100.");
                }
            }
        }


        public async Task<List<TeamSelectionResponse>> SelectBestTeamAsync(TeamSelectionRequest request)
        {
            var players = await GetAllAsync();
            var responses = new List<TeamSelectionResponse>();

            foreach (var requirement in request.PositionSkillRequirements)
            {
                // Validate the position and skill
                if (!AvailablePositions.Contains(requirement.Position.ToLower()))
                {
                    throw new ArgumentException($"Invalid position: {requirement.Position}. Valid positions are: {string.Join(", ", AvailablePositions)}.");
                }

                if (!AvailableSkills.Contains(requirement.Skill.ToLower()))
                {
                    throw new ArgumentException($"Invalid skill: {requirement.Skill}. Valid skills are: {string.Join(", ", AvailableSkills)}.");
                }

                // Find the best player for the given position and skill
                var bestPlayer = players
                    .Where(p => p.Position.Equals(requirement.Position, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(p => p.Skills.FirstOrDefault(s => s.SkillName.Equals(requirement.Skill, StringComparison.OrdinalIgnoreCase))?.Value ?? 0)
                    .FirstOrDefault();

                // Add to response
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

