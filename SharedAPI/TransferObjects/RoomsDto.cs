using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAPI.TransferObjects
{
    public class RoomsDto
    {
        public string? Name { get; set; }

        public string? Access { get; set; }

        public string? Category { get; set; }

        public int NumberOfQuestions { get; set; }
                
        public int MaxCapacity { get; set; }

        public int CurrentCapacity { get; set; }
    }
}
