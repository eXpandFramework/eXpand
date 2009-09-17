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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [DefaultProperty("Name")]
    public class RoleBase : eXpandLiteObject
    {
        private readonly RoleImpl role = new RoleImpl();
        public RoleBase(Session session) : base(session) {}

        [MemberDesignTimeVisibility(false), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorCollectionReturnsNewValueOnEachAccess))]
        public ReadOnlyCollection<IPermission> Permissions{
            get { return role.GetPermissions(new ListConverter<IPersistentPermission, PersistentPermission>(PersistentPermissions)); }
        }

        public string Name{
            get { return role.Name; }
            set{
                role.Name = value;
                OnChanged("Name");
            }
        }

        [Aggregated, Association("Role-PersistentPermissions")]
        public XPCollection<PersistentPermission> PersistentPermissions{
            get { return GetCollection<PersistentPermission>("PersistentPermissions"); }
        }

        public PersistentPermission AddPermission(IPermission permission){
            var result = new PersistentPermission(Session, permission);
            PersistentPermissions.Add(result);
            return result;
        }
    }

    public class Role : RoleBase, IRole, ICustomizableRole{
        public Role(Session session) : base(session) {}

        [Association("User-Role")]
        public XPCollection<User> Users{
            get { return GetCollection<User>("Users"); }
        }
        #region ICustomizableRole Members
        void ICustomizableRole.AddPermission(IPermission permission){
            base.AddPermission(permission);
        }
        #endregion
        #region IRole Members
        IList<IUser> IRole.Users{
            get { return new ListConverter<IUser, User>(Users); }
        }
        #endregion
    }
}