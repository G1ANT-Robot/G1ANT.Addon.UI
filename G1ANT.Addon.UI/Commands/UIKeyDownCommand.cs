using System;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.keydown", Tooltip = "This command start holding key character.")]
    public class UIKeyDownCommand : Command
    {
        public class Arguments : CommandArguments
        {

            [Argument(Required = true, Tooltip = "Key char to be held.")]
            public TextStructure Value { get; set; }

        }

        public UIKeyDownCommand(AbstractScripter scripter) : base(scripter)
        {

        }

        public void Execute(Arguments arguments)
        {
            var keyShort = (VirtualKeyShort) Enum.Parse(typeof(VirtualKeyShort), arguments.Value.Value);
            Keyboard.PressVirtualKeyCode((ushort)keyShort);
            Wait.UntilInputIsProcessed();
        }
    }
}
