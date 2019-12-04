using FlaUI.Core.Input;
using G1ANT.Addon.UI.Helpers;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.keypress", Tooltip = "This sends a key to a desktop application UI element specified by WPath structure")]
    class UIKeyPressCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = false, Tooltip = "Character be to sent into desktop application UI element ")]
            public TextStructure Value { get; set; }
        }

        public UIKeyPressCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var key = VirtualKeyShortConverter.GetVirtualKeys(arguments.Value.Value);
            var keyShort = (ushort)key;
            Keyboard.TypeVirtualKeyCode(keyShort);
            Wait.UntilInputIsProcessed();
        }
    }
}
