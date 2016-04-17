using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.General.Web.SyntaxHighlight {
    public class SyntaxHighlightController:ViewController<DetailView>,IModelExtender{
        private WebPropertyEditor[] _propertyEditors;

        protected override void OnActivated(){
            base.OnActivated();
            _propertyEditors = View.GetItems<WebPropertyEditor>().Where(editor => ((IModelPropertyEditorSyntaxHighlight)editor.Model).SyntaxHighlight.Mode != null).ToArray();
            if (_propertyEditors.Any()){
                WebWindow.CurrentRequestWindow.PagePreRender += CurrentRequestWindowOnPagePreRender;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            WebWindow.CurrentRequestWindow.PagePreRender -= CurrentRequestWindowOnPagePreRender;
        }

        private void CurrentRequestWindowOnPagePreRender(object sender, EventArgs eventArgs){
            if (View != null){
                var currentRequestWindow = WebWindow.CurrentRequestWindow;
                foreach (var propertyEditor in _propertyEditors.Where(editor => editor.Control!=null)){    
                    var actualControl = ((WebControl) propertyEditor.Control);
                    actualControl.Style.Add(HtmlTextWriterStyle.Display, "none");
                    var xmlEncode = (propertyEditor.PropertyValue+"").XMLEncode();
                    var syntaxHighLight = ((IModelPropertyEditorSyntaxHighlight) propertyEditor.Model).SyntaxHighlight;
                    var script = EditorInitScript(syntaxHighLight, xmlEncode, propertyEditor);
                    currentRequestWindow.RegisterStartupScript("test", script);
                    actualControl.Parent.Controls.Add(new LiteralControl{Text = @"
                            <div id='AceEditor" +actualControl.ClientID+ @"'>" +xmlEncode + @"</div>
                    "});
                    if (!string.IsNullOrWhiteSpace(syntaxHighLight.Theme))
                        currentRequestWindow.RegisterClientScriptInclude("AceThemeJs"+propertyEditor.Id, "https://cdnjs.cloudflare.com/ajax/libs/ace/1.2.3/theme-" + syntaxHighLight.Theme+ ".js");
                }

                currentRequestWindow.RegisterClientScriptInclude("Ace", "https://cdnjs.cloudflare.com/ajax/libs/ace/1.2.3/ace.js");
                
            }
        }

        private string EditorInitScript(IModelSyntaxHighLight syntaxHighLight, string xmlEncode,
            WebPropertyEditor propertyEditor){
            var script = @"
                          var editor = ace.edit('AceEditor" + ((WebControl) propertyEditor.Control).ClientID + @"');" +
                         "editor.getSession().setMode('ace/mode/" + syntaxHighLight.Mode + "');" +
                         "editor.setOptions({maxLines: " + xmlEncode.Split(Environment.NewLine.ToCharArray()).Length + @"})" +
                         ConfigureEditor(syntaxHighLight) + BindEditorToControl(propertyEditor) +
                         AssignTheme(syntaxHighLight.Theme) +
                         "editor.resize();";
            return script;
        }

        private string ConfigureEditor(IModelSyntaxHighLight syntaxHighLight){
            return @"
                          editor.setOptions({
                            " + ConfigureEditorCore(syntaxHighLight)
                              + ConfigureRenderer(syntaxHighLight)
                              + ConfigureMouse(syntaxHighLight)
                              + ConfigureSession(syntaxHighLight)
                              + @",
                         });
                        ";
        }

        private string ConfigureSession(IModelSyntaxHighLight syntaxHighLight){
            return GetOptionAssigment(syntaxHighLight.FirstLineNumber, "firstLineNumber") + @"
                            " + GetOptionAssigment(syntaxHighLight.TabSize, "tabSize") + @"
                            overwrite: " + syntaxHighLight.Overwrite.ToString().ToLower() + @",
                            newLineMode: " + syntaxHighLight.NewLineMode.ToString().ToLower() + @",
                            useWorker: " + syntaxHighLight.UseWorker.ToString().ToLower() + @",
                            useSoftTabs: " + syntaxHighLight.UseSoftTabs.ToString().ToLower() + @",
                            wrap: " + syntaxHighLight.ToggleWordWrap.ToString().ToLower();
        }

        private string ConfigureMouse(IModelSyntaxHighLight syntaxHighLight){
            return GetOptionAssigment(syntaxHighLight.ScrollSpeed, "scrollSpeed") + @"
                            " + GetOptionAssigment(syntaxHighLight.DragDelay, "dragDelay") + @"
                            tooltipFollowsMouse: " + syntaxHighLight.TooltipFollowsMouse.ToString().ToLower() + @",
                            dragEnabled: " + syntaxHighLight.DragEnabled.ToString().ToLower() + @",
                            " + GetOptionAssigment(syntaxHighLight.FocusTimeout, "focusTimeout");
        }

        private string ConfigureRenderer(IModelSyntaxHighLight syntaxHighLight){
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
                            " + GetOptionAssigment(syntaxHighLight.MinLines, "value");
        }

        private object ConfigureEditorCore(IModelSyntaxHighLight syntaxHighLight){
            return @"highlightActiveLine: " + syntaxHighLight.HighlightActiveLine.ToString().ToLower() + @",
                            highlightSelectedWord: " + syntaxHighLight.HighlightSelectedWord.ToString().ToLower() + @",
                            cursorStyle: " + syntaxHighLight.CursorStyle.ToString().ToLower() + @",
                            behavioursEnabled: " + syntaxHighLight.BehavioursEnabled.ToString().ToLower() + @",
                            wrapBehavioursEnabled: " + syntaxHighLight.WrapBehavioursEnabled.ToString().ToLower() + @",
                            autoScrollEditorIntoView: " + syntaxHighLight.AutoScrollEditorIntoView.ToString().ToLower() + @",
                            readOnly: " + (syntaxHighLight.ReadOnly ?? View.ViewEditMode == ViewEditMode.View).ToString().ToLower() + @",";
        }


        private string AssignTheme(string theme){
            return !string.IsNullOrEmpty(theme) ? "editor.setTheme('ace/theme/" + theme + "');" : null;
        }

        private string GetOptionAssigment(int value, string optionName){
            return value > 0 ? optionName + ":" + value + "," : null;
        }

        private  string GetOptionAssigment(string value, string optionName){
            return !string.IsNullOrEmpty(value) ? optionName + @": " + value + @"," : null;
        }

        private string BindEditorToControl(WebPropertyEditor propertyEditor){
            if (View.ViewEditMode == ViewEditMode.Edit&&propertyEditor.AllowEdit)
                return @"
                    editor.getSession().on('change', function () {
                            var textArea = document.getElementById('" + ((WebControl) propertyEditor.Control).ClientID + @"').getElementsByTagName('textarea')[0];
                            var value=editor.getSession().getValue();
                            textArea.value=value;
                         });";
            return null;
        }


        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelPropertyEditor,IModelPropertyEditorSyntaxHighlight>();
        }
    }
}
