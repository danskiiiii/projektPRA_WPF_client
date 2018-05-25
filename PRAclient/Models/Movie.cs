using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAclient.Models
{
    class Movie
    {
        public int MovieId { get; set; }      
        public string Title { get; set; }
        public int ProductionYear { get; set; }
        public decimal Budget { get; set; }
        public string Genre { get; set; }       
        public int StudioId { get; set; }
    }
}
