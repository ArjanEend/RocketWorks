﻿using System.Collections.Generic;
using System;
using System.Text;

public class ClassBuilder
{
    private StringBuilder stringBuilder;
    public StringBuilder StringBuilder
    {
        get { return stringBuilder; }
    }

    public ClassBuilder()
    {
        stringBuilder = new StringBuilder();

        BuildImports();
        BuildHeader("Generated", "GeneratedClass", "Object");
        BuildEnding();
    }

    protected void BuildImports(params string[] extras)
    {
        for(int i = 0; i < extras.Length; i++)
        {
            stringBuilder.AppendLine("using " + extras[i]);
        }
    }


    protected void BuildHeader(string nameSpace, string className, string baseClass)
    {
        stringBuilder.AppendLine("////// AUTO GENERATED ////////");
        stringBuilder.AppendLine(string.Format("using namespace {0};", nameSpace));
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine(string.Format("public + class {0} : {1}", className, baseClass));
        stringBuilder.AppendLine("{");
    }

    protected string BuildMethod(string methodName, Type[] methodTypes, string[] paramNames)
    {
        string str = "";

        str += "public void {0}({1} {2})";
        str += "{";
        str += "}";

        return str;
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
        stringBuilder.AppendLine("}");
        stringBuilder.AppendLine("}");
    }
}