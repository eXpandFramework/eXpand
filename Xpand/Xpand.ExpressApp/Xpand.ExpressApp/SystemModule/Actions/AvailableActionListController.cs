using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule.Actions{
    public class AvailableActionListController:ViewController{
        readonly HashSet<ActionBase> _availableActions=new HashSet<ActionBase>();
        public event EventHandler<AvailableActionArgs> AvailableActionListChanged;
        Dictionary<ActionBase,EventHandler<BoolValueChangedEventArgs>> _eventHandlers=new Dictionary<ActionBase, EventHandler<BoolValueChangedEventArgs>>();
        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Frame.Disposing+=FrameOnDisposing;
            _eventHandlers = new Dictionary<ActionBase, EventHandler<BoolValueChangedEventArgs>>();
            EventHandler<BoolValueChangedEventArgs>[] eventHandlers = { null };
            foreach (var action in Frame.Actions()){
                eventHandlers[0] = (sender, args) => { UpdateAvailableList(action); };
                _eventHandlers.Add(action,eventHandlers[0]);
                action.Active.ResultValueChanged+= eventHandlers[0];
                action.Enabled.ResultValueChanged+= eventHandlers[0];
                UpdateAvailableList(action);
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            ((Frame) sender).Disposing-=FrameOnDisposing;
            foreach (var eventHandler in _eventHandlers){
                eventHandler.Key.Active.ResultValueChanged -= eventHandler.Value;
                eventHandler.Key.Enabled.ResultValueChanged -= eventHandler.Value;
            }
        }

        private void UpdateAvailableList(ActionBase actionBase){
            if (actionBase.Enabled && actionBase.Active){
                if (_availableActions.Add(actionBase)){
                    OnAvailableActionListChanged(new AvailableActionArgs(actionBase,true));
                }
            }
            else{
                _availableActions.Remove(actionBase);
                OnAvailableActionListChanged(new AvailableActionArgs(actionBase));
            }
        }

        public HashSet<ActionBase> AvailableActions => _availableActions;

        protected virtual void OnAvailableActionListChanged(AvailableActionArgs availableActionArgs){
            AvailableActionListChanged?.Invoke(this, availableActionArgs);
        }
    }
    public class AvailableActionArgs : EventArgs {
        public AvailableActionArgs(ActionBase actionBase, bool added = false) {
            ActionBase = actionBase;
            Added = added;
        }

        public ActionBase ActionBase { get; }

        public bool Added { get; }
    }

}