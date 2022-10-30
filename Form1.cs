using FFXIAHScrape.Entities;
using FFXIAHScrape.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace FFXIAHScrape
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            
            var serverList1 = Servers.Info.Keys.ToList();
            serverList1.Add("-- Choose Server 1 --");
            serverList1.Sort();
            Server1Drop.DataSource = serverList1;

            var serverList2 = Servers.Info.Keys.ToList();
            serverList2.Add("-- Choose Server 2 --");
            serverList2.Sort();
            Server2Drop.DataSource = serverList2;

            var autoCompleteArray = Items.Info.Keys.ToArray();
            var source = new AutoCompleteStringCollection();
            source.AddRange(autoCompleteArray);
            TextBox.AutoCompleteCustomSource = source;
        }

        private void Server1Drop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Server1Drop.SelectedValue.ToString().Contains("Choose Server"))
                return;

            if (Server1Drop.SelectedValue == Server2Drop.SelectedValue)
            {
                MessageBox.Show($"Seems silly to compare results from {Server1Drop.SelectedValue} to results from itself, no?");
                Server1Drop.SelectedIndex = 0;
            }
        }

        private void Server2Drop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Server2Drop.SelectedValue.ToString().Contains("Choose Server"))
                return;
            
            if (Server1Drop.SelectedValue == Server2Drop.SelectedValue)
            {
                MessageBox.Show($"Seems silly to compare results from {Server2Drop.SelectedValue} to results from itself, no?");
                Server2Drop.SelectedIndex = 0;
            }
        }

        private void AddToListButton_Click(object sender, EventArgs e)
        {
            var inputString = TextBox.Text;

            if (Items.Info.ContainsKey(inputString))
            {
                if (StackCheckBox.Checked)
                {
                    ItemList.Items.Add($"{Items.Info[inputString]}/{inputString.Replace(" ", "-")}/{Constants.stack}");
                    TextBox.Text = "";
                    StackCheckBox.Checked = false;
                    return;
                }

                ItemList.Items.Add($"{Items.Info[inputString]}/{inputString.Replace(" ", "_").Replace("'", "")}");
                TextBox.Text = "";
                StackCheckBox.Checked = false;
            }
            else
            {
                MessageBox.Show("I HATE THIS STRING. NOT ALLOWED!");
                TextBox.Text = "";
            }
        }

        private void RemFromListButton_Click(object sender, EventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
            {
                MessageBox.Show("Select an item to remove before clicking remove button!");
                return;
            }
            ItemList.Items.RemoveAt(ItemList.SelectedIndex);
        }

        private void ItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
                return;

            if (ItemList.SelectedItem.ToString().Contains(Constants.stack))
            {
                StackCheckBox.Checked = true;
            }
            else
            {
                StackCheckBox.Checked = false;
            }
        }

        private void StackCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
                return;

            if (StackCheckBox.Checked && !ItemList.SelectedItem.ToString().Contains(Constants.stack))
            {
                var currVal = ItemList.SelectedItem.ToString();
                var currIndex = ItemList.SelectedIndex;
                ItemList.Items.RemoveAt(currIndex);
                ItemList.Items.Insert(currIndex, $"{currVal}/{Constants.stack}");
            }
            
            if (!StackCheckBox.Checked && ItemList.SelectedItem.ToString().Contains(Constants.stack))
            {
                var currVal = ItemList.SelectedItem.ToString();
                var currIndex = ItemList.SelectedIndex;
                ItemList.Items.RemoveAt(currIndex);
                ItemList.Items.Insert(currIndex, currVal.Replace($"/{Constants.stack}", ""));
            }
        }

        private async void GoButton_Click(object sender, EventArgs e)
        {
            if (ItemList.Items.Count == 0)
            {
                MessageBox.Show("You should add some items first, friend.");
                return;
            }

            var compareList = new List<CompareInfo>();
            foreach (string item in ItemList.Items)
            {
                var itemUri = new Uri($"{Constants.baseUrl}/{item}");
                
                //MessageBox.Show(urlEnd.ToString());
                //var s1 = await ScrapeItem(itemUri, Server1Drop.SelectedValue.ToString());
                //var s2 = await ScrapeItem(itemUri, Server2Drop.SelectedValue.ToString());

                var s1 = await WCScrapeItem(itemUri, Server1Drop.SelectedValue.ToString());
                var s2 = await WCScrapeItem(itemUri, Server2Drop.SelectedValue.ToString());

                compareList.Add(new CompareInfo()
                {
                    ItemName = s1.ItemName,
                    S1_Stock = s1.Stock,
                    S1_Rate = s1.Rate,
                    S1_Median = s1.Median,
                    S2_Stock = s2.Stock,
                    S2_Rate = s2.Rate,
                    S2_Median = s2.Median,
                });
            }
            Result1Grid.DataSource = compareList;
            Result1Grid.Columns[1].HeaderText = $"{Server1Drop.SelectedValue}-Stock";
            Result1Grid.Columns[2].HeaderText = $"{Server1Drop.SelectedValue}-Rate";
            Result1Grid.Columns[3].HeaderText = $"{Server1Drop.SelectedValue}-Median";
            Result1Grid.Columns[4].HeaderText = $"{Server2Drop.SelectedValue}-Stock";
            Result1Grid.Columns[5].HeaderText = $"{Server2Drop.SelectedValue}-Rate";
            Result1Grid.Columns[6].HeaderText = $"{Server2Drop.SelectedValue}-Median";
        }

        private async Task<PriceInfo> WCScrapeItem(Uri itemUri, string server)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("cookie", $"sid={Servers.Info[server]}");
                var result = await client.DownloadStringTaskAsync(itemUri);

                // process html data
                var _htmlDocument = new HtmlDocument();
                _htmlDocument.LoadHtml(result);

                //var searchNode = _htmlDocument.DocumentNode.SelectNodes("//*[text()[contains(., 'Pixie Hairpin')]]");
                HtmlNodeNavigator navigator = (HtmlNodeNavigator)_htmlDocument.CreateNavigator();
                var nameNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[2]/span[1]/span[1]/span[1]";
                string stockNode;
                string rateNode;
                string medianNode;
                var formatTestNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[3]/td[1]";
                if (navigator.SelectSingleNode(formatTestNode).Value == "Info")
                {
                    stockNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[4]/td[2]";
                    rateNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[5]/td[2]";
                    medianNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[6]/td[2]";
                }
                else
                {
                    stockNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[5]/td[2]";
                    rateNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[6]/td[2]";
                    medianNode = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[7]/td[2]";
                }

                return new PriceInfo
                {
                    Server = server,
                    ItemName = navigator.SelectSingleNode(nameNode).Value,
                    Stock = navigator.SelectSingleNode(stockNode).Value,
                    Rate = navigator.SelectSingleNode(rateNode).Value,
                    Median = navigator.SelectSingleNode(medianNode).Value,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*private async Task<PriceInfo> ScrapeItem(Uri itemUri, string server)
        {
            //itemUri = new Uri("https://eoxzkdr9tyug77x.m.pipedream.net");
            
            var sidCookie = new Cookie("sid", Servers.Info[server].ToString());
            CookieContainer cookieJar = new CookieContainer();
            cookieJar.Add(itemUri, sidCookie);
            HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieJar };
            HttpClient client = new HttpClient(handler, true);

            var response = await client.GetAsync(itemUri);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            //return new PriceInfo();

            // process html data
            var _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(result);

            //var searchNode = _htmlDocument.DocumentNode.SelectNodes("//*[text()[contains(., 'Pixie Hairpin')]]");
            HtmlNodeNavigator navigator = (HtmlNodeNavigator)_htmlDocument.CreateNavigator();
            var namePath = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[2]/span[1]/span[1]/span[1]";
            var stockPath = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[4]/td[2]";
            var ratePath = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[5]/td[2]";
            var medianPath = "/html[1]/body[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[6]/td[2]";

            return new PriceInfo
            {
                Server = server,
                ItemName = navigator.SelectSingleNode(namePath).Value,
                Stock = navigator.SelectSingleNode(stockPath).Value,
                Rate = navigator.SelectSingleNode(ratePath).Value,
                Median = navigator.SelectSingleNode(medianPath).Value,
            };
        }*/
    }
}