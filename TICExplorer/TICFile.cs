using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICExplorer
{
    public class TICFile
    {
        public List<Chunk> Chunks;

        public TICFile() {
            Chunks = new List<Chunk>();
        }

        public TICFile(List<Chunk> chunks) {
            Chunks = chunks;
        }

        public byte[] ToBinary() {
            byte[] result = new byte[0];
            for (int i = 0; i < Chunks.Count; i++)
            {
                byte[] chunk = Chunks[i].ToBinary();
                result = result.Concat(chunk).ToArray();
            }
            return result;
        }
    }
}
