using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.DXCore.Controls.XtraGrid;
using EnvDTE;
using EnvDTE80;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.ModelEditor {
    public class GridHelper {
        static GridControl _gridControl;
        private static Events2 _events;
        private static ProjectItemsEvents _eventsProjectItemsEvents;
        private static SolutionEvents _eventsSolutionEvents;

        static void Setup(GridControl gridControl) {
            _gridControl = gridControl;
            SetGridDataSource();
            _events = (Events2) DteExtensions.DTE.Events;
            _eventsProjectItemsEvents = _events.ProjectItemsEvents;
            _eventsProjectItemsEvents.ItemRemoved += EventsOnProjectItemRemoved;
            _eventsProjectItemsEvents.ItemAdded += EventsOnProjectItemAdded;
            _eventsProjectItemsEvents.ItemRenamed += EventsOnProjectItemRenamed;
            _eventsSolutionEvents = _events.SolutionEvents;
            _eventsSolutionEvents.Opened += EventsSolutionEventsOnOpened;
            _eventsSolutionEvents.AfterClosing+=EventsSolutionEventsOnAfterClosing;
        }

        private static void EventsSolutionEventsOnAfterClosing(){
            _eventsSolutionEvents.ProjectAdded -= EventsSolutionEventsOnProjectAdded;
            _eventsSolutionEvents.ProjectRemoved -= EventsSolutionEventsOnProjectRemoved;
        }

        private static void EventsSolutionEventsOnOpened(){
            SetGridDataSource();
            _eventsSolutionEvents.ProjectAdded += EventsSolutionEventsOnProjectAdded;
            _eventsSolutionEvents.ProjectRemoved += EventsSolutionEventsOnProjectRemoved;
        }

        private static void EventsSolutionEventsOnProjectRemoved(Project project){
            RemoveProjectWrappers(ProjectWrapperBuilder.GetProjectItemWrappers(new List<Project>{project}));
        }

        private static void EventsSolutionEventsOnProjectAdded(Project project){
            AddProjectWrappers(ProjectWrapperBuilder.GetProjectItemWrappers(new List<Project>{project}));
        }

        private static void RemoveProjectWrappers(IEnumerable<ProjectItemWrapper> projectWrappers) {
            var list = (BindingList<ProjectItemWrapper>)_gridControl.DataSource;
            foreach (var projectWrapper in projectWrappers) {
                var singleWrapper = list.First(wrapper => wrapper.UniqueName==projectWrapper.UniqueName);
                list.Remove(singleWrapper);
            }
            _gridControl.RefreshDataSource();

        }

        static void EventsOnProjectItemRemoved(ProjectItem projectItem) {
            if (projectItem.Name.EndsWith(".xafml"))
                SetGridDataSource();
        }

        static void EventsOnProjectItemRenamed(ProjectItem projectItem, string oldName) {
            if (projectItem.Name.EndsWith(".xafml"))
                SetGridDataSource();
        }

        static void EventsOnProjectItemAdded(ProjectItem projectItem) {
            if (projectItem.Name.EndsWith(".xafml"))
                SetGridDataSource();
        }

        private static void AddProjectWrappers(IEnumerable<ProjectItemWrapper> projectWrappers) {
            foreach (var projectWrapper in projectWrappers) {
                ((BindingList<ProjectItemWrapper>)_gridControl.DataSource).Add(projectWrapper);
            }
        }
        private static void SetGridDataSource(){
            List<ProjectItemWrapper> projectWrappers = new List<ProjectItemWrapper>();
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() => projectWrappers = ProjectWrapperBuilder.GetProjectItemWrappers().ToList(),CancellationToken.None,TaskCreationOptions.None, TaskScheduler.Default)
                .ContinueWith(task1 =>_gridControl.DataSource = new BindingList<ProjectItemWrapper>(projectWrappers), context);
        }

        public static void Init(GridControl gridControl) {
            Setup(gridControl);
        }
    }
}