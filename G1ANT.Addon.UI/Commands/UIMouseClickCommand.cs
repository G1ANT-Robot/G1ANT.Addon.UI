using System;
using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Enums;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.mouseclick",
        Tooltip = "This command clicks with proper event (left click/right click/double click) a desktop application UI element specified by WPath structure")]
    public class UIMouseClickCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be clicked")]
            public WPathStructure WPath { get; set; }

            [Argument(Required = false, Tooltip = "Mouse event type.")]
            public TextStructure EventType { get; set; } = new TextStructure(9000);

            [Argument(Required = false, Tooltip = "Position in X axis to click if AutomationElement is not found")]
            public IntegerStructure X { get; set; }

            [Argument(Required = false, Tooltip = "Position in Y axis to click if AutomationElement is not found")]
            public IntegerStructure Y { get; set; }

        }

        public UIMouseClickCommand(AbstractScripter scripter) : base(scripter)
        {
        }
        

        public void Execute(Arguments arguments)
        {
            var x = arguments.X?.Value;
            var y = arguments.Y?.Value;
            var eventId = EventTypes.MouseLeftClick;
            if (arguments.EventType.Value != null)
            {
                eventId = (EventTypes)Enum.Parse(typeof(EventTypes), arguments.EventType.Value);
            }
            
            var element = UIElement.FromWPath(arguments.WPath);

            element?.MouseClick(eventId, x, y);
        }
    }
}
