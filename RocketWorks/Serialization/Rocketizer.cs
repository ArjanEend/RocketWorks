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

        //private MemoryStream memStream;
        private Dictionary<uint, Type> idToType = new Dictionary<uint, Type>();
        private Dictionary<Type, uint> typeToId = new Dictionary<Type, uint>();

        public void SetStream(MemoryStream memStream)
        {
            if (writer != null)
                writer.Close();
            if (reader != null)
                reader.Close();
            writer = new BinaryWriter(memStream);
            reader = new BinaryReader(memStream);
        }

        public void WriteObject(object ob, MemoryStream memStream = null)
        {
            if (memStream != null)
                SetStream(memStream);

            //RocketLog.Log(ob.ToString(), this);
            IRocketable rocketable = ob as IRocketable;
            if(rocketable != null)
            {
                writer.Write(typeToId[rocketable.GetType()]);
                rocketable.Rocketize(this);
            } else
            {
                writer.Write(-1);
            }
        }

        public T ReadObject<T>(MemoryStream memStream = null)
        {
            if (memStream != null)
                SetStream(memStream);

            uint type = reader.ReadUInt32();
            if (idToType.ContainsKey(type))
            {
                IRocketable instance = (IRocketable)Activator.CreateInstance(idToType[type]);
                instance.DeRocketize(this);
                return (T)instance;
            }
            return default(T);//reader.ReadUInt32() as T;
        }

    }
}
