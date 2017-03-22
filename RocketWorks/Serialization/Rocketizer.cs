using RocketWorks.Entities;
using RocketWorks.Pooling;
using System;
using System.Collections.Generic;
using System.IO;

namespace RocketWorks.Serialization
{
    public partial class Rocketizer
    {
        private BinaryWriter writer;
        public BinaryWriter Writer { get { return writer; } }
        private BinaryReader reader;
        public BinaryReader Reader { get { return reader; } }

        private Dictionary<Type, IInstanceProvider> instanceProviders = new Dictionary<Type, IInstanceProvider>();

        //private MemoryStream memStream;
        private Dictionary<int, Type> idToType = new Dictionary<int, Type>();
        private Dictionary<Type, int> typeToId = new Dictionary<Type, int>();

        public void SetStream(MemoryStream memStream)
        {
            writer = new BinaryWriter(memStream);
            reader = new BinaryReader(memStream);
        }

        public void AddProvider(IInstanceProvider provider)
        {
            instanceProviders.Add(provider.ObjectType, provider);
        }

        public void WriteObject(object ob, MemoryStream memStream = null)
        {
            if (memStream != null)
                SetStream(memStream);
            try { 

            IRocketable rocketable = ob as IRocketable;
            if(rocketable != null && typeToId.ContainsKey(rocketable.GetType()))
            {

               //     RocketLog.Log("Serialize : " + rocketable.GetType().Name, this);
                writer.Write(typeToId[rocketable.GetType()]);
                rocketable.Rocketize(this);
            } else
            {
                //RocketLog.Log("Could not write: " + ob + ", rocketable: " + (ob != null));
                writer.Write(-1);
            }
            }
            catch(Exception ex)
            {
                RocketLog.Log(ex.ToString(), this);
            }
        }

        public T ReadObject<T>(MemoryStream memStream = null)
        {
            if (memStream != null)
                SetStream(memStream);

            int type = reader.ReadInt32();
            if (idToType.ContainsKey(type))
            {
                //RocketLog.Log("Deserialize : " + idToType[type].Name, this);
                IRocketable instance;
                if (instanceProviders.ContainsKey(idToType[type]))
                    instance = (IRocketable)instanceProviders[idToType[type]].GetInstance();
                else
                    instance = (IRocketable)Activator.CreateInstance(idToType[type]);
                instance.DeRocketize(this);
                //RocketLog.Log("Cast to: " + idToType[type].Name + " : "  + typeof(T).Name);
                try
                {
                    return (T)instance;
                } catch
                {
                    RocketLog.Log("Cast error!");
                    return default(T);
                }
            }
            //RocketLog.Log("Could not find: " + type, this);
            return default(T);//reader.ReadUInt32() as T;
        }
    }
}
