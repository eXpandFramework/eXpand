using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using EnvDTE80;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.ToolWindow.SolutionConfiguration {
    public partial class SolutionConfigurationToolWindowControl : UserControl {
        private static readonly DTE2 DTE = DteExtensions.DTE;

        public class SolutionConfiguration {
            public SolutionConfiguration(SolutionConfiguration2 c) {
                Name = c.Name;
                PlatformName = c.PlatformName;
                Configuration = c;
            }

            public SolutionConfiguration2 Configuration { get; }

            public string Name { get; }

            public string PlatformName { get; }

            public bool Active => DTE.Solution.SolutionBuild.ActiveConfiguration.Name == Name &&
                                    ((SolutionConfiguration2) DTE.Solution.SolutionBuild.ActiveConfiguration).PlatformName == PlatformName;
        }
        public SolutionConfigurationToolWindowControl() {
            InitializeComponent();
            gridControl1.LookAndFeel.Assign(defaultLookAndFeel1.LookAndFeel);
            DTE.WhenSolutionOpen().Do(_ => SetDataSource()).Subscribe();
            SetDataSource();
            gridView1.KeyDown+=GridView1OnKeyDown;
            gridView1.KeyUp+=gridView1_KeyUp;
            gridView1.DoubleClick+=gridView1_DoubleClick;
        }

        private void SetDataSource() {
            gridControl1.DataSource = new BindingList<SolutionConfiguration>(DTE.Solution.SolutionBuild
                .SolutionConfigurations.Cast<SolutionConfiguration2>()
                .Select(c => new SolutionConfiguration(c))
                .ToList());
            
        }


        private void GridView1OnKeyDown(object sender, KeyEventArgs e){
            var gridView = ((GridView)gridControl1.MainView);
            if (gridView.FocusedRowHandle == 0 && e.KeyCode == Keys.Up)
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        private void LoadConfiguration(SolutionConfiguration configuration) {
            
            try{
                configuration.Configuration.Activate();
                SetDataSource();
                DTE.InitOutputCalls($"Configuration {configuration.Name} loaded");
            }
            catch (Exception e){
                DTE.WriteToOutput(e.ToString());
            }
        }

        private void gridView1_KeyUp(object sender, KeyEventArgs e) {
            
            var gridView = ((GridView)gridControl1.MainView);
            if (e.KeyCode == Keys.Return) {
                if (GridControl.AutoFilterRowHandle != gridView.FocusedRowHandle&& GridControl.InvalidRowHandle != gridView.FocusedRowHandle) {
                    var configuration = (SolutionConfiguration)gridView.GetRow(gridView.FocusedRowHandle);
                    LoadConfiguration(configuration);
                } else if (gridView.RowCount > 0)
                    LoadConfiguration((SolutionConfiguration)gridView.GetRow(0));
            }
            else if (e.KeyCode == Keys.F5) {
                SetDataSource();
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e) {
            var gridView = ((GridView)sender);
            LoadConfiguration((SolutionConfiguration)gridView.GetRow(gridView.FocusedRowHandle));
        }

    }
}
