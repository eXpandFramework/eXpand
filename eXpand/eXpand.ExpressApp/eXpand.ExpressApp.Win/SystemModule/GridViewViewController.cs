using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;
using GridListEditor = DevExpress.ExpressApp.Win.Editors.GridListEditor;
using System.Linq;
using NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class GridViewViewController : BaseViewController
    {
        public const string AllowExpandEmptyDetails = "AllowExpandEmptyDetails";
        public const string AutoExpandNewRow = "AutoExpandNewRow";
        public const string AutoSelectAllInEditorAttributeName = "AutoSelectAllInEditor";
        public const string DoNotLoadWhenNoFilterExists = "DoNotLoadWhenNoFilterExists";
        public const string EditorShowModeAttributeName = "EditorShowMode";
        public const string EnterMoveNextColumn = "EnterMoveNextColumn";

        public const string ExtraSerializationProperties = "ExtraSerializationProperties";
        public const string GroupLevelExpandIndex = "GroupLevelExpandIndex";
        public const string HideFieldCaptionOnGroup = "HideFieldCaptionOnGroup";
        public const string IsColumnHeadersVisible = "IsColumnHeadersVisible";
        //        public const string NewItemRowPositionAttributeName = "NewItemRowPosition";
        public const string SerializeFilterAttributeName = "SerializeFilter";
        public const string UseTabKey = "UseTabKey";


        private GridControl gridControl;
        private GridView mainView;
        private ListViewInfoNodeWrapper model;
        private bool newRowAdded;
        private DevExpress.ExpressApp.SystemModule.FilterController filterController;

        //        private XPDictionary xpDictionary;


        public GridViewViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();




            View.ControlsCreated += View_OnControlsCreated;



            model = new ListViewInfoNodeWrapper(View.Info);
        }

        ///<summary>
        ///
        ///<para>
        ///Updates the Application Model.
        ///
        ///</para>
        ///
        ///</summary>
        ///
        ///<param name="dictionary">
        ///		A <b>Dictionary</b> object that provides access to the Application Model's nodes.
        ///
        ///            </param>
        public override void UpdateModel(Dictionary dictionary)
        {
            base.UpdateModel(dictionary);
            var wrappers = new ApplicationNodeWrapper(dictionary).Views.Items.Where(wrapper => wrapper is ListViewInfoNodeWrapper);
            foreach (ListViewInfoNodeWrapper wrapper in wrappers)
            {
                wrapper.Node.SetAttribute(IsColumnHeadersVisible, true.ToString());
                wrapper.Node.SetAttribute(UseTabKey, true.ToString());
            }

            //            setFromXpDictionary(dictionary);
        }

        //        private void setShowNewItemRowInListView(XPClassInfo xpClassInfo, Dictionary dictionary,
        //                                                 XPMemberInfo xpMemberInfo)
        //        {
        //            Attribute attribute = xpMemberInfo.FindAttributeInfo(typeof (ShowNewItemRowInListView));
        //            if (attribute != null)
        //            {
        //                string attributeValue = xpClassInfo.ClassType.Name + "_" + xpMemberInfo.Name +
        //                                        "_ListView";
        //                setNewItemRowPositionValue(dictionary, attributeValue);
        //            }
        //        }

        //        private void setShowNewItemRowInListView(XPClassInfo xpClassInfo,
        //                                                 ShowNewItemRowInListView showNewItemRowInListView,
        //                                                 Dictionary dictionary)
        //        {
        //            if (showNewItemRowInListView != null)
        //            {
        //                string attributeValue = xpClassInfo.ClassType.Name +
        //                                        "_ListView";
        //                setNewItemRowPositionValue(dictionary, attributeValue);
        //            }
        //        }

        /*
                private void setNewItemRowPositionValue(Dictionary dictionary, string attributeValue)
                {
        //            DictionaryNode dictionaryNode =
        //                dictionary.RootNode.GetChildNode(ViewsNodeWrapper.NodeName).GetChildNode(ListView.InfoNodeName,
        //                                                                                         "ID",
        //                                                                                         attributeValue);
        //            dictionaryNode.SetAttribute(NewItemRowPositionAttributeName,
        //                                        NewItemRowPosition.Top.ToString());
                }
        */

        /*
                private void setFromXpDictionary(Dictionary dictionary)
                {
                    ICollection collectClassInfos = xpDictionary.Classes;
                    foreach (XPClassInfo xpClassInfo in collectClassInfos)
                    {
                        var showNewItemRowInListView =
                            xpClassInfo.FindAttributeInfo(typeof (ShowNewItemRowInListView)) as ShowNewItemRowInListView;
                        setShowNewItemRowInListView(xpClassInfo, showNewItemRowInListView, dictionary);
                        foreach (XPMemberInfo xpMemberInfo in xpClassInfo.CollectionProperties)
                            setShowNewItemRowInListView(xpClassInfo, dictionary, xpMemberInfo);
                    }
                }
        */

        //        private void View_OnInfoSynchronized(object sender, EventArgs e)
        //        {
        //            var gridcontrol = View.Control as GridControl;
        //            if (gridcontrol != null)
        //            {
        //                var gridView = (GridView) gridcontrol.FocusedView;
        //                string[] strings = GetExtraSerializationProperties();
        //                if (strings.Length > 0 && strings[0] != "" &&
        //                    File.Exists(View.Id + typeof (MoreSerializer).Name + ".xml"))
        //                {
        //                    MoreSerializer.LoadFilter(gridView, View.Id + typeof (MoreSerializer).Name + ".xml",
        //                                              strings);
        //                }
        //            }
        //        }

        //        protected override void OnDeactivating()
        //        {
        //            base.OnDeactivating();
        //
        //
        //            var gridcontrol = View.Control as GridControl;
        //            if (gridcontrol != null)
        //            {
        //                string[] strings = GetExtraSerializationProperties();
        //                if (strings.Length > 0 && strings[0] != "")
        //                {
        //                    string path = View.Id + typeof (MoreSerializer).Name + ".xml";
        //                    if (File.Exists(path))
        //                        File.Delete(path);
        //                    using (
        //                        var stream =
        //                            new FileStream(path, FileMode.CreateNew))
        //                        MoreSerializer.SaveFilter(mainView, stream, strings);
        //                }
        //            }
        //        }

        //        private string[] GetExtraSerializationProperties()
        //        {
        //            string attributeValue = View.Info.GetAttributeValue(ExtraSerializationProperties, "");
        //            if (attributeValue == "")
        //                attributeValue = null;
        //            if (View.Info.GetAttributeBoolValue(SerializeFilterAttributeName, true))
        //                attributeValue += "ActiveFilterEnabled,ActiveFilterString,MRUFilters,ActiveFilter";
        //            return (attributeValue + "").Split(',');
        //        }







        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            gridControl = View.Control as GridControl;
            if (gridControl == null)
                return;
            //            ((GridControl) View.Control).ServerMode =
            //                Application.Info.GetChildNode("Options").GetAttributeBoolValue("UseServerMode");
            gridControl.HandleCreated += GridControl_OnHandleCreated;


            mainView = gridControl.MainView as GridView;
            if (mainView != null)
            {
                mainView.FocusedRowChanged += GridView_OnFocusedRowChanged;
                mainView.InitNewRow += GridView_OnInitNewRow;
                mainView.ShownEditor += MainViewOnShownEditor;
                SetOptions(mainView, model);
            }


            if (View.Info.GetAttributeBoolValue(DoNotLoadWhenNoFilterExists, false) &&
                ((GridView)gridControl.MainView).FilterPanelText ==
                string.Empty)
            {
                if (mainView != null) mainView.ActiveFilter.Changed += ActiveFilter_OnChanged;

                filterController = Frame.GetController<DevExpress.ExpressApp.SystemModule.FilterController>();
                filterController.FullTextFilterAction.Execute += FullTextFilterAction_Execute;
                SetDoNotLoadWhenFilterExistsCriteria();
            }
        }

        private void FullTextFilterAction_Execute(object sender, DevExpress.ExpressApp.Actions.ParametrizedActionExecuteEventArgs e)
        {
            ClearDoNotLoadWhenFilterExistsCriteria();
        }

        private void MainViewOnShownEditor(object sender, EventArgs args)
        {
            var view = (GridView)sender;
            if (view.IsFilterRow(view.FocusedRowHandle))
                view.ActiveEditor.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
        }


        private void SetDoNotLoadWhenFilterExistsCriteria()
        {
            ((ListView)View).CollectionSource.Criteria[DoNotLoadWhenNoFilterExists] = new BinaryOperator("Oid", Guid.NewGuid());
        }

        private void ClearDoNotLoadWhenFilterExistsCriteria()
        {
            ((ListView)View).CollectionSource.Criteria[DoNotLoadWhenNoFilterExists] = null;
        }

        private void ActiveFilter_OnChanged(object sender, EventArgs e)
        {
            if (((GridView)gridControl.MainView).FilterPanelText !=
                string.Empty)
                ClearDoNotLoadWhenFilterExistsCriteria();
            else
                SetDoNotLoadWhenFilterExistsCriteria();
        }


        /*
                [Obsolete("only for 8.1")]
                private void MainView_OnCustomDrawGroupRow(object sender, CustomDrawObjectEventArgs e)
                {
                    var gridGroupRowInfo = (GridGroupRowInfo) e.Info;

                    DictionaryNode childNodeByPath = Application.Info.GetChildNodeByPath(
                        "Views\\ListView[@ID='" + View.Id + "']\\Columns\\ColumnInfo[@PropertyName='" +
                        gridGroupRowInfo.Column.FieldName.Replace("!", "") + "']");
                    string text = null;
                    var regexObj = new Regex(@"\(Count=([\d]*)");
                    string value = regexObj.Match(gridGroupRowInfo.GroupText).Groups[1].Value;
                    if (value == "")
                        return;
                    if (gridGroupRowInfo.EditValue is DateTime)
                    {
                        if (
                            !string.IsNullOrEmpty(
                                 childNodeByPath.GetAttributeValue(ColumnInfoNodeWrapper.DisplayFormatAttribute)))
                        {
                            regexObj = new Regex(@"\{0:([^}]*)");

                            string toString = ((DateTime) gridGroupRowInfo.EditValue).ToString(
                                regexObj.Match(childNodeByPath.GetAttributeValue(
                                                   ColumnInfoNodeWrapper.DisplayFormatAttribute)).Groups[1].Value);

                            text = gridGroupRowInfo.Column.Caption + ":[#image]" +
                                   toString + " (Count=" + value + ")";
                        }
                    }
                    if (childNodeByPath.GetAttributeBoolValue(HideFieldCaptionOnGroup))
                    {
                        if (text == null)
                            text = gridGroupRowInfo.GroupText;
                        gridGroupRowInfo.GroupText =
                            text.Replace(gridGroupRowInfo.Column.Caption + ":", "").Replace("Count=", "");
                    }
                }
        */

        private void GridView_OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (newRowAdded && mainView.IsValidRowHandle(e.FocusedRowHandle))
            {
                newRowAdded = false;
                if (model.Node.GetAttributeBoolValue(AutoExpandNewRow))
                    mainView.ExpandMasterRow(e.FocusedRowHandle);
            }
        }

        ///<summary>
        ///
        ///<para>
        ///Returns the Schema extension which is combined with the entire Schema when loading the Application Model.
        ///
        ///</para>
        ///
        ///</summary>
        ///
        ///<returns>
        ///The <b>Schema</b> object that represents the Schema extension to be added to the application's entire Schema.
        ///
        ///</returns>
        ///
        public override Schema GetSchema()
        {
            string CommonTypeInfos =
                @"<Element Name=""Application"">
                    <Element Name=""Views"">
                        <Element Name=""ListView"" >
                            <Element Name=""Columns"">
                                    <Element Name=""ColumnInfo"">
                                        <Attribute Name=""" +
                HideFieldCaptionOnGroup +
                @""" Choice=""True,False""/>
                                    </Element>  
                                </Element>
                            <Attribute Name=""" +
                IsColumnHeadersVisible +
                @""" Choice=""True,False""/>
                            <Attribute Name=""" +
                AllowExpandEmptyDetails +
                @""" Choice=""False,True""/>
                            <Attribute Name=""" +
                AutoExpandNewRow +
                @""" Choice=""False,True""/>
                            <Attribute Name=""" +
                EnterMoveNextColumn +
                @""" Choice=""False,True""/>
                
                            
                            <Attribute Name=""" +
                ExtraSerializationProperties +
                @""" />
                            <Attribute Name=""" +
                GroupLevelExpandIndex +
                @""" Choice=""False,True""/>
                            <Attribute Name=""" +
                UseTabKey +
                @""" Choice=""False,True""/>
                            <Attribute Name=""" +
                AutoSelectAllInEditorAttributeName +
                @""" Choice=""False,True""/>
                            <Attribute Name=""" +
                SerializeFilterAttributeName +
                @""" Choice=""False,True""/>
                            <Attribute Name=""" +
                DoNotLoadWhenNoFilterExists +
                @""" Choice=""False,True""/>
                            <Attribute Name=""" +
                EditorShowModeAttributeName +
                @""" Choice=""{" + typeof(EditorShowMode).FullName +
                @"}""/>
                        </Element>
                    </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }


        private void GridView_OnInitNewRow(object sender, InitNewRowEventArgs e)
        {
            newRowAdded = true;
        }


        public static void SetOptions(GridView gridView, ListViewInfoNodeWrapper listViewInfoNodeWrapper)
        {
            gridView.OptionsView.NewItemRowPosition = (NewItemRowPosition)Enum.Parse(typeof(NewItemRowPosition), new SupportNewItemRowNodeWrapper(listViewInfoNodeWrapper.Node).NewItemRowPosition.ToString());
            gridView.OptionsBehavior.EditorShowMode = EditorShowMode.Click;
            gridView.OptionsBehavior.Editable = true;
            gridView.OptionsBehavior.AllowIncrementalSearch = true;
            gridView.OptionsBehavior.AutoSelectAllInEditor = false;
            gridView.OptionsBehavior.AutoPopulateColumns = false;
            gridView.OptionsBehavior.FocusLeaveOnTab = true;
            gridView.OptionsBehavior.AutoExpandAllGroups = listViewInfoNodeWrapper.Node.GetAttributeBoolValue(GridListEditor.AutoExpandAllGroups, false);
            gridView.OptionsSelection.MultiSelect = true;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
            gridView.OptionsNavigation.AutoFocusNewRow = true;
            gridView.OptionsNavigation.AutoMoveRowFocus = true;
            gridView.OptionsView.ShowDetailButtons = false;
            gridView.OptionsDetail.EnableMasterViewMode = false;
            gridView.OptionsView.ShowIndicator = true;
            gridView.OptionsView.ShowGroupPanel = listViewInfoNodeWrapper.Node.GetAttributeBoolValue(GridListEditor.IsGroupPanelVisible, false);
            gridView.OptionsView.ShowFooter = listViewInfoNodeWrapper.Node.GetAttributeBoolValue(GridListEditor.IsFooterVisible, true);
            gridView.OptionsView.ShowAutoFilterRow = listViewInfoNodeWrapper.IsFilterPanelVisible;
            gridView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            gridView.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
            gridView.ActiveFilterEnabled = listViewInfoNodeWrapper.Node.GetAttributeBoolValue(GridListEditor.IsActiveFilterEnabled, true);

            gridView.OptionsDetail.AllowExpandEmptyDetails =
                listViewInfoNodeWrapper.Node.GetAttributeBoolValue(AllowExpandEmptyDetails,
                                                                   false);

            gridView.OptionsNavigation.EnterMoveNextColumn =
                listViewInfoNodeWrapper.Node.GetAttributeBoolValue(EnterMoveNextColumn,
                                                                   false);

            gridView.OptionsNavigation.UseTabKey = listViewInfoNodeWrapper.Node.GetAttributeBoolValue(UseTabKey,
                                                                                                      false);


            gridView.OptionsView.ShowColumnHeaders =
                listViewInfoNodeWrapper.Node.GetAttributeBoolValue(IsColumnHeadersVisible, true);
            //            DevExpress.XtraGrid.Views.Grid.NewItemRowPosition newItemRowPosition =
            //                listViewInfoNodeWrapper.Node.GetAttributeEnumValue(NewItemRowPositionAttributeName,
            //                                                                   DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.
            //                                                                       None);
            //            gridView.OptionsView.NewItemRowPosition = newItemRowPosition;
            gridView.OptionsBehavior.AutoSelectAllInEditor =
                listViewInfoNodeWrapper.Node.GetAttributeBoolValue(AutoSelectAllInEditorAttributeName,
                                                                   true);
            gridView.OptionsBehavior.EditorShowMode =
                listViewInfoNodeWrapper.Node.GetAttributeEnumValue(EditorShowModeAttributeName,
                                                                   EditorShowMode.MouseUp);
            gridView.OptionsView.ShowFooter = listViewInfoNodeWrapper.Node.GetAttributeBoolValue(GridListEditor.IsFooterVisible, true);
        }

        private void GridControl_OnHandleCreated(object sender, EventArgs e)
        {
            mainView.GridControl.ForceInitialize();


            string value = View.Info.GetAttributeValue(GroupLevelExpandIndex);
            if (!string.IsNullOrEmpty(value))
            {
                int int32 = Convert.ToInt32(value);
                if (mainView.GroupCount == int32)
                    for (int i = -1; ; i--)
                    {
                        if (!mainView.IsValidRowHandle(i)) return;
                        if (mainView.GetRowLevel(i) < int32 - 1)
                            mainView.SetRowExpanded(i, true);
                    }
            }
        }


    }
}