#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                 }
{                                                                   }
{       Copyright (c) 2000-2009 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2009 Developer Express Inc.
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace eXpand.Persistent.TaxonomyImpl{
    [Browsable(false)]
    public class PropertyBag : TaxonomyBaseObject, IPropertyBag{
        private readonly PropertyBagImpl propertyBag;

        public PropertyBag(Session session)
            : base(session){
            propertyBag = new PropertyBagImpl(this);
        }

        [Aggregated, Association("PropertyBag-PropertyValue", typeof (PropertyValue))]
        public XPCollection<PropertyValue> PropertyValues{
            get { return GetCollection<PropertyValue>("PropertyValues"); }
        }

        [NonPersistent]
        public IPropertyDescriptorContainer Descriptors{
            get { return propertyBag.Descriptors; }
            set { propertyBag.Descriptors = value; }
        }
        #region IPropertyBag Members
        IList<IPropertyValue> IPropertyBag.PropertyValues{
            get { return new ListConverter<IPropertyValue, PropertyValue>(PropertyValues); }
        }

        public IPropertyValue CreateValue(IPropertyDescriptor descriptor){
            return new PropertyValue(descriptor as PropertyDescriptor, this);
        }

        public object this[string name]{
            get { return propertyBag[name]; }
            set{
                propertyBag[name] = value;
                OnChanged(name);
            }
        }

        public PropertyDescriptorCollection GetItemProperties(System.ComponentModel.PropertyDescriptor[] listAccessors){
            return propertyBag.GetItemProperties(listAccessors);
        }

        public string GetListName(System.ComponentModel.PropertyDescriptor[] listAccessors){
            return propertyBag.GetListName(listAccessors);
        }
        #endregion
    }
}