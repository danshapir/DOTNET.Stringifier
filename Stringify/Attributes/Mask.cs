using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stringify.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class Mask : Attribute
    { }
}
