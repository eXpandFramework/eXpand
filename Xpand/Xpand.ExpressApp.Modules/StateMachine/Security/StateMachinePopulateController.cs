using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.StateMachine.Xpo;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.ExpressApp.SystemModule;
using Xpand.Utils.Helpers;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.StateMachine.Security {
    public class StateMachinePopulateController : PopulateController<IStateMachineTransitionPermission> {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var stringLookupPropertyEditor = GetPropertyEditor(permission => permission.StateCaption) as IStringLookupPropertyEditor;
            if (stringLookupPropertyEditor != null)
                stringLookupPropertyEditor.ItemsCalculating += StringLookupPropertyEditorOnItemsCalculating;
        }
        void StringLookupPropertyEditorOnItemsCalculating(object sender, HandledEventArgs handledEventArgs) {
            var propertyEditor = GetPropertyEditor(permission => permission.StateMachineName);
            if (propertyEditor != null && View.IsControlCreated) {
                var stateMachineTransitionPermission = ((IStateMachineTransitionPermission)View.CurrentObject);
                var readOnlyCollection = GetStateCaptions(propertyEditor);
                stateMachineTransitionPermission.SyncStateCaptions(readOnlyCollection, propertyEditor.ControlValue as string);
            }
        }

        ReadOnlyCollection<string> GetStateCaptions(PropertyEditor propertyEditor) {
            var stateMachineName = propertyEditor.ControlValue as string;
            return ObjectSpace.GetObjects<XpoState>(state => state.StateMachine.Name == stateMachineName).Select(
                    state => state.Caption).ToList().AsReadOnly();
        }


        protected override string GetPredefinedValues(IModelMember wrapper) {
            IList<XpoStateMachine> xpoStateMachines = ObjectSpace.GetObjects<XpoStateMachine>(null);
            return xpoStateMachines.Select(machine => machine.Name).AggregateWith(";");
        }

        protected override Expression<Func<IStateMachineTransitionPermission, object>> GetPropertyName() {
            return permission => permission.StateMachineName;
        }
    }
}