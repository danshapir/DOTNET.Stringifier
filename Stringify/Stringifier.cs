using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using FastMember;
using Stringify.Attributes;

namespace Stringify
{
    public static class Stringifier
    {

        private static ILog _logger = null;

        static Stringifier()
        {
            try
            {
                _logger = LogManager.GetLogger(typeof(Stringifier));
            }
            catch (Exception ex)
            {

            }
        }

        // A cached constructor
        private static readonly ConcurrentDictionary<Type, TypeAccessor> Accessors = new ConcurrentDictionary<Type, TypeAccessor>();
        private static readonly ConcurrentDictionary<string, Type> TypeDictionary = new ConcurrentDictionary<string, Type>();

        public static string Stringify<T>(this T source)
        {
            try
            {
                // If primitive just return it
                if (source != null && (typeof(T).IsPrimitive || typeof(T).IsValueType)) return source.ToString();

                var str = string.Empty;
                var args = new List<object>();
                var index = 0;
                var sourceType = source.GetType();
                var accessor = GetAccessor(sourceType);

                // We are counting that if you have an attribute extension class, it will be named ORIGINAL_Attribute
                var metaType = GetType(sourceType.FullName + "_Attribute," + sourceType.Assembly.FullName);
                if (metaType == null)
                {
                    metaType = sourceType;
                }
                var metaAccessor = (metaType == null ? null : GetAccessor(metaType));
                var properties = accessor.GetMembers();

                // Get the meta properties
                var metaProperties = (metaAccessor == null ? null : metaAccessor.GetMembers());
                var length = properties.Count;

                foreach (var property in properties)
                {
                    // If meta exists, find matching property
                    var metaProperty = (metaProperties == null
                        ? null
                        : metaProperties.FirstOrDefault(p => p.Name == property.Name));

                    // If hide log - don't display
                    if (metaProperty != null && metaProperty.IsDefined(typeof(HideLog))) continue;

                    // If Encrypted - encrypt
                    var propertyValue = accessor[source, property.Name];

                    if (propertyValue != null)
                    {

                        str += property.Name + "={" + index++ + "}";
                        if (index != length)
                        {
                            str += ", ";
                        }

                        args.Add(ObjectToString(propertyValue, metaProperty));
                    }
                }

                return String.Format(str, args.ToArray());
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
                return string.Empty;
            }
        }

        private static string ObjectToString(Object obj, Member metaProperty = null)
        {
            if (obj != null && metaProperty != null)
            {
                if (metaProperty.IsDefined(typeof(Mask))) return string.Empty.PadLeft(obj.ToString().Length, 'X');
            }

            if (obj is string) return obj as string;

            if (obj is ICollection) return (obj as ICollection).Count.ToString();

            if (obj == null) return string.Empty;

            if (obj.GetType().IsPrimitive || obj.GetType().IsValueType) return obj.ToString();

            return "{" + Stringify(obj) + "}";
        }

        private static TypeAccessor GetAccessor(Type type)
        {
            return Accessors.GetOrAdd(type, TypeAccessor.Create(type));
        }

        private static Type GetType(string type)
        {
            return TypeDictionary.GetOrAdd(type, Type.GetType(type));
        }

    }
}
