using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.DXCore.Controls.XtraGrid;
using DevExpress.DXCore.PlugInCore;
using EnvDTE;
using Xpand.CodeRush.Plugins.Extensions;

namespace Xpand.CodeRush.Plugins.ModelEditor {
    public class GridBinder {
        readonly GridControl _gridControl;

        GridBinder(GridControl gridControl, DXCoreEvents events) {
            _gridControl = gridControl;
            SetGridDataSource();
            events.ProjectItemRemoved += EventsOnProjectItemRemoved;
            events.ProjectItemAdded += EventsOnProjectItemAdded;
            events.ProjectItemRenamed += EventsOnProjectItemRenamed;
            events.SolutionOpened += SetGridDataSource;
            events.ProjectAdded += project => AddProjectWrappers(ProjectWrapperBuilder.GetProjectItemWrappers(new List<Project> { project }));
            events.ProjectRemoved += project1 => RemoveProjectWrappers(ProjectWrapperBuilder.GetProjectItemWrappers(new List<Project> { project1 }));
        }
        private void RemoveProjectWrappers(IEnumerable<ProjectItemWrapper> projectWrappers) {
            var list = (BindingList<ProjectItemWrapper>)_gridControl.DataSource;
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

        private void AddProjectWrappers(IEnumerable<ProjectItemWrapper> projectWrappers) {
            foreach (var projectWrapper in projectWrappers) {
                ((BindingList<ProjectItemWrapper>)_gridControl.DataSource).Add(projectWrapper);
            }
        }
        private void SetGridDataSource(){
            List<ProjectItemWrapper> projectWrappers = null;
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNewNow(() => projectWrappers = ProjectWrapperBuilder.GetProjectItemWrappers().ToList())
                .ContinueWith(task1 => { _gridControl.DataSource = new BindingList<ProjectItemWrapper>(projectWrappers); }, context);
        }

        public static void Init(GridControl gridControl, DXCoreEvents events) {
            new GridBinder(gridControl, events);
        }
    }
}