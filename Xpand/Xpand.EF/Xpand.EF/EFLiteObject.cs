

using System;
using System.ComponentModel;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp {
    public class EFLiteObject : IObjectSpaceLink, INotifyPropertyChanged {
        #region IObjectSpaceLink members (see http://help.devexpress.com/#Xaf/clsDevExpressExpressAppIObjectSpaceLinktopic)
        // Use the Object Space to access other entities from IXafEntityObject methods (see http://help.devexpress.com/#Xaf/CustomDocument3707).
        private IObjectSpace objectSpace;
        public IObjectSpace ObjectSpace {
            get { return objectSpace; }
            set { objectSpace = value; }
        }
        #endregion


        /// <summary>
        /// Raize PropertyChanged
        /// </summary>
        /// <param name="propertyName">Name of changed property</param>
        protected void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Notify ObjectProperty changed by PropertyChanged Event for <see cref="System.float"/> Type property. 
        /// Only triggerd when value changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="property">Variable to store property value in</param>
        /// <param name="value">
        public void SetPropertyValue(string propertyName, ref float property, float value) {
            SetPropertyValue<float>(propertyName, ref property, value);
        }

        /// <summary>
        /// Notify ObjectProperty changed by PropertyChanged Event for <see cref="System.Double"/> Type property. 
        /// Only triggerd when value changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="property">Variable to store property value in</param>
        /// <param name="value">
        public void SetPropertyValue(string propertyName, ref Double property, Double value) {
            SetPropertyValue<Double>(propertyName, ref property, value);
        }

        /// <summary>
        /// Notify ObjectProperty changed by PropertyChanged Event for <see cref="System.String"/> Type property. 
        /// Only triggerd when value changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="property">Variable to store property value in</param>
        /// <param name="value">
        public void SetPropertyValue(string propertyName, ref string property, string value) {
            SetPropertyValue<string>(propertyName, ref property, value);
        }

        /// <summary>
        /// Notify ObjectProperty changed by PropertyChanged Event for <see cref="System.Int32"/> Type property. 
        /// Only triggerd when value changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="property">Variable to store property value in</param>
        /// <param name="value">
        public void SetPropertyValue(string propertyName, ref int property, int value) {
            SetPropertyValue<int>(propertyName, ref property, value);
        }

        /// <summary>
        /// Notify ObjectProperty changed by PropertyChanged Event for <see cref="System.DateTime"/> Type property. 
        /// Only triggerd when value changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="property">Variable to store property value in</param>
        /// <param name="value">
        public void SetPropertyValue(string propertyName, ref DateTime property, DateTime value) {
            SetPropertyValue<DateTime>(propertyName, ref property, value);
        }

        /// <summary>
        /// Notify ObjectProperty changed by PropertyChanged Event for <T> Type property. 
        /// Only triggerd when value changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="property">Variable to store property value in</param>
        /// <param name="value">New value to set</param>
        protected void SetPropertyValue<T>(string propertyName, ref T property, T value) {
            if (property != null) {
                if (!property.Equals(value)) {
                    property = value;
                    NotifyPropertyChanged(propertyName);
                }
            } else {
                property = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
