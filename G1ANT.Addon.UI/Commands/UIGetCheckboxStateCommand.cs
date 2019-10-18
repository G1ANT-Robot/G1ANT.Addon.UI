using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.getcheckboxstate", Tooltip = "This command returns targeted checkbox's state")]
    public class UIGetCheckboxStateCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "WPath to checkbox or radiobutton")]
            public WPathStructure WPath { get; set; }

            [Argument(Required = true, Tooltip = "Name of a variable where the command's result will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public UIGetCheckboxStateCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = UIElement.FromWPath(arguments.WPath);
            if (element != null)
            {
                var result = element.GetToggledState();
                Scripter.Variables.SetVariableValue(arguments.Result.Value, new TextStructure(result.ToString()));
            }
        }
    }
}
