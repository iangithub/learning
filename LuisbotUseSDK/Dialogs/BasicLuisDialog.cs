using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using LuisBot;
using System.Net.Http;
using System.IO;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        private static CurrencyInfo _currencyInfo = new CurrencyInfo();

        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {

        }


        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            var ans = "我聽不懂啦";
            await this.ShowLuisResult(context, ans);
        }

        [LuisIntent("詢問匯率")]
        public async Task CurrencyRateIntent(IDialogContext context, LuisResult result)
        {
            var ans = "無法提供";

            //取得幣別
            var currenctytype = ((List<object>)result.Entities[0].Resolution["values"])[0].ToString();

            ans = await GetCurrencyInfo(currenctytype);

            //回覆
            await this.ShowLuisResult(context, ans);
        }

        [LuisIntent("訂車票")]
        public async Task TickIntent(IDialogContext context, LuisResult result)
        {
            var ans = "被黃牛搶光了";
            await this.ShowLuisResult(context, ans);
        }

        private async Task ShowLuisResult(IDialogContext context, string ans)
        {
            await context.PostAsync(ans);
            context.Wait(MessageReceived);
        }

        private async Task<string> GetCurrencyInfo(string currency)
        {
            string result = @"現鈔買入 {0}，賣出 {1}，即期買入{2}，賣出{3}，
以上匯率來自台銀公告匯率資訊僅供參考, 請以實際承作匯率為主。 ";

            //真正取匯率
            await GetCurrencyRate();

            if (_currencyInfo != null)
            {
                var item = _currencyInfo.Rates.Find(x => x.CurrencyType == currency);
                if (item != null)
                {
                    result = string.Format(result, item.BuyCash, item.SellCash, item.BuySpot, item.SellSpot);
                }
                else
                {
                    result = "無法提供這個幣別的資訊";
                }
            }
            return result;
        }

        private async Task GetCurrencyRate()
        {
            if (_currencyInfo.Rates == null)
            {
                await DownloadCurrency();
                _currencyInfo.LastTime = DateTime.Now;
            }
            else
            {
                if (_currencyInfo.LastTime.Hour < DateTime.Now.Hour || _currencyInfo.LastTime.Date < DateTime.Now.Date)
                {
                    if (_currencyInfo.Rates != null)
                    {
                        _currencyInfo.Rates.Clear();
                    }
                    await DownloadCurrency();
                    _currencyInfo.LastTime = DateTime.Now;
                }
            }
        }

        private async Task DownloadCurrency()
        {
            var line = string.Empty;
            _currencyInfo.Rates = new System.Collections.Generic.List<CurrencyRate>();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://rate.bot.com.tw/xrt/flcsv/0/day"))
                {
                    using (Stream st = await response.Content.ReadAsStreamAsync())
                    {
                        using (var streader = new StreamReader(st))
                        {
                            var i = 0;
                            while ((line = streader.ReadLine()) != null)
                            {
                                string[] split = line.Split(',');
                                if (i > 0)
                                {
                                    var item = new CurrencyRate();
                                    item.CurrencyType = split[0];
                                    item.BuyCash = decimal.Parse(split[2]);
                                    item.BuySpot = decimal.Parse(split[3]);
                                    item.SellCash = decimal.Parse(split[12]);
                                    item.SellSpot = decimal.Parse(split[13]);
                                    _currencyInfo.Rates.Add(item);
                                }
                                i++;
                            }
                        }
                    }
                }
            }
        }

    }
}