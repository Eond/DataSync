using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KnownColor = System.Drawing.KnownColor;

namespace DataSyncProject
{
    class oldBUFile
    {
        static protected System.Drawing.Color errorColor = System.Drawing.Color.DarkRed;
        static protected System.Drawing.Color okColor = System.Drawing.Color.Green;
        static protected System.Drawing.Color warnColor = System.Drawing.Color.DarkGray;
        static protected System.Drawing.Color noColor = System.Drawing.Color.Black;
        public bool Exists { get { return blnExists; } }
        private TreeNode tncFiles;
        private bool blnExists;
        public oldBUFile(string home)
        {
            tncFiles = new TreeNode(home);
            Update();
        }
        public oldBUFile(TreeNode home)
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
    /// <summary>
    /// Assemble et valide les informations de sauvegarde d'un fichier sur le poste.
    /// </summary>
    class BUFile
    {
        /// <summary>
        /// Liste de couleurs pour l'affichage.
        /// </summary>
        public class Color
        {
            /// <summary>
            /// Texte normal.
            /// </summary>
            public const KnownColor Normal = KnownColor.Black;
            /// <summary>
            /// Élément qui sera ignoré.
            /// </summary>
            public const KnownColor Ignore = KnownColor.DarkGray;
            /// <summary>
            /// Élément en erreur ou en besoin d'action.
            /// </summary>
            public const KnownColor Error = KnownColor.DarkRed;
            /// <summary>
            /// Élément requierant une attention pariculière
            /// </summary>
            public const KnownColor Warn = KnownColor.YellowGreen;
            /// <summary>
            /// Élément sans problème.
            /// </summary>
            public const KnownColor Ok = KnownColor.DarkGreen;
            /// <summary>
            /// Il est impossible de créer un objet de classe <see cref="Color"/>. 
            /// </summary>
            private Color()
            {
            }
        }
        /// <summary>
        /// Contient le chemin complet du fichier.
        /// <para>Accès externe par <see cref="FullPath"/>.</para>
        /// </summary>
        private PathObj _fullPath;
        /// <summary>
        /// Contient le préfixe de l'emplacement de travail sur le poste actuel.
        /// <para>Accès externe par <see cref="Prefix"/>.</para>
        /// </summary>
        private PathObj _prefix;
        /// <summary>
        /// Contient le chemin après le préfix pour atteindre le fichier.
        /// <para>Acces externe par <see cref="Local"/>.</para>
        /// </summary>
        private PathObj _local;
        /// <summary>
        /// Liste des emplacements de sauvegarde.
        /// <para>Accès externe par <see cref="Dest"/>.</para>
        /// </summary>
        private List<PathObj> _dest = new List<PathObj>();
        /// <summary>
        /// Date et heure de dernière modification.
        /// </summary>
        private DateTime _lastChanges;
        /// <summary>
        /// Liste des dates de dernières modifications des fichiers sauvegardés.
        /// </summary>
        private List<DateTime> _destLastChanges = new List<DateTime>();
        /// <summary>
        /// Couleur du texte dans l'interface.
        /// <para>Accès externe par <see cref="TextColor"/>.</para>
        /// </summary>
        private KnownColor _textColor = Color.Normal;
        /// <summary>
        /// Couleur du fond dans l'interface.
        /// <para>Accès externe par <see cref="BackColor"/>.</para>
        /// </summary>
        private KnownColor _backColor = Color.Normal;
        /// <summary>
        /// Retourne la valeur de la couleur voulu pour le texte.
        /// </summary>
        public KnownColor TextColor
        {
            get
            {
                return _textColor;
            }
        }
        /// <summary>
        /// Retourne la valeur de la couleur voulu pour le fond.
        /// </summary>
        public KnownColor BackColor
        {
            get
            {
                return _backColor;
            }
        }
        /// <summary>
        /// Retourne la liste des destinations de sauvegarde.
        /// </summary>
        public List<PathObj> Dest
        {
            get
            {
                return _dest;
            }
        }
        /// <summary>
        /// Retourne le chemin complet du fichier.
        /// </summary>
        public string FullPath
        {
            get { return _fullPath; }
        }
        /// <summary>
        /// Permet la lecture et l'écriture du préfixe.
        /// </summary>
        public string Prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                PathObj nPath = value;
                if (!nPath.ForceAbsPath())
                    throw new ArgumentException("La nouvelle valeur pour le préfixe est invalide dans le contexte actuel.");
                if (nPath.Exists && FullPath.Contains(nPath) && nPath.Attributes.HasFlag(FileAttributes.Directory))
                {
                    PathObj _old = _prefix;
                    _prefix = nPath;
                    _local = _fullPath - _prefix;
                    if (_local == "" || _local == _fullPath)
                    {
                        _prefix = _old;
                        throw new ArgumentException("La valeur fournie ne fonctionne pas.");
                    }
                }
                else
                    throw new ArgumentException("Mauvaise valeur pour le nouveau préfixe.");
            }
        }
        /// <summary>
        /// Permet la lecture du chemin suivant le <see cref="Prefix"/>.
        /// </summary>
        public string Local
        {
            get
            {
                return _local;
            }
        }
        /// <summary>
        /// Retourne les derniers attributs connus du fichier.
        /// </summary>
        public FileAttributes Attributes
        {
            get
            {
                return _fullPath.Attributes;
            }
        }
        public ErrorCode LastErrorCodes { get; private set; }
        /// <summary>
        /// Constructeur initial. L'objet est créé seulement si la référence est valide.
        /// </summary>
        /// <param name="file">Chemin complet pointant un fichier sur le poste local.</param>
        public BUFile(string file)
        {
            _fullPath = file;
            if (!_fullPath.ForceAbsPath())
                throw new ArgumentException("Fichier invalide.");
            IsValid(true);
            Prefix = _fullPath.GetPathRoot();
        }
        /// <summary>
        /// Constructeur par copie. On sait que l'objet a déjà exister, donc on va contourner certaines vérifications.
        /// </summary>
        /// <param name="file">Fichier duquel nous allons récupérer les informations.</param>
        public BUFile(BUFile file)
        {
            _fullPath = file.FullPath;
            _prefix = file.Prefix;
            _dest = file.Dest;
            IsValid();
        }
        /// <summary>
        /// Surcharge de <see cref="IsValid()"/>. Valide la référence et initie les information lorsque possible.
        /// <para><seealso cref="LastErrorCodes"/></para>
        /// </summary>
        /// <param name="init">Indique si nous sommes en initiation. Retourne toujours vrai dans ce cas ou sort en erreur.</param>
        /// <returns>Vrai si tout est valide pour la référence et qu'il y a au moins une destination de sauvegarde valide. Si en initiation, toujours Vrai.</returns>
        private bool IsValid(bool init)
        {
            bool retrun = true;
            LastErrorCodes = ErrorCode.None;

            if (string.IsNullOrWhiteSpace(_fullPath))
                throw new System.ArgumentNullException("Référence invalide.");

            if (init) // On est en initiation de l'objet.
            {
                if (Attributes.HasFlag(FileAttributes.Directory))
                    throw new DirectoryFileException();
            }
            else // L'objet est déjà initié.
            {
                if (!_fullPath.Exists)
                {
                    LastErrorCodes &= ErrorCode.FileMissing;
                    retrun = false;
                }
                else
                {
                    if (Attributes.HasFlag(FileAttributes.Directory))
                    {
                        LastErrorCodes &= ErrorCode.IsFolder;
                        retrun = false;
                    }

                    _lastChanges = File.GetLastWriteTime(_fullPath);

                    if (Dest.Count <= 0)
                    {
                        LastErrorCodes &= ErrorCode.NoDest;
                        retrun = false;
                    }
                    else
                    {
                        bool found = false;
                        Dest.ForEach(p => {
                            if (p.Exists)
                            {
                                _destLastChanges[Dest.IndexOf(p)] = File.GetLastWriteTime(p);
                                found = true;
                            }
                            else if (p.CanCreate)
                                found = true;
                        });
                        if (!found)
                        {
                            LastErrorCodes &= ErrorCode.NoValidDest;
                            retrun = false;
                        }
                    }

                    if (Attributes.HasFlag(FileAttributes.Archive))
                        _textColor = Color.Error;

                    _destLastChanges.ForEach(d =>
                    {
                        if (d > _lastChanges)
                        {
                            _backColor = Color.Warn;
                        }
                    });
                }
                if (!_prefix.Exists)
                {
                    LastErrorCodes &= ErrorCode.PrefixInvalid;
                    retrun = false;
                }
            }

            return retrun;
        }
        /// <summary>
        /// Valide la référence.
        /// </summary>
        /// <returns>Vrai si tout est valide et qu'au moins une destination de sauvegarde est valide.</returns>
        public bool IsValid()
        {
            return IsValid(false);
        }
        public void AddDest(PathObj dest)
        {
            if (!_prefix.Exists)
                throw new Exception("Préfixe de station invalide.");
            if (dest.Exists && dest.Attributes.HasFlag(FileAttributes.Directory))
            {
                if (dest.GetLastFolder() != _prefix.GetLastFolder())
                    dest.Append(_prefix.GetLastFolder());
                
            }
            else
                throw new System.ArgumentException("La destination de sauvegarde n'est pas un dossier valide.");
        }
        /// <summary>
        /// Ajoute un tableau de <see cref="PathObj"/> aux destinations.
        /// </summary>
        /// <param name="dests">Tableau de destinations.</param>
        public void AddDestRange(PathObj[] dests)
        {
            foreach (PathObj dest in dests)
            {
                AddDest(dest);
            }
        }
        /// <summary>
        /// Ajoute une liste de <see cref="PathObj"/> aux destinations.
        /// </summary>
        /// <param name="dests">Liste de destinations.</param>
        public void AddDestRange(List<PathObj> dests)
        {
            dests.ForEach(dest => AddDest(dest));
        }
        /// <summary>
        /// Liste des codes d'erreur de BUFile
        /// </summary>
        [Flags]
        public enum ErrorCode : short
        {
            /// <summary>
            /// Pas d'erreur
            /// </summary>
            None = 0,
            /// <summary>
            /// La référence au fichier est invalide ou le dossier est manquant.
            /// </summary>
            FileMissing,
            /// <summary>
            /// La référence est un dossier. Doit être un fichier.
            /// </summary>
            IsFolder,
            /// <summary>
            /// Pas de dossier de sauvegarde.
            /// </summary>
            NoDest,
            /// <summary>
            /// Pas de dossier de sauvegarde valide.
            /// </summary>
            NoValidDest,
            /// <summary>
            /// Le préfixe n'est pas valide dans le contexte actuel.
            /// </summary>
            PrefixInvalid
        }
        /// <summary>
        /// Exception de référence invalide. On a reçu un dossier alors qu'on voulait un fichier.
        /// </summary>
        class DirectoryFileException : Exception
        {
            /// <summary>
            /// Message d'erreur.
            /// </summary>
            //ublic override string Message { get; }
            /// <summary>
            /// Initialisation avec un message personalisé.
            /// </summary>
            /// <param name="msg">Message à passer au système.</param>
            public DirectoryFileException(string msg) : base(msg)
            {
                if (string.IsNullOrWhiteSpace(msg))
                    msg = "La référence doit être un fichier.";
            }
            /// <summary>
            /// Initialisation de base. Le messag eest généré automatiquement.
            /// </summary>
            public DirectoryFileException() : base("La référence doit être un fichier.")
            {
            }
        }
    }
}
