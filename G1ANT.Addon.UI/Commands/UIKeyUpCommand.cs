using FlaUI.Core.Input;
using G1ANT.Addon.UI.Helpers;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.keyup", Tooltip = "This command releases key character from being held.")]
    public class UIKeyUpCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Key char to be released.")]
            public TextStructure Value { get; set; }
        }

        public UIKeyUpCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var key = VirtualKeyShortConverter.GetVirtualKeys(arguments.Value.Value);
            var keyShort = (ushort) key;
            Keyboard.ReleaseVirtualKeyCode(keyShort);
            Wait.UntilInputIsProcessed();
        }
    }
}