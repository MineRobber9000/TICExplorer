using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICExplorer
{
    public enum ChunkType : Byte { 
        CHUNK_NONE = 0,
        CHUNK_TILES = 1,
        CHUNK_SPRITES = 2,
        CHUNK_MAPS = 4,
        CHUNK_CODE = 5,
        CHUNK_FLAGS = 6,
        CHUNK_SAMPLES = 9,
        CHUNK_WAVEFORM = 10,
        CHUNK_PALETTE = 12,
        CHUNK_MUSIC = 14,
        CHUNK_PATTERNS = 15,
        CHUNK_DEFAULT = 17,
        CHUNK_SCREEN = 18,
        CHUNK_BINARY = 19,
        CHUNK_COVER_DEP = 3,
        CHUNK_PATTERNS_DEP = 13,
        CHUNK_CODE_ZIP = 16
    }
    public class Chunk
    {
        public int Bank;
        public ChunkType Type;
        public List<byte> Data;

        public Chunk(ChunkType type) {
            Bank = 0;
            Type = type;
            Data = new List<byte>();
        }

        public Chunk(ChunkType type, int bank) { 
            Bank = bank;
            Type = type;
            Data = new List<byte>();
        }

        public Chunk(ChunkType type, int bank, List<byte> data) {
            Bank = bank;
            Type = type;
            Data = data;
        }

        public byte[] ToBinary() {
            byte h1 = (byte)((Bank << 5) + (byte)Type);
            byte h2 = (byte)(Data.Count & 0xFF);
            byte h3 = (byte)((Data.Count >> 8)&0xFF);
            byte h4 = 0x00;
            byte[] header = { h1, h2, h3, h4 };
            return header.Concat(Data).ToArray();
        }
    }
}
