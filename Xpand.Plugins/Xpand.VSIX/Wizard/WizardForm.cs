using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using Microsoft.Build.Construction;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Wizard{
    public partial class WizardForm : Form,IDTE2Provider{
        public Language Language { get; set; }

        public WizardForm(){
            InitializeComponent();
            buttonFinish.Click += (sender, args) => Finish();
            gridControl1.KeyUp+=OnKeyUp;
            gridControl1.KeyDown += (sender, e) =>{
                if (e.KeyCode == Keys.Up){
                    var selectedRows = gridView.GetSelectedRows();
                    if (selectedRows.Intersect(new[] {-1,0}).Any())
                        gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
                }
            };
            gridControl1.KeyPress += (sender, args) => args.Handled =
                        args.KeyChar == Convert.ToChar(Keys.Space) || args.KeyChar == Convert.ToChar(Keys.Enter) ||
                        args.KeyChar == Convert.ToChar(Keys.Return);
        }

        private void OnKeyUp(object sender, KeyEventArgs e){
            if (e.KeyCode == Keys.Return)
                Finish();
            else if (e.KeyCode == Keys.Space){
                var addition = Modules.Any(module => module.Install)&&!Modules.All(module => module.Install);
                foreach (var selectedRow in gridView.GetSelectedRows()){
                    if (!gridView.IsGroupRow(selectedRow)){
                        if (gridView.GetRow(selectedRow) is XpandModule xpandModule) {
                            xpandModule.Install =addition || !xpandModule.Install;
                        }
                    }
                }
                gridView.RefreshData();
            }
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control){
                for (int i = 0; i < gridView.RowCount; i++){
                    gridView.SelectRow(i);
                }
            }
        }

        private void Finish(){
            Visible = false;
            Application.DoEvents();
            DialogResult = DialogResult.OK;
            var modules = Modules.Where(module => module.Install).ToArray();
            ModulesInstaller.Install(modules,ExistingSolution);
            this.DTE2().ExecuteCommand("File.SaveAll");
            var dotNetVersion = modules.Max(module => module.DotNetVersion);
            bool needsReload = false;
            if (dotNetVersion > GetCurrentDotNetVersion()){
                UpdateDotNetVersion(dotNetVersion);
                needsReload = true;
            }
            if (modules.Any(module => module.IsWorldCreator)) {
                InstallMonoCecil();
                needsReload = true;
            }
            if (needsReload) {
                ReloadSolution();
            }
        }

        void ReloadSolution() {
            var solutionFile = this.DTE2().Solution.FileName;
            Observable.Return(solutionFile).Delay(TimeSpan.FromMilliseconds(1000)).Subscribe(
                s => this.DTE2().Solution.Open(s));
            this.DTE2().Solution.Close();
        }
        private void InstallMonoCecil() {
            var msBuildProjects = DteExtensions.DTE.Solution.GetMsBuildProjects().ToArray();
            foreach (var dteProject in DteExtensions.DTE.Solution.Projects().Where(project => project.IsApplicationProject())) {
                var project = msBuildProjects.First(_ => _.FullPath==dteProject.FileName);
                var reference = project.Xml.ItemGroups
                    .SelectMany(element => element.Items)
                    .First(_ => _.ElementName=="Reference" &&_.Include.StartsWith("Xpand"));
                
                var projectItemGroupElement = ((ProjectItemGroupElement) reference.Parent);
                foreach (var s in new[] {"", ".Mdb", ".Pdb", ".Rocks"}) {
                    projectItemGroupElement.AddItem("Reference",$"Mono.Cecil{s}, Version=0.10.1.0, Culture=neutral, PublicKeyToken=50cebf1cceb9d05e, processorArchitecture=MSIL")
                        .AddMetadata("HintPath", $@"..\packages\Mono.Cecil.0.10.1\lib\net40\Mono.Cecil{s}.dll");
                }
                var itemGroup = (ProjectItemGroupElement)project.Xml.ItemGroups
                    .SelectMany(element => element.Items)
                    .First(_ => _.ElementName=="Compile").Parent;
                itemGroup.AddItem("None", "packages.config");
                project.Save();
                var text =@"<?xml version=""1.0"" encoding=""utf-8""?><packages><package id=""Mono.Cecil"" version=""0.10.1"" targetFramework=""net461"" /></packages>";
                var projectDirectory = $"{Path.GetDirectoryName(dteProject.FileName)}";
                File.WriteAllText(Path.Combine(projectDirectory,"packages.config"),text);
            }
        }

        private void UpdateDotNetVersion(Version version){
            foreach (var project in DteExtensions.DTE.Solution.Projects()){
                var name = project.FileName;
                var allText = File.ReadAllText(name);
                allText=Regex.Replace(allText, "<TargetFrameworkVersion>v(.*)</TargetFrameworkVersion>",
                    $"<TargetFrameworkVersion>v{version}</TargetFrameworkVersion>",
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                File.WriteAllText(name,allText);    
            }
        }

        private Version GetCurrentDotNetVersion(){
            var name = DteExtensions.DTE.Solution.Projects().First().FileName;
            var allText = File.ReadAllText(name);
            var regexObj = new Regex("<TargetFrameworkVersion>v(.*)</TargetFrameworkVersion>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            return new Version(regexObj.Match(allText).Groups[1].Value); 
        }

        public IList<XpandModule> Modules { get; set; }
        public bool ExistingSolution { get; set; }
        public bool FavorAgnostic { get; set; }

        protected override void OnLoad(EventArgs e){
            base.OnLoad(e);
            if (!Modules.Any())
                DialogResult = DialogResult.None;
            else {
                var dataSource = new BindingList<XpandModule>(Modules);
                dataSource.ListChanged+=DataSourceOnListChanged;
                gridControl1.DataSource = dataSource;
                gridControl1.Focus();
                gridView.Focus();
                gridView.FocusedColumn=gridView.Columns.Cast<GridColumn>().First(column => column.FieldName==nameof(XpandModule.Module));
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            }
        }

        private void DataSourceOnListChanged(object sender, ListChangedEventArgs e){
            if (e.ListChangedType == ListChangedType.ItemChanged){
                var module = ((BindingList<XpandModule>) sender)[e.NewIndex];
                if (module.Install && FavorAgnostic){
                    var projects = this.DTE2().Solution.Projects();
                    var agnosticProject = projects.First(project => project.GetPlatform()==Platform.Agnostic);
                    var platformProject = projects.First(project => project.GetPlatform()==module.Platform);
                    var text = $"The agnostic version of {module.Module} will be installed in {agnosticProject.Name} and the {module.Platform} version in {platformProject.Name}";
                    Message(text);
                }
            }
        }

        private void Message(string text,bool error=false){
            Task.Factory.StartNew(() => Thread.Sleep(10000))
                .ContinueWith(task => labelMessage.Text = null, TaskScheduler.FromCurrentSynchronizationContext());
            labelMessage.Text = text;
            if (error)
                labelMessage.ForeColor=Color.Red;
        }
    }

}
