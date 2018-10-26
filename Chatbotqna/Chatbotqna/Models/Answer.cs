using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Chatbotqna.Models
{
    public class AnswerResult
    {
        [JsonProperty("answers")]
        public List<Answer> Answers { get; set; }
    }
    public class Answer
    {
        [JsonProperty("answer")]
        public string AnswerStr { get; set; }
        [JsonProperty("questions")]
        public List<string> Questions { get; set; }
        [JsonProperty("score")]
        public float Score { get; set; }
    }
}