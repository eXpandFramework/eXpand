using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using Xpand.Persistent.Base.General;

namespace ExcelImporterTester.Module.Web.Controllers {
    class LookupTemplate : ITemplate {
        private readonly string _clientInstance;

        //Handle the editor's client-side event to emulate the behavior of standard ASPxClientTextEdit.KeyDown grid editor. 
        const string BatchEditKeyDown =
            @"function(s, e) {
                var keyCode = ASPxClientUtils.GetKeyCode(e.htmlEvent);
                if (keyCode !== ASPx.Key.Tab && keyCode !== ASPx.Key.Enter) 
                    return;
                var moveActionName = e.htmlEvent.shiftKey ? 'MoveFocusBackward' : 'MoveFocusForward';
                var clientGridView = s.grid;
                if (clientGridView.batchEditApi[moveActionName]()) {
                    ASPxClientUtils.PreventEventAndBubble(e.htmlEvent);
                    window.batchPreventEndEditOnLostFocus = true;
                }
            }";
        //Handle the editor's client-side event to emulate the behavior of standard ASPxClientEdit.LostFocus grid editor. 
        const string BatchEditLostFocus =
            @"function (s, e) {
                var clientGridView = s.grid;
                if (!window.batchPreventEndEditOnLostFocus)
                    clientGridView.batchEditApi.EndEdit();
                window.batchPreventEndEditOnLostFocus = false;
            }";
        public IEnumerable<object> Objects { get; private set; }
        public LookupTemplate(IEnumerable<object> objects,string clientInstance){
            _clientInstance = clientInstance;
            Objects = objects;
        }
        public void InstantiateIn(Control container) {


            ASPxComboBox comboBox = new ASPxComboBox{
                Width = Unit.Percentage(100),
                ClientInstanceName = _clientInstance
            };
            comboBox.ClientSideEvents.KeyDown = BatchEditKeyDown;

            comboBox.ClientSideEvents.LostFocus = BatchEditLostFocus;

            ListEditItem notAssignedItem = new ListEditItem("N/A", null);
            comboBox.Items.Add(notAssignedItem);

            foreach (var currentObject in Objects) {
                var memberInfo = currentObject.GetTypeInfo().KeyMember;
                if (memberInfo != null){
                    var key = memberInfo.GetValue(currentObject);
                    var item = new ListEditItem(currentObject.ToString(), key );
                    comboBox.Items.Add(item);
                }
                else{
                    var o = ((dynamic) currentObject);
                    comboBox.Items.Add(o.Name, o.Key);
                }
            }

            container.Controls.Add(comboBox);
        }
    }
}
