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
        
        public async Task Action(ListBox itemList, DataGridView resultGrid, Label modeLabel, string serverName)
        {
            var undercutAlertItems = new List<UndercutAlertReturn>();
            foreach (string item in itemList.Items)
            {
                var myPrice = JsonConvert.DeserializeObject<UndercutAlertInfo>(item).MyPrice;
                var itemUri = _ffxiahHelper.GetItemUri(item);

                var itemInfo = (UndercutAlertReturn)await _ffxiahHelper.ScrapeItem(itemUri, serverName, Modes.UndercutAlert);
                itemInfo.MyListPrice = myPrice;
                undercutAlertItems.Add(itemInfo);
            }

            resultGrid.DataSource = undercutAlertItems;
            resultGrid.Columns["Server"].Visible = false;
            resultGrid.Columns["ItemName"].DisplayIndex = 0;
            resultGrid.Columns["MyListPrice"].DisplayIndex = 1;
            resultGrid.Columns["LastSalePrice"].DisplayIndex = 2;

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
            modeLabel.Text = $"LastRefreshed: {DateTime.Now.ToString("hh:mm:ss")}";
        }
    }
}