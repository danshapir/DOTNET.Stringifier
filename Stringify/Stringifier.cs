using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#if NETSTANDARD1_3_OR_GREATER
using System.Reflection;
#endif
using FastMember;
using Stringify.Attributes;

namespace Stringify
{
    public static class Stringifier
    {
        // A cached constructor
        private static readonly ConcurrentDictionary<Type, TypeAccessor> Accessors = new ConcurrentDictionary<Type, TypeAccessor>();
        private static readonly ConcurrentDictionary<string, Type> TypeDictionary = new ConcurrentDictionary<string, Type>();

        public static string Stringify<T>(this T source)
        {
            try
            {
                if (source == null)
                {
                    return string.Empty;
                }

#if NET452_OR_GREATER
                var sourceType = source?.GetType();
				var sourceTypeInfo = sourceType;
#elif NETSTANDARD1_3_OR_GREATER
				var sourceType = source?.GetType();
				var sourceTypeInfo = sourceType?.GetTypeInfo();
#endif

				// If primitive just return it
				if (source != null 
                    && (sourceTypeInfo.IsPrimitive || sourceTypeInfo.IsValueType || sourceTypeInfo.IsPrimitive || sourceTypeInfo.IsValueType || sourceType == typeof(String))
                    && sourceType.Name != typeof(KeyValuePair<,>).Name)
                {
                    return source.ToString();
                }
                else if (source is IEnumerable)
                {
                    var enumerable = ((IEnumerable)source).Cast<object>();
                    return $"[{string.Join(",", enumerable.Select(i => $"{{{i.Stringify()}}}"))}]";
                }

                var str = string.Empty;
                var args = new List<object>();
                var index = 0;
                var accessor = GetAccessor(sourceType);

                // We are counting that if you have an attribute extension class, it will be named ORIGINAL_Attribute
                var metaType = GetType(sourceType.FullName + "_Attribute," + sourceTypeInfo.Assembly.FullName);
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

                    var propertyValue = accessor[source, property.Name];

                    // If null or should be hidden to log - don't display
                    if (propertyValue == null || NeedToBeHidden(metaProperties, property))
                    {
                        length--;  // do not count this field when need to understand if should add ", " separator
                        continue;
                    }

                    str = string.Concat(str, property.Name, "={", index++, "}");
                    if (index < length)
                    {
                        str = string.Concat(str, ", ");
                    }

                    args.Add(ObjectToString(propertyValue, metaProperty));
                }

                return string.Format(str, args.ToArray());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static string ObjectToString(Object obj, Member metaProperty = null)
        {
            if (obj != null && metaProperty != null)
            {
                if (metaProperty.IsDefined(typeof(Mask)))
                {
                    return string.Empty.PadLeft(obj.ToString().Length, 'X');
                }
            }

            if (obj is string)
            {
                return obj as string;
            }

            if (obj is ICollection)
            {
                return (obj as ICollection).Count.ToString();
            }

            if (obj is IEnumerable) // IEnumerable is higher than ICollection
            {
                return (obj as IEnumerable).Cast<object>().Count().ToString();
            }

            if (obj == null)
            {
                return string.Empty;
            }

#if NET452_OR_GREATER
                var sourceType = obj?.GetType();
				var sourceTypeInfo = sourceType;
#elif NETSTANDARD1_3_OR_GREATER
			var sourceType = obj?.GetType();
			var sourceTypeInfo = sourceType?.GetTypeInfo();
#endif

			if (sourceTypeInfo.IsPrimitive || sourceTypeInfo.IsValueType)
            {
                return obj.ToString();
            }

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

        private static bool NeedToBeHidden(MemberSet metaProperties, Member propertyToCheck)
        {
            // If meta exists, find matching property
            var metaProperty = (metaProperties == null
                ? null
                : metaProperties.FirstOrDefault(p => p.Name == propertyToCheck.Name));

            // If hide log - don't display
            return (metaProperty != null && metaProperty.IsDefined(typeof(HideLog)));
        }
    }
}
