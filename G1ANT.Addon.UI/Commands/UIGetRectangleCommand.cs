using System.Drawing;
using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.getrectangle",
        Tooltip = "This command gets a bounding box of a desktop application UI element specified by WPath structure")]
    public class UIGetRectangleCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be located as a bounding box")]
            public UIComponentStructure WPath { get; set; }

            [Argument(Required = true, Tooltip = "Name of a variable where the command's result will be stored in rectangle structure")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public UIGetRectangleCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = arguments.WPath.Value;
            if (element != null)
            {
                var rect = element.GetRectangle();
                Scripter.Variables.SetVariableValue(arguments.Result.Value,
                    new RectangleStructure(Rectangle.FromLTRB((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom), null, Scripter));
            }
        }
    }
}
