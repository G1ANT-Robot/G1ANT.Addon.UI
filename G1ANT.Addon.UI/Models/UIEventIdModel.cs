
using FlaUI.Core.Identifiers;

namespace G1ANT.Addon.UI.Models
{
    public static class UIEventIdModel
    {
        public static readonly EventId MouseLeftClick = new EventId(9000, "MouseLeftClick");
        
        public static readonly EventId MouseRightClick = new EventId(9001, "MouseRightClick");
        
        public static readonly EventId MouseDoubleClick = new EventId(9002, "MouseDoubleClick");
        
        public static readonly EventId KeyDown = new EventId(9003, "KeyDown");
        
        public static readonly EventId KeyUp = new EventId(9004, "KeyUp");
        
        public static readonly EventId KeyPress = new EventId(9005, "KeyPress");
    }
}