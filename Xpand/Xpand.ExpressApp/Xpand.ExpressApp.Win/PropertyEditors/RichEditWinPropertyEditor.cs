using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.CodeParser;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Services;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.PropertyEditors {

    public interface IModelMemberViewItemRichEdit:IModelNode {
//        [ModelBrowsable(typeof(ModelMemberViewItemRichEditVisibilityCalculator))]
        IModelRichEdit RichEdit { get; }
    }

    public interface IModelRichEdit:IModelNode{
        [DefaultValue("rtf")]
        string HighLightExtension { get; set; }
        [DefaultValue(true)]
        bool PrintXML { get; set; }
    }

    public class ModelMemberViewItemRichEditVisibilityCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName){
            return ((IModelMemberViewItem) node).PropertyEditorType == typeof (RichEditWinPropertyEditor);
        }
    }

    [PropertyEditor(typeof(string),  false)]
    public class RichEditWinPropertyEditor : WinPropertyEditor, IInplaceEditSupport,IComplexViewItem {

        public RichEditWinPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            ControlBindingProperty = "Text";
        }

        public new RichEditControl Control {
            get { return ((RichEditControl)base.Control); }
        }

        public override bool IsCaptionVisible {
            get { return false; }
        }

        protected override void OnAllowEditChanged(){
            base.OnAllowEditChanged();
            if (Control != null) Control.ReadOnly = !AllowEdit;
        }

        public DevExpress.XtraEditors.Repository.RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemRtfEditEx();
        }

        protected override object CreateControlCore() {
            var richEditControl = new RichEditControl {
                ReadOnly = !AllowEdit,
                ActiveViewType = RichEditViewType.Draft,
                Dock = DockStyle.Fill,
                LayoutUnit = DocumentLayoutUnit.Pixel,
                Location = new Point(2, 28),
                Name = "richEditControl"
            };
            richEditControl.Options.AutoCorrect.DetectUrls = false;
            richEditControl.Options.AutoCorrect.ReplaceTextAsYouType = false;
            richEditControl.Options.DocumentCapabilities.Bookmarks = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.CharacterStyle = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.HeadersFooters = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.Hyperlinks = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.InlinePictures = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.Numbering.Bulleted = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.Numbering.MultiLevel = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.Numbering.Simple = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.ParagraphFormatting = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.Paragraphs = DocumentCapability.Enabled;
            richEditControl.Options.DocumentCapabilities.ParagraphStyle = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.Sections = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.Tables = DocumentCapability.Disabled;
            richEditControl.Options.DocumentCapabilities.TableStyle = DocumentCapability.Disabled;
            richEditControl.Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
            richEditControl.Text = @"richEditControl1";
            richEditControl.Views.DraftView.AllowDisplayLineNumbers = true;
            richEditControl.Views.DraftView.Padding = new Padding(70, 4, 0, 0);
            richEditControl.InitializeDocument += richEditControl_InitializeDocument;


            richEditControl.AddService(typeof(ISyntaxHighlightService), new SyntaxHighlightService(this));
            return richEditControl;
        }

        void richEditControl_InitializeDocument(object sender, EventArgs e) {
            var document = Control.Document;
            document.BeginUpdate();
            try {
                document.DefaultCharacterProperties.FontName = "Courier New";
                document.DefaultCharacterProperties.FontSize = 10;
                document.Sections[0].Page.Width = Units.InchesToDocumentsF(100);

                SizeF tabSize = Control.MeasureSingleLineString("    ", document.DefaultCharacterProperties);
                TabInfoCollection tabs = document.Paragraphs[0].BeginUpdateTabs(true);
                try {
                    for (int i = 1; i <= 30; i++) {
                        var tab = new TabInfo { Position = i * tabSize.Width };
                        tabs.Add(tab);
                    }
                }
                finally {
                    document.Paragraphs[0].EndUpdateTabs(tabs);
                }
            }
            finally {
                document.EndUpdate();
            }
        }

        public virtual string GetRichEditHighLightExtension(){
            return ((IModelMemberViewItemRichEdit) Model).RichEdit.HighLightExtension;
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application){
            objectSpace.Committing+=ObjectSpaceOnCommitting;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            if ((((IModelMemberViewItemRichEdit) Model).RichEdit.PrintXML&&!string.IsNullOrEmpty((string) PropertyValue)))
                PropertyValue = PropertyValue.ToString().XMLPrint();
        }
    }

    #region SyntaxHighlightService
    public class SyntaxHighlightService : ISyntaxHighlightService {
        #region Fields
        readonly SyntaxHighlightInfo _syntaxHighlightInfo;
        private readonly RichEditWinPropertyEditor _editor;

        #endregion


        public SyntaxHighlightService(RichEditWinPropertyEditor editor){
            _editor = editor;
            _syntaxHighlightInfo = new SyntaxHighlightInfo();
        }

        #region ISyntaxHighlightService Members
        public void ForceExecute() {
            Execute();
        }

        public void Execute() {
            TokenCollection tokens = Parse(_editor.Control.Text);
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

        ITokenCategoryHelper CreateTokenizer(){
            return TokenCategoryHelperFactory.CreateHelperForFileExtensions(( _editor.GetRichEditHighLightExtension()));
        }

        void HighlightSyntax(TokenCollection tokens) {
            if (tokens == null || tokens.Count == 0)
                return;
            var document = _editor.Control.Document;
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
            if (_editor.Control.Document.Paragraphs.Count < token.Range.Start.Line)
                return null;
            int paragraphStart = DocumentHelper.GetParagraphStart(_editor.Control.Document.Paragraphs[token.Range.Start.Line - 1]);
            int tokenStart = paragraphStart + token.Range.Start.Offset - 1;
            if (token.Range.End.Line != token.Range.Start.Line)
                paragraphStart = DocumentHelper.GetParagraphStart(_editor.Control.Document.Paragraphs[token.Range.End.Line - 1]);

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
            var item = new SyntaxHighlightProperties{ForeColor = foreColor};
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
