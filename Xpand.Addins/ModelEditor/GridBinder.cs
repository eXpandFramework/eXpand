using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.DXCore.Controls.XtraGrid;
using DevExpress.DXCore.PlugInCore;
using EnvDTE;

namespace XpandAddIns.ModelEditor {
    public class GridBinder {
        readonly GridControl _gridControl;

        GridBinder(GridControl gridControl, DXCoreEvents events) {
            _gridControl = gridControl;
            SetGridDataSource();
            events.ProjectItemRemoved += EventsOnProjectItemRemoved;
            events.ProjectItemAdded += EventsOnProjectItemAdded;
            events.ProjectItemRenamed += EventsOnProjectItemRenamed;
            events.SolutionOpened += SetGridDataSource;
            events.ProjectAdded += project => AddProjectWrappers(ProjectWrapperBuilder.GetProjectWrappers(new List<Project> { project }));
            events.ProjectRemoved += project1 => RemoveProjectWrappers(ProjectWrapperBuilder.GetProjectWrappers(new List<Project> { project1 }));
        }
        private void RemoveProjectWrappers(IEnumerable<ProjectWrapper> projectWrappers) {
            var list = (BindingList<ProjectWrapper>)_gridControl.DataSource;
            foreach (var projectWrapper in projectWrappers) {
                var singleWrapper = list.Single(wrapper => wrapper.UniqueName==projectWrapper.UniqueName);
                list.Remove(singleWrapper);
            }
            _gridControl.RefreshDataSource();

        }

        void EventsOnProjectItemRemoved(ProjectItem projectItem) {
            if (projectItem.Name.EndsWith(".xafml"))
                SetGridDataSource();
        }

        void EventsOnProjectItemRenamed(ProjectItem projectItem, string oldName) {
            if (projectItem.Name.EndsWith(".xafml"))
                SetGridDataSource();
        }

        void EventsOnProjectItemAdded(ProjectItem projectItem) {
            if (projectItem.Name.EndsWith(".xafml"))
                SetGridDataSource();
        }

        private void AddProjectWrappers(IEnumerable<ProjectWrapper> projectWrappers) {
            foreach (var projectWrapper in projectWrappers) {
                ((BindingList<ProjectWrapper>)_gridControl.DataSource).Add(projectWrapper);
            }
        }
        private void SetGridDataSource(){
            List<ProjectWrapper> projectWrappers = null;
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() => projectWrappers = ProjectWrapperBuilder.GetProjectWrappers().ToList())
                .ContinueWith(task1 => { _gridControl.DataSource = new BindingList<ProjectWrapper>(projectWrappers); }, context);
        }

        public static void Init(GridControl gridControl, DXCoreEvents events) {
            new GridBinder(gridControl, events);
        }
    }
}