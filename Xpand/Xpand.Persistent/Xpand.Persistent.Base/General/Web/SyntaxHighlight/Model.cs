using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Web.SyntaxHighlight {
    public enum CursorStyle {
        Ace,
        Slim,
        Smooth,
        Wide
    }

    public enum SelectionStyle {
        Line,
        Text
    }

    [ModelAbstractClass]
    public interface IModelPropertyEditorSyntaxHighlight:IModelPropertyEditor {
        [ModelBrowsable(typeof(SyntaxHighlightVisibilityCalculator))]
        IModelSyntaxHighLight SyntaxHighlight { get; }
    }

    public class SyntaxHighlightVisibilityCalculator:IModelIsVisible{
        public bool IsVisible(IModelNode node, string propertyName){
            return ((IModelPropertyEditorSyntaxHighlight) node).RowCount > 0;
        }
    }

    public interface IModelSyntaxHighLight : IModelNode {
        [Category("Editor")]
        bool ToggleWordWrap { get; set; }
        [Category("Editor")]
        bool LineHighlight { get; set; }
        [Category("Editor")]
        bool ShowPrintMargin { get; set; }
        [Category("Editor")]
        bool? ReadOnly { get; set; }
        [Category("Editor")]
        SelectionStyle SelectionStyle { get; set; }
        [Category("Editor")]
        [DefaultValue(true)]
        bool HighlightActiveLine { get; set; }
        [Category("Editor")]
        [DefaultValue(true)]
        bool HighlightSelectedWord { get; set; }
        [Category("Editor")]
        CursorStyle CursorStyle { get; set; }
        [Category("Editor")]
        bool WrapBehavioursEnabled { get; set; }
        [Category("Editor")]
        bool BehavioursEnabled { get; set; }
        [Category("Editor")]
        bool AutoScrollEditorIntoView { get; set; }
        [Category("Renderer")]
        bool VScrollBarAlwaysVisible { get; set; }
        [Category("Renderer")]
        bool HScrollBarAlwaysVisible { get; set; }
        [Category("Renderer")]
        [DefaultValue(true)]
        bool HighlightGutterLine { get; set; }
        [Category("Renderer")]
        bool AnimatedScroll { get; set; }
        [Category("Renderer")]
        [DefaultValue(true)]
        bool ShowInvisibles { get; set; }
        [Category("Renderer")]
        bool PrintMarginColumn { get; set; }
        [Category("Renderer")]
        bool FadeFoldWidgets { get; set; }
        [Category("Renderer")]
        [DefaultValue(true)]
        bool ShowFoldWidgets { get; set; }
        [Category("Renderer")]
        [DefaultValue(true)]
        bool ShowLineNumbers { get; set; }
        [Category("Renderer")]
        [DefaultValue(true)]
        bool ShowGutter { get; set; }
        [Category("Renderer")]
        bool DisplayIndentGuides { get; set; }
        [Category("Renderer")]
        bool ScrollPastEnd { get; set; }
        [Category("Renderer")]
        bool FixedWidthGutter { get; set; }
        [Category("Renderer")]
        [Description("number or css font-size string")]
        string FontSize { get; set; }
        [Category("Renderer")]
        int MinLines { get; set; }
        [Category("Renderer")]
        [TypeConverter(typeof(SyntaxHighLightThemesProvider))]
        string Theme { get; set; }
        [Category("Mouse")]
        int ScrollSpeed { get; set; }
        [Category("Mouse")]
        int DragDelay { get; set; }
        [Category("Mouse")]
        int FocusTimeout { get; set; }
        [Category("Mouse")]
        bool TooltipFollowsMouse { get; set; }
        [Category("Mouse")]
        bool DragEnabled { get; set; }
        [Category("Session")]
        [TypeConverter(typeof(SyntaxHighLightModeProvider))]
        string Mode { get; set; }
        [Category("Session")]
        int FirstLineNumber { get; set; }
        [Category("Session")]
        int TabSize { get; set; }
        [Category("Session")]
        bool FoldStyle { get; set; }
        [Category("Session")]
        bool Overwrite { get; set; }
        [Category("Session")]
        bool NewLineMode { get; set; }
        [Category("Session")]
        bool UseWorker { get; set; }
        [Category("Session")]
        bool UseSoftTabs { get; set; }
        [Category("Extensions")]
        bool UseElasticTabstops { get; set; }
        [Category("Extensions")]
        bool EnableMultiselect { get; set; }
        [Category("Extensions")]
        bool EnableEmmet { get; set; }
        [Category("Extensions")]
        bool EnableBasicAutocompletion { get; set; }
        [Category("Extensions")]
        bool EnableSnippets { get; set; }
        [Category("Extensions")]
        bool Spellcheck { get; set; }
    }
}
