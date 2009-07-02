using System;
using System.ComponentModel;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class SchemaHelper
    {
        public event EventHandler<AttibuteCreatedEventArgs> AttibuteCreating;
        #region OnAttibuteCreating
        /// <summary>
        /// Triggers the AttibuteCreating event.
        /// </summary>
        protected virtual void OnAttibuteCreating(AttibuteCreatedEventArgs ea)
        {
            if (AttibuteCreating != null)
                AttibuteCreating(null/*this*/, ea);
        }
        #endregion
        public string Serialize<T>(bool includeBaseTypes)
        {
            string schema = null;
            foreach (var property in typeof(T).GetProperties())
            {
                if (!includeBaseTypes&&typeof(T)!=property.DeclaringType)
                    continue;
                if (property.PropertyType == typeof (bool))
                    schema += GetAttribute("<Attribute Name=\"" + property.Name + "\" Choice=\"True,False\"/>");
                else if (typeof (Enum).IsAssignableFrom(property.PropertyType))
                    schema += GetAttribute("<Attribute Name=\"" + property.Name + "\" Choice=\"{" + property.PropertyType.FullName +
                                           "}\"/>");
                else
                    schema += GetAttribute("<Attribute Name=\"" + property.Name + "\"/>");
            }
            return schema;
        }

        private string GetAttribute(string s)
        {
            var args = new AttibuteCreatedEventArgs(s);
            OnAttibuteCreating(args);
            if (args.Handled)
                s=args.Attribute;
            return s;
        }
    }

    public class AttibuteCreatedEventArgs : HandledEventArgs
    {
        public string Attribute { get; set; }

        public AttibuteCreatedEventArgs(string attribute)
        {
            Attribute = attribute;
        }
    }
}