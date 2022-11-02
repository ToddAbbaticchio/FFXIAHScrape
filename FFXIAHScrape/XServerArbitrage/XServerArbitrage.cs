using FFXIAHScrape.Entities;
using FFXIAHScrape.FFXIAHScrape._SharedModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFXIAHScrape.FFXIAHScrape.XServerArbitrage
{
    public class XServerArbitrage
    {
        readonly FFXIAhHelper _ffxiahHelper = new FFXIAhHelper();

        public async Task Action(ListBox itemList, DataGridView resultGrid, Label modeLabel, string s1Name, string s2Name)
        {
            var compareList = new List<CompareInfo>();
            foreach (string item in itemList.Items)
            {
                var itemUri = _ffxiahHelper.GetItemUri(item);
                var s1 = (XServerArbitrageReturn)await _ffxiahHelper.ScrapeItem(itemUri, s1Name, Modes.XServerArbitrage);
                var s2 = (XServerArbitrageReturn)await _ffxiahHelper.ScrapeItem(itemUri, s2Name, Modes.XServerArbitrage);

                compareList.Add(new CompareInfo()
                {
                    ItemName = s1.ItemName,
                    S1_Stock = s1.Stock,
                    S1_Rate = s1.Rate,
                    S1_Median = s1.LastSalePrice,
                    S2_Stock = s2.Stock,
                    S2_Rate = s2.Rate,
                    S2_Median = s2.LastSalePrice,
                });
            }

            // add results to the resultGrid
            resultGrid.DataSource = compareList;
            // add server names to column headers
            resultGrid.Columns[1].HeaderText = $"{s1Name}-Stock";
            resultGrid.Columns[2].HeaderText = $"{s1Name}-Rate";
            resultGrid.Columns[3].HeaderText = $"{s1Name}-LastSalePrice";
            resultGrid.Columns[4].HeaderText = $"{s2Name}-Stock";
            resultGrid.Columns[5].HeaderText = $"{s2Name}-Rate";
            resultGrid.Columns[6].HeaderText = $"{s2Name}-LastSalePrice";
            
            resultGrid.ClearSelection();
            modeLabel.Text = $"LastRefreshed: {DateTime.Now.ToString("hh:mm:ss")}";
        }
    }
}