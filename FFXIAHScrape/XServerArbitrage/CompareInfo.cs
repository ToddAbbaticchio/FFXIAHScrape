using FFXIAHScrape.FFXIAHScrape._SharedModels;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FFXIAHScrape.FFXIAHScrape.XServerArbitrage
{
    public class CompareInfo
    {
        public string ItemName { get; set; }
        public string S1_Stock { get; set; }
        public string S1_Rate { get; set; }
        public decimal S1_Price { get; set; }
        public List<string> S1_Last3 { get; set; }
        public string S2_Stock { get; set; }
        public string S2_Rate { get; set; }
        public decimal S2_Price { get; set; }
        public List<string> S2_Last3 { get; set; }
        public string StrProfit { get; set; }
        public string StrMargin { get; set; }
        public decimal Profit { get; set; }
        public decimal Margin { get; set; }

        public CompareInfo(XServerArbitrageReturn s1, XServerArbitrageReturn s2)
        {
            Regex cleanRate = new Regex("(\\d+.\\d{2})");
            var s1Rate = cleanRate.Match(s1.Rate).Groups[1].ToString();
            var s2Rate = cleanRate.Match(s2.Rate).Groups[1].ToString();

            ItemName = (s1.ItemName == s2.ItemName) ? s1.ItemName : "query mismatch!";
            S1_Stock = s1.Stock;
            S1_Rate = s1Rate;
            S1_Price = Convert.ToDecimal(s1.LastSalePrice);
            S2_Stock = s2.Stock;
            S2_Rate = s2Rate;
            S2_Price = Convert.ToDecimal(s2.LastSalePrice);
            Profit = S2_Price - S1_Price;
            Margin = Profit / S1_Price;
            StrProfit = Profit.ToString("C0");
            StrMargin = Margin.ToString("P");
        }
    }
}