using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Process = System.Diagnostics.Process;

namespace Xpand.VSIX.ToolWindow.ModelEditor {
    public class ModelEditorRunner {
        private readonly DTE2 _dte = DteExtensions.DTE;

        public IObservable<Unit> Start(ProjectItemWrapper projectItemWrapper) {
            string outputFileName = projectItemWrapper.OutputFileName;
            var fullPath = projectItemWrapper.FullPath;
            string assemblyPath = Path.Combine(fullPath, Path.Combine(projectItemWrapper.OutputPath, outputFileName));
            if (!File.Exists(assemblyPath)) {
                MessageBox.Show($@"Assembly {assemblyPath} not found, recompilling....", null, MessageBoxButtons.OK);
                return Observable.Start(() => projectItemWrapper.Project.Build()).Where(b => b)
                    .SelectMany(_ => Start(projectItemWrapper));
            }
            return MePath(projectItemWrapper)
                .SelectMany(path => StartMEProcess(projectItemWrapper, assemblyPath, path));
        }

        private IObservable<string> MePath(ProjectItemWrapper projectItemWrapper) {
            if (projectItemWrapper.TargetFramework != null && (projectItemWrapper.TargetFramework.StartsWith("netcore") ||
                                                               projectItemWrapper.TargetFramework == "netstandard2.1")) {
                var assembly = projectItemWrapper.GetType().Assembly;
                var ns = $"{typeof(ModelEditorRunner).Namespace}.WinDesktop.";
                var resources = Resources(projectItemWrapper, assembly, ns);
                return  BufferUntilCompleted(WriteFiles(resources)).ObserveOn(System.Reactive.Concurrency.Scheduler.Default)
                    .SelectMany(strings => ConfigureEnvironment(projectItemWrapper, strings.ToObservable()).Concat(strings.ToObservable()))
                    .FirstAsync(s => {
                        var fileName = Path.GetFileName(s);
                        return !fileName.EndsWith("nuget.exe")&&fileName.EndsWith(".exe");
                    });
            }
            return GridHelper.ExtractME();
        }

        static IObservable<TSource[]> BufferUntilCompleted<TSource>(IObservable<TSource> source){
            var allEvents = source.Publish().RefCount();
            return allEvents.Buffer(allEvents.LastAsync()).Select(list => list.ToArray());
        }


        private static IObservable<(string path, Stream stream)> Resources(ProjectItemWrapper projectItemWrapper, Assembly assembly, string ns) 
            => assembly.GetManifestResourceNames().Where(s => s.StartsWith(ns))
                .Select(s => {
                    var path = $"{Path.GetFullPath($"{projectItemWrapper.FullPath}{projectItemWrapper.OutputPath}")}\\{s.Replace(ns, "")}";
                    if (File.Exists(path)) {
                        File.Delete(path);
                    }

                    return (path,stream:assembly.GetManifestResourceStream(s));
                }).ToObservable().Publish().RefCount();

        private static IObservable<string> WriteFiles(IObservable<(string path, Stream stream)> resources) 
            => resources
                .Select(t => {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(t.path);
                    if (fileNameWithoutExtension == "Xpand.XAF.ModelEditor.WinDesktop.runtimeconfig.dev") {
                        using var streamReader = new StreamReader(t.stream!);
                        var readToEnd = streamReader.ReadToEnd();
                        readToEnd = readToEnd.Replace("Tolis", Environment.UserName);
                        File.WriteAllText(t.path, readToEnd);
                    }
                    else {
                        File.WriteAllBytes(t.path, Bytes(t.stream));
                    }
                    return t.path;
                });

        private static IObservable<string> ConfigureEnvironment(ProjectItemWrapper projectItemWrapper, IObservable<string> resources) 
            => resources.FirstAsync(s => Path.GetFileName(s).EndsWith("nuget.exe"))
                .SelectMany(s => Observable.StartAsync(async () => {
                    var dte2 = DteExtensions.DTE;
                    await RestoreDependencies(s);
                    var project = projectItemWrapper.Project;
                    var projectProperty = project.GetProperty("CopyLocalLockFileAssemblies");
                    if (projectProperty == null || projectProperty.EvaluatedValue != "true") {
                        dte2.WriteToOutput($"Modifying {Path.GetFileName(projectItemWrapper.UniqueName)}");
                        project.SetProperty("CopyLocalLockFileAssemblies", "true");
                        project.Save(projectItemWrapper.UniqueName);
                        dte2.WriteToOutput($"Building {Path.GetFileName(projectItemWrapper.UniqueName)}");
                        project.Build("restore");
                    }
                    return s;
                }))
                .IgnoreElements();

        private static string Dependencies(string s) {
            var dependencies = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText($"{Path.GetDirectoryName(s)}\\Xpand.XAF.ModelEditor.WinDesktop.deps.json"));
            var refs = string.Join(Environment.NewLine,
                ((IEnumerable<JProperty>) ((dynamic) ((IEnumerable<JProperty>) dependencies.targets.Properties()).First().Value).Properties())
                .Where(jProperty => !jProperty.Name.StartsWith("Xpand.XAF.ModelEditor.WinDesktop"))
                .Select(property => {
                    var strings = property.Name.Split('/');
                    var path = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.nuget\\packages\\{strings[0]}\\{strings[1]}";
                    return !Directory.Exists(path) ? @$"<PackageReference Include=""{strings[0]}"" Version=""{strings[1]}""/>" : null;
                })
                .Where(s1 => s1!=null));
            return !string.IsNullOrEmpty(refs) ? $"<ItemGroup>{refs}</ItemGroup>" : null;
        }

        private static async Task RestoreDependencies(string s) {
            var dependencies = Dependencies(s);
            if (!string.IsNullOrEmpty(dependencies)) {
                var csProjPath = $"{Path.GetTempPath()}\\Xpand.XAF.ModelEditor.WinDesktop";
                if (!Directory.Exists(csProjPath)) {
                    Directory.CreateDirectory(csProjPath);
                }
                DteExtensions.DTE.WriteToOutput($"Creating dependecies project {csProjPath}");
                await Execute("dotnet", "new classlib -f \"net5.0\" --force",csProjPath);
                csProjPath=$"{Path.GetTempPath()}\\Xpand.XAF.ModelEditor.WinDesktop\\Xpand.XAF.ModelEditor.WinDesktop.csproj";
            
                File.WriteAllText(csProjPath,dependencies);

                DteExtensions.DTE.WriteToOutput("Restoring packages");
                await Execute(s, $"restore {csProjPath} -noCache -Force -Recursive",Path.GetDirectoryName(csProjPath));
            }
        }

        private static IObservable<Unit> Execute(string path, string arguments, string workingDirectory) {
            var process = new Process {
                StartInfo = {
                    FileName = path,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                }
            };
            process.Start();
            DteExtensions.DTE.WriteToOutput(process.StandardOutput.ReadToEnd());
            return process.WaitForExitAsync().ToObservable().Select(_ => Unit.Default);
        }

        static byte[] Bytes(Stream stream){
            if (stream is MemoryStream memoryStream){
                return memoryStream.ToArray();
            }

            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        IObservable<Unit> StartMEProcess(ProjectItemWrapper projectItemWrapper, string assemblyPath, string mePath) 
            => Observable.Return(Unit.Default).Do(_ => {
                try {
                    var fullPath = projectItemWrapper.FullPath;
                    var destFileName = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assemblyPath) + "",
                        Path.GetFileName(mePath) + ""));
                    KillProcess(destFileName);
                    if (!string.Equals(mePath, destFileName, StringComparison.OrdinalIgnoreCase)) {
                        File.Copy(mePath, destFileName, true);
                        var configPath = Path.Combine(Path.GetDirectoryName(mePath) + "",
                            Path.GetFileName(mePath) + ".config");
                        if (File.Exists(configPath)) {
                            _dte.WriteToOutput("Copying App.config");
                            File.Copy(configPath,
                                Path.Combine(Path.GetDirectoryName(destFileName) + "", Path.GetFileName(configPath)),
                                true);
                        }
                    }

                    StartME(projectItemWrapper, assemblyPath, fullPath, destFileName);
                }
                catch (Exception e) {
                    MessageBox.Show(e.ToString());
                }

            });

        private void StartME(ProjectItemWrapper projectItemWrapper, string assemblyPath, string fullPath, string destFileName) {
            string debugMe = OptionClass.Instance.DebugME ? "d" : null;
            string arguments = String.Format("{0} {4} \"{1}\" \"{3}\" \"{2}\"", debugMe, Path.GetFullPath(assemblyPath),
                fullPath, projectItemWrapper.LocalPath, projectItemWrapper.IsApplicationProject);
            if (File.Exists(destFileName))
                try {
                    _dte.WriteToOutput($"Starting {destFileName} with arguments {arguments}");
                    Process.Start(destFileName, arguments);
                }
                catch (IOException) {
                    MessageBox.Show(
                        @"You have probably open the same model from another ME instance. If not please report this with reproduction details in eXpandFramework bugs forum");
                }
            else
                MessageBox.Show($@"Model editor not found at {destFileName}");
        }

        public void KillProcess(string path){
            const string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using var searcher = new ManagementObjectSearcher(wmiQueryString);
            using var results = searcher.Get();
            var query = Process.GetProcesses()
                .Join(results.Cast<ManagementObject>(), p => p.Id, mo => (int) (uint) mo["ProcessId"],
                    (p, mo) => new{
                        Process = p,
                        Path = (string) mo["ExecutablePath"],
                        CommandLine = (string) mo["CommandLine"],
                    });
            foreach (var item in query.Where(arg => arg.Path==path)) {
                item.Process.Kill();
            }
        }

    }
}