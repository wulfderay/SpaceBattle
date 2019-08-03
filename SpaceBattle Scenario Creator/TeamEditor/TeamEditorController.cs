using System;
using System.Windows.Controls;
using Spacebattle.Configuration.Schema;
using SpaceBattle_Scenario_Creator.Model;
using SpaceBattle_Scenario_Creator.ShipDetails;
using SpaceBattle_Scenario_Creator.TreeView;

namespace SpaceBattle_Scenario_Creator.View
{
    public class TvController
    {
        TeamModel _teamModel;

        public TeamModel TvModel
        {
            get { return _teamModel; }
            set {
                _teamModel = value;
                _tvView.Display(_teamModel);
            }
        }
        private TvView _tvView;
        private ShipDetailsView _shipDetailsView;
        private ShipSchema currentlySelectedShip;

        public TvController(TvView tvView, ShipDetailsView shipDetailsView)
        {
            _tvView = tvView;
            _tvView.OnShipSelected += _tvView_OnShipSelected;
            _shipDetailsView = shipDetailsView;
            _shipDetailsView.ShipNameChanged += shipNameChanged;
        }

        private void shipNameChanged(object sender, string shipName)
        {
            // gonna need a new controller for this.
            if (currentlySelectedShip != null)
            {
                currentlySelectedShip.Name = shipName;
                _tvView.Display(_teamModel);
            }
        }


        private void _tvView_OnShipSelected(object sender, ShipSchema shipSchema)
        {
            currentlySelectedShip = null;
            _shipDetailsView.Display(shipSchema);
            currentlySelectedShip = shipSchema;
        }
    }
}
