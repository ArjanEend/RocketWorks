using System.Collections.Generic;
using System;
using System.Text;

public class ClassBuilder
{
    private StringBuilder stringBuilder;
    public StringBuilder StringBuilder
    {
        get { return stringBuilder; }
    }

    private bool hasNamespace = false;

    private string name = "";
    public string Name { get { return name; } }

    public ClassBuilder()
    {
        stringBuilder = new StringBuilder();
    }

    protected void BuildImports(params string[] extras)
    {
        for(int i = 0; i < extras.Length; i++)
        {
            stringBuilder.AppendLine("using " + extras[i] + ";\n");
        }
    }


    protected void BuildHeader(string nameSpace, string className, string baseClass, bool partial = false, string classOrStruct = "class")
    {
        stringBuilder.AppendLine("////// AUTO GENERATED ////////");
        hasNamespace = !string.IsNullOrEmpty(nameSpace);
        if (hasNamespace)
            stringBuilder.AppendLine(string.Format("namespace {0}", nameSpace)).Append("{");
        if(string.IsNullOrEmpty(baseClass))
            stringBuilder.AppendLine(string.Format("public {2}{3} {0}{1}", className, baseClass, partial ? "partial " : "", classOrStruct));
        else
            stringBuilder.AppendLine(string.Format("public {2}{3} {0} : {1}", className, baseClass, partial ? "partial " : "", classOrStruct));
        name = className;
        stringBuilder.AppendLine("{");
    }

    protected void BuildMethod(string methodName, string returnName, string lines, params string[] methodTypes)
    {
        stringBuilder.Append("public " + returnName + " " + methodName + "(");
        for(int i = 0; i < methodTypes.Length; i++)
        {
            stringBuilder.Append(methodTypes[i] + " var_" + methodTypes[i].ToLower());
        }
        stringBuilder.Append(")\n");
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine(lines);
        stringBuilder.AppendLine("}");
    }

    protected void BuildConstructor(string lines, params string[] methodTypes)
    {
        stringBuilder.Append("public " + name + "(");
        for (int i = 0; i < methodTypes.Length; i++)
        {
            stringBuilder.Append(methodTypes[i] + " var_" + methodTypes[i].ToLower());
        }
        stringBuilder.Append(")\n");
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine(lines);
        stringBuilder.AppendLine("}");
    }

    protected string BuildVariable(Type variableType, string variableName, bool getter, bool setter)
    {
        string str = "";
        string publicName = variableName;
        publicName = publicName.Substring(1);
        publicName = str.Substring(0, 1).ToUpper() + publicName;

        str += "private {0} {2}";
        if (getter || setter)
        {
            str += "public {0} {2}";
            str += "{";
            if (getter)
                str += "get {return {0}}";
            if (setter)
                str += "set {{0} = value}";

            str += "}";
        }

        return str;
    }

    protected void BuildEnding()
    {
        if(hasNamespace)
            stringBuilder.AppendLine("}");
        stringBuilder.AppendLine("}");
    }
}