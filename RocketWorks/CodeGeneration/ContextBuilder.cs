using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RocketWorks.CodeGeneration
{
    public class ContextBuilder : ClassBuilder
    { 
        public ContextBuilder(Type type, string fieldName)
        {
            BuildImports("System", "Implementation.Components");
            BuildHeader(type.Namespace, fieldName + "CTX", type.ToGenericTypeString());
            BuildEnding();

            RocketLog.Log(StringBuilder.ToString());
        }
    }
}
