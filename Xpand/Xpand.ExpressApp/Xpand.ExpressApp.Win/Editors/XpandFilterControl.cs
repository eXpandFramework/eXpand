using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.FilterEditor;
using Fasterflect;
using Xpand.ExpressApp.SystemModule.Search;
using Xpand.Xpo.CustomFunctions;
using Xpand.Xpo.Filtering;
using System.ComponentModel;

namespace Xpand.ExpressApp.Win.Editors {

    public static class FilterControlExtensions{
        public static void RaisePopupMenuShowingX(this IXpandFilterControl filterControl,PopupMenuShowingEventArgs e){
            if (e.MenuType == FilterControlMenuType.Clause && filterControl.ModelMembers != null){
                var criteriaOperator = new XpandNodeToCriteriaProcessor().Process(e.CurrentNode);
                var operandProperty = criteriaOperator.GetOperators().OfType<OperandProperty>().FirstOrDefault();
                if (!ReferenceEquals(operandProperty,null)){
                    var modelMember =filterControl.ModelMembers.Cast<IModelMemberFullTextContains>().FirstOrDefault(member 
                        => member.FullText && member.Name == operandProperty.PropertyName);
                    if (modelMember != null){
                        var dxMenuItem =new DXMenuItem(ClauseTypeEnumHelper.GetMenuStringByClauseType(ClauseTypeEnumHelper.FullText),filterControl.OnClauseClick){Tag = ClauseTypeEnumHelper.FullText};
                        e.Menu.Items.Add(dxMenuItem);
                    }
                }
            }
        }

        public static WinFilterTreeNodeModel CreateXModel(this IXpandFilterControl filterControl){
            var control = (FilterControl) filterControl;
            return new XpandFilterTreeNodeModel(control);
        }

        public static void OnClauseClick(this IXpandFilterControl filterControl,object sender, EventArgs e){
            var filterControlFocusInfo = filterControl.FocusInfo;
            var node = (ClauseNode)filterControlFocusInfo.Node;
            node.Operation = (ClauseType)((DXMenuItem)sender).Tag;
            filterControl.RefreshTreeAfterNodeChange();
            filterControl.RaiseFilterChanged(new FilterChangedEventArgs(FilterChangedAction.OperationChanged, node));
            FilterControlFocusInfo fi = filterControlFocusInfo.OnRight();
            if (fi.Node == filterControlFocusInfo.Node)
                filterControl.FocusInfo=fi;
        }
    }

    public interface IXpandFilterControl{
        IEnumerable<IModelMember> ModelMembers { get; }
        Func<CriteriaOperator> Criteria { get; }
        WinFilterTreeNodeModel CreateModel();
        FilterControlFocusInfo FocusInfo { get; set; }
        void RefreshTreeAfterNodeChange();
        void RaiseFilterChanged(FilterChangedEventArgs args);
        void RaisePopupMenuShowing(PopupMenuShowingEventArgs e);
    }

    public class XpandGridFilterControl(Func<CriteriaOperator> criteria, Func<IEnumerable<IModelMember>> modelMembers)
        : GridFilterControl(FilterCriteriaDisplayStyle.Default), IXpandFilterControl {
        public IEnumerable<IModelMember> ModelMembers => modelMembers();

        public Func<CriteriaOperator> Criteria => criteria;

        WinFilterTreeNodeModel IXpandFilterControl.CreateModel() => this.CreateXModel();

        protected override WinFilterTreeNodeModel CreateModel() => ((IXpandFilterControl)this).CreateModel();

        FilterControlFocusInfo IXpandFilterControl.FocusInfo {
            get => FocusInfo;
            set => FocusInfo = value;
        }

        void IXpandFilterControl.RefreshTreeAfterNodeChange() {
            this.CallMethod("RefreshTreeAfterNodeChange");
        }

        void IXpandFilterControl.RaiseFilterChanged(FilterChangedEventArgs args) {
            RaiseFilterChanged(args);
        }

        protected override void RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
            base.RaisePopupMenuShowing(e);
            ((IXpandFilterControl) this).RaisePopupMenuShowing(e);
        }

        void IXpandFilterControl.RaisePopupMenuShowing(PopupMenuShowingEventArgs e){
            this.RaisePopupMenuShowingX(e);
        }
    }

    public class XpandFilterControl(Func<CriteriaOperator> criteria, Func<IEnumerable<IModelMember>> fullTextMembers)
        : FilterControl, IXpandFilterControl {
        public event Action<BaseEdit> EditorActivated;

        protected void InvokeEditorActivated(BaseEdit baseEdit) {
            Action<BaseEdit> activated = EditorActivated;
            if (activated != null && baseEdit != null) activated(baseEdit);
        }

        protected override void RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
            base.RaisePopupMenuShowing(e);
            ((IXpandFilterControl)this).RaisePopupMenuShowing(e);
        }

        void IXpandFilterControl.RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
            this.RaisePopupMenuShowingX(e);
        }

        protected override void OnFocusedElementChanged() {
            base.OnFocusedElementChanged();
            InvokeEditorActivated(ActiveEditor);
        }

        protected override void ShowElementMenu(ElementType type) {
            base.ShowElementMenu(type);
            InvokeEditorActivated(ActiveEditor);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            InvokeEditorActivated(ActiveEditor);
        }


        public IEnumerable<IModelMember> ModelMembers => fullTextMembers();

        public Func<CriteriaOperator> Criteria => criteria;

        WinFilterTreeNodeModel IXpandFilterControl.CreateModel() => this.CreateXModel();

        protected override WinFilterTreeNodeModel CreateModel() => ((IXpandFilterControl) this).CreateModel();

        FilterControlFocusInfo IXpandFilterControl.FocusInfo{
            get => FocusInfo;
            set => FocusInfo=value;
        }

        void IXpandFilterControl.RefreshTreeAfterNodeChange(){
            this.CallMethod("RefreshTreeAfterNodeChange");
        }

        void IXpandFilterControl.RaiseFilterChanged(FilterChangedEventArgs args){
            RaiseFilterChanged(args);
        }
    }

    public static class ClauseTypeEnumHelper {
        static readonly int BaseValue = Convert.ToInt32(Enum.GetValues(typeof(ClauseType)).Cast<ClauseType>().Max());

        public static int FullText => BaseValue + 1;

        public static string GetMenuStringByClauseType(int clauseType) => clauseType == FullText ? "HasText" : string.Empty;
    }

    public class XpandClauseNode(FilterTreeNodeModel model) : ClauseNode(model) {
        protected override void ChangeElement(NodeEditableElement element, object value) {
            if (element.ElementType == ElementType.Operation && !Enum.IsDefined(typeof(ClauseType), value)) {
                Model.BeginUpdate();
                Operation = (ClauseType)value;
                var clauseType = (int)value;
                if (clauseType == ClauseTypeEnumHelper.FullText)
                    FilterControlHelpers.ForceAdditionalParamsCount(AdditionalOperands, 1);
                Model.EndUpdate(FilterChangedAction.OperationChanged, this);
                FilterControlFocusInfo fi = FocusInfo.OnRight();
                if (fi.Node == FocusInfo.Node)
                    FocusInfo = fi;
            }
            else base.ChangeElement(element, value);
        }
    }

    public class XpandNodeToCriteriaProcessor : NodeToCriteriaProcessor {
        public override object Visit(IClauseNode clauseNode) {
            if (Enum.IsDefined(typeof(ClauseType), clauseNode.Operation))
                return base.Visit(clauseNode);
            int operation = Convert.ToInt32(clauseNode.Operation);
            if (operation == ClauseTypeEnumHelper.FullText)
                return new FunctionOperator(FullTextContainsFunction.FunctionName, clauseNode.FirstOperand, clauseNode.AdditionalOperands[0]);
            throw new NotImplementedException();
        }
    }


    public interface IXpandFilterTreeNodeModel{
        void CreateTree(CriteriaOperator criteria);
    }

    public class CriteriaToTreeProcessorBase(INodesFactory nodesFactory, IList<CriteriaOperator> skippedHolder)
        : CriteriaToTreeProcessor(nodesFactory, skippedHolder);
    public class XpandCriteriaToTreeProcessor(INodesFactory nodesFactory, IList<CriteriaOperator> skippedHolder)
        : CriteriaToTreeProcessorBase(nodesFactory, skippedHolder), IClientCriteriaVisitor<INode> {
        public object ProcessX(CriteriaOperator criteriaOperator) => this.CallMethod("Process", [typeof (CriteriaOperator)], criteriaOperator);

        INode ICriteriaVisitor<INode>.Visit(FunctionOperator theOperand){
            var skippedHolder = new List<CriteriaOperator>();
            var visit = ((IClientCriteriaVisitor<INode>) new CriteriaToTreeProcessorBase(Factory, skippedHolder)).Visit(theOperand);
            if (skippedHolder.Contains(theOperand)&&theOperand.OperatorType==FunctionOperatorType.Custom){
                skippedHolder.Remove(theOperand);
                return Factory.Create((ClauseType) ClauseTypeEnumHelper.FullText, (OperandProperty) theOperand.Operands[1],
                    [theOperand.Operands[2]]);
            }
            return visit;
        }
    }
    public class XpandFilterTreeNodeModel : WinFilterTreeNodeModel, IXpandFilterTreeNodeModel{
        private bool _isUpdating;
        public XpandFilterTreeNodeModel(FilterControl control) : base(control) => OnNotifyControl+=OnOnNotifyControl;

        private void OnOnNotifyControl(FilterChangedEventArgs info){
            if (info.Action == FilterChangedAction.RebuildWholeTree && info.CurrentNode == null&&!_isUpdating){
                var criteriaOperator = ((IXpandFilterControl)Control).Criteria();
                if (!ReferenceEquals(criteriaOperator,null)){
                    _isUpdating = true;
                    BeginUpdate();
                    RootNode = null;
                    var processor = new XpandCriteriaToTreeProcessor(CreateNodesFactory(), new List<CriteriaOperator>());

                    var node = (Node) processor.ProcessX(criteriaOperator);
                    if (AllowCreateDefaultClause && node == null){
                        node = CreateCriteriaByDefaultProperty();
                    }
                    RootNode = node as GroupNode;
                    if (RootNode == null){
                        RootNode = CreateGroupNode(null);
                        if (node != null){
                            RootNode.AddNode(node);
                        }
                    }
                    FocusInfo = new FilterControlFocusInfo(RootNode, 0);
                    EndUpdate(FilterChangedAction.RebuildWholeTree);
                    _isUpdating = false;
                }

            }
        }


        void IXpandFilterTreeNodeModel.CreateTree(CriteriaOperator criteria){
            CreateTree(criteria);
        }


        public override ClauseNode CreateClauseNode() => new XpandClauseNode(this);

        public override CriteriaOperator ToCriteria(INode node) => new XpandNodeToCriteriaProcessor().Process(node);

        public override string GetMenuStringByType(ClauseType type) => Enum.IsDefined(typeof(ClauseType), type) ? base.GetMenuStringByType(type) :
            ClauseTypeEnumHelper.GetMenuStringByClauseType((int)type);
    }

    public class FilterEditorControl(Func<IEnumerable<IModelMember>> fullTextMembers)
        : DevExpress.XtraFilterEditor.FilterEditorControl {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string EditorText {
            get => base.EditorText;
            set => base.EditorText = value;
        }

        protected override bool CanBeDisplayedByTree(CriteriaOperator criteria){
            var criteriaOperators = new List<CriteriaOperator>();
            var processor = new XpandCriteriaToTreeProcessor(new FilterControlNodesFactory(Tree.Model), criteriaOperators);
            processor.ProcessX(criteria);
            return !criteriaOperators.Any();
        }

        protected override FilterControl CreateTreeControl(){
            return new XpandFilterControl(() => CriteriaOperator.Parse(EditorText),fullTextMembers);
        }
    }
}