using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ChatbotDialog.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string _name = string.Empty;
        private string _orderno = string.Empty;

        public Task StartAsync(IDialogContext context)
        {
            //等待第一則對話訊息進來，並呼叫MessageReceivedAsync進行處理
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //當MessageReceivedAsync被呼叫時，由IAwaitable<object>取得對話訊息
            var activity = await result as Activity;

            //送出歡迎訊息
            await this.SendWelcomeMessageAsync(context);

        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {

            await context.PostAsync("您好，我是訂單智能服務機器人");

            //將對話控制權轉交另一個Dialog，當NameDialog完成後，呼叫NameDialogResumeAfter
            context.Call(new NameDialog(), this.NameDialogResumeAfter);
        }

        private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                _name = await result;

                //將對話控制權轉交另一個Dialog，當OrderDialog完成後，呼叫OrderDialogResumeAfter
                context.Call(new OrderDialog(_name), this.OrderDialogResumeAfter);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("抱歉，我無法理解您的意思，我們再重新開始");

                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task OrderDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                _orderno = await result;

                await context.PostAsync($"你好 { _name } ,你的訂單編號為: { _orderno }.");
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("抱歉，我無法理解您的意思，我們再重新開始");
            }
            finally
            {
                await this.SendWelcomeMessageAsync(context);
            }
        }
    }
}