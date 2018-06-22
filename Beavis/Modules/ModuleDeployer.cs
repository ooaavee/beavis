using System;
using System.IO;
using System.Linq;

namespace Beavis.Modules
{
    public class ModuleDeployer
    {
        private static readonly string[] ExcludedFiles = {
            "Beavis.runtimeconfig.dev.json"
        };

        private readonly ModuleHostingEnvironment _hostingEnvironment;

        public ModuleDeployer(ModuleHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public ModuleHandle Deploy(ModuleInfo module)
        {            
            string appBaseDirectory = AppContext.BaseDirectory;

            DirectoryInfo moduleBaseDirectory = _hostingEnvironment.NewModuleBaseDirectory(module);

            DirectoryCopy(appBaseDirectory, moduleBaseDirectory.FullName, true);

            var handle = new ModuleHandle(module, moduleBaseDirectory);
            return handle;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                // Ignore excluded files!
                if (ExcludedFiles.Any(x => String.Equals(x, file.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
