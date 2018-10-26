using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ChatbotDialog.Dialogs
{
    [Serializable]
    public class NameDialog : IDialog<string>
    {
        private int _trycnt = 0;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("請問您的姓名?");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            if (!string.IsNullOrEmpty(activity.Text))
            {
                //關閉本次對話並從stack中移除，回到前一個dialog
                context.Done(activity.Text);
            }
            else
            {
                _trycnt++;

                if (_trycnt < 3)
                {
                    //使用者輸入不正確的資訊，重新再試著問一下使用者
                    await context.PostAsync("我無法理解您說的. 請問您的姓名?");

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("抱歉，您已經超過重試次數"));
                }
            }
        }
    }
}