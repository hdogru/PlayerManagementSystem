using System.ComponentModel.DataAnnotations;

namespace DomainLayer
{
    public class Player
    {
        public string Name { get; set; }
        public string Position { get; set; }

        [MinLength(1, ErrorMessage = "A player must have at least one skill.")]
        public List<Skill> Skills { get; set; }
    }

}