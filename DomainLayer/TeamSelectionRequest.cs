using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class TeamSelectionRequest
    {
        public List<PositionSkillRequest> PositionSkillRequirements { get; set; }
    }

    public class PositionSkillRequest
    {
        public string Position { get; set; }
        public string Skill { get; set; }
    }

}
