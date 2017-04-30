using RocketWorks.Entities;
using RocketWorks.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RocketWorks.CodeGeneration
{
    class ComponentBuilder : ClassBuilder
    {
        public ComponentBuilder(Type type)
        {
            BuildImports("RocketWorks.Serialization", "System", "System.IO");
            string str = type.IsValueType ? "struct" : "class";
            BuildHeader(type.Namespace, type.ToGenericTypeString(), "IRocketable", true, str);
            name = type.Name;
            string generationLines = "";
            string deserializeLines = "";
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType.Name == "String")
                {
                    /*generationLines += string.Format("byte[] {0}Bytes = System.Text.Encoding.Unicode.GetBytes({0});\n", fields[i].Name);
                    generationLines += string.Format("var_rocketizer.Writer.Write({0}Bytes.Length);\n", fields[i].Name);
                    generationLines += string.Format("var_rocketizer.Writer.Write({0}Bytes);", fields[i].Name);

                    deserializeLines += string.Format("int {0}Length = var_rocketizer.Reader.ReadInt32();\n", fields[i].Name);
                    deserializeLines += string.Format("byte[] {0}Bytes = var_rocketizer.Reader.ReadBytes({0}Length);\n", fields[i].Name);
                    deserializeLines += string.Format("{0} = System.Text.Encoding.Unicode.GetString({0}Bytes);", fields[i].Name);*/
                    generationLines += string.Format("var_binarywriter.Write({0});", fields[i].Name);
                    deserializeLines += string.Format("{0} = var_binaryreader.ReadString();", fields[i].Name);
                } else if (fields[i].FieldType.Name == "DateTime")
                {
                    generationLines += string.Format("ulong {0}Ticks = (ulong)({0} - new DateTime(1970, 1, 1)).TotalMilliseconds;", fields[i].Name);
                    generationLines += string.Format("var_binarywriter.Write({0}Ticks);", fields[i].Name);

                    deserializeLines += string.Format("{0} = new DateTime(1970, 1, 1).AddMilliseconds(var_binaryreader.ReadUInt64());", fields[i].Name);
                }
                else if (fields[i].FieldType.IsPrimitive)
                {
                    generationLines += "var_binarywriter.Write(" + fields[i].Name + ");";
                    string readerFunction = "Read" + fields[i].FieldType.Name;

                    deserializeLines += string.Format("{1} = var_binaryreader.{0}();", readerFunction, fields[i].Name);
                } else if (fields[i].FieldType.IsArray)
                {
                    generationLines += string.Format("var_binarywriter.Write({0}.Length);", fields[i].Name);
                } else if (fields[i].FieldType.GetInterfaces().Contains(typeof(IList)))
                {
                    generationLines += string.Format("var_binarywriter.Write({0}.Count);", fields[i].Name);
                    generationLines += string.Format("for (int i = 0; i < {0}.Count; i++)", fields[i].Name);
                    generationLines += "{";
                    generationLines += "var_rocketizer.WriteObject(" + fields[i].Name + ", var_binarywriter);";
                    generationLines += "}";

                    deserializeLines += string.Format("int var_{0}_length = var_binaryreader.ReadInt32();", fields[i].Name);
                    deserializeLines += string.Format("for (int i = 0; i < var_{0}_length; i++)", fields[i].Name);
                    deserializeLines += "{";
                    deserializeLines += string.Format("{0}.Add(var_rocketizer.ReadObject<{1}>(var_binaryreader));", fields[i].Name, fields[i].FieldType.GetGenericArguments()[0].FullName);
                    deserializeLines += "}";
                }
                else
                {
                    generationLines += "var_rocketizer.WriteObject(" + fields[i].Name + ", var_binarywriter);";
                    deserializeLines += string.Format("{1} = var_rocketizer.ReadObject<{0}>(var_binaryreader);", fields[i].FieldType.FullName, fields[i].Name);
                }
            }
            BuildMethod("Rocketize", "public", "void", generationLines, "Rocketizer", "BinaryWriter");
            BuildMethod("DeRocketize", "public", "void", deserializeLines, "Rocketizer", "BinaryReader");
            BuildMethod("RocketizeReference", "public", "void", "", "Rocketizer");

            BuildEnding();

            RocketLog.Log(StringBuilder.ToString());
        }

    }
}
