using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.getcheckstate", Tooltip = "This command returns targeted checkbox or radiobutton check state. Result can be: On, Off or Undefined")]
    public class UIGetCheckStateCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "WPath to element")]
            public UIComponentStructure WPath { get; set; }

            [Argument(Required = true, Tooltip = "Name of a variable where the command's result will be stored. Result can be: On, Off or Undefined")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public UIGetCheckStateCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = arguments.WPath.Value;
            var result = element.GetToggledState();
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new TextStructure(result.ToString()));
        }
    }
}
