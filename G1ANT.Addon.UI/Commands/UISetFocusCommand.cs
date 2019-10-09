using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.setfocus",
        Tooltip = "This command sets focus on a UI element of a desktop application specified by WPath structure")]
    public class UISetFocusCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be focused on")]
            public WPathStructure WPath { get; set; }
        }

        public UISetFocusCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = UIElement.FromWPath(arguments.WPath);
            element?.SetFocus();
        }
    }
}
