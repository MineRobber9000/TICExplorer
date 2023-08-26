namespace TICExplorer
{
    public partial class MainForm : Form
    {
        private bool Dirty;
        private bool Loaded;
        private TICFile File;
        private String Filename;
        private String PrefDirectory {
            get {
                return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"com.nesbox.tic","TIC-80");
            }
        }
#pragma warning disable CS8618 // All of the fields get set, I don't know why it thinks they don't
        public MainForm()
#pragma warning restore CS8618
        {
            InitializeComponent();
            NewFile();
            treeView1.ContextMenuStrip = new ContextMenuStrip();
            treeView1.ContextMenuStrip.Items.Add("Add Chunk").Click+=new System.EventHandler(this.AddChunk_Click);
        }

        private void UpdateTitle() {
            this.Text = this.Filename+(this.Dirty ? "*" : "")+" - TIC Explorer";
        }

        private void UpdateTreeView() {
            this.treeView1.BeginUpdate();
            this.treeView1.Nodes.Clear();
            foreach (Chunk chunk in File.Chunks) {
                treeView1.Nodes.Add(new ChunkTreeNode(this,chunk));
            }
            this.treeView1.EndUpdate();
        }

        private void NewFile(bool update) {
            this.Dirty = false;
            this.Loaded = false;
            this.File = new TICFile();
            this.Filename = "NewFile.tic";
            if (update) {
                this.UpdateTreeView();
                this.UpdateTitle();
            }
        }

        private void NewFile() {
            NewFile(true);
        }

        private void Open(String filename) {
            NewFile(false);
            Filename = filename;
            Loaded = true;
            this.UpdateTitle();
            using (var stream = System.IO.File.Open(filename, FileMode.Open)) {
                using (var reader = new BinaryReader(stream)) {
                    while (reader.BaseStream.Position != reader.BaseStream.Length) {
                        byte h1 = reader.ReadByte();
                        byte h2 = reader.ReadByte();
                        byte h3 = reader.ReadByte();
                        byte h4 = reader.ReadByte();
                        int bank = h1 >> 5;
                        ChunkType type = (ChunkType)(h1 & 0x1f);
                        List<byte> data = new List<byte>();
                        int size = (h3 << 8) + h2;
                        for (; (size > 0 && (reader.BaseStream.Position!=reader.BaseStream.Length)); size--) { 
                            data.Add(reader.ReadByte());
                        }
                        File.Chunks.Add(new Chunk(type,bank,data));
                    }
                }
            }
            this.UpdateTreeView();
        }

        private void Save(String filename) {
            Filename = filename;
            using (var stream = System.IO.File.Open(filename, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(File.ToBinary());
                }
            }
        }

        private void Save() {
            Save(Filename);
        }

        public void MarkDirty() {
            Dirty = true;
            this.UpdateTreeView();
            this.UpdateTitle();
        }

        private void AddChunkToFile()
        {
            File.Chunks.Add(new Chunk(ChunkType.CHUNK_NONE));
            this.MarkDirty();
        }

        public void RemoveChunkFromFile(Chunk c) {
            File.Chunks.Remove(c);
            this.MarkDirty();
        }

        private void NewFileButton_Click(object sender, EventArgs e)
        {
            if (this.Dirty)
            {
                DialogResult result;
                if (this.Loaded)
                {
                    result = MessageBox.Show("Do you want to save " + this.Filename + " before starting a new file?", "New File", MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    result = MessageBox.Show("Do you want to save the file before starting a new file?", "New File", MessageBoxButtons.YesNoCancel);
                }
                if (result == DialogResult.Yes)
                {
                    this.Save();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            NewFile();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (this.Dirty)
            {
                DialogResult result;
                if (this.Loaded)
                {
                    result = MessageBox.Show("Do you want to save " + this.Filename + " before opening a new file?", "Open", MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    result = MessageBox.Show("Do you want to save the file before opening a new file?", "Open", MessageBoxButtons.YesNoCancel);
                }
                if (result == DialogResult.Yes)
                {
                    this.SaveButton_Click(sender,e);
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "TIC files (*.tic)|*.tic|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = PrefDirectory;
                openFileDialog.CheckFileExists = true;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Open(openFileDialog.FileName);
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e) {
            if (Loaded)
            {
                Save();
                Dirty = false;
                this.UpdateTitle();
            }
            else {
                SaveAsButton_Click(sender, e);
            }
        }

        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "TIC files (*.tic)|*.tic|All files (*.*)|*.*";
                saveFileDialog.InitialDirectory = PrefDirectory;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Save(saveFileDialog.FileName);
                    Loaded = true;
                    Dirty = false;
                    this.UpdateTitle();
                }
            }
        }

        private void AddChunk_Click(object? sender, EventArgs e)
        {
            AddChunkToFile();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Dirty)
            {
                DialogResult result;
                if (this.Loaded)
                {
                    result = MessageBox.Show("Do you want to save " + this.Filename + " before closing TICExplorer?", "Close", MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    result = MessageBox.Show("Do you want to save the file before closing TICExplorer?", "Close", MessageBoxButtons.YesNoCancel);
                }
                if (result == DialogResult.Yes)
                {
                    this.SaveButton_Click(sender,e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}