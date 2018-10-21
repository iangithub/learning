using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot
{
    public class CurrencyInfo
    {
        public DateTime LastTime { get; set; }
        public List<CurrencyRate> Rates { get; set; }
    }

    public class CurrencyRate
    {
        public String CurrencyType { get; set; }
        public decimal BuyCash { get; set; }
        public decimal BuySpot { get; set; }
        public decimal SellCash { get; set; }
        public decimal SellSpot { get; set; }
    }
}