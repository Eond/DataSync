using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOPath = System.IO.Path;

namespace DataSyncProject
{
    /// <summary>
    /// Permet les opérations de chemin système sur un objet déclaré comme tel, pas juste sur une chaîne.
    /// </summary>
    class PathObj
    {
        /// <summary>
        /// Chemin d'accès enregistré.
        /// <para>Accès externe par <see cref="Path"/>.</para>
        /// </summary>
        private string _path;

        /// <summary>
        /// Chemin d'accès.
        /// </summary>
        public string Path
        {
            get
            {
                if (Exists)
                    return GetFullPath();
                return _path;
            }
        }

        /// <summary>
        /// Initialise et valide l'objet.
        /// </summary>
        /// <param name="file">Chemin d'accès. Doit contenir au moins un niveau pour être considéré un chemin d'accès système.</param>
        public PathObj(string file)
        {
            if (!file.Contains(IOPath.DirectorySeparatorChar))
                throw new System.ArgumentException("La chaîne fournie ne comporte aucun séparateur de dossier.");
            bool found = false;
            foreach (char c in IOPath.GetInvalidFileNameChars())
                found = found || file.Contains(c);
            if (found)
                throw new System.ArgumentException("La chaîne contient des caractères interdits dans les chemins d'accès.");
            _path = file;
        }

        public PathObj(IEnumerable<string> arr)
        {
            /*if (arr.All(<s, b> =>
            {
             TODO: Verifier charactères invalides.
            })*/
            if (arr.Count() == 1)
                _path = arr.First() + Separator;
            else
            {
                arr.ToList().ForEach(s =>
                {
                    if (s == arr.Last())
                        _path += s;
                    else
                        _path += s + Separator;
                });
            }
        }

        /// <summary>
        /// Retourne les informations relatives au répertoire de <see cref="Path"/>.
        /// </summary>
        /// <returns></returns>
        public string GetDirectoryName()
        {
            return IOPath.GetDirectoryName(Path);
        }

        /// <summary>
        /// Retourne l'extension de <see cref="Path"/>.
        /// </summary>
        /// <returns></returns>
        public string GetExtension()
        {
            return IOPath.GetExtension(Path);
        }

        /// <summary>
        /// Retourne le nom et l'extension de fichier de <see cref="Path"/>.
        /// </summary>
        /// <returns></returns>
        public string GetFileName()
        {
            return IOPath.GetFileName(Path);
        }

        /// <summary>
        /// Retourne le nom de fichier de <see cref="Path"/> sans l'extension.
        /// </summary>
        /// <returns></returns>
        public string GetFileNameWithoutExtension()
        {
            return IOPath.GetFileNameWithoutExtension(Path);
        }

        /// <summary>
        /// Retourne le chemin d'accès absolu de <see cref="Path"/>.
        /// </summary>
        /// <returns></returns>
        public string GetFullPath()
        {
            return IOPath.GetFullPath(_path);
        }

        /// <summary>
        /// Obtient les informations relatives au répertoire racine de <see cref="Path"/>.
        /// </summary>
        /// <returns></returns>
        public string GetPathRoot()
        {
            if (IsPathRooted())
                return IOPath.GetPathRoot(Path);
            return "";
        }

        /// <summary>
        /// Détermine si <see cref="Path"/> inclut une extension de nom de fichier.
        /// </summary>
        /// <returns></returns>
        public bool HasExtension()
        {
            return IOPath.HasExtension(Path);
        }

        /// <summary>
        /// Obtient une value indiquant si <see cref="Path"/> contient une racine.
        /// </summary>
        /// <returns></returns>
        public bool IsPathRooted()
        {
            return IOPath.IsPathRooted(Path);
        }

        /// <summary>
        /// Conversion manuelle en chaîne de caractères.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Path;
        }

        /// <summary>
        /// Retourne la liste de l'arborescence de dossier de <see cref="Path"/>.
        /// </summary>
        /// <returns></returns>
        public string[] GetPathList()
        {
            return Path.Split(IOPath.DirectorySeparatorChar);
        }

        /// <summary>
        /// Conversion automatique en chaîne de caractères.
        /// </summary>
        /// <param name="path">Chemin à convertir.</param>
        public static implicit operator string(PathObj path)
        {
            return path.Path;
        }

        /// <summary>
        /// Conversion automatique d'une chaîne de caractère en PathObj.
        /// </summary>
        /// <param name="str">Chaîne à convertir.</param>
        public static implicit operator PathObj(string str)
        {
            return new PathObj(str);
        }

        /// <summary>
        /// Indique si on peut créer l'arborescence, en théorie.
        /// </summary>
        public bool CanCreate { get { return File.Exists(GetPathRoot()); } }

        /// <summary>
        /// Indique si le chemin d'accès existe dans l'environement actuel.
        /// </summary>
        public bool Exists { get { return File.Exists(_path); } }

        /// <summary>
        /// Renvoie les attributs du chemin d'accès.
        /// </summary>
        public FileAttributes Attributes
        {
            get
            {
                return File.GetAttributes(Path);
            }
        }

        /// <summary>
        /// Ré-écrit le chemin d'accès enregistré pour son chemin absolu.
        /// </summary>
        /// <returns>Vrai si le chemin était valide dans le contexte actuel et donc que le changement fut effectué.</returns>
        public bool ForceAbsPath()
        {
            if (Exists)
            {
                _path = GetFullPath();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retire le nom de fichier du chemin d'accès, s'il y en avait un.
        /// </summary>
        /// <returns>Le chemin d'accès sans nom de fichier</returns>
        public string GetFolders()
        {
            string[] list = GetPathList();
            if (list.Count() <= 1)
                throw new IndexOutOfRangeException("Pas assez d'éléments dans l'arborescence.");
            string aPath = "";
            string bPath = "";
            for (int i = 0; i < list.Count(); i++)
            {
                string padding = "";
                if (!list[i].Contains(Separator))
                    padding = Separator.ToString();
                if (i < list.Count() - 1)
                    bPath += list[i] + padding;
                if (i == list.Count() - 1)
                    aPath += list[i];
                else
                    aPath += list[i] + padding;
            }

            if (File.Exists(aPath))
            {
                if (File.GetAttributes(aPath).HasFlag(FileAttributes.Directory))
                    return aPath;
            }

            if (File.Exists(bPath))
            {
                if (File.GetAttributes(bPath).HasFlag(FileAttributes.Directory))
                    return bPath;
            }

            return "";
        }

        /// <summary>
        /// Retourne le dernier dossier du chemin d'accès.
        /// </summary>
        /// <returns></returns>
        public string GetLastFolder()
        {
            PathObj folders = GetFolders();
            if (folders == "")
                throw new IndexOutOfRangeException("Le chemin d'accès est invalide.");
            string[] list = folders.GetPathList();
            return list[list.Count() - 1];
        }

        /// <summary>
        /// Ajoute un niveau à l'arborescence actuelle. Si le chemin d'accès pointe un fichier, le dossier est ajouté en avant du fichier.
        /// </summary>
        /// <param name="level">Nom du dossier à ajouter.</param>
        public void Append(string level)
        {
            bool valid = true;
            foreach (char c in IOPath.GetInvalidPathChars())
            {
                valid = valid && !level.Contains(c);
            }
            if (!valid)
                throw new ArgumentException("Nom de dossier invalide.");
            string aPath = GetFolders();
            string file = "";
            if (!Attributes.HasFlag(FileAttributes.Directory))
                file = Separator + GetFileName();
            _path = aPath + Separator + level + file;
        }

        /// <summary>
        /// Raccourcis pour avoir le caractère de séparation de dossier.
        /// </summary>
        public static char Separator
        {
            get
            {
                return IOPath.DirectorySeparatorChar;
            }
        }
        /// <summary>
        /// Retire une portion de chemin d'accès.
        /// </summary>
        /// <param name="aPath">Chemin d'accès principal. Doit être plus gros.</param>
        /// <param name="bPath">Chemin d'accès à retirer. Doit être inclu dans aPath.</param>
        /// <returns></returns>
        public static PathObj operator-(PathObj aPath, PathObj bPath)
        {
            string strA = aPath.ToString();
            string strB = bPath.ToString();
            List<string> arrA = new List<string>(aPath.GetPathList());
            List<string> arrB = new List<string>(bPath.GetPathList());

            if (strA.Contains(strB) && arrA.Contains(arrB[0]))
            {
                int b = 0;
                int a = 0;
                while (a < arrA.Count() && arrA[a] != arrB[b])
                    a++;
                while (b < arrB.Count())
                {
                    if (arrA[a] != arrB[b] || a >= arrA.Count())
                        break;
                    arrA.RemoveAt(a);
                    b++;
                }
            }

            return aPath;
        }
    }
}
