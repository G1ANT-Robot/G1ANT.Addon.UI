using G1ANT.Addon.UI.Api;
using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace G1ANT.Addon.UI.Structures
{
    [Structure(Name = "uicomponent", Default = "", AutoCreate = false, Tooltip = "This structure stores UI element information")]
    public class UIComponentStructure : StructureTyped<UIElement>
    {
        public UIComponentStructure(string value, string format = "", AbstractScripter scripter = null) :
            base(value, format, scripter)
        {
        }

        public UIComponentStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
        }

        public override IList<string> Indexes 
        {
            get => Value.AvailableProperties;
        }

        public override Structure Get(string index = "")
        {
            if (string.IsNullOrWhiteSpace(index))
                return this;

            if (!Indexes.Contains(index.ToLower()))
                throw new ArgumentException($"Index {index} is not supported by UI component {Value.ToWPath().Value}");

            var val = Value.GetPropertyValue(index);
            if (val is List<object> list)
                return new ListStructure(list, "", Scripter);
            return Scripter.Structures.CreateStructure(val, "", val?.GetType());
        }

        public override void Set(Structure structure, string index = null)
        {
            if (structure == null || structure.Object == null)
                throw new ArgumentNullException(nameof(structure));
            else if (string.IsNullOrWhiteSpace(index))
                Value = UIElement.FromWPath(structure.ToString());
            else
                Value.SetPropertyValue(index, structure.Object);
        }

        public override string ToString(string format)
        {
            return Value.ToWPath().ToString();
        }

        protected override UIElement Parse(string value, string format = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Cannot create {Attributes.Name} from empty string");

            return UIElement.FromWPath(value) ?? throw new ParseStructureException($"Control '{value}' does not exist");
        }

    }
}
