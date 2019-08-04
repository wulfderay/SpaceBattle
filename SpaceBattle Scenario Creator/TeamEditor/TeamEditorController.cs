using Spacebattle.Configuration.Schema;
using SpaceBattle_Scenario_Creator.Commands;
using SpaceBattle_Scenario_Creator.Model;
using SpaceBattle_Scenario_Creator.ShipDetails;
using SpaceBattle_Scenario_Creator.TreeView;
using System.Collections.Generic;
using System.Windows.Input;

namespace SpaceBattle_Scenario_Creator.View
{
    public class TeamEditorController
    {
        TeamModel _teamModel;

        public TeamModel TeamModel
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
        private Stack<ICommand> _undoStack;

        public TeamEditorController(TvView tvView, ShipDetailsView shipDetailsView, Stack<ICommand> undoStack)
        {
            _tvView = tvView;
            _tvView.OnShipSelected += _tvView_OnShipSelected;
            _shipDetailsView = shipDetailsView;
            _shipDetailsView.ShipNameChanged += shipNameChanged;
            _undoStack = undoStack;
            _tvView.OnDeleteCrewDeck += _tvView_OnDeleteCrewDeck;
            _tvView.OnDeleteEngine += _tvView_OnDeleteEngine;
            _tvView.OnDeleteReactor += _tvView_OnDeleteReactor;
            _tvView.OnDeleteShield += _tvView_OnDeleteShield;
            _tvView.OnDeleteWeapon += _tvView_OnDeleteWeapon;


        }
        
        private void OnDelete<T>(List<T> list, T item)
        {
            if (list == null|| !list.Contains(item))
                return;
            var index = list.IndexOf(item);
            list.Remove(item);
            _undoStack.Push(new ActionCommand(() =>
            {
                list.Insert(index, item);
                _tvView.Display(_teamModel);
            }));
            _tvView.Display(_teamModel);
        }

        private void _tvView_OnDeleteWeapon(WeaponSchema obj)
        {
            if (currentlySelectedShip == null)
                return;
            OnDelete(currentlySelectedShip.Weapons, obj);
        }

        private void _tvView_OnDeleteReactor(ReactorSchema obj)
        {
            if (currentlySelectedShip == null)
                return;
            OnDelete(currentlySelectedShip.Reactors, obj);

        }

        private void _tvView_OnDeleteShield(ShieldSchema obj)
        {
            if (currentlySelectedShip == null)
                return;
            OnDelete(currentlySelectedShip.Shields, obj);

        }

        private void _tvView_OnDeleteEngine(EngineSchema obj)
        {
            if (currentlySelectedShip == null)
                return;
            OnDelete(currentlySelectedShip.Engines, obj);
        }

        private void _tvView_OnDeleteCrewDeck(CrewDeckSchema obj)
        {
            if (currentlySelectedShip == null)
                return;
            OnDelete(currentlySelectedShip.CrewDecks, obj);
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
