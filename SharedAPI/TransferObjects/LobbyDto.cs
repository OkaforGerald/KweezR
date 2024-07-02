using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedAPI.TransferObjects
{
    public class LobbyDto
    {
        public string? Name { get; set; }

        public string? Access { get; set; }

        public string? Category { get; set; }

        public int NumberOfQuestions { get; set; }

        public int Capacity { get; set; }

        public List<string>? Players { get; set; }
    }
}
