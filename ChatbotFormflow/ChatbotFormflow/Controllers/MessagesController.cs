using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ChatbotFormflow.FormFlows;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace ChatbotFormflow
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, TshirtOrderDialog);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        internal static IDialog<TshirtOrder> TshirtOrderDialog()
        {
            return Chain.From(() => FormDialog.FromForm(TshirtOrder.BuildForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        var completed = await order;

                        /*
                         * 處理訂單資料的程序...
                         * ......
                         * ......
                         */

                        //await context.PostAsync("訂單已完成");
                        await context.PostAsync(JsonConvert.SerializeObject(completed));

                    }
                    catch (FormCanceledException<TshirtOrder> e)
                    {
                        await context.PostAsync("系統發生錯誤，請稍後再試");
                    }
                });
        }
    }


}