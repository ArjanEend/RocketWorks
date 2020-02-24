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

    protected string name = "";
    protected string baseName = "";
    public string Name { get { return name; } }
    protected string nameSpace = "";
    protected string baseNamespace = "";
    public string FullName { get { return nameSpace + "." + name; } }
    public string BaseName { get { return baseNamespace + "." + baseName; } }

    public ClassBuilder()
    {
        stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("#pragma warning disable");
    }

    protected void BuildImports(params string[] extras)
    {
        for (int i = 0; i < extras.Length; i++)
        {
            if (!string.IsNullOrEmpty(extras[i]))
                stringBuilder.AppendLine("using " + extras[i] + ";");
        }
    }


    protected void BuildHeader(string nameSpace, string className, string baseClass, bool partial = false, string classOrStruct = "class")
    {
        stringBuilder.AppendLine("////// AUTO GENERATED ////////");
        hasNamespace = !string.IsNullOrEmpty(nameSpace);
        if (hasNamespace)
            stringBuilder.AppendLine(string.Format("namespace {0}", nameSpace)).Append("{");
        if (string.IsNullOrEmpty(baseClass))
            stringBuilder.AppendLine(string.Format("public {2}{3} {0}{1}", className, baseClass, partial ? "partial " : "", classOrStruct));
        else
            stringBuilder.AppendLine(string.Format("public {2}{3} {0} : {1}", className, baseClass, partial ? "partial " : "", classOrStruct));
        name = className;
        baseName = baseClass;
        this.nameSpace = nameSpace;
        stringBuilder.AppendLine("{");
    }

    protected void BuildMethod(string methodName, string modifier = "public", string returnName = "void", string lines = "", params string[] methodTypes)
    {
        stringBuilder.Append(modifier + " " + returnName + " " + methodName + "(");
        for (int i = 0; i < methodTypes.Length; i++)
        {
            stringBuilder.Append(methodTypes[i] + " var_" + methodTypes[i].ToLower());
            if (i != methodTypes.Length - 1)
                stringBuilder.Append(", ");
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
            if (i + 1 < methodTypes.Length)
                stringBuilder.Append(", ");
        }
        stringBuilder.Append(")\n");
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine(lines);
        stringBuilder.AppendLine("}");
    }

    protected void BuildVariable(Type variableType, string variableName, bool getter, bool setter)
    {
        BuildVariable(variableType.FullName, variableName, getter, setter);
    }

    protected void BuildVariable(string variableType, string variableName, bool getter, bool setter, bool initialize = false)
    {
        string str = "";
        variableName = variableName.Replace(variableName.Substring(0, 1), variableName.Substring(0, 1).ToLower());
        string publicName = variableName;

        publicName = publicName.Substring(1);
        publicName = variableName.Substring(0, 1).ToUpper() + publicName;

        str += string.Format("private {0} {1}", variableType, variableName);

        if (initialize)
            str += " = new " + variableType + "()";

        str += ";";

        if (getter || setter)
        {
            str += string.Format("public {0} {1}", variableType, publicName); ;
            str += "{ ";
            if (getter)
                str += "get { return " + variableName + "; }";
            if (setter)
                str += "set { " + variableName + " = value; }";

            str += " }";
        }
        stringBuilder.AppendLine(str);
    }

    protected void BuildProperty(string propertyName, string propertyType, string getLines = "", string setLines = "")
    {
        stringBuilder.Append("public " + propertyType + " " + propertyName + "{");

        stringBuilder.Append("get {");
        stringBuilder.AppendLine(getLines);
        stringBuilder.AppendLine("}");
        stringBuilder.Append("set {");
        stringBuilder.AppendLine(setLines);
        stringBuilder.AppendLine("}");
        stringBuilder.Append("}");
    }

    protected void BuildEnding()
    {
        if (hasNamespace)
            stringBuilder.AppendLine("}");
        stringBuilder.AppendLine("}");
        stringBuilder.AppendLine("#pragma warning restore");
    }
}