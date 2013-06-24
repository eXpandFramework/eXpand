using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Web.ASPxHtmlEditor;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web.Model {
    
    [ModelAbstractClass]
    public interface IModelPropertyHtmlEditor : IModelPropertyEditor {
        IModelHtmlEditor HtmlEditor { get; }
    }

    public interface IModelHtmlEditor:IModelNodeEnabled {
        IModelHtmlEditorToolBars ToolBars { get; }
    }
    [ModelNodesGenerator(typeof(ModelHtmlEditorToolBarNodesGenerator))]
    [ModelReadOnly(typeof(ModelReadOnlyCalculator))]
    public interface IModelHtmlEditorToolBars:IModelList<IModelHtmlEditorToolBar>,IModelNode {
    }

    public interface IModelHtmlEditorToolBar:IModelNodeEnabled {
        IModelHtmlEditorToolBarItems Items { get; }
    }
    [ModelReadOnly(typeof(ModelReadOnlyCalculator))]
    public interface IModelHtmlEditorToolBarItems : IModelList<IModelHtmlEditorToolBarItem>,IModelNode {
    }

    public interface IModelHtmlEditorToolBarItem:IModelNodeEnabled {
    }

    public class ModelHtmlEditorToolBarNodesGenerator : ModelNodesGeneratorBase {
        static readonly IEnumerable<HtmlEditorToolbar> _htmlEditorToolbars;

        static ModelHtmlEditorToolBarNodesGenerator() {
            var htmlEditor = new ASPxHtmlEditor();
            htmlEditor.CreateDefaultToolbars(true);
            _htmlEditorToolbars = htmlEditor.Toolbars.Select(toolbar => toolbar);
        }

        protected override void GenerateNodesCore(ModelNode node) {
            var editorToolBars = (IModelHtmlEditorToolBars) node;
            foreach (var toolbar in _htmlEditorToolbars) {
                var modelHtmlEditorToolBar = editorToolBars.AddNode<IModelHtmlEditorToolBar>(toolbar.Name);
                foreach (var editorToolbarItem in toolbar.Items.ToList()) {
                    modelHtmlEditorToolBar.Items.AddNode<IModelHtmlEditorToolBarItem>(editorToolbarItem.CommandName);
                }
            }
        }
    }
}
