using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stringify.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HideLog : Attribute
    { }
}
