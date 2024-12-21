using System;

namespace Stringify.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class Mask : Attribute
    { }
}