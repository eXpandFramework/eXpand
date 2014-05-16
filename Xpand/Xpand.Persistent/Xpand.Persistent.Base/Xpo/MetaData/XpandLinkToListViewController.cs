using System;
using System.ComponentModel;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.Xpo.MetaData{
    public class XpandLinkToListViewController : Controller {
        private Link _link;
        private View _previousView;
        private void Application_ViewShowing(object sender, ViewShowingEventArgs e) {
            if ((e.TargetFrame is Window) && (e.SourceFrame == Frame)) {
                if ((Frame.View is ObjectView)
                        && (e.View is DetailView)
                        && (Link != null) && (Link.ListView != null) && !(Frame.View.ObjectTypeInfo.IsAssignableFrom(e.View.ObjectTypeInfo))) {
                    e.View.Tag = Link;
                    e.View.Disposing+=ViewOnDisposing;
                }
            }
        }

        private void ViewOnDisposing(object sender, CancelEventArgs cancelEventArgs){
            var view = ((View) sender);
            view.Disposing-=ViewOnDisposing;
            view.Tag = null;
        }

        private void Frame_ViewChanging(object sender, ViewChangingEventArgs e) {
            _previousView = Frame.View;
        }
        private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
            if (_link != null) {
                if (_previousView is ListView) {
                    _link.ListView = null;
                }
                _previousView = null;
                _link = null;
                OnLinkChanged();
            }
            View view = Frame.View;
            var listView = view as ListView;
            if (listView != null) {
                _link = new Link(listView);
                OnLinkChanged();
            }
        }
        protected virtual void OnLinkChanged() {
            if (LinkChanged != null) {
                LinkChanged(this, EventArgs.Empty);
            }
        }
        protected override void OnActivated() {
            base.OnActivated();
            Frame.ViewChanging += Frame_ViewChanging;
            Frame.ViewChanged += Frame_ViewChanged;
            var view = Frame.View as ListView;
            if ( view != null) {
                _link = new Link(view);
                OnLinkChanged();
            }
            if (Frame != null && Frame.Application != null) {
                Frame.Application.ViewShowing += Application_ViewShowing;
            }
        }
        protected override void OnDeactivated() {
            Frame.ViewChanging -= Frame_ViewChanging;
            Frame.ViewChanged -= Frame_ViewChanged;
        }
        protected override void Dispose(bool disposing) {
            if (Frame != null && Frame.Application != null) {
                Frame.Application.ViewShowing -= Application_ViewShowing;
            }
            if (_link != null) {
                if (Frame != null && Frame.View is ListView) {
                    _link.ListView = null;
                }
                _link = null;
            }
            LinkChanged = null;
            _previousView = null;
            base.Dispose(disposing);
        }
        public Link Link {
            get { return _link; }
            set {
                if (Frame.View is ListView) {
                    throw new InvalidOperationException();
                }
                _link = value;
                OnLinkChanged();
            }
        }
        public event EventHandler LinkChanged;
    }
}