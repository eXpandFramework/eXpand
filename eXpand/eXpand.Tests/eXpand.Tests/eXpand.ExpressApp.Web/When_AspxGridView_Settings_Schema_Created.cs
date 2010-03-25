using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;
using DevExpress.ExpressApp.Win.Editors;
using eXpand.ExpressApp.Web.SystemModule;
using Machine.Specifications;

namespace eXpand.Tests.eXpand.ExpressApp.Web
{
    [Subject("ASPxGridView Schema")]
    public class When_AspxGridView_Settings_Schema_Created
    {
        static DictionaryNode _aspxGridViewNode;
        static DictionaryNode _listView;
        static Schema _schema;
        static GridOptionsController _gridOptionsController;

        Establish context = () => {
            _gridOptionsController = new GridOptionsController();
            
        };

        Because of = () => {
            var propertyInfo = typeof(XafGridView).GetProperty("OptionsView");
            _schema = _gridOptionsController.GetSchema();
            _listView = _schema.RootNode.FindChildElementByPath("Views/ListView") as DictionaryNode;
        };

        It should_contain_a_ListView_childNode_with_ASPxGridView_id = () => {
            _aspxGridViewNode = _listView.FindChildNode("ASPxGridView");
            _aspxGridViewNode.ShouldBeNull();
        };

    }
}
