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
using DevExpress.Data.Filtering;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace eXpand.Persistent.TaxonomyImpl{
    public class AuditDataItemPersistent : TaxonomyBaseObject, IAuditDataItemPersistent<AuditedObjectWeakReference>{
        private AuditedObjectWeakReference auditedObject;
        private AuditOperationType auditOperationType;
        private XPDelayedProperty description = new XPDelayedProperty();
        private DateTime modifiedOn;
        private XPWeakReference newObject;
        private XPDelayedProperty newValue = new XPDelayedProperty();
        private XPWeakReference oldObject;
        private XPDelayedProperty oldValue = new XPDelayedProperty();
        private string operationType;
        private string propertyName;
        private string userName;

        public AuditDataItemPersistent(Session session, string userName, DateTime modifiedOn, string description)
            : base(session){
            this.userName = userName;
            this.modifiedOn = modifiedOn;
            Description = description;
        }

        public AuditDataItemPersistent(Session session)
            : base(session) {}

        [NonPersistent, MemberDesignTimeVisibility(false)]
        public AuditOperationType AuditOperationType{
            get { return auditOperationType; }
            set{
                auditOperationType = value;
                operationType = value.ToString();
            }
        }
        #region IAuditDataItemPersistent<AuditedObjectWeakReference> Members
        [Indexed]
        public string UserName{
            get { return userName; }
            set { userName = value; }
        }

        [Indexed]
        public DateTime ModifiedOn{
            get { return modifiedOn; }
            set { modifiedOn = value; }
        }

        [Indexed]
        public string OperationType{
            get { return operationType; }
            set { operationType = value; }
        }

        [Size(2048), Delayed, MemberDesignTimeVisibility(true)]
        public string Description{
            get { return GetDelayedPropertyValue<string>("Description"); }
            set { SetDelayedPropertyValue<string>("Description", value); }
        }

        [Association("AuditedObjectWeakReference-AuditDataItemPersistent"), MemberDesignTimeVisibility(false)]
        public AuditedObjectWeakReference AuditedObject{
            get { return auditedObject; }
            set { auditedObject = value; }
        }

        [Aggregated, MemberDesignTimeVisibility(false)]
        public XPWeakReference OldObject{
            get { return oldObject; }
            set { oldObject = value; }
        }

        [Aggregated, MemberDesignTimeVisibility(false)]
        public XPWeakReference NewObject{
            get { return newObject; }
            set { newObject = value; }
        }

        [Delayed, Size(1024)]
        public string OldValue{
            get { return GetDelayedPropertyValue<string>("OldValue"); }
            set { SetDelayedPropertyValue<string>("OldValue", value); }
        }

        [Delayed, Size(1024)]
        public string NewValue{
            get { return GetDelayedPropertyValue<string>("NewValue"); }
            set { SetDelayedPropertyValue<string>("NewValue", value); }
        }

        public string PropertyName{
            get { return propertyName; }
            set { propertyName = value; }
        }
        #endregion
    }

    public class AuditedObjectWeakReference : BaseAuditedObjectWeakReference{
        public AuditedObjectWeakReference(Session session)
            : base(session) {}

        public AuditedObjectWeakReference(Session session, object target)
            : base(session, target) {}

        [Association("AuditedObjectWeakReference-AuditDataItemPersistent", typeof (AuditDataItemPersistent))]
        public XPCollection<AuditDataItemPersistent> AuditDataItems{
            get { return GetCollection<AuditDataItemPersistent>("AuditDataItems"); }
        }

        public static XPCollection<AuditDataItemPersistent> GetAuditTrail(Session session, object obj){
            var auditObjectWR = session.FindObject<AuditedObjectWeakReference>(
                new GroupOperator(
                    new BinaryOperator("TargetType", session.GetObjectType(obj)),
                    new BinaryOperator("TargetKey", KeyToString(session.GetKeyValue(obj)))
                    ));
            if (auditObjectWR != null){
                auditObjectWR.AuditDataItems.BindingBehavior = CollectionBindingBehavior.AllowNone;
                return auditObjectWR.AuditDataItems;
            }
            else
                return null;
        }
    }
}