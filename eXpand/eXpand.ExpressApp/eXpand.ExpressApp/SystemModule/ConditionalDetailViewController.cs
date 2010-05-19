using System;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.Enums;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.SystemModule
{
    public enum ConditionalDetailViewNewMode
    {
        Strict,
        IncludeSubclasses
    }

    public interface IModelListViewConditionalDetailViews
    {
        IModelConditionalDetailViews ConditionalDetailViews { get; set; }
    }

    public interface IModelListViewEnableOpenActionInMasterDetailMode
    {
        bool EnableOpenActionInMasterDetailMode { get; set; }
    }

    public interface IModelConditionalDetailViews : IModelNode, IModelList<IModelConditionalDetailView>, IModelListViewEnableOpenActionInMasterDetailMode
    {
    }

    public interface IModelConditionalDetailView : IModelNode
    {
        [ModelPersistentName("ID"), Required]
        string Id { get; set; }
        [DataSourceProperty("Application.BOModel")]
        IModelClass ClassName { get; set; }
        string Criteria { get; set; }
        DetailViewType Mode { get; set; }
        ConditionalDetailViewNewMode NewModeBehavior { get; set; }
        [ModelPersistentName("DetailViewID"), DataSourceProperty("Application.Views")]
        IModelDetailView DetailView { get; set; }
    }

    public class ConditionalDetailViewController : ViewController<ListView>, IModelExtender
    {
        public const string ConditionalDetailViewsAttributeName = "ConditionalDetailViews";
        bool isActive;
        NewObjectViewController _newObjectViewController;
        ListViewProcessCurrentObjectController _listViewProcessCurrentObjectController;
        IModelListViewConditionalDetailViews model;

        public event ChooseCustomDetailViewEventHandler CustomChooseDetailView;

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewConditionalDetailViews>();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            model = View.Model as IModelListViewConditionalDetailViews;
            if (model != null && model.ConditionalDetailViews != null && model.ConditionalDetailViews.Count > 0)
            {
                isActive = true;
                _newObjectViewController = Frame.GetController<NewObjectViewController>();
                _newObjectViewController.NewObjectAction.Executed += NewObjectAction_Executed;
                _listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                _listViewProcessCurrentObjectController.CustomProcessSelectedItem += CustomProcessSelectedItem;
                View.ProcessSelectedItem += ConditionalDetailViewController_ProcessSelectedItem;
                View.CreateCustomCurrentObjectDetailView += ConditionalDetailViewController_CreateCustomCurrentObjectDetailView;
            }
        }

        protected override void OnDeactivating()
        {
            if (isActive)
            {
                _newObjectViewController.NewObjectAction.Executed -= NewObjectAction_Executed;
                _listViewProcessCurrentObjectController.CustomProcessSelectedItem -= CustomProcessSelectedItem;
                View.ProcessSelectedItem -= ConditionalDetailViewController_ProcessSelectedItem;
                View.CreateCustomCurrentObjectDetailView -= ConditionalDetailViewController_CreateCustomCurrentObjectDetailView;
            }

            base.OnDeactivating();
        }

        void ConditionalDetailViewController_ProcessSelectedItem(object sender, EventArgs e)
        {
            //Show popup detailview even in ListViewAndDetailView-mode?

            if (!View.IsRoot && View.Model.MasterDetailMode == MasterDetailMode.ListViewAndDetailView
                && !model.ConditionalDetailViews.EnableOpenActionInMasterDetailMode)
            {
                if (_listViewProcessCurrentObjectController.ProcessCurrentObjectAction.Enabled && _listViewProcessCurrentObjectController.ProcessCurrentObjectAction.Active)
                {
                    _listViewProcessCurrentObjectController.ProcessCurrentObjectAction.DoExecute();
                }
            }
        }

        void CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            //this is called when XAF wants to open a new root DetailView for the selected object in the ListView

            var detailViewID = FindAppplicableDetailViewID(ObjectSpace, DetailViewType.Root, e.InnerArgs.CurrentObject, null, "");
            if (detailViewID == null)
            {
                e.Handled = false;
            }
            else
            {
                ObjectSpace os = Application.GetObjectSpaceToShowViewFrom(Frame);
                object objInTargetOS;
                if (os != View.ObjectSpace)
                {
                    if (!(os is NestedObjectSpace) && View.ObjectSpace.IsNewObject(e.InnerArgs.CurrentObject))
                    {
                        throw new InvalidOperationException(ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(ExceptionId.AnUnsavedObjectCannotBeShown));
                    }
                    objInTargetOS = os.GetObject(e.InnerArgs.CurrentObject);
                }
                else
                {
                    objInTargetOS = e.InnerArgs.CurrentObject;
                }

                e.InnerArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(os, detailViewID.Id, true,
                                                                                          objInTargetOS);

                var propertyCollectionSource = View.CollectionSource as PropertyCollectionSource;
                if (propertyCollectionSource != null)
                {
                    if (propertyCollectionSource.MemberInfo.IsAggregated && !View.AllowEdit)
                    {
                        e.InnerArgs.ShowViewParameters.CreatedView.AllowEdit.SetItemValue(
                            "From ListView with aggregated read-only property collection", false);
                    }
                }

                e.Handled = true;
            }
        }


        void ConditionalDetailViewController_CreateCustomCurrentObjectDetailView(object sender,
                                                                                 CreateCustomCurrentObjectDetailViewEventArgs
                                                                                     e)
        {
            //is called by XAF to switch nested DetailViews(in MasterDetailMode == ListViewAndDetailView)
            e.DetailViewId = FindAppplicableDetailViewID(ObjectSpace, DetailViewType.Nested, e.ListViewCurrentObject,
                                                         null, View.DetailViewId).Id;
        }

        void NewObjectAction_Executed(object sender, ActionBaseEventArgs e)
        {
            if (e.ShowViewParameters.CreatedView == null)
                return;

            var detailViewID = FindAppplicableDetailViewID(ObjectSpace, DetailViewType.New,
                                                              e.ShowViewParameters.CreatedView.CurrentObject,
                                                              e.ShowViewParameters.CreatedView.ObjectTypeInfo.Type, "");

            if (detailViewID != null)
            {
                e.ShowViewParameters.CreatedView.SetInfo(detailViewID);
            }
        }

        /// <summary>
        /// Searches for the DetailViewID that satisfies all supplied criteria in the ConditionalDetailViews-Node
        /// </summary>
        /// <param name="os"></param>
        /// <param name="detailViewType">the kind of DetailView to be created</param>
        /// <param name="obj">the object for which to find the correct DetailView</param>
        /// <param name="newObjType"></param>
        /// <param name="defaultDetailViewID"></param>
        /// <returns></returns>
        protected virtual IModelDetailView FindAppplicableDetailViewID(ObjectSpace os, DetailViewType detailViewType, object obj,
                                                             Type newObjType, string defaultDetailViewID)
        {
            var eventArgs = new ChooseCustomDetailViewEventArgs(View, DetailViewType.New, obj);
            OnCustomChooseDetailView(eventArgs);
            if (eventArgs.Handled && !string.IsNullOrEmpty(eventArgs.DetailViewID))
            {
                return Application.Model.Views[eventArgs.DetailViewID] as IModelDetailView;
            }

            IModelDetailView result = (from item in model.ConditionalDetailViews
                                       where NodeMatches(os, item, detailViewType, newObjType, obj)
                                       select item.DetailView).FirstOrDefault() ??
                                      Application.Model.Views[defaultDetailViewID] as IModelDetailView;

            return result;
        }

        void OnCustomChooseDetailView(ChooseCustomDetailViewEventArgs eventArgs)
        {
            if (CustomChooseDetailView != null)
            {
                CustomChooseDetailView(this, eventArgs);
            }
        }

        protected virtual bool NodeMatches(ObjectSpace os, IModelConditionalDetailView conditionalDetailView,
                                           DetailViewType detailViewType,
                                           Type newObjType, object obj)
        {
            bool result = false;

            //First check: Does the node match the requested DetailView-mode?
            bool modeMatch = conditionalDetailView.Mode == DetailViewType.All
                             || (conditionalDetailView.Mode == DetailViewType.NestedAndRoot &&
                              (detailViewType == DetailViewType.Nested || detailViewType == DetailViewType.Root))
                             || (conditionalDetailView.Mode == DetailViewType.Nested && detailViewType == DetailViewType.Nested)
                             || (conditionalDetailView.Mode == DetailViewType.Root && detailViewType == DetailViewType.Root)
                             || (conditionalDetailView.Mode == DetailViewType.New && detailViewType == DetailViewType.New);

            if (modeMatch)
            {
                //Second check: Does the type match?
                try
                {
                    Type desiredClass = null;
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        desiredClass = assembly.GetType(conditionalDetailView.ClassName.TypeInfo.FullName);
                        if (desiredClass != null)
                            break;
                    }


                    if (detailViewType == DetailViewType.New)
                    {
                        if (conditionalDetailView.NewModeBehavior == ConditionalDetailViewNewMode.Strict)
                        {
                            result = (desiredClass == newObjType);
                        }
                        else
                        {
                            result = newObjType.IsSubclassOf(desiredClass);
                        }
                    }
                    else
                    {
                        //The node's desired class must not be a subclass of the actual object's class:
                        if (desiredClass != null && !desiredClass.IsSubclassOf(obj.GetType()))
                        {
                            //And final check: Does the object meet the specified criteria:
                            string criteria = conditionalDetailView.Criteria.Trim();
                            result = String.IsNullOrEmpty(criteria) || ObjectMeetsCriteria(os, obj, criteria);
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }
            return result;
        }


        /// <summary>
        /// Checks whether an object meets the supplied criteria string. Can be overridden to implement
        /// custom criteria syntax, such as Security considerations
        /// </summary>
        /// <param name="os"></param>
        /// <param name="obj">The object to check</param>
        /// <param name="criteria">The criteria string to be satisfied</param>
        /// <returns>true if object meets criteria</returns>
        protected virtual bool ObjectMeetsCriteria(ObjectSpace os, object obj, string criteria)
        {
            bool? isObjectFitForCriteria = os.IsObjectFitForCriteria(obj, CriteriaOperator.Parse(criteria));
            if (isObjectFitForCriteria != null) return isObjectFitForCriteria.Value;
            return false;
        }
    }

    public delegate void ChooseCustomDetailViewEventHandler(object sender, ChooseCustomDetailViewEventArgs e);

    public class ChooseCustomDetailViewEventArgs : EventArgs
    {
        readonly View callingView;
        readonly object currentObject;
        readonly DetailViewType requestedDetailViewType;

        public ChooseCustomDetailViewEventArgs(View callingView, DetailViewType requestedDetailViewType,
                                               object currentObject)
        {
            this.callingView = callingView;
            this.requestedDetailViewType = requestedDetailViewType;
            this.currentObject = currentObject;
            Handled = false;
        }

        /// <summary>
        /// The requested type of DetailView to be shown
        /// </summary>
        public DetailViewType RequestedDetailViewType
        {
            get { return requestedDetailViewType; }
        }


        /// <summary>
        /// The view in which the request for a new DetailView was invoked
        /// </summary>
        public View CallingView
        {
            get { return callingView; }
        }

        /// <summary>
        /// The object to be displayed in the new DetailView (is null when 
        /// requesting to display a DetailView in New-mode!)
        /// </summary>
        public object CurrentObject
        {
            get { return currentObject; }
        }

        public string DetailViewID { get; set; }

        public bool Handled { get; set; }
    }
}