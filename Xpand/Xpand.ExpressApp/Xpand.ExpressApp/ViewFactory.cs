using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp {
    public class ViewFactory {

        static void CheckDetailViewId(String detailViewId, Type objectType) {
            if (String.IsNullOrEmpty(detailViewId)) {
                throw new Exception(
                    SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToFindADetailViewForType,
                                                                 objectType.FullName));
            }
        }

        public static XpandListView CreateListView(XafApplication xafApplication, string viewId, CollectionSourceBase collectionSource,
                                              bool isRoot) {
            IModelView modelView = xafApplication.FindModelView(viewId);
            if (modelView == null) {
                throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.NodeWasNotFound, viewId));
            }
            var modelListView = ((IModelListView)modelView);
            if (modelListView == null) {
                throw new ArgumentException(string.Format(
                    "A '{0}' node was passed while a '{1}' node was expected. Node id: '{2}'",
                    typeof(IModelDetailView).Name, typeof(IModelListView).Name, modelListView.Id));
            }
            var result = new XpandListView(collectionSource, xafApplication, isRoot);
            result.SetModel(modelListView);
            return result;
        }

        public static DetailView CreateDetailView(XafApplication xafApplication, string viewId, IObjectSpace objectSpace, object obj, bool isRoot) {

            if (obj == null)
                throw new ArgumentNullException("obj");

            CheckDetailViewId(viewId, obj.GetType());

            IModelView modelView = xafApplication.FindModelView(viewId);
            if (!(modelView is IModelDetailView)) {
                throw new ArgumentException(string.Format(
                    "A '{0}' node was passed while a '{1}' node was expected. Node id: '{2}'",
                    null, typeof(IModelDetailView).Name, viewId));
            }

            
            
            
            var detailView = new XpandDetailView(objectSpace, obj, xafApplication, isRoot) { DelayedItemsInitialization = xafApplication.DelayedViewItemsInitialization };
            detailView.SetModel(modelView);
            return detailView;
        }
    }
}