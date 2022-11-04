using FFXIAHScrape.Entities;
using FFXIAHScrape.FFXIAHScrape._SharedModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            //resultGrid.Columns["Server"].Visible = false;
            resultGrid.Columns["Server"].DisplayIndex = 0;
            resultGrid.Columns["ItemName"].DisplayIndex = 1;
            resultGrid.Columns["Stock"].DisplayIndex = 2;
            resultGrid.Columns["LastSalePrice"].DisplayIndex = 3;

            foreach (DataGridViewRow row in resultGrid.Rows)
            {
                var gridStock = Convert.ToInt16(row.Cells[0].Value);

                if (gridStock == 0)
                {
                    row.Cells[0].Style.BackColor = Color.Red;
                }
                else
                {
                    row.Cells[0].Style.BackColor = Color.LightGreen;
                }
            }
            resultGrid.ClearSelection();
            modeLabel.Text = $"LastRefreshed: {DateTime.Now:hh:mm:ss}";
        }
    }
}