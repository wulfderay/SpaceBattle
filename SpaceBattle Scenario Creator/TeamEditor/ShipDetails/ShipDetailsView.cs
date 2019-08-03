using System;
using System.Linq;
using System.Windows.Controls;
using Spacebattle.Configuration.Schema;
using SpaceBattle_Scenario_Creator.TeamEditor.ShipDetails;

namespace SpaceBattle_Scenario_Creator.ShipDetails
{
    public class ShipDetailsView
    {
        public event EventHandler<string> ShipNameChanged;
        private ShipDetailsViewContext _context;

        public ShipDetailsView(ShipDetailsViewContext shipDetailsViewContext)
        {
            _context = shipDetailsViewContext;
            _context.ShipNameTextBox.TextChanged += (sender, textChangedArg) => { ShipNameChanged?.Invoke(this, _context.ShipNameTextBox.Text); };
        }

        internal void Display(ShipSchema shipSchema)
        {
            _context.ShipNameTextBox.Text = shipSchema.Name;
            ConstructShipStats();
            
        }

        private void ConstructShipStats()
        {
          // _context.ShipStatsDataGrid.
        }
    }
}