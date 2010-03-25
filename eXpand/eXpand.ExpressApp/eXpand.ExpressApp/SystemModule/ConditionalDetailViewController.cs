using System;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.Enums;
using ViewIdRefNodeProvider = DevExpress.ExpressApp.Core.DictionaryHelpers.ViewIdRefNodeProvider;

namespace eXpand.ExpressApp.SystemModule {
    public partial class ConditionalDetailViewController : ViewController<ListView> {
        public const string ConditionalDetailViewsAttributeName = "ConditionalDetailViews";
        
        DictionaryNode _wrapper;
        bool isActive;

        NewObjectViewController _newObjectViewController;
        ListViewProcessCurrentObjectController _listViewProcessCurrentObjectController;

        public ConditionalDetailViewController() {
            InitializeComponent();
            RegisterActions(components);
        }

        public event ChooseCustomDetailViewEventHandler CustomChooseDetailView;
        public class ConditionalDetailViewsWrapper : NodeWrapper
        {
            public const string ConditionalDetailViewAttributeName = "ConditionalDetailView";
        }

        public override Schema GetSchema() {
            string s1 = @"<Element Name=""Application"">
                        <Element Name=""BOModel"" >
                          <Element Name=""Class"" >
                            <Attribute Name=""DefaultListView_EnableOpenActionInMasterDetailMode"" Choice=""True,False="" />     
                          </Element>
                        </Element>
                        <Element Name=""Views"">
                            <Element Name=""ListView"">
                              <Element Name=""" +ConditionalDetailViewsAttributeName + @""" IsNewNode=""True"">
                                <Attribute Name=""EnableOpenActionInMasterDetailMode"" DefaultValueExpr=""SourceNode=BOModel\Class\@Name=..\@ClassName; SourceAttribute=@DefaultListView_EnableOpenActionInMasterDetailMode"" Choice=""True,False="" />
                                <Element Name=""" + ConditionalDetailViewsWrapper.ConditionalDetailViewAttributeName + @""" KeyAttribute=""ID"" DisplayAttribute=""ID"" Multiple=""True""  IsNewNode=""True"">
                                  <Attribute Name=""" +ConditionalDetailViewWrapper.IDAttributeName+ @""" Required=""True"" IsNewNode=""True""/>
                                  <Attribute Name="""+ConditionalDetailViewWrapper.ClassNameAttributeName + @""" RefNodeName=""{" +typeof(ClassRefNodeProvider).FullName + @"}ClassName=..\..\@ClassName"" IsNewNode=""True""/>
                                  <Attribute Name=""" + ConditionalDetailViewWrapper.CriteriaAttributeName + @""" IsNewNode=""True""/>
                                  <Attribute Name="""+ConditionalDetailViewWrapper.ModeAttributeName + @""" Choice=""{"+typeof(DetailViewType).FullName + @"}"" IsNewNode=""True"" />
                                  <Attribute Name=""" + ConditionalDetailViewWrapper.NewModeBehaviorAttributeName + @""" Choice=""Strict,IncludeSubclasses"" IsNewNode=""True"" />
                                  <Attribute Name=""" + ConditionalDetailViewWrapper.DetailViewIDAttributeName + @""" RefNodeName=""{" + typeof(ViewIdRefNodeProvider).FullName + @"}ClassName=@ClassName;ViewType=DetailView;IncludeBaseClasses=True"" IsNewNode=""True""/>
                                  <Attribute Name=""" + ConditionalDetailViewWrapper.IndexAttributeName + @""" IsNewNode=""True"" />
                                </Element>
                              </Element>
                            </Element>
                      </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s1));
        }

        protected override void OnActivated() {
            base.OnActivated();
            _wrapper = View.Info.GetChildNode("ConditionalDetailViews");
            _newObjectViewController = Frame.GetController<NewObjectViewController>();
            _newObjectViewController.NewObjectAction.Executed += NewObjectAction_Executed;
            if (_wrapper != null && _wrapper.ChildNodeCount > 0) {
                isActive = true;
                View.CreateCustomCurrentObjectDetailView +=ConditionalDetailViewController_CreateCustomCurrentObjectDetailView;
                _listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                _listViewProcessCurrentObjectController.CustomProcessSelectedItem +=CustomProcessSelectedItem;
                View.ProcessSelectedItem +=ConditionalDetailViewController_ProcessSelectedItem;
            }
        }

        protected override void OnDeactivating() {
            _newObjectViewController.NewObjectAction.Executed -= NewObjectAction_Executed;
            if (isActive) {
                View.CreateCustomCurrentObjectDetailView -=ConditionalDetailViewController_CreateCustomCurrentObjectDetailView;
                _listViewProcessCurrentObjectController.CustomProcessSelectedItem -=CustomProcessSelectedItem;
                View.ProcessSelectedItem -=ConditionalDetailViewController_ProcessSelectedItem;
            }

            base.OnDeactivating();
        }

        void ConditionalDetailViewController_ProcessSelectedItem(object sender, EventArgs e) {
            //Show popup detailview even in ListViewAndDetailView-mode?

            if (!View.IsRoot && View.Info.GetAttributeValue("MasterDetailMode") == "ListViewAndDetailView"
                && _wrapper.GetAttributeBoolValue("EnableOpenActionInMasterDetailMode", false)) {
                if (_listViewProcessCurrentObjectController.ProcessCurrentObjectAction.Enabled && _listViewProcessCurrentObjectController.ProcessCurrentObjectAction.Active) {
                    _listViewProcessCurrentObjectController.ProcessCurrentObjectAction.DoExecute();
                }
            }
        }

        void CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
            //this is called when XAF wants to open a new root DetailView for the selected object in the ListView

            string detailViewID = FindAppplicableDetailViewID(ObjectSpace, DetailViewType.Root,e.InnerArgs.CurrentObject, null, "");
            if (string.IsNullOrEmpty(detailViewID)) {
                e.Handled = false;
            }
            else {
                ObjectSpace os = Application.GetObjectSpaceToShowViewFrom(Frame);
                object objInTargetOS;
                if (os != View.ObjectSpace) {
                    if (!(os is NestedObjectSpace) && View.ObjectSpace.IsNewObject(e.InnerArgs.CurrentObject)) {
                        throw new InvalidOperationException(ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(ExceptionId.AnUnsavedObjectCannotBeShown));
                    }
                    objInTargetOS = os.GetObject(e.InnerArgs.CurrentObject);
                }
                else {
                    objInTargetOS = e.InnerArgs.CurrentObject;
                }

                e.InnerArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(os, detailViewID, true,
                                                                                          objInTargetOS);

                var propertyCollectionSource = View.CollectionSource as PropertyCollectionSource;
                if (propertyCollectionSource != null) {
                    if (propertyCollectionSource.MemberInfo.IsAggregated && !View.AllowEdit) {
                        e.InnerArgs.ShowViewParameters.CreatedView.AllowEdit.SetItemValue(
                            "From ListView with aggregated read-only property collection", false);
                    }
                }

                e.Handled = true;
            }
        }


        void ConditionalDetailViewController_CreateCustomCurrentObjectDetailView(object sender,
                                                                                 CreateCustomCurrentObjectDetailViewEventArgs
                                                                                     e) {
            //is called by XAF to switch nested DetailViews(in MasterDetailMode == ListViewAndDetailView)
            e.DetailViewId = FindAppplicableDetailViewID(ObjectSpace, DetailViewType.Nested, e.ListViewCurrentObject,
                                                         null, View.DetailViewId);
        }

        void NewObjectAction_Executed(object sender, ActionBaseEventArgs e) {
            if (e.ShowViewParameters.CreatedView == null)
                return;

            string detailViewID = FindAppplicableDetailViewID(ObjectSpace, DetailViewType.New,
                                                              e.ShowViewParameters.CreatedView.CurrentObject,
                                                              e.ShowViewParameters.CreatedView.ObjectTypeInfo.Type, "");

            if (detailViewID != "") {
                var quickDetailView =
                    Application.Model.RootNode.FindChildElementByPath(@"Views\DetailView[@ID='" + detailViewID + "']") as
                    DictionaryNode;
                if (quickDetailView != null) {
                    e.ShowViewParameters.CreatedView.SetInfo(quickDetailView);
                }
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
        protected virtual string FindAppplicableDetailViewID(ObjectSpace os, DetailViewType detailViewType, object obj,
                                                             Type newObjType, string defaultDetailViewID) {
            var eventArgs = new ChooseCustomDetailViewEventArgs(View, DetailViewType.New, obj);
            OnCustomChooseDetailView(eventArgs);
            if (eventArgs.Handled && !string.IsNullOrEmpty(eventArgs.DetailViewID)){
                return eventArgs.DetailViewID;
            }

            string result = string.Empty;
            if (_wrapper != null) {
                DictionaryNodeCollection coll = _wrapper.GetChildNodes("ConditionalDetailView", false);

                int i = 0;
                while (i < coll.Count) {
                    if (NodeMatches(os, coll[i], detailViewType, newObjType, obj)) {
                        result = coll[i].GetAttributeValue("DetailViewID");
                        break;
                    }
                    i++;
                }
            }

            if (string.IsNullOrEmpty(result))
                result = defaultDetailViewID;

            return result;
        }

        void OnCustomChooseDetailView(ChooseCustomDetailViewEventArgs eventArgs) {
            if (CustomChooseDetailView != null) {
                CustomChooseDetailView(this, eventArgs);
            }
        }

        protected virtual bool NodeMatches(ObjectSpace os, DictionaryNode conditionalDetailViewNode,
                                           DetailViewType detailViewType,
                                           Type newObjType, object obj) {
            bool result = false;

            //First check: Does the node match the requested DetailView-mode?
            bool modeMatch = conditionalDetailViewNode.GetAttributeValue("Mode") == "All"
                             ||(conditionalDetailViewNode.GetAttributeValue("Mode") == "NestedAndRoot" &&
                              (detailViewType == DetailViewType.Nested || detailViewType == DetailViewType.Root))
                             ||(conditionalDetailViewNode.GetAttributeValue("Mode") == "Nested" &&detailViewType == DetailViewType.Nested)
                             ||(conditionalDetailViewNode.GetAttributeValue("Mode") == "Root" &&detailViewType == DetailViewType.Root)
                             ||(conditionalDetailViewNode.GetAttributeValue("Mode") == "New" &&detailViewType == DetailViewType.New);

            if (modeMatch) {
                //Second check: Does the type match?
                try {
                    Type desiredClass = null;
                    foreach (Assembly assembly in  AppDomain.CurrentDomain.GetAssemblies()) {
                        desiredClass = assembly.GetType(conditionalDetailViewNode.GetAttributeValue("ClassName"));
                        if (desiredClass != null)
                            break;
                    }


                    if (detailViewType == DetailViewType.New) {
                        if (conditionalDetailViewNode.GetAttributeValue("NewModeBehavior", "Strict") == "Strict") {
                            result = (desiredClass == newObjType);
                        }
                        else {
                            result = newObjType.IsSubclassOf(desiredClass);
                        }
                    }
                    else {
                        //The node's desired class must not be a subclass of the actual object's class:
                        if (desiredClass != null && !desiredClass.IsSubclassOf(obj.GetType())) {
                            //And final check: Does the object meet the specified criteria:
                            string criteria = conditionalDetailViewNode.GetAttributeValue("Criteria").Trim();
                            result = String.IsNullOrEmpty(criteria) || ObjectMeetsCriteria(os, obj, criteria);
                        }
                    }
                }
                catch {
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
        protected virtual bool ObjectMeetsCriteria(ObjectSpace os, object obj, string criteria) {
            bool? isObjectFitForCriteria = os.IsObjectFitForCriteria(obj, CriteriaOperator.Parse(criteria));
            if (isObjectFitForCriteria != null) return isObjectFitForCriteria.Value;
            return false;
        }
    }

    public delegate void ChooseCustomDetailViewEventHandler(object sender, ChooseCustomDetailViewEventArgs e);

    public class ChooseCustomDetailViewEventArgs : EventArgs {
        readonly View callingView;
        readonly object currentObject;
        readonly DetailViewType requestedDetailViewType;

        public ChooseCustomDetailViewEventArgs(View callingView, DetailViewType requestedDetailViewType,
                                               object currentObject) {
            this.callingView = callingView;
            this.requestedDetailViewType = requestedDetailViewType;
            this.currentObject = currentObject;
            Handled = false;
        }

        /// <summary>
        /// The requested type of DetailView to be shown
        /// </summary>
        public DetailViewType RequestedDetailViewType {
            get { return requestedDetailViewType; }
        }


        /// <summary>
        /// The view in which the request for a new DetailView was invoked
        /// </summary>
        public View CallingView {
            get { return callingView; }
        }

        /// <summary>
        /// The object to be displayed in the new DetailView (is null when 
        /// requesting to display a DetailView in New-mode!)
        /// </summary>
        public object CurrentObject {
            get { return currentObject; }
        }

        public string DetailViewID { get; set; }

        public bool Handled { get; set; }
    }
}