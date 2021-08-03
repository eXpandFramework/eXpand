using System;
using DevExpress.ExpressApp.Web;
using Xpand.Extensions.StringExtensions;

namespace Xpand.Persistent.Base.General.Web.SyntaxHighlight{
    public class AceEditor{
        public void Configure(string editorID, IModelSyntaxHighLight syntaxHighLight, bool allowEdit,string text){
            var script = @"var timer = setInterval(function(){
                                clearInterval(timer);
                                if (!document.getElementById('AceEditor" + editorID + @"')) return;
                                var editor = ace.edit('AceEditor" + editorID + @"');
                                editor.getSession().setMode('ace/mode/" + syntaxHighLight.Mode + @"');" +
                                ConfigureEditor(syntaxHighLight, allowEdit) +
                                BindEditorToControl(allowEdit, editorID) +
                                AssignTheme(syntaxHighLight) +
                                @"editor.resize();
                            },1);";
            RegisterThemeScript(syntaxHighLight);
            WebWindow.CurrentRequestWindow.RegisterStartupScript("AceEditor" +editorID+ "InitScript", script);
            RegisterJsLibScript();
        }


        private string ConfigureEditor(IModelSyntaxHighLight syntaxHighLight, bool allowEdit) {
            return @"editor.setOptions({" 
                   + ConfigureEditorCore(syntaxHighLight, allowEdit)
                   + ConfigureRenderer(syntaxHighLight)
                   + ConfigureMouse(syntaxHighLight)
                   + ConfigureSession(syntaxHighLight)
                   + @",});";
        }

        private string ConfigureSession(IModelSyntaxHighLight syntaxHighLight) {
            return GetOptionAssigment(syntaxHighLight.FirstLineNumber, "firstLineNumber") + @"
                            " + GetOptionAssigment(syntaxHighLight.TabSize, "tabSize") + @"
                            overwrite: " + syntaxHighLight.Overwrite.ToString().ToLower() + @",
                            newLineMode: " + syntaxHighLight.NewLineMode.ToString().ToLower() + @",
                            useWorker: " + syntaxHighLight.UseWorker.ToString().ToLower() + @",
                            useSoftTabs: " + syntaxHighLight.UseSoftTabs.ToString().ToLower() + @",
                            wrap: " + syntaxHighLight.ToggleWordWrap.ToString().ToLower();
        }

        private string ConfigureMouse(IModelSyntaxHighLight syntaxHighLight) {
            return GetOptionAssigment(syntaxHighLight.ScrollSpeed, "scrollSpeed") + @"
                            " + GetOptionAssigment(syntaxHighLight.DragDelay, "dragDelay") + @"
                            tooltipFollowsMouse: " + syntaxHighLight.TooltipFollowsMouse.ToString().ToLower() + @",
                            dragEnabled: " + syntaxHighLight.DragEnabled.ToString().ToLower() + @",
                            " + GetOptionAssigment(syntaxHighLight.FocusTimeout, "focusTimeout");
        }

        private string ConfigureRenderer(IModelSyntaxHighLight syntaxHighLight) {
            return @"vScrollBarAlwaysVisible: " + syntaxHighLight.VScrollBarAlwaysVisible.ToString().ToLower() + @",
                            hScrollBarAlwaysVisible: " + syntaxHighLight.HScrollBarAlwaysVisible.ToString().ToLower() + @",
                            highlightGutterLine: " + syntaxHighLight.HighlightGutterLine.ToString().ToLower() + @",
                            animatedScroll: " + syntaxHighLight.AnimatedScroll.ToString().ToLower() + @",
                            showInvisibles: " + syntaxHighLight.ShowInvisibles.ToString().ToLower() + @",
                            showPrintMargin: " + syntaxHighLight.ShowPrintMargin.ToString().ToLower() + @",
                            printMarginColumn: " + syntaxHighLight.PrintMarginColumn.ToString().ToLower() + @",
                            fadeFoldWidgets: " + syntaxHighLight.FadeFoldWidgets.ToString().ToLower() + @",
                            showFoldWidgets: " + syntaxHighLight.ShowFoldWidgets.ToString().ToLower() + @",
                            showLineNumbers: " + syntaxHighLight.ShowLineNumbers.ToString().ToLower() + @",
                            showGutter: " + syntaxHighLight.ShowGutter.ToString().ToLower() + @",
                            displayIndentGuides: " + syntaxHighLight.DisplayIndentGuides.ToString().ToLower() + @",
                            scrollPastEnd: " + syntaxHighLight.ScrollPastEnd.ToString().ToLower() + @",
                            scrollPastEnd: " + syntaxHighLight.ScrollPastEnd.ToString().ToLower() + @",
                            fixedWidthGutter: " + syntaxHighLight.FixedWidthGutter.ToString().ToLower() + @",
                            " + GetOptionAssigment(syntaxHighLight.FontSize, "fontSize") + @"
                            " + GetOptionAssigment(syntaxHighLight.MaxLines, "maxLines") + @"
                            " + GetOptionAssigment(syntaxHighLight.MinLines, "minLines");
        }

        private object ConfigureEditorCore(IModelSyntaxHighLight syntaxHighLight, bool allowEdit) {
            return @"highlightActiveLine: " + syntaxHighLight.HighlightActiveLine.ToString().ToLower() + @",
                            highlightSelectedWord: " + syntaxHighLight.HighlightSelectedWord.ToString().ToLower() + @",
                            cursorStyle: " + syntaxHighLight.CursorStyle.ToString().ToLower() + @",
                            behavioursEnabled: " + syntaxHighLight.BehavioursEnabled.ToString().ToLower() + @",
                            wrapBehavioursEnabled: " + syntaxHighLight.WrapBehavioursEnabled.ToString().ToLower() + @",
                            autoScrollEditorIntoView: " + syntaxHighLight.AutoScrollEditorIntoView.ToString().ToLower() + @",
                            readOnly: " + (syntaxHighLight.ReadOnly ?? !allowEdit).ToString().ToLower() + @",";
        }


        private string AssignTheme(IModelSyntaxHighLight syntaxHighLight) {
            return !String.IsNullOrEmpty(syntaxHighLight.Theme) ? "editor.setTheme('ace/theme/" + syntaxHighLight.Theme + "');" : null;
        }

        private string GetOptionAssigment(int value, string optionName) {
            return value > 0 ? optionName + ":" + value + "," : null;
        }

        private string GetOptionAssigment(string value, string optionName) {
            return !String.IsNullOrEmpty(value) ? optionName + @": " + value + @"," : null;
        }

        private string BindEditorToControl(bool allowEdit, string editorID){
            if (allowEdit)
                return @"
                    var editorElement = document.getElementById('" + editorID + @"');
                    var textArea = editorElement.getElementsByTagName('textarea')[0];
                    editor.getSession().on('change', function(){
                        var editorText = editor.getSession().getValue();
                        textArea.value=editorText;
                    });
                    textArea.style.display = 'none';
                    ";
            return @"
                    var editorElement = document.getElementById('" + editorID + @"');
                    var span = editorElement.getElementsByTagName('span')[0];
                    span.style.display = 'none';
                    ";
        }

        public string CreateContainer(string editorId,string text){
            return $@"<div id='AceEditor{editorId}'>{(text+"").XmlEncode()}</div>";
        }


        private void RegisterThemeScript(IModelSyntaxHighLight syntaxHighLight){
            if (!string.IsNullOrWhiteSpace(syntaxHighLight.Theme))
                WebWindow.CurrentRequestWindow.RegisterClientScriptInclude("AceThemeJs" + syntaxHighLight.Theme, "https://cdnjs.cloudflare.com/ajax/libs/ace/1.2.3/theme-" + syntaxHighLight.Theme + ".js");
        }

        public static void RegisterJsLibScript(){
            WebWindow.CurrentRequestWindow.RegisterClientScriptInclude("Ace", "https://cdnjs.cloudflare.com/ajax/libs/ace/1.2.3/ace.js");
        }
    }
}