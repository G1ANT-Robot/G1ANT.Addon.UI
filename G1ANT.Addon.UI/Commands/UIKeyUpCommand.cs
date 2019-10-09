using System;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.keyup", Tooltip = "This command releases key character from being typed.")]
    public class UIKeyUpCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be clicked")]
            public WPathStructure WPath { get; set; }

            [Argument(Required = true, Tooltip = "Key char to be released.")]
            public TextStructure Value { get; set; }

        }

        public UIKeyUpCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = UIElement.FromWPath(arguments.WPath);            
            element.Release((VirtualKeyShort)Enum.Parse(typeof(VirtualKeyShort), arguments.Value.Value));
            Wait.UntilInputIsProcessed();
        }
    }
}
