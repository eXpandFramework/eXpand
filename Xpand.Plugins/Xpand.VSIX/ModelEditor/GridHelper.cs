using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraGrid;
using EnvDTE;
using EnvDTE80;
using Mono.Cecil;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.ModelEditor {
    public class GridHelper {
        static GridControl _gridControl;
        private static Events2 _events;
        private static SolutionEvents _eventsSolutionEvents;
        private static IEnumerable<FileSystemWatcher> _fileSystemWatchers;
        private static TaskScheduler _currentSynchronizationContext;

        static void Setup(GridControl gridControl) {
            _gridControl = gridControl;
            _currentSynchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();
            SetGridDataSource();
            _events = (Events2) DteExtensions.DTE.Events;
            _eventsSolutionEvents = _events.SolutionEvents;
            _eventsSolutionEvents.Opened += EventsSolutionEventsOnOpened;
            _eventsSolutionEvents.AfterClosing+=EventsSolutionEventsOnAfterClosing;
        }

        private static void EventsSolutionEventsOnAfterClosing(){
            if (_fileSystemWatchers != null)
                foreach (var fileSystemWatcher in _fileSystemWatchers){
                    fileSystemWatcher.Changed -= SystemWatcherOnChanged;
                }
        }

        public static string ExtractME() {
            try {
                var resourceStream =
                    typeof(ModelToolWindow).Assembly.GetManifestResourceStream(
                        "Xpand.VSIX.ModelEditor.Xpand.ExpressApp.ModelEditor.exe");
                var mePath = Path.Combine($"{Path.GetTempPath()}\\XpandModelEditor",
                    $"Xpand.ExpressApp.ModelEditor{DateTime.Now.Ticks}.exe");
                Debug.Assert(resourceStream != null, "resourceStream != null");
                var bytes = new byte[(int) resourceStream.Length];
                resourceStream.Read(bytes, 0, bytes.Length);
                var directoryName = $"{Path.GetDirectoryName(mePath)}";
                if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
                File.WriteAllBytes(mePath, bytes);

                using (var assemblyDefinition =
                    AssemblyDefinition.ReadAssembly(mePath, new ReaderParameters {ReadWrite = true})) {
                    var dxVersion = Version.Parse(DteExtensions.DTE.Solution.GetDXVersion(false));
                    DteExtensions.DTE.WriteToOutput($"Patching ME for version {dxVersion}");
                    var moduleDefinition = assemblyDefinition.MainModule;
                    var dxReferences = moduleDefinition.AssemblyReferences.Where(reference =>reference.Name.StartsWith("DevExpress"));
                    foreach (var assemblyNameReference in dxReferences.Where(reference => reference.Version != dxVersion).ToArray()) {
                        assemblyNameReference.Name=assemblyNameReference.Name.Replace($"{assemblyNameReference.Version.Major}.{assemblyNameReference.Version.Minor}",$"{dxVersion.Major}.{dxVersion.Minor}");
                        assemblyNameReference.Version = dxVersion;
                        var newReference = AssemblyNameReference.Parse(assemblyNameReference.FullName);
                        moduleDefinition.AssemblyReferences.Remove(assemblyNameReference);
                    
                        moduleDefinition.AssemblyReferences.Add(newReference);
                        foreach (var typeReference in moduleDefinition.GetTypeReferences()
                            .Where(_ => _.Scope == assemblyNameReference).ToArray()) typeReference.Scope = newReference;
                    }

                    assemblyDefinition.Write();
                }


                return mePath;
            }
            catch (Exception e) {
                DteExtensions.DTE.LogError(e.ToString());
                DteExtensions.DTE.WriteToOutput(e.ToString());
                throw;
            }
        }

        private static void EventsSolutionEventsOnOpened() {
            ModelEditorRunner.MePath = ExtractME();
            SetGridDataSource();
            try{
                _fileSystemWatchers =GetFileSystemWatchers();
            }
            catch (Exception e){
                DteExtensions.DTE.WriteToOutput(e.ToString());
            }
        }

        private static IEnumerable<FileSystemWatcher> GetFileSystemWatchers(){
            var solution = DteExtensions.DTE.Solution;
            var fileSystemWatchers = new[]{CreateFileSystemWatcher(solution.FullName) }
                .Concat(solution.Projects().Where(project => File.Exists(project.FullName)).Select(project =>CreateFileSystemWatcher(project.FullName))).ToArray();
            foreach (var fileSystemWatcher in fileSystemWatchers) {
                fileSystemWatcher.Changed += SystemWatcherOnChanged;
            }
            return fileSystemWatchers;
        }

        private static FileSystemWatcher CreateFileSystemWatcher(string fileName){
            return new FileSystemWatcher(Path.GetDirectoryName(fileName) +""){NotifyFilter = NotifyFilters.LastWrite,Filter = Path.GetFileName(fileName),EnableRaisingEvents = true};
        }

        private static void SystemWatcherOnChanged(object sender, FileSystemEventArgs e){
            try{
                foreach (var fileSystemWatcher in _fileSystemWatchers){
                    fileSystemWatcher.Changed-=SystemWatcherOnChanged;
                    fileSystemWatcher.Dispose();
                }
                _fileSystemWatchers=GetFileSystemWatchers();
                SetGridDataSource();
            }
            catch (Exception exception){
                DteExtensions.DTE.WriteToOutput(exception.ToString());
            }
        }

        private static void SetGridDataSource(){
            var projectWrappers = new List<ProjectItemWrapper>();
            Task.Factory.StartNew(() => projectWrappers = ProjectWrapperBuilder.GetProjectItemWrappers().ToList())
                .ContinueWith(task1 => {
                    if (task1.Exception != null){
                        DteExtensions.DTE.LogError(task1.Exception.ToString());
                        DteExtensions.DTE.WriteToOutput(task1.Exception.ToString());
                    }
                    _gridControl.DataSource = new BindingList<ProjectItemWrapper>(projectWrappers.GroupBy(wrapper => wrapper.FullPath).Select(_ => _.First()).ToArray());
                },_currentSynchronizationContext);
        }

        public static void Init(GridControl gridControl) {
            Setup(gridControl);
        }
    }
}