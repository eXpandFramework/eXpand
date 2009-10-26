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
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Localization;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using System.ComponentModel;
using DevExpress.ExpressApp;

namespace eXpand.Persistent.TaxonomyImpl {
    public class HCategory : BaseObject, IHCategory {
        private string name;
        private HCategory parent;
        private void CheckCircularReferences(HCategory obj) {
            HCategory currentObj = obj;
            while(currentObj != null) {
                if(currentObj == this) {
                    throw new UserFriendlyException(ExceptionId.CircularReference);
                }
                currentObj = currentObj.Parent;
            }
        }
        public HCategory(Session session) : base(session) { }
        public HCategory(Session session, string name)
            : this(session) {
            this.name = name;
        }
        [Association("HCategoryParent-HCategoryChild")]
        public XPCollection<HCategory> Children {
            get { return GetCollection<HCategory>("Children"); }
        }
        [Persistent, Association("HCategoryParent-HCategoryChild")]
        public HCategory Parent {
            get { return parent; }
            set {
                if(!IsLoading) {
                    CheckCircularReferences(value);
                }
                parent = value;
                OnChanged("Parent");
            }
        }
        public string Name {
            get { return name; }
            set {
                name = value;
                OnChanged("Name");
            }
        }
        IBindingList ITreeNode.Children {
            get { return Children as IBindingList; }
        }
        ITreeNode IHCategory.Parent {
            get { return Parent as IHCategory; }
            set { Parent = value as HCategory; }
        }
        ITreeNode ITreeNode.Parent {
            get { return Parent as ITreeNode; }
        }
    }
}