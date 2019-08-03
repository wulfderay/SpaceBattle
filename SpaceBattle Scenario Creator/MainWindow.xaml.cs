using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Spacebattle.Configuration.Schema;
using SpaceBattle_Scenario_Creator.View;
using SpaceBattle_Scenario_Creator.Model;
using SpaceBattle_Scenario_Creator.TreeView;

namespace SpaceBattle_Scenario_Creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TvController _tvController;
        string fileName;
        public MainWindow()
        {
            InitializeComponent();
            _tvController = new TvController(
                new TvView(treeView), 
                new ShipDetails.ShipDetailsView(
                    new TeamEditor.ShipDetails.ShipDetailsViewContext()
                    {
                        ShipStatsDataGrid = shipDetailsStatsGrid,
                        ShipNameTextBox = shipDetailsShipName
                    }));
        }

        private void OnFileOpenClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true)
            {
                MessageBox.Show($"Could not open file.");
                return;
            }
            string json = "";
            try
            {
                json = File.ReadAllText(openFileDialog.FileName);

            }
            catch (Exception openFileException)
            {
                MessageBox.Show($"Could not open file: {openFileException.Message}");
                return;
            }

            var shipSchemas = JsonConvert.DeserializeObject<List<ShipSchema>>(json);
            if (shipSchemas == null)
            {
                MessageBox.Show($"Could not parse json from {openFileDialog.FileName}");
                return;
            }
            fileName = openFileDialog.FileName;
            _tvController.TvModel = new TeamModel() { shipSchemas = shipSchemas };
        }

        private void OnFileCloseClicked(object sender, RoutedEventArgs e)
        {
            fileName = null;
            _tvController.TvModel = null;
        }
        private void OnFileSaveClicked(object sender, RoutedEventArgs e)
        {
            if ( fileName == null)
            {
                MessageBox.Show($"Could not save: No open file.");
                return;
            }
            var json = JsonConvert.SerializeObject(_tvController.TvModel.shipSchemas, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }
    }
}
