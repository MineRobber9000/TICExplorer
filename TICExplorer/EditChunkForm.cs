using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace TICExplorer
{
    public partial class EditChunkForm : Form
    {
        private Chunk Chunk;
        private ChunkType LastSelection;
        public bool ChangesMade;
        public EditChunkForm(Chunk chunk)
        {
            InitializeComponent();
            Chunk = chunk;
            comboBox1.DataSource = ChunkType.GetValues(typeof(ChunkType));
            ChangesMade = false;
            InterpretChunk();
        }

        private void ReinterpretChunk(ChunkType old) {
            List<byte> data;
            if (old == ChunkType.CHUNK_CODE)
            {
                data = new List<byte>(Encoding.UTF8.GetBytes(textBox1.Text.ReplaceLineEndings("\n")));
            }
            else if (old == ChunkType.CHUNK_CODE_ZIP)
            {
                byte[] code = Encoding.UTF8.GetBytes(textBox1.Text.ReplaceLineEndings("\n"));
                Deflater def = new Deflater(Deflater.BEST_COMPRESSION);
                def.SetInput(code);
                def.Finish();
                int length = 0;
                byte[] output = new byte[0];
                byte[] buffer = new byte[1024];
                while (!def.IsFinished)
                {
                    length+=def.Deflate(buffer);
                    output = output.Concat(buffer).ToArray();
                }
                data = new List<byte>(output.Take(length).ToArray());
            }
            else
            {
                data = new List<byte>();
                foreach (String hex in GetChars(textBox1.Text, 2))
                {
                    if (hex.Length == 1)
                    {
                        MessageBox.Show("The hex string is unbalanced (i.e; not even length).", "Error saving changes to chunk", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    data.Add(Convert.ToByte(hex, 16));
                }
            }
            if ((ChunkType)comboBox1.SelectedItem == ChunkType.CHUNK_CODE)
            {
                textBox1.Text = Encoding.UTF8.GetString(data.ToArray()).ReplaceLineEndings();
            }
            else if ((ChunkType)comboBox1.SelectedItem == ChunkType.CHUNK_CODE_ZIP)
            {
                Inflater inf = new Inflater();
                inf.SetInput(data.ToArray());
                bool stop = false;
                byte[] output = new byte[0];
                byte[] buffer = new byte[1024];
                while (!(inf.IsFinished||inf.IsNeedingInput||stop))
                {
                    try
                    {
                        inf.Inflate(buffer);
                    }
                    catch
                    {
                        stop = true;
                        continue;
                    }
                    output = output.Concat(buffer).ToArray();
                }
                if (inf.IsFinished)
                {
                    textBox1.Text = Encoding.UTF8.GetString(output).ReplaceLineEndings();
                }
                else
                {
                    textBox1.Text = "";
                }
            }
            else
            {
                textBox1.Text = BitConverter.ToString(data.ToArray()).Replace("-", String.Empty);
            }
        }

        private void InterpretChunk() {
            comboBox1.SelectedItem = LastSelection = Chunk.Type;
            numericUpDown1.Value = Chunk.Bank;
            if (Chunk.Type == ChunkType.CHUNK_CODE)
            {
                textBox1.Text = Encoding.UTF8.GetString(Chunk.Data.ToArray()).ReplaceLineEndings();
            }
            else if (Chunk.Type == ChunkType.CHUNK_CODE_ZIP)
            {
                Inflater inf = new Inflater();
                inf.SetInput(Chunk.Data.ToArray());
                bool stop = false;
                byte[] output = new byte[0];
                byte[] buffer = new byte[1024];
                while (!(inf.IsFinished || inf.IsNeedingInput || stop))
                {
                    try
                    {
                        inf.Inflate(buffer);
                    }
                    catch
                    {
                        stop = true;
                        continue;
                    }
                    output = output.Concat(buffer).ToArray();
                }
                if (inf.IsFinished)
                {
                    textBox1.Text = Encoding.UTF8.GetString(output).ReplaceLineEndings();
                }
                else
                {
                    textBox1.Text = "";
                }
            }
            else
            {
                textBox1.Text = BitConverter.ToString(Chunk.Data.ToArray()).Replace("-", String.Empty);
            }
        }

        private IEnumerable<string> GetChars(string str, int iterateCount)
        {
            var words = new List<string>();

            for (int i = 0; i < str.Length; i += iterateCount)
                if (str.Length - i >= iterateCount) words.Add(str.Substring(i, iterateCount));
                else words.Add(str.Substring(i, str.Length - i));

            return words;
        }

        private void SaveButton_Click(object sender, EventArgs e) {
            List<byte> data;
            if ((ChunkType)comboBox1.SelectedItem == ChunkType.CHUNK_CODE)
            {
                data = new List<byte>(Encoding.UTF8.GetBytes(textBox1.Text.ReplaceLineEndings("\n")));
            }
            else if ((ChunkType)comboBox1.SelectedItem == ChunkType.CHUNK_CODE_ZIP)
            {
                byte[] code = Encoding.UTF8.GetBytes(textBox1.Text.ReplaceLineEndings("\n"));
                Deflater def = new Deflater(Deflater.BEST_COMPRESSION);
                def.SetInput(code);
                def.Finish();
                int length = 0;
                byte[] output = new byte[0];
                byte[] buffer = new byte[1024];
                while (!def.IsFinished) {
                    length+=def.Deflate(buffer);
                    output = output.Concat(buffer).ToArray();
                }
                data = new List<byte>(output.Take(length).ToArray());
            }
            else
            {
                data = new List<byte>();
                foreach (String hex in GetChars(textBox1.Text,2)) {
                    if (hex.Length == 1) {
                        MessageBox.Show("The hex string is unbalanced (i.e; not even length).","Error saving changes to chunk",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }
                    data.Add(Convert.ToByte(hex,16));
                }
            }
            ChangesMade = true;
            Chunk.Type = (ChunkType)comboBox1.SelectedItem;
            Chunk.Bank = (int)numericUpDown1.Value;
            Chunk.Data = data;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ReinterpretChunk(LastSelection);
            LastSelection = (ChunkType)comboBox1.SelectedItem;
        }
    }
}
