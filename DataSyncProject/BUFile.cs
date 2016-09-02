using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataSyncProject
{
    class BUFile
    {
        static protected System.Drawing.Color errorColor = System.Drawing.Color.DarkRed;
        static protected System.Drawing.Color okColor = System.Drawing.Color.Green;
        static protected System.Drawing.Color warnColor = System.Drawing.Color.DarkGray;
        static protected System.Drawing.Color noColor = System.Drawing.Color.Black;
        public bool Exists { get { return blnExists; } }
        private TreeNode tncFiles;
        private bool blnExists;
        public BUFile(string home)
        {
            tncFiles = new TreeNode(home);
            Update();
        }
        public BUFile(TreeNode home)
        {
            tncFiles = home;
            isValid();
        }
        private void isValid()
        {
            if (Directory.Exists(GetNodePath()))
            {
                blnExists = true;
                tncFiles.ForeColor = noColor;
            }
            else
            {
                blnExists = false;
                tncFiles.ForeColor = errorColor;
            }
        }

        public void Update()
        {
            isValid();
            if (Exists && tncFiles.Nodes.Count == 0)
            {
                makeTree();
            }
        }

        private string GetNodePath()
        {
            return GetNodePath(tncFiles);
        }

        static public string GetNodePath(TreeNode node)
        {
            return node.ToString().Substring(10);
        }

        private void makeTree()
        {
            TreeNode[] newLevel = newNodeLevel(GetNodePath());
            if (newLevel == null)
                return;
            tncFiles.Nodes.AddRange(newLevel);
        }

        private TreeNode[] newNodeLevel(string strPath)
        {
            List<TreeNode> retrun = new List<TreeNode>();
            bool found = false;
            
            foreach (string dir in Directory.EnumerateDirectories(strPath))
            {
                TreeNode tnNew = new TreeNode(GetPathArray(dir).Last());
                TreeNode[] newLevel = newNodeLevel(dir);
                if (newLevel != null)
                {
                    found = true;
                    tnNew.Nodes.AddRange(newLevel);
                    retrun.Add(tnNew);
                }
                /*else
                {
                    tnNew.ForeColor = System.Drawing.Color.DarkGray;
                }*/
            }

            foreach (string file in Directory.EnumerateFiles(strPath))
            {
                TreeNode tnNew = new TreeNode(file.ToString().Split('\\').Last());
                retrun.Add(tnNew);
                found = true;
            }

            if (found)
                return retrun.ToArray();
            else
                return null;
        }

        public TreeNode getTree()
        {
            return tncFiles;
        }

        public TreeNode getTree(bool chked, Main form)
        {
            TreeNode retrun = new TreeNode(GetNodePath(tncFiles));
            TreeNode[] found = makeTree(tncFiles, chked, GetNodePath(tncFiles), form);
            if (found == null)
                return null;
            retrun.Nodes.AddRange(found);
            return retrun;
        }

        private TreeNode[] makeTree(TreeNode node, bool chked, string parent, Main form)
        {
            List<TreeNode> retrun = new List<TreeNode>();
            bool found = false;
            foreach (TreeNode n in node.Nodes)
            {
                bool added = false;
                if (n.Checked == chked)
                {
                    string file = parent + "\\" + GetNodePath(n);
                    if (Directory.Exists(file) || File.Exists(file))
                    {
                        retrun.Add(new TreeNode(GetNodePath(n)));

                        if (form.addSyncFile(file))
                        {
                            // TODO: check last modified date with backup if possible.
                            retrun.Last().ForeColor = errorColor;
                        }
                        else if (!File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                        {
                            retrun.Last().ForeColor = okColor;
                        }
                        else
                        {
                            retrun.Last().ForeColor = noColor;
                        }
                        added = true;
                        found = true;
                    }
                }

                if (retrun.Count > 0)
                {
                    TreeNode[] newNodes = makeTree(n, chked, (parent + "\\" + GetNodePath(retrun.Last())), form);
                    if (newNodes != null)
                    {
                        if (!added)
                            retrun.Add(new TreeNode(GetNodePath(n)));
                        retrun.Last().Nodes.AddRange(newNodes);
                        found = true;
                    }
                }
            }

            if (found)
                return retrun.ToArray();
            return null;
        }

        public void Update(TreeNodeCollection nodes)
        {
            tncFiles = nodes[0];
        }
        static public string[] GetPathArray(string path)
        {
            return path.Split('\\');
        }

        static public TreeNode fuseTrees(TreeNode node1, TreeNode node2)
        {
            if (node1.ToString() != node2.ToString())
                return null;
            TreeNode retrun = new TreeNode(GetNodePath(node1));
            TreeNode incep = null;

            // Iterrer sur toutes les nodes enfants de la node 1
            for (int i = 0; i < node1.Nodes.Count; i++)
            {
                bool found = false;

                // Iterrer sur toutes les nodes enfants de la node 2
                for (int j = 0; j < node2.Nodes.Count; j++)
                {
                    // Appel récursif pour les Nodes enfants de même nom.
                    if (node1.Nodes[i].ToString() == node2.Nodes[j].ToString())
                    {
                        found = true;
                        incep = fuseTrees(node1.Nodes[i], node2.Nodes[j]);
                    }

                    // Ajouter la node si elle n'est pas déjà là (node2).
                    if (!retrun.Nodes.Contains(node2.Nodes[j]))
                    {
                        retrun.Nodes.Add(node2.Nodes[j]);
                    }
                }

                if (!found)
                {
                    // Ajouter la node si elle n'est pas déjà là (node1).
                    if (!retrun.Nodes.Contains(node1.Nodes[i]))
                    {
                        retrun.Nodes.Add(node1.Nodes[i]);
                    }
                }
                else
                {
                    // On ajoute les nodes enfants communs trouvés cette fois-ci.
                    int j = retrun.Nodes.IndexOf(node1.Nodes[i]);
                    retrun.Nodes[j].Nodes.Add(incep);
                }
                incep = null;
            }

            return retrun;
        }

        public string getLastFolder()
        {
            return GetPathArray(GetNodePath()).Last();
        }
    }
}
