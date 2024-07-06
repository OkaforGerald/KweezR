using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace SharedAPI.TransferObjects
{
    public class CreateRoomDto
    {
        public string? Name { get; set; }

        public RoomAccess Access { get; set; }

        public QuizCategory Category { get; set; }

        public int MaxCapacity { get; set; }

        public int NumberOfQuestions { get; set; }

        public string? Password { get; set; }

        public string? OwnerId { get; set; }
    }
}
