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

namespace DataSyncProject
{
    public partial class Main : Form
    {
        private List<oldBUFile> WorkspacesData = new List<oldBUFile>();
        private int wsPos = -1;
        private List<string>[] param = { };
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
            foreach (oldBUFile file in WorkspacesData)
            {
                if (file.getLastFolder() == oldBUFile.GetPathArray(loc).Last())
                    found = true;
            }
            if (!found)
            {
                oldBUFile NewColl = new oldBUFile(loc);

                WorkspacesData.Add(NewColl);
                addLocation(loc, lstWorkSpaces);
            }
            else
            {
                ShowError("Un dossier avec le nom '" + oldBUFile.GetPathArray(loc).Last() + "' est déjà enregistré.", "Dossier existant");
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

            foreach (int i in pos)
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
            List<string>[] ret = { filesToSync, getBUFolders(false), makeWSPrefixes() };
            param = ret;
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

        private void syncStart(int maxPrg)
        {
            this.InvokeEx(f => f.Enabled = false);
            this.InvokeEx(f => f.pgbSync.Maximum = maxPrg);
            this.InvokeEx(f => f.pgbSync.Minimum = 0);
            this.InvokeEx(f => f.pgbSync.Step = 1);
            this.InvokeEx(f => f.pgbSync.Value = 0);
        }

        private void syncEnd()
        {
            this.InvokeEx(f => f.Enabled = true);
            this.InvokeEx(f => f.UpdateFileList());
        }

        private void Synchronize()
        {
            List<string> src = param[0];
            List<string> dest = param[1];
            List<string> prefixes = param[2];
            string filename = (prefixes[0].Contains(FILENAME) ? "" : (FILENAME + "\\"));
            syncStart(src.Count * dest.Count);

            try
            {
                dest.ForEach(strD =>
                {
                    string padding = "";
                    if (strD.Last() != '\\')
                        padding = "\\";
                    if (!Directory.Exists(strD + padding + filename))
                        Directory.CreateDirectory(strD + padding + filename);
                    src.ForEach(str =>
                    {
                        string localPath = "";
                        int j;

                        for (j = 0; j < prefixes.Count; j++)
                        {
                            if (str.Contains(prefixes[j]))
                                break;
                        }

                        if (j >= prefixes.Count)
                            throw new IndexOutOfRangeException("Les fichiers et les dossiers ne concordent pas.");

                        string prefix = oldBUFile.GetPathArray(prefixes[j]).Last();
                        string file = oldBUFile.GetPathArray(str).Last();
                        localPath = str.Replace(file, "").Replace(prefixes[j], "");

                        string strDest = strD + padding + filename + prefix + localPath + file;
                        if (File.Exists(strDest))
                            File.Copy(str, strDest, true);
                        else
                        {
                            string[] path = oldBUFile.GetPathArray(strDest);
                            string newPath = "";
                            for (int i = 0; i < path.Length - 1; i++)
                            {
                                if (i != 0)
                                    newPath = newPath + "\\";
                                newPath = newPath + path[i];
                            }

                            Directory.CreateDirectory(newPath);
                            File.Copy(str, strDest);
                        }
                        this.InvokeEx(f => f.pgbSync.PerformStep());

                        if (File.GetAttributes(str).HasFlag(FileAttributes.Archive))
                            File.SetAttributes(str, (FileAttributes)(File.GetAttributes(str) - FileAttributes.Archive));
                    });
                });
            }
            catch (Exception e)
            {
                ShowError(e.Message, "Erreur dans la Synchronisation");
            }
            finally
            {
                syncEnd();
            }
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
            foreach (oldBUFile data in WorkspacesData)
            {
                TreeNode node = data.getTree(true, this);
                if (node == null)
                    break;
                if (lstFiles.Nodes.Contains(node))
                {
                    int i = lstFiles.Nodes.IndexOf(node);
                    TreeNode fused = oldBUFile.fuseTrees(lstFiles.Nodes[i], node);
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
            foreach (oldBUFile data in WorkspacesData)
                blnData = blnData || data.Exists;
            ok = ok && blnData;
            ok = ok && lstBackupLoc.Items.Count > 0;
            ok = ok && lstFiles.Nodes.Count > 0;

            btnSync.Enabled = ok && filesToSync.Count > 0;
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

        private void btnForcedBackUp_Click(object sender, EventArgs e)
        {
            List<string> lstSrcFiles = getWSFileList();
            List<string> lstDestFolders = getBUFolders(false);
            List<string>[] ret = { lstSrcFiles, lstDestFolders, makeWSPrefixes() };
            param = ret;
            Thread sync = new Thread(Synchronize);
            sync.Start();
        }

        private List<string> getBUFolders(bool addFilename)
        {
            List<string> retrun = new List<string>();

            foreach (string folder in lstBackupLoc.Items)
            {
                if (Directory.Exists(folder))
                {
                    string newValue = folder;
                    if (addFilename)
                    {
                        if (folder.Last().Equals("\\"))
                            newValue += "\\";
                        newValue += FILENAME + "\\";
                    }
                    retrun.Add(newValue);
                }
            }

            return retrun;
        }

        private List<string> getWSFolders()
        {
            List<string> retrun = new List<string>();

            foreach (oldBUFile folder in WorkspacesData)
            {
                string fd = oldBUFile.GetNodePath(folder.getTree());

                if (Directory.Exists(fd))
                {
                    retrun.Add(fd);
                }
            }

            return retrun;
        }

        private List<string> getWSFileList()
        {
            List<string> retrun = new List<string>();
            WorkspacesData.ForEach((oldBUFile folder) =>
            {
                List<string> test = makeFileList(oldBUFile.GetNodePath(folder.getTree()));

                if (test != null)
                    retrun.AddRange(test);
            });

            return retrun;
        }

        private List<string> getBUFileList()
        {
            List<string> retrun = new List<string>();
            foreach (string folder in lstBackupLoc.Items)
            {
                string padding = "";
                if (folder.Last() != '\\')
                {
                    padding = "\\";
                }
                List<string> test = makeFileList(folder+padding+FILENAME);

                if (test != null)
                    retrun.AddRange(test);
            }

            return retrun;
        }

        private List<string> makeFileList(string parent)
        {
            if (Directory.Exists(parent))
            {
                List<string> retrun = new List<string>();

                foreach (string path in Directory.EnumerateDirectories(parent))
                {
                    List<string> test = makeFileList(path);
                    if (test != null)
                        retrun.AddRange(test);
                }

                foreach (string file in Directory.EnumerateFiles(parent))
                {
                    retrun.Add(file);
                }

                return retrun;
            }

            return null;
        }

        private void btnForcedHome_Click(object sender, EventArgs e)
        {
            List<string> lstSrcFiles = getBUFileList();
            List<string> lstDestFolders = getWSFolders();
            List<string>[] ret = { lstSrcFiles, lstDestFolders, getBUFolders(true) };
            param = ret;
            Thread sync = new Thread(Synchronize);
            sync.Start();
        }
    }
}
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