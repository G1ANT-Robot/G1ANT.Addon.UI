using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.mouseclick",
        Tooltip = "This command clicks with proper event (left click/right click/double click) a desktop application UI element specified by WPath structure")]
    public class MouseClickCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be clicked")]
            public WPathStructure WPath { get; set; }

            [Argument(Required = false, Tooltip = "Mouse event ID. 9000 - Left click, 9001 - Right click. Default value = 9000")]
            public IntegerStructure EventId { get; set; } = new IntegerStructure(9000);

            [Argument(Required = false, Tooltip = "Position in X axis to click if AutomationElement is not found")]
            public IntegerStructure X { get; set; }

            [Argument(Required = false, Tooltip = "Position in Y axis to click if AutomationElement is not found")]
            public IntegerStructure Y { get; set; }

        }

        public MouseClickCommand(AbstractScripter scripter) : base(scripter)
        {
        }
        

        public void Execute(Arguments arguments)
        {
            var x = arguments.X?.Value;
            var y = arguments.Y?.Value;
            var eventId = arguments.EventId.Value;
            var element = UIElement.FromWPath(arguments.WPath);

            element?.MouseClick(eventId, x, y);
        }
    }
}
