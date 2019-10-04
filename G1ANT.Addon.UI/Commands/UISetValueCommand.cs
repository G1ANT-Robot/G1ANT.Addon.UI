using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.setvalue",
        Tooltip = "This command set new value for a desktop application UI element specified by WPath structure")]
    public class UISetValueCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element in which new value will be set.")]
            public WPathStructure WPath { get; set; }

            [Argument(Required = true, Tooltip = "Desktop application UI element's control type.")]
            public IntegerStructure ControlTypeId { get; set; } = new IntegerStructure();

            [Argument(Required = true, Tooltip = "New value to be set.")]
            public TextStructure Value { get; set; } = new TextStructure();


        }

        public UISetValueCommand(AbstractScripter scripter) : base(scripter)
        {
        }


        public void Execute(Arguments arguments)
        {
            var element = UIElement.FromWPath(arguments.WPath);
            element?.SetValue(arguments.ControlTypeId.Value, arguments.Value.Value);
        }
    }
}
