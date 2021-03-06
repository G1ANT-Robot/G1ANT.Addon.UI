using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.settext",
        Tooltip = "This command inserts text into a specified UI element of a desktop application window")]
    public class UISetTextCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application window to be referred to")]
            public UIComponentStructure WPath { get; set; }

            [Argument(Required = true, Tooltip = "Text to be inserted into a specified UI element")]
            public TextStructure Text { get; set; }
        }

        public UISetTextCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = arguments.WPath.Value;
            element.SetText(arguments.Text.Value, (int)arguments.Timeout.Value.TotalMilliseconds);
        }
    }
}
