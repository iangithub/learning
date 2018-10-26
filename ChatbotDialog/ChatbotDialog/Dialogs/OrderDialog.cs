using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ChatbotDialog.Dialogs
{
    [Serializable]
    public class OrderDialog : IDialog<string>
    {
        private string _name = string.Empty;
        private string _orderno = string.Empty;
        private int _trycnt = 0;

        public OrderDialog(string name)
        {
            _name = name;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"您好,{ _name }, 請問您要查詢的訂單編號?");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            if (!string.IsNullOrEmpty(activity.Text))
            {
                if (activity.Text.Length > 10)
                {
                    _trycnt++;

                    if (_trycnt < 3)
                    {
                        await context.PostAsync("訂單編號錯誤，請重新輸入");
                        context.Wait(this.MessageReceivedAsync);
                    }
                    else
                    {
                        context.Fail(new TooManyAttemptsException("抱歉，您已超過重試次數"));
                    }
                }
                else
                {
                    //關閉本次對話並從stack中移除，回到前一個dialog
                    context.Done(activity.Text);
                }
            }
            else
            {
                _trycnt++;

                if (_trycnt < 3)
                {
                    await context.PostAsync("訂單編號錯誤，請重新輸入");
                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("抱歉，您已超過重試次數"));
                }
            }
        }
    }
}