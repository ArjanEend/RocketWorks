using RocketWorks.Serialization;
using System;
using System.Reflection;

namespace RocketWorks.CodeGeneration
{
    class ComponentBuilder : ClassBuilder
    {
        public ComponentBuilder(Type type)
        {

            /*
            void Rocketize(Rocketizer rocketizer);
            void DeRocketize(Rocketizer rocketizer);
            void RocketizeReference(Rocketizer rocketizer);*/
            BuildImports("RocketWorks.Serialization");
            BuildHeader(type.Namespace, type.Name, "IRocketable");
            string generationLines = "";
            string deserializeLines = "";
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if(fields[i].FieldType.Name == "String")
                {
                    generationLines += string.Format("byte[] {0}Bytes = System.Text.Encoding.Unicode.GetBytes({0});", fields[i].Name);
                    generationLines += string.Format("var_rocketizer.Writer.Write({0}Bytes.Length);", fields[i].Name);
                    generationLines += string.Format("var_rocketizer.Writer.Write({0}Bytes);", fields[i].Name);
                }
                else if(fields[i].FieldType.IsPrimitive)
                {
                    generationLines += "var_rocketizer.Writer.Write(" + fields[i].Name +");";
                    string readerFunction = "Read" + fields[i].FieldType.Name;

                    deserializeLines += string.Format("var_rocketizer.Reader.{0}({1});", readerFunction, fields[i].Name);
                } else
                {
                    generationLines += "var_rocketizer.WriteObject(" + fields[i].Name + ");";
                }
            }
            BuildMethod("Rocketize", "void", generationLines, "Rocketizer");
            BuildMethod("DeRocketize", "void", deserializeLines, "Rocketizer");
            BuildMethod("RocketizeReference", "void", "", "Rocketizer");

            BuildEnding();

            RocketLog.Log(StringBuilder.ToString());
        }

    }
}
