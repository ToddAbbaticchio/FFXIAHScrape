using FFXIAHScrape.Entities;
using FFXIAHScrape.FFXIAHScrape._SharedModels;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFXIAHScrape
{
    public class FFXIAhHelper
    {
        public async Task<ItemQueryReturnBase> ScrapeItem(Uri itemUri, string server, Modes currMode)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("cookie", $"sid={Servers.Info[server]}");
                var result = await client.DownloadStringTaskAsync(itemUri);

                // process html data
                var _htmlDocument = new HtmlDocument();
                _htmlDocument.LoadHtml(result);
                HtmlNodeNavigator navigator = (HtmlNodeNavigator)_htmlDocument.CreateNavigator();

                // snag XPaths to named nodes
                var nameNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[2]/span[1]/span[1]/span[1]";
                var stockNode = GetValueXPath(_htmlDocument, "Stock");
                var rateNode = GetValueXPath(_htmlDocument, "Rate");
                var medianNode = GetValueXPath(_htmlDocument, "Median");
                var priceHistoryNode = GetPriceHistoryXPath(_htmlDocument);

                // jump through hoops to pull price history set out of javascript dumbness
                var phNodeValue = navigator.SelectSingleNode(priceHistoryNode).Value;
                Regex findSalesJson = new Regex("Item\\.sales = (\\[.*?\\])");
                var jsonString = findSalesJson.Match(phNodeValue).Groups[1].ToString();
                var priceHistory = JsonConvert.DeserializeObject<List<PriceHistory>>(jsonString);

                // Construct return object

                switch (currMode)
                {
                    case Modes.RareItemAlert:
                        return new RareItemAlertReturn
                        {
                            Server = server,
                            ItemName = navigator.SelectSingleNode(nameNode).Value,
                            Stock = navigator.SelectSingleNode(stockNode).Value,
                            LastSalePrice = priceHistory[0].price.ToString(),
                        };
                    case Modes.UndercutAlert:
                        return new UndercutAlertReturn
                        {
                            Server = server,
                            ItemName = navigator.SelectSingleNode(nameNode).Value,
                            LastSalePrice = priceHistory[0].price.ToString(),
                        };
                    case Modes.XServerArbitrage:
                        return new XServerArbitrageReturn
                        {
                            Server = server,
                            ItemName = navigator.SelectSingleNode(nameNode).Value,
                            Stock = navigator.SelectSingleNode(stockNode).Value,
                            Rate = navigator.SelectSingleNode(rateNode).Value,
                            LastSalePrice = priceHistory[0].price.ToString(),
                        };
                }
                throw new Exception("why is this happening here?");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Uri GetItemUri(string itemJson)
        {
            var itemInfo = JsonConvert.DeserializeObject<UndercutAlertInfo>(itemJson);
            var itemUri = (itemInfo.Stack)
                ? new Uri($"{Constants.baseUrl}/{itemInfo.Id}/{itemInfo.Item}/{Constants.stack}")
                : new Uri($"{Constants.baseUrl}/{itemInfo.Id}/{itemInfo.Item}");

            return itemUri;
        }

        private string GetValueXPath(HtmlDocument htmlDoc, string valName)
        {
            var getNameField = htmlDoc.DocumentNode.SelectNodes($"//*[text()[contains(., '{valName}')]]").Where(x => x.InnerText == valName).First().XPath;
            string result = Regex.Replace(getNameField, "td\\[1\\]$", "td[2]");
            return result;
        }

        private string GetPriceHistoryXPath(HtmlDocument htmlDoc)
        {
            var historyXPath = htmlDoc.DocumentNode.SelectNodes($"//script[contains(text(), 'Item.sales')]").Where(x => x.ParentNode.Name == "head").First().XPath;
            return historyXPath;
        }
    }
}
