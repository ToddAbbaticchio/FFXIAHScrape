using FFXIAHScrape.Entities;
using FFXIAHScrape.FFXIAHScrape._SharedModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FFXIAHScrape.FFXIAHScrape.XServerArbitrage
{
    public class XServerArbitrage
    {
        readonly FFXIAhHelper _ffxiahHelper = new FFXIAhHelper();

        public async Task Action(ListBox itemList, DataGridView resultGrid, Label modeLabel, string s1Name, string s2Name)
        {
            if (s1Name.Contains("Choose Server") || s2Name.Contains("Choose Server"))
            {
                MessageBox.Show("Servers must be selected before scraping!");
                throw new Exception("Servers not selected.");
            }

            var compareList = new List<CompareInfo>();
            foreach (string item in itemList.Items)
            {
                var itemUri = _ffxiahHelper.GetItemUri(item);
                var s1 = (XServerArbitrageReturn)await _ffxiahHelper.ScrapeItem(itemUri, s1Name, Modes.XServerArbitrage);
                var s2 = (XServerArbitrageReturn)await _ffxiahHelper.ScrapeItem(itemUri, s2Name, Modes.XServerArbitrage);

                compareList.Add(new CompareInfo(s1, s2));
            }

            // add results to the resultGrid
            resultGrid.DataSource = compareList;
            resultGrid.Columns[9].Visible = false;
            resultGrid.Columns[10].Visible = false;

            // modify server specific headers with server first initial
            resultGrid.Columns[1].HeaderText = $"{s1Name.Substring(0,1)}-Stock";
            resultGrid.Columns[2].HeaderText = $"{s1Name.Substring(0, 1)}-Rate";
            resultGrid.Columns[3].HeaderText = $"{s1Name.Substring(0, 1)}-LastPrice";
            resultGrid.Columns[4].HeaderText = $"{s2Name.Substring(0, 1)}-Stock";
            resultGrid.Columns[5].HeaderText = $"{s2Name.Substring(0, 1)}-Rate";
            resultGrid.Columns[6].HeaderText = $"{s2Name.Substring(0, 1)}-LastPrice";
            resultGrid.Columns[7].HeaderText = "Profit";
            resultGrid.Columns[8].HeaderText = "Margin";

            // sort mode
            resultGrid.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
            resultGrid.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
            resultGrid.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;
            resultGrid.Columns[4].SortMode = DataGridViewColumnSortMode.Automatic;
            resultGrid.Columns[5].SortMode = DataGridViewColumnSortMode.Automatic;
            resultGrid.Columns[6].SortMode = DataGridViewColumnSortMode.Automatic;
            resultGrid.Columns[7].SortMode = DataGridViewColumnSortMode.Automatic;
            resultGrid.Columns[8].SortMode = DataGridViewColumnSortMode.Automatic;

            // fun with colors!
            foreach (DataGridViewRow row in resultGrid.Rows)
            {
                var profit = (decimal)row.Cells[9].Value;
                var margin = (decimal)row.Cells[10].Value;
                var profitable = (profit > 0) ? true : false;

                switch (profitable)
                {
                    case true when (margin < (decimal).2):
                        row.Cells[7].Style.BackColor = Color.Orange;
                        row.Cells[8].Style.BackColor = Color.Orange;
                        break;
                    case true when (margin < (decimal).5):
                        row.Cells[7].Style.BackColor = Color.LemonChiffon;
                        row.Cells[8].Style.BackColor = Color.LemonChiffon;
                        break;
                    case true when (margin > (decimal).5):
                        row.Cells[7].Style.BackColor = Color.LightGreen;
                        row.Cells[8].Style.BackColor = Color.LightGreen;
                        break;
                    case false:
                        row.Cells[7].Style.BackColor = Color.Tomato;
                        row.Cells[8].Style.BackColor = Color.Tomato;
                        break;
                }
            }

            resultGrid.ClearSelection();
            modeLabel.Text = $"LastRefreshed: {DateTime.Now.ToString("hh:mm:ss")}";
        }
    }
}