using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.gettext",
        Tooltip = "This command gets text (name, title, label etc.) of a desktop application UI element specified by WPath structure")]
    public class UIGetTextCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to get text from")]
            public UIComponentStructure WPath { get; set; }

            [Argument(Required = true, Tooltip = "Name of a variable where the command's result will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public UIGetTextCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = arguments.WPath.Value;
            if (element != null)
            {
                var text = element.GetText();
                Scripter.Variables.SetVariableValue(arguments.Result.Value, new TextStructure(text, null, Scripter));
            }
        }
    }
}
