using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace MediaBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            //context.Wait(MessageReceivedAsync2);
            //context.Wait(MessageReceivedAsync3);

            return Task.CompletedTask;
        }

        //receive file
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            foreach (var item in activity.Attachments)
            {
                //檔案位置
                var itemurl = item.ContentUrl;

                //using (var webClient = new WebClient())
                //{
                //    webClient.DownloadFile(itemurl, "your local path");
                //}

            }

            await context.PostAsync(JsonConvert.SerializeObject(activity));

            context.Wait(MessageReceivedAsync);
        }

        //send img
        private async Task MessageReceivedAsync2(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            Activity response = ((Activity)context.Activity).CreateReply();

            Attachment att1 = new Attachment();
            att1.ContentType = "image/png";
            att1.ContentUrl = "https://www.dominos.com.tw/upload/CKUpload/0073df29-6e72-435f-9d2d-adf2e081035a_320_188.jpg";

            Attachment att2 = new Attachment();
            att2.ContentType = "image/png";
            att2.ContentUrl = "https://www.dominos.com.tw/upload/CKUpload/6589c2ad-66f9-4add-9599-f4e33ea74a62_320_188.jpg";

            Attachment att3 = new Attachment();
            att3.ContentType = "image/png";
            att3.ContentUrl = "https://www.dominos.com.tw/upload/CKUpload/7e3db03a-0f28-46c5-9743-cae36336bf2b_320_188.jpg";

            response.Attachments.Add(att1);
            response.Attachments.Add(att2);
            response.Attachments.Add(att3);

            response.Text = "Pizza口味";

            await context.PostAsync(response);

            context.Wait(MessageReceivedAsync);
        }


        //send file
        private async Task MessageReceivedAsync3(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            Activity response = ((Activity)context.Activity).CreateReply();

            Attachment att1 = new Attachment();
            att1.ContentType = "application/pdf";
            att1.ContentUrl = "https://intelligent-information.blog/wp-content/uploads/2017/09/A-Primer-AI-and-Chatbots-in-Technical-Communication.pdf";
            //att1.ContentUrl = HttpContext.Current.Server.MapPath("~/files/aichatbot.pdf");
            att1.Name = "ai chatbot.pdf";
            response.Attachments.Add(att1);

            response.Text = "ai chatbot";

            await context.PostAsync(response);

            context.Wait(MessageReceivedAsync);
        }

    }
}