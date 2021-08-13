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

        public WPathPropertiesSelectorButton()
        {
            InitializeComponent();
            DropDownOpening += OnDropdownOpening;
            Initialize();
        }

        private void Initialize()
        {
            LoadSettings();
            foreach (var property in WPathBuilder.SearchByProperties)
                AddSearchByProperty(property.Key, property.Value);
            initialized = true;
        }

        private void SaveSettings()
        {
            Registry.SetValue(RegistryPath, typeof(WPathPropertiesSelectorButton).Name, JsonConvert.SerializeObject(WPathBuilder.SearchByProperties));
        }

        private void LoadSettings()
        {
            var value = (string)Registry.GetValue(RegistryPath, typeof(WPathPropertiesSelectorButton).Name, null);
            if (value != null)
            {
                try
                {
                    var savedProperties = JsonConvert.DeserializeObject<Dictionary<string, bool>>(value);
                    foreach (var property in savedProperties)
                        WPathBuilder.SearchByProperties[property.Key] = property.Value;
                }
                catch
                { }
            }
        }

        private void AddSearchByProperty(string name, bool isChecked)
        {
            var item = DropDownItems.Add(name) as ToolStripMenuItem;
            item.Checked = true;
            item.CheckOnClick = true;
            item.CheckState = isChecked ? CheckState.Checked : CheckState.Unchecked;
            item.Tag = name;
            item.CheckStateChanged += OnCheckStateChanged;
        }

        private void OnCheckStateChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                var nbOfSelected = WPathBuilder.SearchByProperties.Count(x => x.Value);
                if (!(item.CheckState == CheckState.Unchecked && nbOfSelected == 1))
                {
                    WPathBuilder.SearchByProperties[item.Tag.ToString()] = item.CheckState == CheckState.Checked;
                    SaveSettings();
                }
            }
        }

        private void OnDropdownOpening(object sender, EventArgs e)
        {
            if (!initialized)
                Initialize();

            foreach (ToolStripMenuItem item in DropDownItems)
                item.CheckState = WPathBuilder.SearchByProperties[item.Tag.ToString()] ? CheckState.Checked : CheckState.Unchecked;
        }

    }
}
