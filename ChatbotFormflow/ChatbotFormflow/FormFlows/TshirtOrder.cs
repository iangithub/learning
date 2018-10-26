using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace ChatbotFormflow.FormFlows
{

    public enum ColorOptions
    {
        黑, 白, 藍, 紅, 綠, 黃
    };
    public enum SizeOptions
    {
        S, M, L, XL, XXL
    };
    public enum PayOptions
    {
        信用卡, 貨到付款
    };

    [Serializable]
    public class TshirtOrder
    {
        [Prompt("請選擇顏色 {||}")]
        public ColorOptions? Color;
        [Prompt("請選擇尺吋 {||}")]
        public SizeOptions? Size;
        [Prompt("請選擇付款方式 {||}")]
        public PayOptions? Pay;

        public static IForm<TshirtOrder> BuildForm()
        {

            OnCompletionAsyncDelegate<TshirtOrder> OrderProcess = async (context, state) =>
            {
                //do order process
                //........
               
                await context.PostAsync("感謝您的訂購，我們將會盡快處理您的訂單");
            };

            //return new FormBuilder<TshirtOrder>()
            //        .Message("請選擇您所要訂購的 T Shirt")
            //        .Build();

            return new FormBuilder<TshirtOrder>()
                    .Message("請選擇您所要訂購的 T Shirt")
                    .OnCompletion(OrderProcess)
                    .Build();


            //return new FormBuilder<TshirtOrder>()
            //        .Message("請選擇您所要訂購的 T Shirt")
            //        .Field(nameof(Color))
            //        .Field(nameof(Size))
            //        .Field(nameof(Pay))
            //        .Confirm("這是您所選擇的資料，顏色:{Color}，顏色尺吋:{Size}，付款方式:{Pay}，正確請輸入1，錯誤請輸入2 ")
            //        .Message("感謝您的訂購，我們將會盡快處理您的訂單")
            //        .Build();

            //return new FormBuilder<TshirtOrder>()
            //        .Message("請選擇您所要訂購的 T Shirt")
            //        .Field(nameof(Color))
            //        .Field(nameof(Size))
            //        .Field(nameof(Pay))
            //        .Confirm("這是您所選擇的資料，顏色:{Color}，顏色尺吋:{Size}，付款方式:{Pay} {||} ")
            //        .Message("感謝您的訂購，我們將會盡快處理您的訂單")
            //        .Build();

        }
    };
}