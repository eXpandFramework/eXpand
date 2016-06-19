using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.General {
    public abstract class ObjectObserver<TObject> where TObject : class {
        public event EventHandler<ObjectChangedEventArgs<TObject>> ObjectChanged;

        protected virtual void OnChanged(ObjectChangedEventArgs<TObject> e) {
            EventHandler<ObjectChangedEventArgs<TObject>> handler = ObjectChanged;
            handler?.Invoke(this, e);
        }

        public event EventHandler<ObjectsManipulatingEventArgs<TObject>> ObjectDeleted;

        protected virtual void OnDeleted(ObjectsManipulatingEventArgs<TObject> e) {
            EventHandler<ObjectsManipulatingEventArgs<TObject>> handler = ObjectDeleted;
            handler?.Invoke(this, e);
        }

        public event EventHandler<ObjectsManipulatingEventArgs<TObject>> ObjectDeleting;

        protected virtual void OnDeleting(ObjectsManipulatingEventArgs<TObject> e) {
            EventHandler<ObjectsManipulatingEventArgs<TObject>> handler = ObjectDeleting;
            handler?.Invoke(this, e);
        }

        public event EventHandler<ObjectManipulatingEventArgs<TObject>> ObjectSaved;

        protected virtual void OnSaved(ObjectManipulatingEventArgs<TObject> e) {
            EventHandler<ObjectManipulatingEventArgs<TObject>> handler = ObjectSaved;
            handler?.Invoke(this, e);
        }

        public event EventHandler<ObjectManipulatingEventArgs<TObject>> ObjectSaving;

        protected virtual void OnSaving(ObjectManipulatingEventArgs<TObject> e) {
            EventHandler<ObjectManipulatingEventArgs<TObject>> handler = ObjectSaving;
            handler?.Invoke(this, e);
        }

        protected ObjectObserver(IObjectSpace objectSpace) {
            objectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
            objectSpace.ObjectDeleted += ObjectSpaceOnObjectDeleted;
            objectSpace.ObjectDeleting += ObjectSpaceOnObjectDeleting;
            objectSpace.ObjectSaved += ObjectSpaceOnObjectSaved;
            objectSpace.ObjectSaving += ObjectSpaceOnObjectSaving;
            objectSpace.Disposed+=ObjectSpaceOnDisposed;
            objectSpace.Committing+=ObjectSpaceOnCommitting;
            ObjectSpace = objectSpace;

        }

        private void ObjectSpaceOnDisposed(object sender, EventArgs eventArgs){
            var objectSpace = ((IObjectSpace) sender);
            objectSpace.Committing-=ObjectSpaceOnCommitting;
            objectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
            objectSpace.ObjectDeleted -= ObjectSpaceOnObjectDeleted;
            objectSpace.ObjectDeleting -= ObjectSpaceOnObjectDeleting;
            objectSpace.ObjectSaved -= ObjectSpaceOnObjectSaved;
            objectSpace.ObjectSaving -= ObjectSpaceOnObjectSaving;
            objectSpace.Disposed -= ObjectSpaceOnDisposed;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            OnCommiting(cancelEventArgs);
        }

        protected virtual void OnCommiting(CancelEventArgs cancelEventArgs){
            
        }

        public IObjectSpace ObjectSpace { get; }

        void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs objectManipulatingEventArgs){
            var theObject = objectManipulatingEventArgs.Object as TObject;
            if (theObject != null)
                OnSaving(new ObjectManipulatingEventArgs<TObject>(theObject));
        }

        void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs objectManipulatingEventArgs){
            var theObject = objectManipulatingEventArgs.Object as TObject;
            if (theObject != null)
                OnSaved(new ObjectManipulatingEventArgs<TObject>(theObject));
        }

        void ObjectSpaceOnObjectDeleting(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            var objects = objectsManipulatingEventArgs.Objects.OfType<TObject>().ToArray();
            if (objects.Any())
                OnDeleting(new ObjectsManipulatingEventArgs<TObject>(objects));
        }

        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            var objects = objectsManipulatingEventArgs.Objects.OfType<TObject>().ToArray();
            if (objects.Any())
                OnDeleted(new ObjectsManipulatingEventArgs<TObject>(objects));
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            if (objectChangedEventArgs.Object is TObject)
                OnChanged(new ObjectChangedEventArgs<TObject>(objectChangedEventArgs));
        }
    }

    public class ObjectChangedEventArgs<TObject> : ObjectChangedEventArgs {
        public ObjectChangedEventArgs(object theObject, string propertyName, object oldValue, object newValue)
            : base(theObject, propertyName, oldValue, newValue) {
        }

        public ObjectChangedEventArgs(ObjectChangedEventArgs objectChangedEventArgs)
            : this(
                objectChangedEventArgs.Object, objectChangedEventArgs.PropertyName, objectChangedEventArgs.OldValue,
                objectChangedEventArgs.NewValue) {
        }

        public new TObject Object => (TObject)base.Object;
    }

    public class ObjectManipulatingEventArgs<TObject> : ObjectManipulatingEventArgs {
        public ObjectManipulatingEventArgs(TObject theObject)
            : base(theObject) {
        }
        public new TObject Object => (TObject)base.Object;
    }
    public class ObjectsManipulatingEventArgs<TObject> : ObjectsManipulatingEventArgs {
        public ObjectsManipulatingEventArgs(TObject theObject)
            : base(theObject) {
        }

        public ObjectsManipulatingEventArgs(IEnumerable<TObject> objects)
            : base(objects) {
        }
        public new IEnumerable<TObject> Objects => base.Objects.Cast<TObject>();
    }
}