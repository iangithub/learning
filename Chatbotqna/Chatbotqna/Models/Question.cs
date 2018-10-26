using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Chatbotqna.Models
{
    public class Question
    {
        [JsonProperty("question")]
        public string QuestionStr { get; set; } //要詢問的問題
        [JsonProperty("top")]
        public int Top { get; set; } //取回分數最高的答案筆數
    }
}