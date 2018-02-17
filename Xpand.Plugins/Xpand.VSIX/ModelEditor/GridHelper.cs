using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.DXCore.Controls.XtraGrid;
using EnvDTE;
using EnvDTE80;
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

        private static void EventsSolutionEventsOnOpened(){
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
            List<ProjectItemWrapper> projectWrappers = new List<ProjectItemWrapper>();
            Task.Factory.StartNew(() => projectWrappers = ProjectWrapperBuilder.GetProjectItemWrappers().ToList())
                .ContinueWith(task1 => {
                    if (task1.Exception != null){
                        DteExtensions.DTE.LogError(task1.Exception.ToString());
                        DteExtensions.DTE.WriteToOutput(task1.Exception.ToString());
                    }
                    _gridControl.DataSource = new BindingList<ProjectItemWrapper>(projectWrappers);
                },_currentSynchronizationContext);
        }

        public static void Init(GridControl gridControl) {
            Setup(gridControl);
        }
    }
}