using RocketWorks.Entities;
using RocketWorks.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RocketWorks.CodeGeneration
{
    public class ComponentBuilder : ClassBuilder
    {
        private string generationLines = "";
        private string deserializeLines = "";
        public ComponentBuilder(Type type)
        {
            BuildImports("RocketWorks.Serialization", "System", "System.IO");
            string str = type.IsValueType ? "struct" : "class";
            BuildHeader(type.Namespace, type.ToGenericTypeString(), "IRocketable", true, str);
            name = type.Name;
            
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                ParseType(fields[i].FieldType, fields[i].Name);
            }
            BuildMethod("Rocketize", "public", "void", generationLines, "Rocketizer", "BinaryWriter");
            BuildMethod("DeRocketize", "public", "void", deserializeLines, "Rocketizer", "int", "BinaryReader");
            BuildMethod("RocketizeReference", "public", "void", "", "Rocketizer");

            BuildEnding();

            RocketLog.Log(StringBuilder.ToString());
        }

        private void ParseType(Type type, string name)
        {
            if (type.Name == "String")
            {
                /*generationLines += string.Format("byte[] {0}Bytes = System.Text.Encoding.Unicode.GetBytes({0});\n", fields[i].Name);
                generationLines += string.Format("var_rocketizer.Writer.Write({0}Bytes.Length);\n", fields[i].Name);
                generationLines += string.Format("var_rocketizer.Writer.Write({0}Bytes);", fields[i].Name);

                deserializeLines += string.Format("int {0}Length = var_rocketizer.Reader.ReadInt32();\n", fields[i].Name);
                deserializeLines += string.Format("byte[] {0}Bytes = var_rocketizer.Reader.ReadBytes({0}Length);\n", fields[i].Name);
                deserializeLines += string.Format("{0} = System.Text.Encoding.Unicode.GetString({0}Bytes);", fields[i].Name);*/
                generationLines += string.Format("var_binarywriter.Write({0});", name);
                deserializeLines += string.Format("{0} = var_binaryreader.ReadString();", name);
            }
            else if (type.Name == "DateTime")
            {
                generationLines += string.Format("ulong {0}Ticks = (ulong)({0} - new DateTime(1970, 1, 1)).TotalMilliseconds;", name);
                generationLines += string.Format("var_binarywriter.Write({0}Ticks);", name);

                deserializeLines += string.Format("{0} = new DateTime(1970, 1, 1).AddMilliseconds(var_binaryreader.ReadUInt64());", name);
            }
            else if (type.IsPrimitive)
            {
                generationLines += "var_binarywriter.Write(" + name + ");";
                string readerFunction = "Read" + type.Name;

                deserializeLines += string.Format("{1} = var_binaryreader.{0}();", readerFunction, name);
            }
            else if (type.IsArray)
            {
                int dimensions = type.GetArrayRank();
                string varName = name + "[";
                string newName = name + " =  new " + type.GetElementType().FullName + "[";
                for (int i = 0; i < dimensions; i++)
                {
                    generationLines += string.Format("var_binarywriter.Write({0}.GetLength({1}));", name, i);
                    
                    deserializeLines += string.Format("int var_{0}_length{1} = var_binaryreader.ReadInt32();", name, i);

                    varName += "i" + i;
                    newName += string.Format("var_{0}_length{1}", name, i);
                    if (i != dimensions - 1)
                    {
                        varName += ","; 
                        newName += ",";
                    }
                }
                deserializeLines += newName + "];";
                for (int i = 0; i < dimensions; i++)
                {
                    generationLines += string.Format("\nfor (int i{1} = 0; i{1} < {0}.GetLength({1}); i{1}++)", name, i);
                    generationLines += "{";

                    deserializeLines += string.Format("for (int i{1} = 0; i{1} < var_{0}_length{1}; i{1}++)", name, i);
                    deserializeLines += "{";
                }

                ParseType(type.GetElementType(), varName + "]");

                for (int i = 0; i < dimensions; i++)
                {
                    generationLines += "}";

                    deserializeLines += "}";
                }
            }
            else if (type.GetInterfaces().Contains(typeof(IList)))
            {
                generationLines += string.Format("var_binarywriter.Write({0}.Count);", name);
                generationLines += string.Format("for (int i = 0; i < {0}.Count; i++)", name);
                generationLines += "{";
                generationLines += "var_rocketizer.WriteObject(" + name + "[i], var_binarywriter);";
                generationLines += "}";

                deserializeLines += string.Format("int var_{0}_length = var_binaryreader.ReadInt32();", name);
                deserializeLines += string.Format("for (int i = 0; i < var_{0}_length; i++)", name);
                deserializeLines += "{";
                deserializeLines += string.Format("{0}.Add(var_rocketizer.ReadObject<{1}>(var_int, var_binaryreader));", name, type.GetGenericArguments()[0].FullName);
                deserializeLines += "}";
            }
            else
            {
                generationLines += "var_rocketizer.WriteObject(" + name + ", var_binarywriter);";
                deserializeLines += string.Format("{1} = var_rocketizer.ReadObject<{0}>(var_int, var_binaryreader);", type.FullName, name);
            }
        }

    }
}
