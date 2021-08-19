using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace G1ANT.Addon.UI.Api
{
    public partial class WPathPropertiesSelectorButton : ToolStripDropDownButton
    {
        private bool initialized = false;
        const string RegistryPath = @"HKEY_CURRENT_USER\Software\G1ANT.Robot\G1ANT.Addon.UI";

        static public WPathBuilderOptions Options { get; private set; }

        public WPathPropertiesSelectorButton()
        {
            InitializeComponent();
            DropDownOpening += OnDropdownOpening;
            Initialize();
        }

        private void Initialize()
        {
            LoadSettings();

            var rootDropDown = new ToolStripMenuItem("Root");

            foreach (var property in WPathBuilder.DefaultWPathProperties)
            {
                AddMenuItem(DropDownItems, property.Key, Options.Properties);
                AddMenuItem(rootDropDown.DropDownItems, property.Key, Options.RootProperties);
            }
            DropDownItems.Add(new ToolStripSeparator());
            DropDownItems.Add(rootDropDown);

            initialized = true;
        }

        private void SaveSettings()
        {
            Registry.SetValue(RegistryPath, "WPathBuilderOptions.Properties", string.Join(",", Options.Properties));
            Registry.SetValue(RegistryPath, "WPathBuilderOptions.RootProperties", string.Join(",", Options.RootProperties));
        }

        private void LoadSettings()
        {
            if (Options != null)
                return;

             Options = new WPathBuilderOptions()
             {
                 Properties = new List<string>(),
                 RootProperties = new List<string>()
             };

            var properties = Registry.GetValue(RegistryPath, "WPathBuilderOptions.Properties", null) as string;
            var rootProperties = Registry.GetValue(RegistryPath, "WPathBuilderOptions.RootProperties", null) as string;

            InitOptionsList(Options.Properties, properties);
            InitOptionsList(Options.RootProperties, rootProperties);
        }

        private void InitOptionsList(List<string> propertiesList, string initValue)
        {
            if (initValue != null)
            {
                var list = initValue.Split(',').Concat(propertiesList).Distinct();
                propertiesList.Clear();
                propertiesList.AddRange(list);
            }
            else
            {
                foreach (var property in WPathBuilder.DefaultWPathProperties.Where(x => x.Value))
                    propertiesList.Add(property.Key);
            }
        }

        private void AddMenuItem(ToolStripItemCollection root, string name, List<string> properties)
        {
            var item = root.Add(name) as ToolStripMenuItem;
            item.Checked = true;
            item.CheckOnClick = true;
            item.CheckState = properties.Contains(name) ? CheckState.Checked : CheckState.Unchecked;
            item.Tag = properties;
            item.CheckStateChanged += OnCheckStateChanged;
        }

        private void OnCheckStateChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is List<string> properties)
            {
                var nbOfSelected = properties.Count;
                if (!(item.CheckState == CheckState.Unchecked && nbOfSelected == 1))
                {
                    if (item.CheckState == CheckState.Checked)
                        properties.Add(item.Text);
                    else
                        properties.Remove(item.Text);
                    SaveSettings();
                }
            }
        }

        private void OnDropdownOpening(object sender, EventArgs e)
        {
            if (!initialized)
                Initialize();

            CheckWPathProperties(DropDownItems);
        }

        private void CheckWPathProperties(ToolStripItemCollection items)
        {
            foreach (var item in items)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    if (menuItem.Tag is List<string> properties)
                        menuItem.CheckState = properties.Contains(menuItem.Text) ? CheckState.Checked : CheckState.Unchecked;
                    else
                        CheckWPathProperties(menuItem.DropDownItems);
                }
            }

        }
    }
}
