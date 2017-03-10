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

        //TODO: Make reference/instance management
        private EntityPool pool;
        public EntityPool Pool { set { pool = value; } }

        //private MemoryStream memStream;
        private Dictionary<int, Type> idToType = new Dictionary<int, Type>();
        private Dictionary<Type, int> typeToId = new Dictionary<Type, int>();

        public void SetStream(MemoryStream memStream)
        {
            writer = new BinaryWriter(memStream);
            reader = new BinaryReader(memStream);
        }

        public void WriteObject(object ob, MemoryStream memStream = null)
        {
            if (memStream != null)
                SetStream(memStream);
            try { 

            IRocketable rocketable = ob as IRocketable;
            if(rocketable != null && typeToId.ContainsKey(rocketable.GetType()))
            {
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
                if (idToType[type] == typeof(Entity))
                    instance = pool.GetCleanObject();
                else
                    instance = (IRocketable)Activator.CreateInstance(idToType[type]);
                instance.DeRocketize(this);
                //RocketLog.Log("Cast to: " + idToType[type].Name + " : "  + typeof(T).Name);
                return (T)instance;
            }
            //RocketLog.Log("Could not find: " + type, this);
            return default(T);//reader.ReadUInt32() as T;
        }

    }
}
