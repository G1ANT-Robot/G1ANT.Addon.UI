using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G1ANT.Addon.UI.Api.Interfaces
{
    using CompareFunc = System.Func<G1ANT.Addon.UI.Api.UIElement, int, bool>;

    public interface IXPathUIElementFinder
    {
        UIElement FindDescendant(UIElement elem, CompareFunc compare);
        UIElement FindChild(UIElement elem, CompareFunc compare);
        UIElement FindFollowingSibling(UIElement elem, CompareFunc compare);
        UIElement FindDescendantOrSelf(UIElement elem, CompareFunc compare);
        object Number(string value);
        object EqOperator(object left, object right);
        object AndOperator(object left, object right);
        object OrOperator(object left, object right);
        object Function(string prefix, string name, IList<object> args);
    }
}
