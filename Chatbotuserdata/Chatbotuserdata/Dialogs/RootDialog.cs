using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Chatbotuserdata.Dialogs
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

            StateClient stateClient = activity.GetStateClient();

            //Set Data
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

            UserData userprofile = new UserData()
            {
                Name = "Ian",
                Age = "20"
            };

            userData.SetProperty<UserData>("UserData", userprofile);
            context.UserData.SetValue("publicData", "這是公開的資料");

            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);


            //Get Data
            BotData userData2 = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            var dataresult = userData2.GetProperty<UserData>("UserData");
            string contextuserdata;
            context.UserData.TryGetValue("publicData", out contextuserdata);

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"Hi {dataresult.Name} , You sent {activity.Text} which was {length} characters ");
            await context.PostAsync($"Your UserData is : " + contextuserdata);

            context.Wait(MessageReceivedAsync);
        }

    }

    public class UserData
    {
        public string Name { get; set; }
        public string Age { get; set; }
    }
}