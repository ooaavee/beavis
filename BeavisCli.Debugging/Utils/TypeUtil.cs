using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeavisCli.Debugging.Utils
{
    public static class TypeUtil
    {
        private static readonly Dictionary<Type, string> TypeNames = new Dictionary<System.Type, string>
        {
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(bool), "bool"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
            {typeof(char), "char"},
            {typeof(string), "string"},
            {typeof(object), "object"},
            {typeof(void), "void"}
        };

        public static string GetFriendlyName(Type type, Dictionary<Type, string> translations, bool full)
        {
            string begin = "";

            if (full)
            {
                begin = type.Namespace;

                if (!string.IsNullOrEmpty(begin))
                {
                    begin = begin + ".";
                }
                else
                {
                    begin = "";
                }
            }

            if (translations.ContainsKey(type))
            {
                return translations[type];
            }

            if (type.IsArray)
            {
                return GetFriendlyName(type.GetElementType(), translations, full) + "[]";
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return begin + GetFriendlyName(type.GetGenericArguments()[0], full) + "?";
            }

            if (type.IsGenericType)
            {
                return begin + type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x, full)).ToArray()) + ">";
            }

            return begin + type.Name;
        }

        public static string GetFriendlyName(Type type, bool full)
        {
            return GetFriendlyName(type, TypeNames, full);
        }
    }
}
