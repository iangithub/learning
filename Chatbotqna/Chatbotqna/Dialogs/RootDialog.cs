using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Chatbotqna.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Chatbotqna.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var question = new Question()
            {
                QuestionStr = activity.Text,
                Top = 1
            };

            var ans = await QnAService(question);
            if (ans.Answers.Count > 0)
            {
                await context.PostAsync(ans.Answers[0].AnswerStr);
            }
            else
            {
                await context.PostAsync("很抱歉，我無法回答您的問題");
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task<AnswerResult> QnAService(Question question)
        {
            AnswerResult result = new AnswerResult();
            string url = "https://pcqna2.azurewebsites.net/qnamaker/knowledgebases/9cbcbc3d-c08b-4758-a83d-fb0239f6ec7b/generateAnswer";
            string endpoint_key = "d029ddc1-5484-47fb-a721-b9f0aee9ddcf";

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(url);
                    request.Headers.Add("Authorization", "EndpointKey " + endpoint_key);
                    request.Content = new StringContent(JsonConvert.SerializeObject(question), Encoding.UTF8, "application/json");
                    var response = await client.SendAsync(request);
                    var val = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<AnswerResult>(val);
                }
            }
            return result;
        }
    }
}