using Discord.Webhook;
using FFXIAHScrape.Entities;
using FFXIAHScrape.FFXIAHScrape._SharedModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXIAHScrape.FFXIAHScrape.RareItemAlert
{
    public class RareItemAlert
    {
        readonly FFXIAhHelper _ffxiahHelper = new FFXIAhHelper();

        public async Task Action(ListBox itemList, DataGridView resultGrid, Label modeLabel)
        {
            var alertItems = new List<RareItemAlertReturn>();
            foreach (string item in itemList.Items)
            {
                var itemUri = _ffxiahHelper.GetItemUri(item);
                var itemInfo = JsonConvert.DeserializeObject<RareItemAlertItemInfo>(item);

                var scrapeInfo = (RareItemAlertReturn)await _ffxiahHelper.ScrapeItem(itemUri, itemInfo.Server, Modes.RareItemAlert);
                alertItems.Add(scrapeInfo);
            }

            resultGrid.DataSource = alertItems;
            resultGrid.Columns["Server"].DisplayIndex = 0;
            resultGrid.Columns["ItemName"].DisplayIndex = 1;
            resultGrid.Columns["Stock"].DisplayIndex = 2;
            resultGrid.Columns["LastSalePrice"].DisplayIndex = 3;

            var now = DateTime.Now.ToString("hh:mm:ss");
            var discordMessageList = new List<string>();
            discordMessageList.Add($"Last Updated: {now}");

            var redList = new List<string>();
            var greenList = new List<string>();
            foreach (DataGridViewRow row in resultGrid.Rows)
            {
                var rowInfo = new GridRowInfo(row);
                var msg = $"{rowInfo.Server} — ({rowInfo.Qty}){rowInfo.Item.Replace(" ", "")}";
                if (rowInfo.Qty == 0)
                {
                    row.Cells[0].Style.BackColor = Color.Red;
                    redList.Add(msg);
                }
                else
                {
                    row.Cells[0].Style.BackColor = Color.LightGreen;
                    greenList.Add(msg + $" LastSalePrice({rowInfo.LastSale})");
                }
            }

            if (redList.Any())
                discordMessageList.Add($"```fix{Environment.NewLine}{string.Join(Environment.NewLine, redList)}```");
            if (greenList.Any())
                discordMessageList.Add($"```yaml{Environment.NewLine}{string.Join(Environment.NewLine, greenList)}```");

            // update discord message
            var discord = new DiscordWebhookClient(Constants.discordRareItemWebhook);
            await discord.ModifyMessageAsync(Constants.discordRareItemMessageId, m => m.Content = string.Join(Environment.NewLine, discordMessageList));

            resultGrid.ClearSelection();
            modeLabel.Text = $"LastRefreshed: {now}";
        }
    }
}