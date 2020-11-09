using G1ANT.Addon.UI.Api;
using G1ANT.Language;
using System;
using System.Collections.Generic;

namespace G1ANT.Addon.UI.Structures
{
    [Structure(Name = "uipattern", Default = "", AutoCreate = false, Tooltip = "This structure describes UI pattern allows to access to extra control functionality.")]
    public class UIPatternStructure : StructureTyped<UIPatternWrapper>
    {
        public UIPatternStructure(string value, string format = "", AbstractScripter scripter = null) :
    base(value, format, scripter)
        {
        }

        public UIPatternStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
        }

        public override IList<string> Indexes
        {
            get => Value?.AvailableProperties;
        }

        public override Structure Get(string index = "")
        {
            if (string.IsNullOrWhiteSpace(index))
                return this;

            if (!Indexes.Contains(index.ToLower()))
                throw new ArgumentException($"Index {index} is not supported by UI pattern {Value.PatternName}");

            var val = Value.GetPropertyValue(index);
            return Scripter.Structures.CreateStructure(val, "", val?.GetType());
        }

        public override void Set(Structure structure, string index = null)
        {
            if (structure == null || structure.Object == null)
                throw new ArgumentNullException(nameof(structure));
            else if (string.IsNullOrWhiteSpace(index))
                throw new ArgumentException($"Cannot assign value to the pattern");
            else
                Value.SetPropertyValue(index, structure.Object);
        }

        public override string ToString(string format)
        {
            return Value?.PatternName;
        }

        protected override UIPatternWrapper Parse(string value, string format = null)
        {
            throw new ArgumentException($"Pattern cannot be created from the script");
        }

    }
}
