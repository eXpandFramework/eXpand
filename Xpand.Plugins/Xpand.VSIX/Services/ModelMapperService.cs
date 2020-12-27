using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Principal;
using EnvDTE;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Services{
    public class ModelMapperService {
        public static void Init() {
            DteExtensions.DTE.Events.BuildEvents.OnBuildDone+=BuildEventsOnOnBuildDone;
            DteExtensions.DTE.Events.BuildEvents.OnBuildBegin+=BuildEventsOnOnBuildBegin;
        }

        private static void    BuildEventsOnOnBuildBegin(vsBuildScope scope, vsBuildAction action) {
            var securityIdentifier = WindowsIdentity.GetCurrent().Owner;
            if (securityIdentifier == null || !securityIdentifier.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid)) {
                DteExtensions.DTE.ExecuteCommand("Build.Cancel");
                DteExtensions.DTE.WriteToOutput("VS is not elevated, restart as Administator");
            }
        }

        private static IObservable<Unit> CopyFile(string s, string targetDirectory) 
            => Observable.Start(() => {
                    File.Copy(s, $@"{targetDirectory}\{Path.GetFileName(s)}",true);
                    return Unit.Default;
                })
                .OnErrorResumeNext(Observable.Return(Unit.Default));

        private static void BuildEventsOnOnBuildDone(vsBuildScope scope, vsBuildAction action) {
            var files = DteExtensions.DTE.Solution.Projects()
                .Select(_ => _.FindOutputPath())
                .SelectMany(s => Directory.GetFiles($"{Path.GetDirectoryName(s)}","*.dll"))
                .Distinct().ToArray();
            if (files.Any(s => $"{Path.GetFileName(s)}".Contains("ModelMapper"))) {
                var targetDirectory =$@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\CommonExtensions\DataDesign";
                var modelMapperFiles = $@"{targetDirectory}\ModelMapperFiles.txt";
                var deleteFiles=Observable.Empty<Unit>();
                if (File.Exists(modelMapperFiles)) {
                    deleteFiles = File.ReadAllText(modelMapperFiles).Split(Environment.NewLine.ToCharArray()).ToObservable()
                        .SelectMany(s => Observable.Start(() => File.Delete($@"{targetDirectory}\{s}")).OnErrorResumeNext(Observable.Return(Unit.Default)))
                        .Finally(() => File.Delete(modelMapperFiles));
                }
                DteExtensions.DTE.WriteToOutput($"Copying files to {targetDirectory}",false);
                deleteFiles.Concat(files.ToObservable()
                        .SelectMany(s => CopyFile(s, targetDirectory))
                        .Select(s => Unit.Default))
                    .Finally(() => {
                        File.WriteAllText(modelMapperFiles,string.Join(Environment.NewLine,files.Select(Path.GetFileName)));
                        DteExtensions.DTE.WriteToOutput("Finished",false);
                    })
                    .Subscribe();

            }
            
        }

    }
}