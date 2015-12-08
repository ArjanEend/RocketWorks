using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class ClassBuilder
{

    private StringBuilder stringBuilder;
    public StringBuilder StringBuilder
    {
        get
        { return stringBuilder; }
    }

    public ClassBuilder()
    {
        stringBuilder = new StringBuilder();

        BuildImports(null);
        BuildHeader("Generated", "GeneratedClass", "Object");
        BuildEnding();

        Debug.Log(stringBuilder.ToString());
    }

    private void BuildImports(string[] extras)
    {
        stringBuilder.AppendLine("using UnityEngine;");
        stringBuilder.AppendLine("using System.Collections;");
    }


    private void BuildHeader(string nameSpace, string className, string baseClass)
    {
        stringBuilder.AppendLine("////// AUTO GENERATED ////////");
        stringBuilder.AppendLine(string.Format("using namespace {0};", nameSpace));
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine(string.Format("public class {0} : {1}", className, baseClass));
        stringBuilder.AppendLine("{");
    }

    private string BuildMethod(string methodName, Dictionary<Type, string> methodParams)
    {
        string str = "";

        str += "public void {0}({1} {2})";
        str += "{";
        str += "}";

        return str;
    }

    private string BuildVariable(Type variableType, string variableName, bool getter, bool setter)
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

    private void BuildEnding()
    {
        stringBuilder.AppendLine("}");
        stringBuilder.AppendLine("}");
    }

    

}