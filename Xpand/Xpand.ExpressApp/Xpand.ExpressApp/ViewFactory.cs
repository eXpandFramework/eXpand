using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

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
                throw new ArgumentException(
	                $"A '{typeof(IModelDetailView).Name}' node was passed while a '{typeof(IModelListView).Name}' node was expected. Node id: '{modelListView.Id}'");
            }
            var result = new XpandListView(collectionSource, xafApplication, isRoot);
            result.SetModel(modelListView);
            return result;
        }

        public static DetailView CreateDetailView(XafApplication xafApplication, string viewId, IObjectSpace objectSpace, object obj, bool isRoot, bool enableDelayedObjectLoading) {

            if (obj != null) {
                CheckDetailViewId(viewId, obj.GetType());
            }

            IModelView modelView = xafApplication.FindModelView(viewId);
            if (!(modelView is IModelDetailView)) {
                throw new ArgumentException(
	                $"A '{null}' node was passed while a '{typeof(IModelDetailView).Name}' node was expected. Node id: '{viewId}'");
            }
	        DetailView result;
	        if (enableDelayedObjectLoading) {
				result = new XpandDetailView(objectSpace, null, xafApplication, isRoot);
		        // ReSharper disable once ObjectCreationAsStatement
				new DetailViewCurrentObjectInitializer(result, obj);
			}
			else {
				result = new XpandDetailView(objectSpace, objectSpace.GetObject(obj), xafApplication, isRoot);
			}
	        result.DelayedItemsInitialization = xafApplication.DelayedViewItemsInitialization;
	        result.SetModel(modelView);
            return result;
        }
    }
}