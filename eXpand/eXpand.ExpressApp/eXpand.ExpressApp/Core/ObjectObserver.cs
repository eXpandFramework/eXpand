using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Core {
    public abstract class ObjectObserver<TObject> where TObject : class {
        public event EventHandler<ObjectChangedEventArgs<TObject>> ObjectChanged;

        protected virtual void OnChanged(ObjectChangedEventArgs<TObject> e)
        {
            EventHandler<ObjectChangedEventArgs<TObject>> handler = ObjectChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ObjectsManipulatingEventArgs<TObject>> ObjectDeleted;

        protected virtual void InvokeObjectDeleted(ObjectsManipulatingEventArgs<TObject> e)
        {
            EventHandler<ObjectsManipulatingEventArgs<TObject>> handler = ObjectDeleted;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ObjectsManipulatingEventArgs<TObject>> ObjectDeleting;

        protected virtual void InvokeObjectDeleting(ObjectsManipulatingEventArgs<TObject> e)
        {
            EventHandler<ObjectsManipulatingEventArgs<TObject>> handler = ObjectDeleting;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ObjectManipulatingEventArgs<TObject>> ObjectSaved;

        protected virtual void InvokeObjectSaved(ObjectManipulatingEventArgs<TObject> e)
        {
            EventHandler<ObjectManipulatingEventArgs<TObject>> handler = ObjectSaved;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ObjectManipulatingEventArgs<TObject>> ObjectSaving;

        protected virtual void InvokeObjectSaving(ObjectManipulatingEventArgs<TObject> e)
        {
            EventHandler<ObjectManipulatingEventArgs<TObject>> handler = ObjectSaving;
            if (handler != null) handler(this, e);
        }

        protected ObjectObserver(ObjectSpace objectSpace)
        {
            objectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
            objectSpace.ObjectDeleted += ObjectSpaceOnObjectDeleted;
            objectSpace.ObjectDeleting += ObjectSpaceOnObjectDeleting;
            objectSpace.ObjectSaved += ObjectSpaceOnObjectSaved;
            objectSpace.ObjectSaving += ObjectSpaceOnObjectSaving;
            _objectSpace=objectSpace;

        }
        private readonly ObjectSpace _objectSpace;
        public ObjectSpace ObjectSpace
        {
            get { return _objectSpace; }
        }
        
        void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs objectManipulatingEventArgs)
        {
            if (objectManipulatingEventArgs.Object is TObject)
                InvokeObjectSaving(new ObjectManipulatingEventArgs<TObject>(objectManipulatingEventArgs.Object as TObject));
        }

        void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs objectManipulatingEventArgs)
        {
            if (objectManipulatingEventArgs.Object is TObject)
                InvokeObjectSaved(new ObjectManipulatingEventArgs<TObject>(objectManipulatingEventArgs.Object as TObject));
        }

        void ObjectSpaceOnObjectDeleting(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs)
        {
            IEnumerable<TObject> objects = objectsManipulatingEventArgs.Objects.OfType<TObject>();
            if (objects.Count() > 0)
                InvokeObjectDeleting(new ObjectsManipulatingEventArgs<TObject>(objects));
        }

        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs)
        {
            IEnumerable<TObject> objects = objectsManipulatingEventArgs.Objects.OfType<TObject>();
            if (objects.Count() > 0)
                InvokeObjectDeleted(new ObjectsManipulatingEventArgs<TObject>(objects));
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs)
        {
            if (objectChangedEventArgs.Object is TObject)
                OnChanged(new ObjectChangedEventArgs<TObject>(objectChangedEventArgs));
        }
    }

    public class ObjectChangedEventArgs<TObject> : ObjectChangedEventArgs
    {
        public ObjectChangedEventArgs(object theObject, string propertyName, object oldValue, object newValue)
            : base(theObject, propertyName, oldValue, newValue)
        {
        }

        public ObjectChangedEventArgs(ObjectChangedEventArgs objectChangedEventArgs)
            : this(
                objectChangedEventArgs.Object, objectChangedEventArgs.PropertyName, objectChangedEventArgs.OldValue,
                objectChangedEventArgs.NewValue)
        {
        }

        public new TObject Object
        {
            get { return (TObject)base.Object; }
        }

    }

    public class ObjectManipulatingEventArgs<TObject>:ObjectManipulatingEventArgs {
        public ObjectManipulatingEventArgs(TObject theObject): base(theObject)
        {
        }
        public new TObject Object
        {
            get { return (TObject) base.Object; }
        }

    }
    public class ObjectsManipulatingEventArgs<TObject>:ObjectsManipulatingEventArgs {
        public ObjectsManipulatingEventArgs(TObject theObject)
            : base(theObject)
        {
        }

        public ObjectsManipulatingEventArgs(IEnumerable<TObject> objects)
            : base(objects)
        {
        }
        public new IEnumerable<TObject> Objects
        {
            get { return base.Objects.Cast<TObject>(); }
        }

    }
}