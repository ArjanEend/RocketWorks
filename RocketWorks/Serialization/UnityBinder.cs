using System;
using System.Reflection;
using System.Runtime.Serialization;


namespace RocketWorks.Serialization
{
    public class UnityBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = null;
            if (assemblyName ==
                "Assembly-CSharp, Version=0.0.0.0, Culture = neutral, PublicKeyToken = null")
            {
                assemblyName = Assembly.GetExecutingAssembly().FullName;
                returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));
                return returntype;
            }
            return returntype;
        }
    }
}
