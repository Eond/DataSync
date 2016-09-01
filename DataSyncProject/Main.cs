using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class ISynchronizeInvokeExtensions
{
    public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
    {
        if (@this.InvokeRequired)
        {
            @this.Invoke(action, new object[] { @this });
        }
        else
        {
            action(@this);
        }
    }
}

namespace DataSyncProject
{
    public partial class Main : Form
    {
        private List<BUFile> WorkspacesData = new List<BUFile>();
        private int wsPos = -1;
        public List<string> filesToSync = new List<string>();
        const string FILENAME = "SyncFolder";
        public Main()
        {
            InitializeComponent();
        }

        private void btnAddBackup_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdNewBU = new FolderBrowserDialog();
            DialogResult res = fbdNewBU.ShowDialog();
            if (res == DialogResult.OK)
            {
                addBULocation(fbdNewBU.SelectedPath);
            }
            checkToSync();
        }

        private void addLocation(string loc, ListBox lstBox)
        {
            bool blnExists = lstBox.Items.Contains(loc);
            if (blnExists)
            {
                ShowError("Emplacement déjà enregistrée", "Erreur");
                return;
            }

            lstBox.Items.Add(loc);
        }

        private void addBULocation(string loc)
        {
            if (loc.Contains(FILENAME))
            {
                string old = loc.Substring(loc.IndexOf(FILENAME));
                loc = loc.Replace(old, "");
            }

            addLocation(loc, lstBackupLoc);
        }

        private void addWSLocation(string loc)
        {
            bool found = false;
            foreach(BUFile file in WorkspacesData)
            {
                if (file.getLastFolder() == BUFile.GetPathArray(loc).Last())
                    found = true;
            }
            if (!found)
            {
                BUFile NewColl = new BUFile(loc);

                WorkspacesData.Add(NewColl);
                addLocation(loc, lstWorkSpaces);
            }
            else
            {
                ShowError("Un dossier avec le nom '" + BUFile.GetPathArray(loc).Last() + "' est déjà enregistré.", "Dossier existant");
            }
        }

        private void btnDelBackup_Click(object sender, EventArgs e)
        {
            DeleteBUSelected();
            checkToSync();
        }

        private void DeleteSelected(ListBox lstBox)
        {
            while (lstBox.SelectedItems.Count > 0)
            {
                lstBox.Items.Remove(lstBox.SelectedItem);
            }
        }

        private void DeleteBUSelected()
        {
            DeleteSelected(lstBackupLoc);
        }

        private void DeleteWSSelected()
        {
            List<int> pos = new List<int>();
            foreach (int i in lstWorkSpaces.SelectedIndices)
                pos.Add(i);

            pos.Reverse();

            foreach(int i in pos)
            {
                if (i == wsPos)
                    lstWorkSpaceSave.Nodes.Clear();
                WorkspacesData.RemoveAt(i);
            }

            DeleteSelected(lstWorkSpaces);
        }

        private BackgroundWorker worker = new BackgroundWorker();
        private void btnSync_Click(object sender, EventArgs e)
        {
            Thread sync = new Thread(Synchronize);
            sync.Start();
            /*worker.WorkerReportsProgress = true;
            worker.DoWork += Synchronize;
            worker.RunWorkerCompleted += endSync;
            //worker.ProgressChanged += Worker_ProgressChanged;

            Enabled = false;*/
            //worker.RunWorkerAsync();
            //Synchronize();
        }
        /*private void endSync(object a, object b)
        {
            Enabled = true;
        }*/
        private void Synchronize()
        {
            this.InvokeEx(f => f.Enabled = false);
            List<string> strBU = makeBULocations();
            List<string> strWS = makeWSPrefixes();
            int totalFiles = filesToSync.Count * strBU.Count;
            this.InvokeEx(f => f.pgbSync.Maximum = totalFiles);
            this.InvokeEx(f => f.pgbSync.Minimum = 0);
            this.InvokeEx(f => f.pgbSync.Step = 1);
            this.InvokeEx(f => f.pgbSync.Value = 0);
            /*pgbSync.Maximum = totalFiles;
            pgbSync.Minimum = 0;
            pgbSync.Step = 1;
            pgbSync.Value = 0;*/

            filesToSync.ForEach(file =>
            {
                string localPath = "";
                string prefix = "";
                strWS.ForEach(str =>
                {
                    if (prefix == "" && file.Contains(str))
                    {
                        prefix = BUFile.GetPathArray(str).Last();
                        localPath = file.Replace(str, "");
                    }
                });
                string dest = FILENAME + "\\" + prefix + localPath;
                strBU.ForEach(str =>
                {
                    string padding = "";
                    if (str.Last() != '\\')
                        padding = "\\";
                    string strDest = str + padding + dest;
                    if (File.Exists(strDest))
                        File.Copy(file, strDest, true);
                    else
                    {
                        string[] path = BUFile.GetPathArray(strDest);
                        string newPath = "";
                        for (int i=0; i < path.Length-1; i++)
                        {
                            if (i != 0)
                                newPath = newPath + "\\";
                            newPath = newPath + path[i];
                        }

                        Directory.CreateDirectory(newPath);
                        File.Copy(file, strDest);
                    }
                    this.InvokeEx(f => f.pgbSync.PerformStep());
                });
                //File.SetAttributes(file, (FileAttributes)(File.GetAttributes(file) - FileAttributes.Archive));
            });
            this.InvokeEx(f => f.UpdateFileList());
            this.InvokeEx(f => f.Enabled = true);
        }

        private List<string> makeWSPrefixes()
        {
            List<string> retrun = new List<string>();

            for (int i = 0; i < lstWorkSpaces.Items.Count; i++)
            {
                string loc = lstWorkSpaces.Items[i].ToString();
                if (Directory.Exists(loc))
                {
                    retrun.Add(loc);
                }
                else
                {
                    this.InvokeEx(f => f.lstWorkSpaces.Items.RemoveAt(i));
                    this.InvokeEx(f => f.WorkspacesData.RemoveAt(i));
                    ShowError("L'emplacement '" + loc + "' n'existe pas.", "Dossier de Station manquant");
                    i--;
                }
            }

            return retrun;
        }

        private List<string> makeBULocations()
        {
            List<string> retrun = new List<string>();
            for (int i = 0; i < lstBackupLoc.Items.Count; i++)
            {
                string loc = lstBackupLoc.Items[i].ToString();
                if (Directory.Exists(loc))
                {
                    retrun.Add(loc);
                }
                else
                {
                    ShowError("L'emplacement '" + loc + "' n'existe pas.", "Dossier de Sauvegarde manquant");
                    this.InvokeEx(f => f.lstBackupLoc.Items.RemoveAt(i));
                    i--;
                }
            }
            return retrun;
        }

        private void lstBackupLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iItems = lstBackupLoc.SelectedItems.Count;
            btnChgBackup.Enabled = false;
            btnDelBackup.Enabled = false;

            if (iItems == 1) // On ne peut changer qu'un seul objet à la fois
            {
                btnChgBackup.Enabled = true;
            }

            if (iItems >= 1)
            {
                btnDelBackup.Enabled = true;
            }
        }

        private void btnChgBackup_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdNewBU = new FolderBrowserDialog();
            DialogResult res = fbdNewBU.ShowDialog();
            if (res == DialogResult.OK)
            {
                string loc = fbdNewBU.SelectedPath;
                DeleteBUSelected();
                addBULocation(loc);
                lstBackupLoc.SelectedItem = loc;
            }
            checkToSync();
        }

        private void btnAddWorkSpace_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdNewWS = new FolderBrowserDialog();
            DialogResult res = fbdNewWS.ShowDialog();
            if (res == DialogResult.OK)
            {
                addWSLocation(fbdNewWS.SelectedPath);
            }
            checkToSync();
        }

        private void lstWorkSpaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iItems = lstWorkSpaces.SelectedItems.Count;
            btnChgWorkSpace.Enabled = false;
            btnDelWorkSpace.Enabled = false;
            int pos = -1;

            if (iItems == 1) // On ne peut changer qu'un seul objet à la fois
            {
                btnChgWorkSpace.Enabled = true;
                btnDelWorkSpace.Enabled = true;

                if (wsPos >= 0)
                {
                    WorkspacesData[wsPos].Update(lstWorkSpaceSave.Nodes);
                }

                pos = wsPos = lstWorkSpaces.SelectedIndex;
                lstWorkSpaceSave.Nodes.Clear();

                if (WorkspacesData[pos].Exists)
                {
                    lstWorkSpaceSave.Nodes.Add(WorkspacesData[wsPos].getTree());
                }
                else
                {
                    pos = -1;
                }
            }

            if (iItems > 1)
            {
                if (wsPos >= 0)
                {
                    WorkspacesData[wsPos].Update(lstWorkSpaceSave.Nodes);
                    lstWorkSpaceSave.Nodes.Clear();
                }
                btnDelWorkSpace.Enabled = true;
            }

            if (pos == -1)
                wsPos = -1;
        }

        private void btnChgWorkSpace_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdNewWS = new FolderBrowserDialog();
            DialogResult res = fbdNewWS.ShowDialog();
            if (res == DialogResult.OK)
            {
                string loc = fbdNewWS.SelectedPath;
                DeleteWSSelected();
                addWSLocation(loc);
                lstWorkSpaces.SelectedItem = loc;
            }
            checkToSync();
        }

        private void btnDelWorkSpace_Click(object sender, EventArgs e)
        {
            DeleteWSSelected();
            checkToSync();
        }

        private void lstWorkSpaceSave_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (this.Enabled)
            {
                Application.UseWaitCursor = true;
                this.Enabled = false;
                bool chked = e.Node.Checked;
                foreach (TreeNode node in e.Node.Nodes)
                {
                    ChkBoxChg(node);
                }
                if (e.Node.Parent != null)
                {
                    bool chk = true;
                    foreach (TreeNode node in e.Node.Parent.Nodes)
                    {
                        chk = chk && node.Checked;
                    }
                    e.Node.Parent.Checked = chk;
                }
                WorkspacesData[wsPos].Update(lstWorkSpaceSave.Nodes);
                UpdateFileList();
                Thread.Sleep(100);
                e.Node.Checked = chked;
                this.Enabled = true;
                Application.UseWaitCursor = false;
                checkToSync();
            }
        }

        private void ChkBoxChg(TreeNode node)
        {
            node.Checked = node.Parent.Checked;

            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    ChkBoxChg(n);
                }
            }
        }

        private void lstWorkSpaceSave_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            /*if (this.Enabled)
            {
                Application.UseWaitCursor = true;
                this.Enabled = false;
                bool chked = e.Node.Checked;
                foreach (TreeNode node in e.Node.Nodes)
                {
                    ChkBoxChg(node);
                }
                if (e.Node.Parent != null)
                {
                    bool chk = true;
                    foreach (TreeNode node in e.Node.Parent.Nodes)
                    {
                        chk = chk && node.Checked;
                    }
                    e.Node.Parent.Checked = chk;
                }
                WorkspacesData[wsPos].Update(lstWorkSpaceSave.Nodes);
                UpdateFileList();
                Thread.Sleep(300);
                e.Node.Checked = chked;
                this.Enabled = true;
                Application.UseWaitCursor = false;
            }*/
        }

        private void UpdateFileList()
        {
            lstFiles.Nodes.Clear();
            filesToSync.Clear();
            foreach(BUFile data in WorkspacesData)
            {
                TreeNode node = data.getTree(true, this);
                if (node == null)
                    break;
                if (lstFiles.Nodes.Contains(node))
                {
                    int i = lstFiles.Nodes.IndexOf(node);
                    TreeNode fused = BUFile.fuseTrees(lstFiles.Nodes[i], node);
                    if (fused == null)
                        break;
                    lstFiles.Nodes[i] = fused;
                }
                else
                {
                    lstFiles.Nodes.Add(node);
                }
            }
        }
        public bool addSyncFile(string file)
        {
            if (File.Exists(file))
            {
                if (File.GetAttributes(file).HasFlag(FileAttributes.Archive))
                {
                    if (!filesToSync.Contains(file))
                    {
                        filesToSync.Add(file);
                        return true;
                    }
                }
            }

            return false;
        }

        private void checkToSync()
        {
            bool ok = lstWorkSpaces.Items.Count > 0;
            bool blnData = false;
            foreach (BUFile data in WorkspacesData)
                blnData = blnData || data.Exists;
            ok = ok && blnData;
            ok = ok && lstBackupLoc.Items.Count > 0;
            ok = ok && lstFiles.Nodes.Count > 0;

            btnSync.Enabled = ok;
            btnForcedBackUp.Enabled = ok;
            btnForcedHome.Enabled = ok;
            saveConfig.Enabled = ok;
        }
     
        private void newConfig_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(this, "Êtes-vous sûr de tout effacer?", "Réinitialisation du contenu", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                lstBackupLoc.Items.Clear();
                btnChgBackup.Enabled = false;
                btnDelBackup.Enabled = false;

                lstWorkSpaces.Items.Clear();
                btnChgWorkSpace.Enabled = false;
                btnDelWorkSpace.Enabled = false;

                lstWorkSpaceSave.Nodes.Clear();
                wsPos = -1;

                lstFiles.Nodes.Clear();
                filesToSync.Clear();

                btnSync.Enabled = false;
                btnForcedBackUp.Enabled = false;
                btnForcedHome.Enabled = false;
                saveConfig.Enabled = false;

                pgbSync.Value = 0;
            }
        }

        static public void ShowError(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
