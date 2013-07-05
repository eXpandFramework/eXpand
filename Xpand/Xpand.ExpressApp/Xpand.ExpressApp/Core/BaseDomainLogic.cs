using System.Collections.Generic;
using System.Linq;
using DevExpress.Persistent.Base;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
namespace Xpand.ExpressApp.Core {
    public abstract class BaseDCDomainLogic<T> where T : class {
        private WeakReference _entity;
        private bool _exclusivePropChange;

        /// <summary>
        /// Use the Init function to register onproperty changed events with this domain
        /// logic class
        /// </summary>
        public abstract void Init();

        public virtual void AfterConstruction(T target, IObjectSpace objectSpace) { }
        public virtual void AfterConstruction(T target) {
            //assign the weakreference
            Entity = new WeakReference(target);
        }

        public virtual void OnDeleting(T target, IObjectSpace objectSpace) { }
        public virtual void OnDeleting(T target) { }

        public virtual void OnDeleted(T target, IObjectSpace objectSpace) { }
        public virtual void OnDeleted(T target) { }

        public virtual void OnSaving(T target, IObjectSpace objectSpace) { }
        public virtual void OnSaving(T target) { }

        public virtual void OnSaved(T target, IObjectSpace objectSpace) { }
        public virtual void OnSaved(T target) { }

        public virtual void OnLoaded(T target, IObjectSpace objectSpace) { }
        public virtual void OnLoaded(T target) {
            //recreate the weakreference
            Entity = new WeakReference(target);
        }

        public void RegisterPropertyChanged<TProp>(Expression<Func<T, TProp>> property, Action<T, ChangeEventArgs<TProp>> onChanged) where TProp : class {
            RegisterPropertyChanged(property, x => true, onChanged);
        }

        public void RegisterPropertyChanged<TProp>(Expression<Func<T, TProp>> property, Func<T, bool> onlyWhen, Action<T, ChangeEventArgs<TProp>> onChanged) where TProp : class {
            //parse memberexpression to check if only a property was provided
            var memberExpression = property.Body as MemberExpression;

            if (memberExpression == null)
                return;

            //extract the property name
            var memberName = memberExpression.Member.Name;

            //checks if weakreference was created and alive
            if (Entity == null || Entity.Target == null || !Entity.IsAlive)
                return;

            //if you would let your domain components derive from another class, this will not work
            var dcEntitity = Entity.Target as XPBaseObject;

            //checks if the target of weakreference is a DCEntity
            if (dcEntitity == null)
                return;

            //listen to the entity change event
            dcEntitity.Changed += (sender, args) => {
                //_exclusivePropChange is used to prevent an infinite loop of propertychang ed events,
                //this is possible when a user assigns a new value to the property in the onChange Action
                if (_exclusivePropChange)
                    return;

                //check for registered propertyname
                if (args.PropertyName == null || args.PropertyName != memberName || args.Reason != ObjectChangeReason.PropertyChanged)
                    return;

                //check if the conditions to trigger onChange were met
                if (!onlyWhen(sender as T))
                    return;

                //execute onChanged action
                _exclusivePropChange = true;
                onChanged(sender as T, new ChangeEventArgs<TProp>(args, property));
                _exclusivePropChange = false;
            };
        }

        private WeakReference Entity {
            get { return _entity; }
            set {
                _entity = value;
                Init();
            }
        }

        public class ChangeEventArgs<TProp> : EventArgs where TProp : class {
            private readonly Expression<Func<T, TProp>> _property;

            public T Entity { get; private set; }
            public TProp OldValue { get; private set; }
            public TProp NewValue { get; private set; }

            public void ChangeNewValue(TProp newValue) {
                //create an Action<T, TProp> to set the value of the property we intercept the change for
                var member = (MemberExpression)_property.Body;
                var param = Expression.Parameter(typeof(TProp), "value");
                var set = Expression.Lambda<Action<T, TProp>>(Expression.Assign(member, param), _property.Parameters[0], param);

                //compile Linq expression, and immediatly run it
                set.Compile()(Entity, newValue);
            }

            public ChangeEventArgs(ObjectChangeEventArgs objectChangeEventArgs, Expression<Func<T, TProp>> property) {
                _property = property;

                Entity = objectChangeEventArgs.Object as T;

                NewValue = objectChangeEventArgs.NewValue as TProp;
                OldValue = objectChangeEventArgs.OldValue as TProp;
            }

            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public new Type GetType() {
                return base.GetType();
            }

            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public override string ToString() {
                return base.ToString();
            }

            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public override int GetHashCode() {
                return base.GetHashCode();
            }

            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public override bool Equals(object obj) {
                return base.Equals(obj);
            }
        }
    }

    public abstract class BaseDomainLogic {
        protected static IEnumerable<Type> FindTypeDescenants(Type type) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
            return ReflectionHelper.FindTypeDescendants(typeInfo).Where(info => !info.IsAbstract).Select(info => info.Type);
        }
    }
}
