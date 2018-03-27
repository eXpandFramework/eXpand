using System;
using System.IO;
using System.Reactive.Linq;

namespace Xpand.ExpressApp.ExcelImporter.Services{
    /// <summary>
    ///     This is a wrapper around a file system watcher to use the Rx framework instead of event handlers to handle
    ///     notifications of file system changes.
    /// </summary>
    public class ObservableFileSystemWatcher : IDisposable{
        public readonly FileSystemWatcher Watcher;

        /// <summary>
        ///     Pass an existing FileSystemWatcher instance, this is just for the case where it's not possible to only pass the
        ///     configuration, be aware that disposing this wrapper will dispose the FileSystemWatcher instance too.
        /// </summary>
        /// <param name="watcher"></param>
        public ObservableFileSystemWatcher(FileSystemWatcher watcher){
            Watcher = watcher;

            Changed = Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => Watcher.Changed += h,
                    h => Watcher.Changed -= h)
                .Select(x => x.EventArgs);

            Renamed = Observable
                .FromEventPattern<RenamedEventHandler, RenamedEventArgs>(h => Watcher.Renamed += h,
                    h => Watcher.Renamed -= h)
                .Select(x => x.EventArgs);

            Deleted = Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => Watcher.Deleted += h,
                    h => Watcher.Deleted -= h)
                .Select(x => x.EventArgs);

            Errors = Observable
                .FromEventPattern<ErrorEventHandler, ErrorEventArgs>(h => Watcher.Error += h, h => Watcher.Error -= h)
                .Select(x => x.EventArgs);

            Created = Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => Watcher.Created += h,
                    h => Watcher.Created -= h)
                .Select(x => x.EventArgs);
        }

        /// <summary>
        ///     Pass a function to configure the FileSystemWatcher as desired, this constructor will manage creating and applying
        ///     the configuration.
        /// </summary>
        public ObservableFileSystemWatcher(Action<FileSystemWatcher> configure)
            : this(new FileSystemWatcher()){
            configure(Watcher);
        }

        public IObservable<FileSystemEventArgs> Changed{ get; }
        public IObservable<RenamedEventArgs> Renamed{ get; }
        public IObservable<FileSystemEventArgs> Deleted{ get; }
        public IObservable<ErrorEventArgs> Errors{ get; }
        public IObservable<FileSystemEventArgs> Created{ get; }

        public void Dispose(){
            Watcher.Dispose();
        }

        public void Start(){
            Watcher.EnableRaisingEvents = true;
        }

        public void Stop(){
            Watcher.EnableRaisingEvents = false;
        }
    }
}