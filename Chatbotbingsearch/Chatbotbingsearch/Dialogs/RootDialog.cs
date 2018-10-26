using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chatbotbingsearch.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Chatbotbingsearch.Dialogs
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

            var news = await BingSearch(activity.Text);

            var reply = (context.Activity as Activity).CreateReply("查詢結果：");

            List<ThumbnailCard> cards = new List<ThumbnailCard>();

            foreach (var item in news.Value)
            {
                var card = new ThumbnailCard()
                {
                    Title = item.Name,
                    Text = item.Description,                    
                    Buttons = new List<CardAction>
                    {
                        BuildCardAction(item.Url)
                    }
                };
                reply.Attachments.Add(card.ToAttachment());
            }

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            await new ConnectorClient(new Uri(reply.ServiceUrl)).Conversations.SendToConversationAsync(reply);

            context.Wait(MessageReceivedAsync);
        }

        private async Task<News> BingSearch(string q)
        {
            var result = new News();
            string key = "9f00836db97640468ebd1151c7156839";
            string baseurl = "https://api.cognitive.microsoft.com/bing/v7.0/news/search?";
            baseurl += "q=" + q;
            baseurl += "&category=Sports";
            baseurl += "&count=3";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                string response = await client.GetStringAsync(baseurl);
                result = JsonConvert.DeserializeObject<News>(response);
            }
            return result;
        }

        private CardAction BuildCardAction(string url)
        {
            return new CardAction
            {
                Type = ActionTypes.OpenUrl,
                Title = "詳情",
                Value = url
            };
        }
    }
}