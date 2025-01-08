using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class TeamSelectionResponse
    {
        public string Position { get; set; }
        public string Skill { get; set; }
        public Player? Player { get; set; }
    }

}
