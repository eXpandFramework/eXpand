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

namespace Xpand.ExpressApp.Win.Editors {

    public static class FilterControlExtensions{
        public static void RaisePopupMenuShowingX(this IXpandFilterControl filterControl,PopupMenuShowingEventArgs e){
            if (e.MenuType == FilterControlMenuType.Clause && filterControl.ModelClass != null){
                var criteriaOperator = new XpandNodeToCriteriaProcessor().Process(e.CurrentNode);
                var operandProperty = criteriaOperator.GetOperands().OfType<OperandProperty>().First();
                var modelMember =filterControl.ModelClass.AllMembers.Cast<IModelMemberFullTestContains>()
                        .FirstOrDefault(member => member.FullText && member.Name == operandProperty.PropertyName);
                if (modelMember != null){
                    var dxMenuItem = new DXMenuItem(ClauseTypeEnumHelper.GetMenuStringByClauseType(ClauseTypeEnumHelper.FullText),filterControl.OnClauseClick){Tag = ClauseTypeEnumHelper.FullText};
                    e.Menu.Items.Add(dxMenuItem);
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
        IModelClass ModelClass { get; }
        Func<CriteriaOperator> Criteria { get; }
        WinFilterTreeNodeModel CreateModel();
        FilterControlFocusInfo FocusInfo { get; set; }
        void RefreshTreeAfterNodeChange();
        void RaiseFilterChanged(FilterChangedEventArgs args);
        void RaisePopupMenuShowing(PopupMenuShowingEventArgs e);
    }

    public class XpandGridFilterControl : GridFilterControl, IXpandFilterControl {
        private readonly Func<CriteriaOperator> _criteria=() => null;

        public XpandGridFilterControl(Func<CriteriaOperator> criteria, IModelClass modelClass){
            _criteria = criteria;
            ModelClass = modelClass;
        }

        public IModelClass ModelClass { get; private set; }

        public Func<CriteriaOperator> Criteria{
            get { return _criteria; }
        }

        WinFilterTreeNodeModel IXpandFilterControl.CreateModel() {
            return this.CreateXModel();
        }

        protected override WinFilterTreeNodeModel CreateModel() {
            return ((IXpandFilterControl)this).CreateModel();
        }

        FilterControlFocusInfo IXpandFilterControl.FocusInfo {
            get { return FocusInfo; }
            set { FocusInfo = value; }
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

    public class XpandFilterControl : FilterControl,IXpandFilterControl {
        private readonly Func<CriteriaOperator> _criteria= () => null;
        private readonly Func<IModelClass> _modelClass=() => null;

        public XpandFilterControl(Func<CriteriaOperator> criteria, Func<IModelClass> modelClass){
            _criteria = criteria;
            _modelClass = modelClass;
        }

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


        public IModelClass ModelClass{
            get { return _modelClass(); }
        }

        public Func<CriteriaOperator> Criteria{
            get { return _criteria; }
        }

        WinFilterTreeNodeModel IXpandFilterControl.CreateModel(){
            return this.CreateXModel();
        }

        protected override WinFilterTreeNodeModel CreateModel(){
            return ((IXpandFilterControl) this).CreateModel();
        }

        FilterControlFocusInfo IXpandFilterControl.FocusInfo{
            get { return FocusInfo;}
            set { FocusInfo=value; }
        }

        void IXpandFilterControl.RefreshTreeAfterNodeChange(){
            this.CallMethod("RefreshTreeAfterNodeChange");
        }

        void IXpandFilterControl.RaiseFilterChanged(FilterChangedEventArgs args){
            RaiseFilterChanged(args);
        }
    }

    public static class ClauseTypeEnumHelper {
        static readonly int _baseValue = Convert.ToInt32(Enum.GetValues(typeof(ClauseType)).Cast<ClauseType>().Max());

        public static int FullText {
            get { return _baseValue + 1; }
        }

        public static string GetMenuStringByClauseType(int clauseType){
            return clauseType == FullText ? "HasText" : string.Empty;
        }
    }

    public class XpandClauseNode : ClauseNode {
        public XpandClauseNode(FilterTreeNodeModel model) : base(model) { }

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

    public class CriteriaToTreeProcessorBase : CriteriaToTreeProcessor {
        public CriteriaToTreeProcessorBase(INodesFactory nodesFactory, IList<CriteriaOperator> skippedHolder) : base(nodesFactory, skippedHolder){
        }
    }
    public class XpandCriteriaToTreeProcessor : CriteriaToTreeProcessorBase, IClientCriteriaVisitor {
        public XpandCriteriaToTreeProcessor(INodesFactory nodesFactory, IList<CriteriaOperator> skippedHolder) : base(nodesFactory, skippedHolder){
        }

        public object ProcessX(CriteriaOperator criteriaOperator){
            return this.CallMethod("Process", new[]{typeof (CriteriaOperator)}, criteriaOperator);
        }
        object ICriteriaVisitor.Visit(FunctionOperator theOperand){
            var skippedHolder = new List<CriteriaOperator>();
            var visit = ((IClientCriteriaVisitor) new CriteriaToTreeProcessorBase(Factory, skippedHolder)).Visit(theOperand);
            if (skippedHolder.Contains(theOperand)&&theOperand.OperatorType==FunctionOperatorType.Custom){
                skippedHolder.Remove(theOperand);
                return Factory.Create((ClauseType) ClauseTypeEnumHelper.FullText, (OperandProperty) theOperand.Operands[1],new[]{theOperand.Operands[2]});
            }
            return visit;
        }
    }
    public class XpandFilterTreeNodeModel : WinFilterTreeNodeModel, IXpandFilterTreeNodeModel{
        private bool _isUpdating;
        public XpandFilterTreeNodeModel(FilterControl control) : base(control){
            OnNotifyControl+=OnOnNotifyControl;
        }

        private void OnOnNotifyControl(FilterChangedEventArgs info){
            if (info.Action == FilterChangedAction.RebuildWholeTree && info.CurrentNode == null&&!_isUpdating){
                _isUpdating = true;
                BeginUpdate();
                RootNode = null;
                var processor = new XpandCriteriaToTreeProcessor(CreateNodesFactory(),new List<CriteriaOperator>() );
                var criteriaOperator = ((IXpandFilterControl) Control).Criteria();
                var node = (Node)processor.ProcessX(criteriaOperator);
                if (AllowCreateDefaultClause && node == null) {
                    node = CreateCriteriaByDefaultProperty();
                }
                RootNode = node as GroupNode;
                if (RootNode == null) {
                    RootNode = CreateGroupNode(null);
                    if (node != null) {
                        RootNode.AddNode(node);
                    }
                }
                FocusInfo = new FilterControlFocusInfo(RootNode, 0);
                EndUpdate(FilterChangedAction.RebuildWholeTree);
                _isUpdating=false;

            }
        }


        void IXpandFilterTreeNodeModel.CreateTree(CriteriaOperator criteria){
            CreateTree(criteria);
        }


        public override ClauseNode CreateClauseNode() {
            return new XpandClauseNode(this);
        }

        public override CriteriaOperator ToCriteria(INode node) {
            return new XpandNodeToCriteriaProcessor().Process(node);
        }

        public override string GetMenuStringByType(ClauseType type) {
            return Enum.IsDefined(typeof(ClauseType), type) ? base.GetMenuStringByType(type) :
                ClauseTypeEnumHelper.GetMenuStringByClauseType((int)type);
        }
    }

    public class FilterEditorControl : DevExpress.XtraFilterEditor.FilterEditorControl {
        private readonly Func<IModelClass> _modelClass=() => null;

        public FilterEditorControl(Func<IModelClass> modelClass){
            _modelClass = modelClass;
        }

        public new string EditorText {
            get { return base.EditorText; }
            set { base.EditorText = value; }
        }

        protected override bool CanBeDisplayedByTree(CriteriaOperator criteria){
            var criteriaOperators = new List<CriteriaOperator>();
            var processor = new XpandCriteriaToTreeProcessor(new FilterControlNodesFactory(Tree.Model), criteriaOperators);
            processor.ProcessX(criteria);
            return !criteriaOperators.Any();
        }

        protected override FilterControl CreateTreeControl(){
            return new XpandFilterControl(() => CriteriaOperator.Parse(EditorText),_modelClass);
        }
    }
}