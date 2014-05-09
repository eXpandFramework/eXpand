using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.HtmlPropertyEditor.Web;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Web.ASPxHtmlEditor;
using Xpand.ExpressApp.HtmlPropertyEditor.Web.DialogForms;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web.Model {
    
    [ModelAbstractClass]
    public interface IModelPropertyHtmlEditor : IModelPropertyEditor {
        [ModelBrowsable(typeof(HtmlEditorVisibilityCalculator))]
        IModelHtmlEditor HtmlEditor { get; }
        [ModelBrowsable(typeof(HtmlEditorVisibilityCalculator))]
        IModelHtmlEditorModelAdapters HtmlEditorModelAdapters { get; }
    }

    public class HtmlEditorVisibilityCalculator : EditorTypeVisibilityCalculator<ASPxHtmlPropertyEditor> {
         
    }

    [ModelNodesGenerator(typeof(ModelHtmlEditorAdaptersNodeGenerator))]
    public interface IModelHtmlEditorModelAdapters : IModelList<IModelHtmlEditorModelAdapter>, IModelNode {

    }

    public interface IModelHtmlEditorModelAdapter : IModelCommonModelAdapter<IModelHtmlEditor> {

    }

    [DomainLogic(typeof(IModelHtmlEditorModelAdapter))]
    public class ModelHtmlEditorModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelHtmlEditor> {
        public static IModelList<IModelHtmlEditor> Get_ModelAdapters(IModelHtmlEditorModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }

    public class ModelHtmlEditorAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelHtmlEditor, IModelHtmlEditorModelAdapter> {

    }

    public interface IModelHtmlEditor : IModelModelAdapter {
        IModelHtmlEditorToolBars ToolBars { get; }
        IModelHtmlEditorShortcuts Shortcuts { get; }
        IModelHtmlEditorCustomDialogs CustomDialogs { get; }
    }
    [ModelNodesGenerator(typeof(ModelHtmlEditorCustomDialogNodesGenerator))]
    public interface IModelHtmlEditorCustomDialogs : IModelList<IModelHtmlEditorCustomDialog>,IModelNode {
    }

    public interface IModelHtmlEditorCustomDialog:IModelNodeEnabled {
    }

    public class ModelHtmlEditorCustomDialogNodesGenerator:ModelNodesGeneratorBase {
        static readonly ASPxHtmlEditor _htmlEditor;

        static ModelHtmlEditorCustomDialogNodesGenerator() {
            _htmlEditor = new ASPxHtmlEditor();
            Type customDialogTemplateType = typeof(HtmlEditorCustomDialogTemlate);
            var customDialogTemplateTypeName = customDialogTemplateType.Name;
            var customDialog = new HtmlEditorCustomDialog(ModelHtmlEditorToolBarNodesGenerator.InsertFileCommand,
                                                          "Insert File...",
                                                          "~/UserControls/" + customDialogTemplateTypeName + ".ascx");
            _htmlEditor.CustomDialogs.Add(customDialog);
            customDialog.OkButtonText = "Insert";
            if (Debugger.IsAttached) {
                
                var path = UserContolPath(customDialogTemplateTypeName);
                var contents = customDialogTemplateType.GetResourceString(customDialogTemplateTypeName + ".ascx");
                File.WriteAllText(path, contents);

                const string resourceName =ModelHtmlEditorToolBarNodesGenerator.InsertFileCommand+  ".png";
                var filePath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Images\" + resourceName);
                customDialogTemplateType.WriteResourceToFile(resourceName, filePath);
            }
            
        }

        static string UserContolPath(string resourceName) {
            string path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                       @"UserControls\" + resourceName + ".ascx");
            string directoryName = Path.GetDirectoryName(path) + "";
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }
            return path;
        }

        protected override void GenerateNodesCore(ModelNode node) {
            var modelHtmlEditorCustomDialogs = ((IModelHtmlEditorCustomDialogs) node);
            foreach (var editorCustomDialog in _htmlEditor.CustomDialogs.ToList()) {
                var modelHtmlEditorCustomDialog = modelHtmlEditorCustomDialogs.AddNode<IModelHtmlEditorCustomDialog>(editorCustomDialog.Name);
                modelHtmlEditorCustomDialog.SetValue("Name", editorCustomDialog.Name);
                modelHtmlEditorCustomDialog.SetValue("Caption", editorCustomDialog.Caption);
                modelHtmlEditorCustomDialog.SetValue("FormPath", editorCustomDialog.FormPath);
                modelHtmlEditorCustomDialog.SetValue("OkButtonText", editorCustomDialog.OkButtonText);
            }
        }
    }

    [ModelNodesGenerator(typeof(ModelHtmlEditorToolBarNodesGenerator))]
    public interface IModelHtmlEditorToolBars:IModelList<IModelHtmlEditorToolBar>,IModelNode {
    }

    public interface IModelHtmlEditorToolBar:IModelNodeEnabled {
        IModelHtmlEditorToolBarItems Items { get; }
    }

    public interface IModelHtmlEditorToolBarItem:IModelNodeEnabled {

    }

    public interface IModelToolbarCustomDialogButton : IModelHtmlEditorToolBarItem {
    }

    public interface IModelHtmlEditorToolBarItems : IModelList<IModelHtmlEditorToolBarItem>,IModelNode {
    }

    public interface IModelHtmlEditorShortcuts : IModelList<IModelHtmlEditorShortcut>,IModelNode {
    }

    public interface IModelHtmlEditorShortcut:IModelNodeEnabled{
    }

    public class ModelHtmlEditorToolBarNodesGenerator : ModelNodesGeneratorBase {
        public const string InsertFileCommand = "insertfile";
        static readonly IEnumerable<HtmlEditorToolbar> _htmlEditorToolbars;

        static ModelHtmlEditorToolBarNodesGenerator() {
            var htmlEditor = AsPxHtmlEditor();
            _htmlEditorToolbars = htmlEditor.Toolbars;
        }

        public static ASPxHtmlEditor AsPxHtmlEditor() {
            var htmlEditor = new ASPxHtmlEditor();
            htmlEditor.CreateDefaultToolbars(true);
            
            CreateInsertFileCommand(htmlEditor);
            return htmlEditor;
        }

        static void CreateInsertFileCommand(ASPxHtmlEditor htmlEditor) {
            var editorToolbar =htmlEditor.Toolbars.First(toolbar => toolbar.Items.Any(item => item.CommandName == "insertimagedialog"));
            var editorToolbarItem = editorToolbar.Items.First(item => item.CommandName == "insertimagedialog");
            var index = editorToolbarItem.Index;
            var customToolbarButton = new ToolbarCustomDialogButton(InsertFileCommand, "Insert File...") {
                ViewStyle = ViewStyle.Image
            };
            customToolbarButton.Image.Url = "~/Images/" + InsertFileCommand + ".png";
            editorToolbar.Items.Insert(index, customToolbarButton);
        }

        protected override void GenerateNodesCore(ModelNode node) {
            var editorToolBars = (IModelHtmlEditorToolBars) node;
            CreateModelHtmlEditorToolBar(editorToolBars);
        }

        void CreateModelHtmlEditorToolBar(IModelHtmlEditorToolBars editorToolBars) {
            foreach (var toolbar in _htmlEditorToolbars) {
                var modelHtmlEditorToolBar = editorToolBars.AddNode<IModelHtmlEditorToolBar>(toolbar.Name);
                CreateModelToolbarCustomDialogButton(toolbar, modelHtmlEditorToolBar);
            }
        }

        void CreateModelToolbarCustomDialogButton(HtmlEditorToolbar toolbar,IModelHtmlEditorToolBar modelHtmlEditorToolBar) {
            foreach (var editorToolbarItem in toolbar.Items.ToList()) {
                var toolbarCustomDialogButton = editorToolbarItem as ToolbarCustomDialogButton;
                if (toolbarCustomDialogButton!=null) {
                    var customDialogButton = modelHtmlEditorToolBar.Items.AddNode<IModelToolbarCustomDialogButton>(toolbarCustomDialogButton.Name);
                    customDialogButton.SetValue("Name", toolbarCustomDialogButton.Name);
                    customDialogButton.SetValue("ToolTip", editorToolbarItem.ToolTip);
                    customDialogButton.SetValue("ViewStyle", editorToolbarItem.ViewStyle);
                    var modelNode = customDialogButton.GetNodeByPath("Image");
                    modelNode.SetValue("Url", editorToolbarItem.Image.Url);
                }
                else
                    modelHtmlEditorToolBar.Items.AddNode<IModelHtmlEditorToolBarItem>(editorToolbarItem.CommandName);
            }
        }
    }
}
