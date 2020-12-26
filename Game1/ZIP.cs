using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public static class ZIP
    {       

        /// <summary>
        ///     Unzip the whole file *half written*
        /// </summary>
        /// <param name="_file"></param>
        public static void Unzip(string _zippath, string _folder)
        {
            // make sure folder ends with a "/"
            if (!_folder.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                _folder += Path.DirectorySeparatorChar;



            using (ZipArchive archive = ZipFile.OpenRead(_zippath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Gets the full path to ensure that relative segments are removed.
                    string destinationPath = Path.GetFullPath(Path.Combine(_folder, entry.FullName));

                    // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                    // are case-insensitive.
                    if (destinationPath.StartsWith(_folder, StringComparison.Ordinal))
                        entry.ExtractToFile(destinationPath,true);
                }
            }
        }
    }
}
