

using System.IO;

namespace RocketWorks.Serialization
{
    public class Rocketizer
    {
        private BinaryWriter writer;
        public BinaryWriter Writer { get { return writer; } }
        private BinaryReader reader;
        public BinaryReader Reader { get { return reader; } }

        private MemoryStream memStream;

        public Rocketizer()
        {
            
        }

        public void SetStream(MemoryStream memStream)
        {
            if (writer != null)
                writer.Dispose();
            if (reader != null)
                reader.Dispose();
            writer = new BinaryWriter(memStream);
            reader = new BinaryReader(memStream);
        }

        public void WriteObject(object ob, MemoryStream memStream = null)
        {
            if (memStream != null)
                SetStream(memStream);
            IRocketable rocketable = (IRocketable)ob;
            if(rocketable != null)
            {
                rocketable.Rocketize(this);
            } else
            {
                writer.Write(-1);
            }
        }

        public T ReadObject<T>(MemoryStream memStream = null) where T : class
        {
            if (memStream != null)
                SetStream(memStream);
            //Stub
            return reader.ReadUInt32() as T;
        }

    }
}
