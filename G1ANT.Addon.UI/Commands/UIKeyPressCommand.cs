﻿using System;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using G1ANT.Addon.UI.Api;
using G1ANT.Addon.UI.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.UI.Commands
{
    [Command(Name = "ui.keypress",
        Tooltip = "This sends a key to a desktop application UI element specified by WPath structure")]
    class UIKeyPressCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Desktop application UI element to be clicked")]
            public WPathStructure WPath { get; set; }
            
            [Argument(Required = false, Tooltip = "Character be to sent into desktop application UI element ")]
            public TextStructure Value { get; set; }
        }

        public UIKeyPressCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var element = UIElement.FromWPath(arguments.WPath);
            element.SendKey((VirtualKeyShort)Enum.Parse(typeof(VirtualKeyShort), arguments.Value.Value));

            Wait.UntilInputIsProcessed();
        }
    }
}