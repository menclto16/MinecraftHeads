using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftHeads
{
    public class Question
    {
        public int id { get; set; }
        public string question { get; set; }
    }
    public class QuestionResponse
    {
        public Answer answer { get; set; }
        public Question question { get; set; }
    }
}
