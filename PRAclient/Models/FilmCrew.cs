using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAclient.Models
{
    class FilmCrew
    {
        public int CrewMemberId { get; set; }        
        public string Name { get; set; }
        public string Firstname { get; set; }
        public int Age { get; set; }       
        public int PositionId { get; set; }
    }
}
