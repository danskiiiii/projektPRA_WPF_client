using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRAclient.Models
{
    class Contract
    {
        public int ContractId { get; set; }
        public int Duration { get; set; }
        public int Salary { get; set; }       
        public int CrewMemberId { get; set; }              
        public int MovieId { get; set; }
    }
}
