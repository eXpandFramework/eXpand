using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using DevExpress.CodeParser;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Services;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Utils.Helpers;
using Attribute = System.Attribute;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {

    public class RichEditModelAdapterController : PropertyEditorControlAdapterController<IModelPropertyEditorRichEdit, IModelRichEdit,RichEditWinPropertyEditor> {

        protected override object GetPropertyEditorControl(RichEditWinPropertyEditor richEditWinPropertyEditor){
            return richEditWinPropertyEditor.Control.RichEditControl;
        }

        protected override Expression<Func<IModelPropertyEditorRichEdit, IModelModelAdapter>> GetControlModel(IModelPropertyEditorRichEdit modelPropertyEditorFilterControl){
            return edit => edit.RichEdit;
        }

        protected override IEnumerable<InterfaceBuilderData> CreateBuilderData(){
            var interfaceBuilderData = new InterfaceBuilderData(typeof(RichEditControl)) {
                Act = info => info.Name != "Undo" && info.DXFilter()
            };
            interfaceBuilderData.ReferenceTypes.AddRange(new[] { typeof(CriteriaOperator), typeof(DocumentCapability) });
            yield return interfaceBuilderData;
        }

        protected override Type GetControlType(){
            return typeof (RichEditControl);
        }
    }


    public interface IModelRichEditControl : IModelNode {

    }

    [ModelAbstractClass]
    public interface IModelPropertyEditorRichEdit : IModelPropertyEditor {
        [ModelBrowsable(typeof(ModelMemberViewItemRichEditVisibilityCalculator))]
        IModelRichEdit RichEdit { get; }
    }

    [ModuleUser(typeof(IRichEditUser))]
    public interface IModelRichEdit : IModelModelAdapter {
        [DefaultValue("rtf")]
        string HighLightExtension { get; set; }
        bool PrintXML { get; set; }
        IModelRichEditControl Control { get; }
        [DefaultValue(true)]
        bool ShowToolBars { get; set; }
        [DefaultValue("Text")]
        string ControlBindingProperty { get; set; }
        IModelRichEditModelAdapters ModelAdapters { get; }
    }

    public interface IRichEditUser{
    }

    [ModelNodesGenerator(typeof(ModelRichEditAdaptersNodeGenerator))]
    public interface IModelRichEditModelAdapters : IModelList<IModelRichEditModelAdapter>, IModelNode {

    }

    public class ModelRichEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRichEdit, IModelRichEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRichEditModelAdapter : IModelCommonModelAdapter<IModelRichEdit> {
    }

    [DomainLogic(typeof(IModelRichEditModelAdapter))]
    public class ModelDashboardViewerModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRichEdit> {
        public static IModelList<IModelRichEdit> Get_ModelAdapters(IModelRichEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }

    [DomainLogic(typeof(IModelRichEdit))]
    public class ModelRichEditDomainLogic  {
        public static string Get_ControlBindingProperty(IModelRichEdit modelRichEdit){
            return GetValue(modelRichEdit, attribute => attribute.ControlBindingProperty) as string;
        }

        public static bool Get_ShowToolBars(IModelRichEdit modelRichEdit) {
            var value = GetValue(modelRichEdit, attribute => attribute.ShowToolBars);
            return value != null && (bool)value;
        }

        public static bool Get_PrintXML(IModelRichEdit modelRichEdit){
            var value = GetValue(modelRichEdit, attribute => attribute.PrintXML);
            return value != null && (bool) value;
        }

        public static string Get_HighLightExtension(IModelRichEdit modelRichEdit){
            return GetValue(modelRichEdit,attribute => attribute.HighLightExtension) as string;
        }

        private static object GetValue(IModelRichEdit modelRichEdit,Func<RichEditPropertyEditorAttribute,object> func ){
            var richEdit =  modelRichEdit.Parent as IModelPropertyEditorRichEdit;
            if (richEdit != null){
                var editorType = richEdit.PropertyEditorType;
                if (typeof (RichEditWinPropertyEditor).IsAssignableFrom(editorType)){
                    var editorAttribute =editorType.GetCustomAttributes(typeof (RichEditPropertyEditorAttribute), false)
                        .Cast<RichEditPropertyEditorAttribute>().First();
                    return func(editorAttribute);
                }
                return "rtf";
            }
            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Class,Inherited = false)]
    public sealed class RichEditPropertyEditorAttribute:Attribute{
        private readonly string _highLightExtension;
        private readonly string _controlBindingProperty;
        private readonly bool _showToolBars;
        private readonly bool _printXML;

        public RichEditPropertyEditorAttribute(string highLightExtension, bool showToolBars, bool printXML, string controlBindingProperty){
            _highLightExtension = highLightExtension;
            _showToolBars = showToolBars;
            _printXML = printXML;
            _controlBindingProperty = controlBindingProperty;
        }

        public bool PrintXML{
            get { return _printXML; }
        }

        public string ControlBindingProperty{
            get { return _controlBindingProperty; }
        }

        public string HighLightExtension{
            get { return _highLightExtension; }
            
        }

        public bool ShowToolBars{
            get { return _showToolBars; }
        }
    }

    public class ModelMemberViewItemRichEditVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return typeof(RichEditWinPropertyEditor).IsAssignableFrom(((IModelMemberViewItem)node).PropertyEditorType);
        }
    }

    [PropertyEditor(typeof(string),EditorAliases.RichEditRftPropertyEditor, false)]
    [RichEditPropertyEditor("rtf",true,false,"RtfText")]
    public class RichEditWinPropertyEditor : WinPropertyEditor, IInplaceEditSupport, IComplexViewItem, IPropertyEditor{

        public RichEditWinPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            ControlBindingProperty = ((IModelPropertyEditorRichEdit) model).RichEdit.ControlBindingProperty;
        }

        public new RichEditContainer Control {
            get { return ((RichEditContainer)base.Control); }
        }

        protected void ApplyMinimalConfiiguration(RichEditContainer richEditContainer) {
            richEditContainer.RichEditControl.Options.AutoCorrect.DetectUrls = false;
            richEditContainer.RichEditControl.Options.AutoCorrect.ReplaceTextAsYouType = false;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.Bookmarks = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.CharacterStyle = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.HeadersFooters = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.Hyperlinks = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.InlinePictures = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.Numbering.Bulleted = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.Numbering.MultiLevel = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.Numbering.Simple = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.ParagraphFormatting = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.Paragraphs = DocumentCapability.Enabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.ParagraphStyle = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.Sections = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.Tables = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Options.DocumentCapabilities.TableStyle = DocumentCapability.Disabled;
            richEditContainer.RichEditControl.Views.SimpleView.AllowDisplayLineNumbers = true;
        }

        protected override void OnAllowEditChanged() {
            base.OnAllowEditChanged();
            if (Control != null) Control.RichEditControl.ReadOnly = !AllowEdit;
        }

        public DevExpress.XtraEditors.Repository.RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemRtfEditEx();
        }

        protected override object CreateControlCore() {
            var richEditContainer = new RichEditContainer();
            if (!((IModelPropertyEditorRichEdit)Model).RichEdit.ShowToolBars)
                richEditContainer.HideToolBars();
            richEditContainer.Dock=DockStyle.Fill;
            richEditContainer.RichEditControl.TextChanged += Editor_RtfTextChanged;
            richEditContainer.RichEditControl.AddService(typeof(ISyntaxHighlightService), new SyntaxHighlightService(this));
            return richEditContainer;
        }

        private void Editor_RtfTextChanged(object sender, EventArgs e) {
            if (!inReadValue && (Control.DataBindings.Count > 0)) {
                OnControlValueChanged();
            }
        }

        public virtual string GetRichEditHighLightExtension() {
            return ((IModelPropertyEditorRichEdit)Model).RichEdit.HighLightExtension;
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            objectSpace.Committing += ObjectSpaceOnCommitting;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            if ((((IModelPropertyEditorRichEdit)Model).RichEdit.PrintXML && !string.IsNullOrEmpty((string)PropertyValue)))
                PropertyValue = PropertyValue.ToString().XMLPrint();
        }

        void IPropertyEditor.SetValue(string value){
            Control.Text = value;
        }
    }

    #region SyntaxHighlightService
    public class SyntaxHighlightService : ISyntaxHighlightService {
        #region Fields
        readonly SyntaxHighlightInfo _syntaxHighlightInfo;
        private readonly RichEditWinPropertyEditor _editor;

        #endregion


        public SyntaxHighlightService(RichEditWinPropertyEditor editor) {
            _editor = editor;
            _syntaxHighlightInfo = new SyntaxHighlightInfo();
        }

        #region ISyntaxHighlightService Members
        public void ForceExecute() {
            Execute();
        }

        public void Execute() {
            TokenCollection tokens = Parse(_editor.ControlValue as string);
            HighlightSyntax(tokens);
        }

        #endregion

        TokenCollection Parse(string code) {
            if (string.IsNullOrEmpty(code))
                return null;
            ITokenCategoryHelper tokenizer = CreateTokenizer();
            if (tokenizer == null)
                return new TokenCollection();
            return tokenizer.GetTokens(code);
        }

        ITokenCategoryHelper CreateTokenizer() {
            return TokenCategoryHelperFactory.CreateHelperForFileExtensions((_editor.GetRichEditHighLightExtension()));
        }

        void HighlightSyntax(TokenCollection tokens) {
            if (tokens == null || tokens.Count == 0)
                return;
            var document = _editor.Control.RichEditControl.Document;
            CharacterProperties cp = document.BeginUpdateCharacters(0, 1);

            var syntaxTokens = new List<SyntaxHighlightToken>(tokens.Count);
            foreach (Token token in tokens) {
                HighlightCategorizedToken((CategorizedToken)token, syntaxTokens);
            }
            document.ApplySyntaxHighlight(syntaxTokens);
            document.EndUpdateCharacters(cp);
        }
        void HighlightCategorizedToken(CategorizedToken token, List<SyntaxHighlightToken> syntaxTokens) {
            SyntaxHighlightProperties highlightProperties = _syntaxHighlightInfo.CalculateTokenCategoryHighlight(token.Category);
            SyntaxHighlightToken syntaxToken = SetTokenColor(token, highlightProperties);
            if (syntaxToken != null)
                syntaxTokens.Add(syntaxToken);
        }
        SyntaxHighlightToken SetTokenColor(Token token, SyntaxHighlightProperties foreColor) {
            if (_editor.Control.RichEditControl.Document.Paragraphs.Count < token.Range.Start.Line)
                return null;
            int paragraphStart = DocumentHelper.GetParagraphStart(_editor.Control.RichEditControl.Document.Paragraphs[token.Range.Start.Line - 1]);
            int tokenStart = paragraphStart + token.Range.Start.Offset - 1;
            if (token.Range.End.Line != token.Range.Start.Line)
                paragraphStart = DocumentHelper.GetParagraphStart(_editor.Control.RichEditControl.Document.Paragraphs[token.Range.End.Line - 1]);

            int tokenEnd = paragraphStart + token.Range.End.Offset - 1;
            System.Diagnostics.Debug.Assert(tokenEnd > tokenStart);
            return new SyntaxHighlightToken(tokenStart, tokenEnd - tokenStart, foreColor);
        }
    }
    #endregion

    #region SyntaxHighlightInfo
    public class SyntaxHighlightInfo {
        readonly Dictionary<TokenCategory, SyntaxHighlightProperties> _properties;

        public SyntaxHighlightInfo() {
            _properties = new Dictionary<TokenCategory, SyntaxHighlightProperties>();
            Reset();
        }
        public void Reset() {
            _properties.Clear();
            Add(TokenCategory.Text, DXColor.Black);
            Add(TokenCategory.Keyword, DXColor.Blue);
            Add(TokenCategory.String, DXColor.Brown);
            Add(TokenCategory.Comment, DXColor.Green);
            Add(TokenCategory.Identifier, DXColor.Black);
            Add(TokenCategory.PreprocessorKeyword, DXColor.Blue);
            Add(TokenCategory.Number, DXColor.Red);
            Add(TokenCategory.Operator, DXColor.Black);
            Add(TokenCategory.Unknown, DXColor.Black);
            Add(TokenCategory.XmlComment, DXColor.Gray);

            Add(TokenCategory.CssComment, DXColor.Green);
            Add(TokenCategory.CssKeyword, DXColor.Brown);
            Add(TokenCategory.CssPropertyName, DXColor.Red);
            Add(TokenCategory.CssPropertyValue, DXColor.Blue);
            Add(TokenCategory.CssSelector, DXColor.Blue);
            Add(TokenCategory.CssStringValue, DXColor.Blue);

            Add(TokenCategory.HtmlAttributeName, DXColor.Red);
            Add(TokenCategory.HtmlAttributeValue, DXColor.Blue);
            Add(TokenCategory.HtmlComment, DXColor.Green);
            Add(TokenCategory.HtmlElementName, DXColor.Brown);
            Add(TokenCategory.HtmlEntity, DXColor.Gray);
            Add(TokenCategory.HtmlOperator, DXColor.Black);
            Add(TokenCategory.HtmlServerSideScript, DXColor.Black);
            Add(TokenCategory.HtmlString, DXColor.Blue);
            Add(TokenCategory.HtmlTagDelimiter, DXColor.Blue);
        }
        void Add(TokenCategory category, Color foreColor) {
            var item = new SyntaxHighlightProperties { ForeColor = foreColor };
            _properties.Add(category, item);
        }

        public SyntaxHighlightProperties CalculateTokenCategoryHighlight(TokenCategory category) {
            SyntaxHighlightProperties result;
            return _properties.TryGetValue(category, out result) ? result : _properties[TokenCategory.Text];
        }
    }
    #endregion

    #region CustomRichEditCommandFactoryService
    public class CustomRichEditCommandFactoryService : IRichEditCommandFactoryService {
        readonly IRichEditCommandFactoryService _service;

        public CustomRichEditCommandFactoryService(IRichEditCommandFactoryService service) {
            Guard.ArgumentNotNull(service, "service");
            _service = service;
        }

        #region IRichEditCommandFactoryService Members
        RichEditCommand IRichEditCommandFactoryService.CreateCommand(RichEditCommandId id) {
            if (id.Equals(RichEditCommandId.InsertColumnBreak) || id.Equals(RichEditCommandId.InsertLineBreak) || id.Equals(RichEditCommandId.InsertPageBreak))
                return _service.CreateCommand(RichEditCommandId.InsertParagraph);
            return _service.CreateCommand(id);
        }
        #endregion
    }
    #endregion

    public static class SourceCodeDocumentFormat {
        public static readonly DocumentFormat Id = new DocumentFormat(1325);
    }
    public class SourcesCodeDocumentImporter : PlainTextDocumentImporter {
        internal static readonly FileDialogFilter DialogFilter = new FileDialogFilter("Source Files", new[] { "cs", "vb", "html", "htm", "js", "xml", "css" });
        public override FileDialogFilter Filter { get { return DialogFilter; } }
        public override DocumentFormat Format { get { return SourceCodeDocumentFormat.Id; } }
    }
    public class SourcesCodeDocumentExporter : PlainTextDocumentExporter {
        public override FileDialogFilter Filter { get { return SourcesCodeDocumentImporter.DialogFilter; } }
        public override DocumentFormat Format { get { return SourceCodeDocumentFormat.Id; } }
    }
}
