using System;
using System.Windows.Forms;
using FlaUI.Core.WindowsAPI;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Helpers
{
    public static class VirtualKeyShortConverter
    {
        public static VirtualKeyShort GetVirtualKeys(string keyValue)
        {
            var value = KeyStr.ToModifierKey(keyValue);
            if (value == ModifierKeys.None)
            {
                value = KeyStr.ToModifierKey(keyValue + "+");
                if (value == ModifierKeys.None)
                {
                    var singleKey = KeyStr.ToSingleKey(keyValue).ToString();
                    return (VirtualKeyShort) (Keys) Enum.Parse(typeof(Keys), singleKey);
                }
            }

            var keys = (Keys) Enum.Parse(typeof(Keys), value.ToString());
            return (VirtualKeyShort) Enum.Parse(typeof(VirtualKeyShort), keys.ToString(), true);
        }
    }
}