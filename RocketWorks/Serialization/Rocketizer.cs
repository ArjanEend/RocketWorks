using RocketWorks.Entities;
using RocketWorks.Networking;
using RocketWorks.Pooling;
using System;
using System.Collections.Generic;
using System.IO;

namespace RocketWorks.Serialization
{
    public partial class Rocketizer
    {
        private Dictionary<Type, IInstanceProvider> instanceProviders = new Dictionary<Type, IInstanceProvider>();

        //private MemoryStream memStream;
        private Dictionary<short, Type> idToType = new Dictionary<short, Type>();
        private Dictionary<Type, short> typeToId = new Dictionary<Type, short>();

        public void AddProvider(IInstanceProvider provider)
        {
            instanceProviders.Add(provider.ObjectType, provider);
        }

        public short GetIDFor(Type type)
        {
            if (type == null || !typeToId.ContainsKey(type))
                return -1;
            return typeToId[type];
        }

        public Type GetTypeFor(short id)
        {
            if (!idToType.ContainsKey(id))
                return null;
            return idToType[id];
        }

        public void WriteObject(object ob, NetworkWriter writer)
        {
            try {
                lock (writer)
                {
                    IRocketable rocketable = ob as IRocketable;
                    if (rocketable != null && typeToId.ContainsKey(rocketable.GetType()))
                    {
                        //     RocketLog.Log("Serialize : " + rocketable.GetType().Name, this);
                        writer.Write(typeToId[rocketable.GetType()]);
                        rocketable.Rocketize(this, writer);
                    }
                    else
                    {
                        if (ob != null)
                            RocketLog.Log("Could not write: " + ob + ", rocketable: " + (ob != null));
                        writer.Write((short)-1);
                    }
                }
            }
            catch(Exception ex)
            {
                RocketLog.Log(ex.ToString(), this);
            }
        }

        public T ReadObject<T>(int ownerState, NetworkReader reader)
        {
            short type = reader.ReadInt16();
            if (idToType.ContainsKey(type))
            {
                //RocketLog.Log("Deserialize : " + idToType[type].Name, this);
                IRocketable instance;
                

                    if (instanceProviders.ContainsKey(idToType[type]))
                        instance = (IRocketable)instanceProviders[idToType[type]].GetInstance();
                    else
                        instance = (IRocketable)Activator.CreateInstance(idToType[type]);
                lock (instance)
                {
                    instance.DeRocketize(this, ownerState, reader);
                    //RocketLog.Log("Cast to: " + idToType[type].Name + " : "  + typeof(T).Name);

                    //Hack! needs some nice interfaces to make generic
                    if (instance is EntityReference)
                    {
                        var entRef = (EntityReference)instance;
                        if(entRef.contextType != null && instanceProviders.ContainsKey(entRef.contextType))
                            entRef.pool = (EntityPool)instanceProviders[entRef.contextType];
                        instance = entRef;
                    }
                }
                try
                {
                    return (T)instance;
                } catch
                {
                    RocketLog.LogFormat("Cast error! from: {0} to {1}", this, instance.GetType(), typeof(T));
                    return default(T);
                }
            }
            //RocketLog.Log("Could not find: " + type, this);
            return default(T);//reader.ReadUInt32() as T;
        }
    }
}
