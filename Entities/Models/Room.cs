using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum RoomAccess
    {
        Public = 100,
        Private = 200
    }



    public enum QuizCategory
    {
        Random = 100,
        General = 200,
        Books = 300,
        Film = 400,
        Music = 500,
        VideoGames = 600,
        Mathematics = 700,
        Computers = 800
    }

    public class Room : EntityBase
    {
        public string? Name { get; set; }
        
        public RoomAccess Access { get; set; }

        public QuizCategory Category { get; set; }

        public int MaxCapacity { get; set; }

        public int NumberOfQuestions { get; set; }

        public string? Hash { get; set; }

        public string? Salt { get; set; }

        public bool IsActive { get; set; }

        public bool IsInSession { get; set; }

        public User? Owner { get; set; }
        public string? OwnerId { get; set; }
    }
}
