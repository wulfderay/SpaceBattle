using Spacebattle.Configuration.Schema;
using SpaceBattle_Scenario_Creator.Commands;
using SpaceBattle_Scenario_Creator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpaceBattle_Scenario_Creator.TreeView
{
    public class TvView
    {
        private System.Windows.Controls.TreeView _target;

        public event EventHandler<ShipSchema> OnShipSelected;

        public event Action<ReactorSchema> OnDeleteReactor;
        public event Action<EngineSchema> OnDeleteEngine;
        public event Action<CrewDeckSchema> OnDeleteCrewDeck;
        public event Action<ShieldSchema> OnDeleteShield;
        public event Action<WeaponSchema> OnDeleteWeapon;


        public TvView(System.Windows.Controls.TreeView target)
        {
            _target = target;
        }
        private TreeViewItem GetTreeForShip(ShipSchema shipSchema)
        {
            var shipParent = new TreeViewItem();
            shipParent.Header = shipSchema.Name;
            shipParent.GotFocus += (sender, arg) => OnShipSelected?.Invoke(sender, shipSchema);
            shipParent.IsExpanded = true;
            var reactorsParent = GetTreeForSchemas(shipSchema.Reactors.Select(item=> item), "Reactors", OnDeleteReactor);
            var enginesParent = GetTreeForSchemas(shipSchema.Engines.Select(item => item), "Engines", OnDeleteEngine);
            var shieldsParent = GetTreeForSchemas(shipSchema.Shields.Select(item => item), "Shields", OnDeleteShield);
            var weaponsParent = GetTreeForSchemas(shipSchema.Weapons.Select(item => item), "Weapons", OnDeleteWeapon);
            var crewdecksParent = GetTreeForSchemas(shipSchema.CrewDecks.Select(item => item), "CrewDecks", OnDeleteCrewDeck);

            shipParent.Items.Add(reactorsParent);
            shipParent.Items.Add(shieldsParent);
            shipParent.Items.Add(weaponsParent);
            shipParent.Items.Add(enginesParent);
            shipParent.Items.Add(crewdecksParent);
            return shipParent;
        }

        private TreeViewItem GetTreeForSchemas<T>(IEnumerable<T> schemas, string rootName, Action<T> OnDeleteAction)
        {
            
            var parent = new TreeViewItem();
            parent.Header = rootName;
            parent.IsExpanded = true;
            foreach (var schema in schemas)
            {
                var rootItem = new TreeViewItem();
                
                rootItem.InputBindings.Add(new KeyBinding(new ActionCommand(() => { OnDeleteAction?.Invoke(schema); }), Key.Delete, ModifierKeys.None));
                PropertyInfo[] properties = schema.GetType().GetProperties();

                foreach (var property in properties)
                {
                    // if (property.PropertyType == typeof(string) && property.GetValue(this) != null)
                    if (property.Name == "Name")
                    {
                        rootItem.Header = property.GetValue(schema).ToString();
                        continue;
                    }
                    var propertyParent = new TreeViewItem();
                    propertyParent.Header = property.Name;
                    var propertyChild = new TreeViewItem();
                    var value = property.GetValue(schema) != null ? property.GetValue(schema) : null;
                    propertyChild.Header = value;
                    propertyParent.Items.Add(propertyChild);
                    if ( value != null)
                        rootItem.Items.Add(propertyParent);


                }
                parent.Items.Add(rootItem);
            }
            return parent;
        }


        public void Display(TeamModel model)
        {
            _target.Items.Clear();
            if (model == null)
                return;
            foreach (var shipSchema in model.shipSchemas)
            {
                _target.Items.Add(GetTreeForShip(shipSchema));
            }
        }
    }
}
