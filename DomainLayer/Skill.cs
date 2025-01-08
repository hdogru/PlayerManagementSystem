using System.ComponentModel.DataAnnotations;

namespace DomainLayer
{

    public class Skill
    {
        [Required]
        public string SkillName { get; set; }
        public int Value { get; set; }
    }
}