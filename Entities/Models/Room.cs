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
        General = 9,
        Books = 10,
        Film = 11,
        Music = 12,
        VideoGames = 15,
        Mathematics = 19,
        Computers = 18,
        Sports = 21,
        Geography = 22,
        Art = 25
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
