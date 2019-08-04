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
using SpaceBattle_Scenario_Creator.Commands;
using System.Windows.Input;

namespace SpaceBattle_Scenario_Creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DEFAULT_TITLE = "Ship Battle Scenario Creator";
        TeamEditorController _tvController;

        Stack<ICommand> _undoStack = new Stack<ICommand>();
        string fileName;
        public MainWindow()
        {
            InitializeComponent();
            Title = DEFAULT_TITLE;
            TvView tview = new TvView(treeView);
            DataContext = this;
            InputBindings.Add(new KeyBinding(new ActionCommand(() => OnUndo(this, null)), Key.Z, ModifierKeys.Control));
            _tvController = new TeamEditorController(
                tview, 
                new ShipDetails.ShipDetailsView(
                    new TeamEditor.ShipDetails.ShipDetailsViewContext()
                    {
                        ShipStatsDataGrid = shipDetailsStatsGrid,
                        ShipNameTextBox = shipDetailsShipName
                    }), 
                _undoStack);
        }

        private void OnUndo(object sender, RoutedEventArgs e)
        {
            if (_undoStack.Count > 0)
                _undoStack.Pop().Execute(null);
        }

        private void OnFileOpenClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true)
            {
                // user exited. No need to tell them we can't open file.
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
            Title = fileName;
            _tvController.TeamModel = new TeamModel() { shipSchemas = shipSchemas };
        }

        private void OnFileCloseClicked(object sender, RoutedEventArgs e)
        {
            fileName = null;
            Title = DEFAULT_TITLE;
            _tvController.TeamModel = null;
        }
        private void OnFileSaveClicked(object sender, RoutedEventArgs e)
        {
            if ( fileName == null)
            {
                MessageBox.Show($"Could not save: No open file.");
                return;
            }
            var json = JsonConvert.SerializeObject(_tvController.TeamModel.shipSchemas, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        private void OnFileSaveAsClicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if ( dialog.ShowDialog() != true)
            {
                // no need to say anything: user cancelled.
                return;
            }
            fileName = dialog.FileName;

            var json = JsonConvert.SerializeObject(_tvController.TeamModel.shipSchemas, Formatting.Indented);
            File.WriteAllText(fileName, json); // need a proper exception around here.
            Title = fileName;
        }
    }
}
