using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.StateMachine.Xpo;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.StateMachine {
    public class StateMachinePopulateController : PopulateController<StateMachineTransitionPermission> {
        //        string _xpoStates;

        protected override void OnActivated() {
            base.OnActivated();


            var stringLookupPropertyEditor = View.GetItems<PropertyEditor>().Where(editor => editor.PropertyName == "StateCaption").OfType<IStringLookupPropertyEditor>().SingleOrDefault();
            if (stringLookupPropertyEditor != null)
                stringLookupPropertyEditor.ItemsCalculating += StringLookupPropertyEditorOnItemsCalculating;
            //This will never fire
            //            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
        }

        void StringLookupPropertyEditorOnItemsCalculating(object sender, HandledEventArgs handledEventArgs) {
            var propertyEditor = View.GetItems<PropertyEditor>().Where(editor => editor.PropertyName == PropertyName).SingleOrDefault();
            if (propertyEditor != null && propertyEditor.Control != null) {
                var stateMachineTransitionPermission = ((StateMachineTransitionPermission)View.CurrentObject);
                stateMachineTransitionPermission.SyncInfo(ObjectSpace, propertyEditor.ControlValue as string);
            }
        }



        protected override string GetPredefinedValues(IModelMember wrapper) {
            IList<XpoStateMachine> xpoStateMachines = ObjectSpace.GetObjects<XpoStateMachine>(null);
            return xpoStateMachines.Select(machine => machine.Name).AggregateWith(";");
        }

        protected override Expression<Func<StateMachineTransitionPermission, object>> GetPropertyName() {
            return permission => permission.StateMachineName;
        }
    }
}