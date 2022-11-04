using FFXIAHScrape.Entities;
using FFXIAHScrape.FFXIAHScrape._SharedModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXIAHScrape.FFXIAHScrape.UndercutAlert
{
    public class UndercutAlert
    {
        readonly FFXIAhHelper _ffxiahHelper = new FFXIAhHelper();
        
        public async Task Action(ListBox itemList, DataGridView resultGrid, Label modeLabel)
        {
            var undercutAlertItems = new List<UndercutAlertReturn>();
            foreach (string item in itemList.Items)
            {
                var itemInfo = JsonConvert.DeserializeObject<UndercutAlertItemInfo>(item);
                var myPrice = itemInfo.MyPrice;
                var itemUri = _ffxiahHelper.GetItemUri(item);

                var scrapeInfo = (UndercutAlertReturn)await _ffxiahHelper.ScrapeItem(itemUri, itemInfo.Server, Modes.UndercutAlert);
                scrapeInfo.MyListPrice = myPrice;
                undercutAlertItems.Add(scrapeInfo);
            }

            resultGrid.DataSource = undercutAlertItems;
            //resultGrid.Columns["Server"].Visible = false;
            resultGrid.Columns["Server"].DisplayIndex = 0;
            resultGrid.Columns["ItemName"].DisplayIndex = 1;
            resultGrid.Columns["MyListPrice"].DisplayIndex = 2;
            resultGrid.Columns["LastSalePrice"].DisplayIndex = 3;

            foreach (DataGridViewRow row in resultGrid.Rows)
            {
                var gridMyPrice = Convert.ToInt64(row.Cells[0].Value);
                var gridLastSalePrice = Convert.ToInt64(row.Cells[1].Value);

                if (gridMyPrice > gridLastSalePrice)
                {
                    row.Cells[0].Style.BackColor = Color.Red;
                }
                else
                {
                    row.Cells[0].Style.BackColor = Color.LightGreen;
                }
            }
            resultGrid.ClearSelection();
            //modeLabel.Text = $"LastRefreshed: {DateTime.Now.ToString("hh:mm:ss")}";
            modeLabel.Text = $"LastRefreshed: {DateTime.Now:hh:mm:ss}";
        }
    }
}