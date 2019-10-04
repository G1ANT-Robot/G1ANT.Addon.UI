using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.sendkey",
        Tooltip = "This sends a key to a desktop application UI element specified by WPath structure")]
    class UISendKeyCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be clicked")]
            public WPathStructure WPath { get; set; }

            //TODO! - tooltip
            [Argument(Required = false, Tooltip = "Mouse event ID.")]
            public IntegerStructure EventId { get; set; } = new IntegerStructure(9000);

            [Argument(Required = false, Tooltip = "Characters to send into desktop application UI element ")]
            public TextStructure Key { get; set; }
        }

        public UISendKeyCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = UIElement.FromWPath(arguments.WPath);
            element?.Click();
        }
    }
}
