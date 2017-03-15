using System;
using System.Linq;

namespace RocketWorks.CodeGeneration
{
    public static class TypeExtensions
    {
        public static string ToGenericTypeString(this Type t, bool fullName = false)
        {
            if (!t.IsGenericType)
                return fullName ? t.FullName : t.Name;
            string genericTypeName = fullName ? t.GetGenericTypeDefinition().FullName : t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeString(ta, fullName)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }
    }
}