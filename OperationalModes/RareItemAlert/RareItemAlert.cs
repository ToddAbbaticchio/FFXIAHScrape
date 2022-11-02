using FFXIAHScrape.Entities;
using FFXIAHScrape.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXIAHScrape.OperationalModes.RareItemAlert
{
    public class RareItemAlert
    {
        readonly FFXIAhHelper _ffxiahHelper = new FFXIAhHelper();

        public async Task Action(ListBox itemList, DataGridView resultGrid, Label modeLabel, string serverName)
        {
            var alertItems = new List<RareItemAlertReturn>();
            foreach (string item in itemList.Items)
            {
                var itemUri = _ffxiahHelper.GetItemUri(item);

                var itemInfo = (RareItemAlertReturn)await _ffxiahHelper.ScrapeItem(itemUri, serverName, Modes.RareItemAlert);
                alertItems.Add(itemInfo);
            }

            resultGrid.DataSource = alertItems;
            resultGrid.Columns["Server"].Visible = false;
            resultGrid.Columns["ItemName"].DisplayIndex = 0;
            resultGrid.Columns["Stock"].DisplayIndex = 1;
            resultGrid.Columns["LastSalePrice"].DisplayIndex = 2;

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
            modeLabel.Text = $"LastRefreshed: {DateTime.Now.ToString("hh:mm:ss")}";
        }
    }
}