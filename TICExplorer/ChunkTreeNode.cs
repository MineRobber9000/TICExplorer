using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICExplorer
{
    public class ChunkTreeNode : TreeNode
    {
        private MainForm parent;
        private Chunk Chunk;

        public ChunkTreeNode(MainForm form, Chunk chunk) {
            parent = form;
            Chunk = chunk;
            Text = $"{chunk.Type} (bank {chunk.Bank})";
            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Items.Add("Edit Chunk").Click += new System.EventHandler(this.EditChunk_Click);
            ContextMenuStrip.Items.Add("Delete Chunk").Click += new System.EventHandler(this.DeleteChunk_Click);
        }

        private void DeleteChunk_Click(object? sender, EventArgs e)
        {
            parent.RemoveChunkFromFile(Chunk);
        }

        private void EditChunk_Click(object? sender, EventArgs e)
        {
            EditChunkForm ecf = new EditChunkForm(Chunk);
            ecf.ShowDialog();
            if (ecf.ChangesMade) {
                parent.MarkDirty();
            }
        }
    }
}
