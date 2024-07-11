using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace SharedAPI.TransferObjects
{
    public class QuestionDto
    {
        public int Response_Code { get; set; }

        public List<Questions> Results { get; set; }
    }

    public class Questions
    {
        public string? Type { get; set; }

        public string? Difficulty { get; set; }

        public string? Category { get; set; }

        public string? Question { get; set; }

        public string? Correct_Answer { get; set; }

        public List<string>? Incorrect_Answers { get; set; } = new List<string>();
    }
}
