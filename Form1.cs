﻿using FFXIAHScrape.Entities;
using FFXIAHScrape.FFXIAHScrape._SharedModels;
using FFXIAHScrape.FFXIAHScrape.RareItemAlert;
using FFXIAHScrape.FFXIAHScrape.UndercutAlert;
using FFXIAHScrape.FFXIAHScrape.XServerArbitrage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FFXIAHScrape
{
    public partial class Form1 : Form
    {
        public bool AutoRefreshMode { get; set; }
        public int RefreshTimer { get; set; }

        readonly UndercutAlert _undercutAlert = new UndercutAlert();
        readonly RareItemAlert _rareItemAlert = new RareItemAlert();
        readonly XServerArbitrage _xServerArbitrage = new XServerArbitrage();

        public Form1()
        {
            InitializeComponent();

            ModeDrop.DataSource = Enum.GetValues(typeof(Modes));

            var autoCompleteArray = Items.Info.Keys.ToArray();
            var source = new AutoCompleteStringCollection();
            source.AddRange(autoCompleteArray);

            TextBox.AutoCompleteCustomSource = source;

            var serverList1 = Servers.Info.Keys.ToList();
            serverList1.Add("-- Choose Server 1 --");
            serverList1.Sort();
            Server1Drop.DataSource = serverList1;

            var serverList2 = Servers.Info.Keys.ToList();
            serverList2.Add("-- Choose Server 2 --");
            serverList2.Sort();
            Server2Drop.DataSource = serverList2;

            ItemList.AllowDrop = true;
            ItemList.DragDrop += ItemList_DragDrop;
            ItemList.DragEnter += ItemList_DragEnter;
        }

        #region Field Updates
        private void ModeDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            var stackCheckPos1 = new System.Drawing.Point(172, 107);
            var stackCheckPos2 = new System.Drawing.Point(164, 130);
            var goButtonPos1 = new System.Drawing.Point(139, 139);
            var goButtonPos2 = new System.Drawing.Point(142, 153);

            switch (ModeDrop.SelectedValue)
            {
                case Modes.RareItemAlert:
                    Server2Drop.SelectedIndex = -1;
                    Server2Drop.Hide();
                    ListPrice.Hide();
                    StackCheckBox.Location = stackCheckPos1;
                    GoButton.Location = goButtonPos1;
                    RefreshTimer = 60;
                    ListPrice.Text = "";
                    break;
                case Modes.UndercutAlert:
                    Server2Drop.SelectedIndex = -1;
                    Server2Drop.Hide();
                    ListPrice.Show();
                    ListPrice.Text = "Enter Listed Price...";
                    StackCheckBox.Location = stackCheckPos2;
                    GoButton.Location = goButtonPos2;
                    RefreshTimer = 300;
                    break;
                case Modes.XServerArbitrage:
                    Server2Drop.Show();
                    ListPrice.Hide();
                    StackCheckBox.Location = stackCheckPos1;
                    GoButton.Location = goButtonPos1;
                    Server2Drop.SelectedIndex = 0;
                    RefreshTimer = 3600;
                    ListPrice.Text = "";
                    break;
            }

            GoButton.Text = "Check FFXI-AH";
            AutoRefreshMode = false;
            ModeDisplay.Text = "";
            StackCheckBox.Checked = false;
            ItemList.Items.Clear();
            Result1Grid.DataSource = null;
        }

        private void Server1Drop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Server1Drop.SelectedIndex == -1 || Server1Drop.SelectedValue.ToString().Contains("Choose Server"))
                return;

            if (Server1Drop.SelectedValue == Server2Drop.SelectedValue)
            {
                MessageBox.Show($"Seems silly to compare results from {Server1Drop.SelectedValue} to results from itself, no?");
                Server1Drop.SelectedIndex = 0;
            }
        }

        private void Server2Drop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Server2Drop.SelectedIndex == -1 || Server2Drop.SelectedValue.ToString().Contains("Choose Server"))
                return;
            
            if (Server1Drop.SelectedValue == Server2Drop.SelectedValue)
            {
                MessageBox.Show($"Seems silly to compare results from {Server2Drop.SelectedValue} to results from itself, no?");
                Server2Drop.SelectedIndex = 0;
            }
        }

        private void ItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
                return;

            var selectedItemInfo = JsonConvert.DeserializeObject<ItemListInfoBase>(ItemList.SelectedItem.ToString());
            if (selectedItemInfo.Stack == true)
            {
                StackCheckBox.Checked = true;
            }
            else
            {
                StackCheckBox.Checked = false;
            }
        }

        private void ItemList_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // Handle FileDrop data.
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    return;
                }
                else
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length > 1)
                    {
                        MessageBox.Show("One file at a time please!");
                        return;
                    }
                    if (Path.GetExtension(files[0]) != ".json")
                    {
                        MessageBox.Show($"The provided file is a {Path.GetExtension(files[0])}, please provide a .json file");
                        return;
                    }
                    foreach (var mode in Enum.GetValues(typeof(Modes)))
                    {
                        var modeText = mode.ToString();
                        var fName = Path.GetFileName(files[0]);
                        var modeDrop = ModeDrop.SelectedValue.ToString();

                        if (fName.Contains(modeText) && modeDrop == modeText)
                            break;

                        if (fName.Contains(modeText) && modeDrop != modeText)
                        {
                            MessageBox.Show($"This file is for {modeText} but the current mode is set to {modeDrop}");
                            return;
                        }
                    }

                    var droppedFilePath = files[0];
                    string droppedFileName = Path.GetFileName(files[0]);

                    DialogResult dialogResult;
                    dialogResult = MessageBox.Show($"Import: {droppedFilePath}?", "Import item list from file?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        var fileStream = new FileStream(droppedFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        var streamReader = new StreamReader(fileStream, Encoding.Default);
                        var allData = streamReader.ReadToEnd();
                        var lines = Regex.Split(allData, "\n").ToList();
                        fileStream.Close();
                        streamReader.Close();

                        ItemList.Items.Clear();
                        foreach (var line in lines)
                        {
                            if (string.IsNullOrEmpty(line))
                                continue;
                            ItemList.Items.Add(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing file list!{Environment.NewLine}{ex.Message}");
            }
        }

        private void ItemList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void StackCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
                return;

            var itemInfo = JsonConvert.DeserializeObject<ItemListInfoBase>(ItemList.SelectedItem.ToString());

            switch (ModeDrop.SelectedValue)
            {
                case Modes.UndercutAlert:
                    itemInfo = JsonConvert.DeserializeObject<UndercutAlertItemInfo>(ItemList.SelectedItem.ToString());
                    break;
            }

            if (StackCheckBox.Checked && itemInfo.Stack == false)
            {
                var currIndex = ItemList.SelectedIndex;
                ItemList.Items.RemoveAt(currIndex);
                itemInfo.Stack = true;
                ItemList.Items.Insert(currIndex, JsonConvert.SerializeObject(itemInfo));
            }

            if (!StackCheckBox.Checked && itemInfo.Stack == true)
            {
                var currIndex = ItemList.SelectedIndex;
                ItemList.Items.RemoveAt(currIndex);
                itemInfo.Stack = false;
                ItemList.Items.Insert(currIndex, JsonConvert.SerializeObject(itemInfo));
            }
        }
        
        #endregion

        #region Button Clicks
        private void AddToListButton_Click(object sender, EventArgs e)
        {
            var inputString = TextBox.Text;

            if (Items.Info.ContainsKey(inputString))
            {
                switch (ModeDrop.SelectedValue)
                {
                    case Modes.RareItemAlert:
                        ItemList.Items.Add(JsonConvert.SerializeObject(
                            new
                            {
                                Server = Server1Drop.SelectedValue.ToString(),
                                Item = TextBox.Text.Replace(" ", "-").Replace("'", ""),
                                Id = Items.Info[TextBox.Text],
                                Stack = StackCheckBox.Checked,
                            }
                        ));
                        break;

                    case Modes.UndercutAlert:
                        var validPrice = int.TryParse(ListPrice.Text, out var listPrice);
                        if (!validPrice)
                        {
                            MessageBox.Show($"Enter a valid number for your list price. {ListPrice.Text} is a nogo!");
                            return;
                        }

                        ItemList.Items.Add(JsonConvert.SerializeObject(
                            new
                            {
                                Server = Server1Drop.SelectedValue.ToString(),
                                Item = TextBox.Text.Replace(" ", "-").Replace("'", ""),
                                Id = Items.Info[TextBox.Text],
                                Stack = StackCheckBox.Checked,
                                MyPrice = listPrice,
                            }
                        ));
                        break;

                    case Modes.XServerArbitrage:
                        ItemList.Items.Add(JsonConvert.SerializeObject(
                            new
                            {
                                Item = TextBox.Text.Replace(" ", "-").Replace("'", ""),
                                Id = Items.Info[TextBox.Text],
                                Stack = StackCheckBox.Checked,
                            }
                        ));
                        break;
                }

                TextBox.Text = "";
                ListPrice.Text = "";
                StackCheckBox.Checked = false;
            }
            else
            {
                MessageBox.Show("THIS ISNT A REAL ITEM. NOT ALLOWED!");
                TextBox.Text = "";
                ListPrice.Text = "";
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

        private void SaveListButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog exportItemList = new SaveFileDialog();
            exportItemList.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            exportItemList.Title = "Export Item List";
            exportItemList.CheckFileExists = false;
            exportItemList.CheckPathExists = true;
            exportItemList.DefaultExt = "json";
            exportItemList.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            exportItemList.FilterIndex = 2;
            exportItemList.RestoreDirectory = true;
            if (exportItemList.ShowDialog() == DialogResult.OK)
            {
                string finalName;
                try
                {
                    var itemList = new List<string>();
                    foreach (var item in ItemList.Items)
                    {
                        itemList.Add(item.ToString());
                    }

                    var mode = ModeDrop.SelectedValue.ToString();
                    var mode1 = ModeDrop.SelectedText;
                    var mode2 = ModeDrop;
                    var fileName = Path.GetFileName(exportItemList.FileName);
                    finalName = exportItemList.FileName.Replace(fileName, $"{ModeDrop.SelectedValue}-{fileName}");
                    
                    File.WriteAllLines(finalName, itemList);
                    MessageBox.Show($"Item list exported to: {finalName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting item list: {ex.Message}");
                }
            }
        }

        private async void GoButton_Click(object sender, EventArgs e)
        {
            if (ItemList.Items.Count == 0) return;
            if (!AutoRefreshMode)
            {
                try
                {
                    AutoRefreshMode = true;
                    GoButton.Text = "Stop Scraping";

                    while (AutoRefreshMode == true)
                    {
                        switch (ModeDrop.SelectedValue)
                        {
                            case Modes.RareItemAlert:
                                await _rareItemAlert.Action(ItemList, Result1Grid, ModeDisplay);
                                break;
                            case Modes.UndercutAlert:
                                await _undercutAlert.Action(ItemList, Result1Grid, ModeDisplay);
                                break;

                            case Modes.XServerArbitrage:
                                await _xServerArbitrage.Action(ItemList, Result1Grid, ModeDisplay, Server1Drop.SelectedValue.ToString(), Server2Drop.SelectedValue.ToString());
                                break;
                        }
                        BetterWaitWithoutLockingUi(TimeSpan.FromMinutes(5));
                    }
                }
                catch
                {
                    AutoRefreshMode = false;
                    GoButton.Text = "Check FFXI-AH";
                }
            }
            else
            {
                AutoRefreshMode = false;
                GoButton.Text = "Check FFXI-AH";
            }
            ModeDisplay.Text = "";
        }
        
        #endregion

        public void BetterWaitWithoutLockingUi(TimeSpan length)
        {
            DateTime start = DateTime.Now;
            TimeSpan restTime = new TimeSpan(200000); // 20 milliseconds
            while (true)
            {
                System.Windows.Forms.Application.DoEvents();
                TimeSpan remainingTime = start.Add(length).Subtract(DateTime.Now);
                if (remainingTime > restTime)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("1: {0}", remainingTime));
                    // Wait an insignificant amount of time so that the
                    // CPU usage doesn't hit the roof while we wait.
                    System.Threading.Thread.Sleep(restTime);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("2: {0}", remainingTime));
                    if (remainingTime.Ticks > 0)
                        System.Threading.Thread.Sleep(remainingTime);
                    break;
                }
            }
        }

    }
}