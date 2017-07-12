using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anketbaz.Database.Database;
using Newtonsoft.Json;

namespace Anketbaz.Data.Models
{

    public class GuestAnswer
    {
        public long AnswerId { get; set; }
        public long QuestionId { get; set; }
        public long GuestId { get; set; }

    }
    public class Answer
    {
        public long AnswerId { get; set; }

        public string AnswerType { get; set; }

        public string Content { get; set; }

        public int VoteCount { get; set; }
    }

    public class Question
    {
        public List<Answer> Answers { get; set; }

        public string Content { get; set; }

        public string QuestionType { get; set; }

        public long QuestionId { get; set; }

        public int VoteCount { get; set; }
    }

    public class PollModel
    {
        public string PollTitle { get; set; }
        public long PollId { get; set; }
        public string IsPassword { get; set; }
        public string Password { get; set; }
        public string IsCookieCheck { get; set; }
        public string Fields { get; set; }
        public string Active { get; set; }
        public List<Question> Questions { get; set; }
    }


    public class AnalysePollData
    {

        public PollModel Poll { get; set; }
        public List<dynamic> GuestList { get; set; }
        public List<GuestAnswer> GuestAnswers { get; set; }
        public string AvarageComplateSeconds { get; set; }
        public string AvarageComplateTime { get; set; }
        public string TotalComplate { get; set; }
        public string TotalView { get; set; }
        public string AnalyseTime { get; set; }
        public string AnalyseDateTime { get; set; }
    }
}
