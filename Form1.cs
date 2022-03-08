using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FileManager
{
    public partial class Form1 : Form
    {
        private string _mainRoot => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Downloads\\";
        private DirectoryInfo _nodeDirInfo;

        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            InitializeDirectories(null, _mainRoot);
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode;
            DirectoryInfo info = new DirectoryInfo(_mainRoot);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            InitializeDirectories(e);
        }

        private void InitializeDirectories(TreeNodeMouseClickEventArgs e = null, string root =null)
        {
            if (e != null)
            {
                TreeNode newSelected = e.Node;
                _nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            }
            else
            {
                _nodeDirInfo = new DirectoryInfo(root);
            }

            listView1.Items.Clear();

            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in _nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"),
             new ListViewItem.ListViewSubItem(item,
                dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            foreach (FileInfo file in _nodeDirInfo.GetFiles())
            {
                if (file.Extension == ".txt")
                {
                    item = new ListViewItem(file.Name, 1);
                    subItems = new ListViewItem.ListViewSubItem[]
                        {
                           new ListViewItem.ListViewSubItem(item, "File")
                          ,new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString())
                          ,new ListViewItem.ListViewSubItem(item, file.Length.ToString())
                        };

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                MenuItem cashMenuItem2 = new MenuItem("-");
                m.MenuItems.Add(cashMenuItem2);

                MenuItem delMenuItem = new MenuItem("Delete Object");
                delMenuItem.Click += delegate (object sender2, EventArgs e2)
                {
                    Delete(sender);
                };
                m.MenuItems.Add(delMenuItem);

                m.Show(listView1, new Point(e.X, e.Y));
            }
        }

        private void Delete(object sender)
        {
            ListView lView = (ListView)sender;
            var current = _nodeDirInfo.FullName;
            var name = $"{current}\\{lView.FocusedItem.Text}";

            if (File.Exists(name))
            {
                Unlock();
                File.Delete(name);
            }

            if (Directory.Exists(name))
            {
                Unlock();
                Directory.Delete(name);
            }

            InitializeDirectories(null, current);
            textBox1.Text = string.Empty;
        }

        private void Unlock()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                var current = _nodeDirInfo.FullName;
                var dir = $"{current}\\{textBox1.Text}";
                if (this.radioButton1.Checked)
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                }
                else
                {
                    var file = $"{dir}.txt";
                    if (!File.Exists(file))
                        File.Create(file);
                }
                InitializeDirectories(null, current);
                textBox1.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("El nombre del archivo/directorio no puede estar vacio");
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Visible = !radioButton1.Checked;
        }
    }
}