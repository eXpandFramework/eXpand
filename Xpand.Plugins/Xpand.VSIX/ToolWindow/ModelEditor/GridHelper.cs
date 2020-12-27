using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DevExpress.XtraGrid;
using EnvDTE;
using EnvDTE80;
using Mono.Cecil;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.ToolWindow.ModelEditor {
    public class GridHelper {
        static GridControl _gridControl;
        private static Events2 _events;
        private static SolutionEvents _eventsSolutionEvents;
        
        static IObservable<Unit> Setup(GridControl gridControl,bool models=true) {
            if (models) {
                _gridControl = gridControl;
            }
            _events = (Events2) DteExtensions.DTE.Events;
            _eventsSolutionEvents = _events.SolutionEvents;
            var dataStoreFromWatchingFiles = Observable.FromEvent<_dispSolutionEvents_OpenedEventHandler, Unit>(
                    h => () => h(Unit.Default), h => _eventsSolutionEvents.Opened += h, h => _eventsSolutionEvents.Opened -= h)
                .SelectMany(_ => SetGridDataSource())
                .SelectMany(_ => GetFileSystemWatchers().ToObservable()
                    .SelectMany(watcher => Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Changed += h, h => watcher.Changed -= h)
                        .Where(pattern =>pattern.EventArgs.ChangeType==WatcherChangeTypes.Created||pattern.EventArgs.ChangeType==WatcherChangeTypes.Deleted ))
                    .TakeUntil(Observable.FromEvent<_dispSolutionEvents_AfterClosingEventHandler, Unit>(
                        h => () => h(Unit.Default), h => _eventsSolutionEvents.AfterClosing += h, h => _eventsSolutionEvents.AfterClosing -= h)))
                .ObserveOn(_gridControl)
                .Do(pattern => SetGridDataSource())
                .Select(pattern => Unit.Default);

            return SetGridDataSource(models)
                .Merge(dataStoreFromWatchingFiles);
        }

        public static IObservable<string> ExtractME()
            => Observable.Return(Unit.Default)
                .Select(_ => {
                    try {
                        var resourceStream = typeof(ModelToolWindow).Assembly.GetManifestResourceStream("Xpand.VSIX.ToolWindow.ModelEditor.Xpand.XAF.ModelEditor.exe");
                        var mePath = Path.Combine($"{Path.GetTempPath()}\\XpandModelEditor",
                            "Xpand.XAF.ModelEditor.exe");
                        Debug.Assert(resourceStream != null, "resourceStream != null");
                        var bytes = new byte[(int) resourceStream.Length];
                        resourceStream.Read(bytes, 0, bytes.Length);
                        var directoryName = $"{Path.GetDirectoryName(mePath)}";
                        if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
                        File.WriteAllBytes(mePath, bytes);
                        var xpandModelEditorAppConfigPath =
                            $"{Environment.GetEnvironmentVariable("XpandModelEditorAppConfigPath")}";
                        if (File.Exists(xpandModelEditorAppConfigPath)) {
                            File.Copy(xpandModelEditorAppConfigPath, $"{mePath}.config");
                        }
                        else {
                            DteExtensions.DTE.WriteToOutput("XpandModelEditorAppConfigPath enviromental variable is not set");
                        }
                        PatchAssemblyXAFVersion(mePath);
                        return mePath;
                    }
                    catch (Exception e) {
                        DteExtensions.DTE.LogError(e.ToString());
                        DteExtensions.DTE.WriteToOutput(e.ToString());
                        return null;
                    }
                })
                .Where(s => s!=null);

        private static void PatchAssemblyXAFVersion(string path) {
            using var assemblyDefinition =
                AssemblyDefinition.ReadAssembly(path, new ReaderParameters {ReadWrite = true});
            var dxVersion = Version.Parse(DteExtensions.DTE.Solution.GetDXVersion(false));
            DteExtensions.DTE.WriteToOutput($"Patching {Path.GetFileNameWithoutExtension(path)} for version {dxVersion}");
            var moduleDefinition = assemblyDefinition.MainModule;
            var dxReferences = moduleDefinition.AssemblyReferences.Where(reference => reference.Name.StartsWith("DevExpress"));
            foreach (var assemblyNameReference in dxReferences.Where(reference => reference.Version != dxVersion).ToArray()) {
                assemblyNameReference.Name = assemblyNameReference.Name.Replace(
                    $"{assemblyNameReference.Version.Major}.{assemblyNameReference.Version.Minor}",
                    $"{dxVersion.Major}.{dxVersion.Minor}");
                assemblyNameReference.Version = dxVersion;
                var newReference = AssemblyNameReference.Parse(assemblyNameReference.FullName);
                moduleDefinition.AssemblyReferences.Remove(assemblyNameReference);

                moduleDefinition.AssemblyReferences.Add(newReference);
                foreach (var typeReference in moduleDefinition.GetTypeReferences()
                    .Where(_ => _.Scope == assemblyNameReference).ToArray()) typeReference.Scope = newReference;
            }

            assemblyDefinition.Write();
        }

        private static IEnumerable<FileSystemWatcher> GetFileSystemWatchers(){
            var solution = DteExtensions.DTE.Solution;
            return new[]{CreateFileSystemWatcher(solution.FullName) }
                .Concat(solution.Projects().Where(project => File.Exists(project.FullName)).Select(project =>CreateFileSystemWatcher(project.FullName))).ToArray();
        }

        private static FileSystemWatcher CreateFileSystemWatcher(string fileName) 
            => new FileSystemWatcher(Path.GetDirectoryName(fileName) +""){NotifyFilter = NotifyFilters.LastWrite,Filter = Path.GetFileName(fileName),EnableRaisingEvents = true};


        private static IObservable<Unit> SetGridDataSource(bool models=true) 
            => Observable.Start(() => ProjectWrapperBuilder.GetProjectItemWrappers(models).ToList())
                .ObserveOn(_gridControl)
                .Select(list => {
                    _gridControl.DataSource = new BindingList<ProjectItemWrapper>(list
                        .GroupBy(wrapper => wrapper.LocalPath).Select(_ => _.First()).ToArray());
                    return Unit.Default;
                })
                .Catch<Unit, Exception>(e => {
                    DteExtensions.DTE.LogError(e.ToString());
                    DteExtensions.DTE.WriteToOutput(e.ToString());
                    return Observable.Empty<Unit>();
                });

        public static IObservable<Unit> Init(GridControl gridControl,bool models=true) 
            => Setup(gridControl,models);
    }
}