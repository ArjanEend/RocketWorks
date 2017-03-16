using RocketWorks.Serialization;
using System;
using System.Reflection;

namespace RocketWorks.CodeGeneration
{
    class ComponentBuilder : ClassBuilder
    {
        public ComponentBuilder(Type type)
        {
            BuildImports("RocketWorks.Serialization", "System");
            string str = type.IsValueType ? "struct" : "class";
            BuildHeader(type.Namespace, type.ToGenericTypeString(), "IRocketable", true, str);
            name = type.Name;
            string generationLines = "";
            string deserializeLines = "";
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                if(fields[i].FieldType.Name == "String")
                {
                    /*generationLines += string.Format("byte[] {0}Bytes = System.Text.Encoding.Unicode.GetBytes({0});\n", fields[i].Name);
                    generationLines += string.Format("var_rocketizer.Writer.Write({0}Bytes.Length);\n", fields[i].Name);
                    generationLines += string.Format("var_rocketizer.Writer.Write({0}Bytes);", fields[i].Name);

                    deserializeLines += string.Format("int {0}Length = var_rocketizer.Reader.ReadInt32();\n", fields[i].Name);
                    deserializeLines += string.Format("byte[] {0}Bytes = var_rocketizer.Reader.ReadBytes({0}Length);\n", fields[i].Name);
                    deserializeLines += string.Format("{0} = System.Text.Encoding.Unicode.GetString({0}Bytes);", fields[i].Name);*/
                    generationLines += string.Format("var_rocketizer.Writer.Write({0});", fields[i].Name);
                    deserializeLines += string.Format("{0} = var_rocketizer.Reader.ReadString();", fields[i].Name);
                } else if(fields[i].FieldType.Name == "DateTime")
                {
                    generationLines += string.Format("ulong {0}Ticks = (ulong)({0} - new DateTime(1970, 1, 1)).TotalMilliseconds;", fields[i].Name);
                    generationLines += string.Format("var_rocketizer.Writer.Write({0}Ticks);", fields[i].Name);

                    deserializeLines += string.Format("{0} = new DateTime(1970, 1, 1).AddMilliseconds(var_rocketizer.Reader.ReadUInt64());", fields[i].Name);
                }
                else if(fields[i].FieldType.IsPrimitive)
                {
                    generationLines += "var_rocketizer.Writer.Write(" + fields[i].Name +");";
                    string readerFunction = "Read" + fields[i].FieldType.Name;

                    deserializeLines += string.Format("{1} = var_rocketizer.Reader.{0}();", readerFunction, fields[i].Name);
                } else
                {
                    generationLines += "var_rocketizer.WriteObject(" + fields[i].Name + ");";
                    deserializeLines += string.Format("{1} = var_rocketizer.ReadObject<{0}>();", fields[i].FieldType.FullName, fields[i].Name);
                }
            }
            BuildMethod("Rocketize", "public", "void", generationLines, "Rocketizer");
            BuildMethod("DeRocketize", "public", "void", deserializeLines, "Rocketizer");
            BuildMethod("RocketizeReference", "public", "void", "", "Rocketizer");

            BuildEnding();

            RocketLog.Log(StringBuilder.ToString());
        }

    }
}
