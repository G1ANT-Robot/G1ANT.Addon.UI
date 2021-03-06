using FlaUI.Core.Identifiers;
using FlaUI.Core.Input;
using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.click",
        Tooltip = "This command clicks a desktop application UI element specified by WPath structure")]
    public class UIClickCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be clicked")]
            public UIComponentStructure WPath { get; set; }

        }

        public UIClickCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = arguments.WPath.Value;
            if(element != null)
            {
                element.Click();
                Wait.UntilInputIsProcessed();
            }
        }
    }
}
