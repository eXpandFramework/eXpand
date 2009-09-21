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
using System;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    public class PropertyDescriptor : BaseObject, IPropertyDescriptor{
        private string code;
        private PropertyDescriptorImpl propertyDescriptor;

        public PropertyDescriptor(Session session) : base(session){
            propertyDescriptor = new PropertyDescriptorImpl();
        }

        public PropertyDescriptor(Session session, string name, Type valueType) : this(session){
            propertyDescriptor = new PropertyDescriptorImpl(name, valueType);
            code = name.Substring(0, Math.Min(4, name.Length));
        }

        [Association("PropertyBagDescriptor-PropertyDescriptor")]
        public XPCollection<PropertyBagDescriptor> PropertyBags{
            get { return GetCollection<PropertyBagDescriptor>("PropertyBags"); }
        }

        [Indexed(Unique = true)]
        [Size(4)]
        [RuleRequiredField("PropertyDescriptor Code required", "Save", "")]
        [RuleUniqueValue("PropertyDescriptor Code is unique", "Save", "The 'Code' property must have a unique value")]
        public string Code{
            get { return code; }
            set { code = value; }
        }
        #region IPropertyDescriptor
        public string Description{
            get { return propertyDescriptor.Description; }
            set{
                propertyDescriptor.Description = value;
                OnChanged();
            }
        }

        public string Type{
            get { return propertyDescriptor.Type; }
            set { propertyDescriptor.Type = value; }
        }

        public string Name{
            get { return propertyDescriptor.Name; }
            set{
                propertyDescriptor.Name = value;
                OnChanged();
            }
        }

        public Type ValueType{
            get { return propertyDescriptor.ValueType; }
        }
        #endregion
    }
}