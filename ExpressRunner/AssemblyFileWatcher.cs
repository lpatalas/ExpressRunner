using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner
{
    public class AssemblyFileWatcher
    {
        private readonly string assemblyFilePath;
        private readonly AssemblyTestGroup assemblyTestGroup;
        private readonly FileSystemWatcher fileSystemWatcher;

        public AssemblyFileWatcher(AssemblyTestGroup assemblyTestGroup, string sourceFilePath)
        {
            if (assemblyTestGroup == null)
                throw new ArgumentNullException("assemblyTestGroup");
            if (string.IsNullOrEmpty(sourceFilePath))
                throw new ArgumentNullException("sourceFilePath");

            this.assemblyFilePath = sourceFilePath.ToLower();
            this.assemblyTestGroup = assemblyTestGroup;
            this.fileSystemWatcher = CreateAssemblyFileWatcher(sourceFilePath);

            HookAssemblyEvents();
        }

        private FileSystemWatcher CreateAssemblyFileWatcher(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var watcher = new FileSystemWatcher
            {
                Filter = fileName,
                IncludeSubdirectories = false,
                Path = directory,
            };

            watcher.Changed += OnAssemblyFileChanged;
            watcher.Created += OnAssemblyFileChanged;
            watcher.Deleted += OnAssemblyFileChanged;
            watcher.Renamed += OnAssemblyFileChanged;
            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private async void OnAssemblyFileChanged(object sender, FileSystemEventArgs e)
        {
            if (string.Equals(assemblyFilePath, e.FullPath, StringComparison.OrdinalIgnoreCase))
            {
                Trace.TraceInformation("Reloading " + assemblyTestGroup.Name);
                await assemblyTestGroup.ReloadAsync();
            }
        }

        private void HookAssemblyEvents()
        {
            assemblyTestGroup.ReloadStarting += assemblyTestGroup_ReloadStarting;
            assemblyTestGroup.ReloadFinished += assemblyTestGroup_ReloadFinished;
        }

        private void assemblyTestGroup_ReloadStarting(object sender, EventArgs e)
        {
            fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void assemblyTestGroup_ReloadFinished(object sender, EventArgs e)
        {
            fileSystemWatcher.EnableRaisingEvents = true;
        }
    }
}
