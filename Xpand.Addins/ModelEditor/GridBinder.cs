﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.DXCore.Controls.XtraGrid;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;
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
        private void SetGridDataSource() {
            _gridControl.DataSource = new BindingList<ProjectWrapper>(ProjectWrapperBuilder.GetProjectWrappers().ToList());
            var gridView = ((GridView)_gridControl.MainView);
            gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        public static void Init(GridControl gridControl, DXCoreEvents events) {
            new GridBinder(gridControl, events);
        }
    }
}