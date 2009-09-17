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

using System.Collections.Generic;
using System.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [ImageName("BO_User"), System.ComponentModel.DisplayName("User")]
    public class User : Person, IUserWithRoles, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser{
        private readonly UserImpl user;
        private readonly List<IPermission> permissions;

        public User(Session session) : base(session){
            permissions = new List<IPermission>();
            user = new UserImpl(this);
        }

        public void ReloadPermissions(){
            Roles.Reload();
            foreach (Role role in Roles){
                role.PersistentPermissions.Reload();
            }
        }

        public bool ComparePassword(string password){
            return user.ComparePassword(password);
        }

        public void SetPassword(string password){
            user.SetPassword(password);
        }

#if MediumTrust
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Persistent]
		public string StoredPassword {
			get { return user.StoredPassword; }
			set {
				user.StoredPassword = value;
				OnChanged("StoredPassword");
			}
		}
#else
        [Persistent]
        private string StoredPassword{
            get { return user.StoredPassword; }
            set{
                user.StoredPassword = value;
                OnChanged("StoredPassword");
            }
        }
#endif

        [Association("User-Role")]
        public XPCollection<Role> Roles{
            get { return GetCollection<Role>("Roles"); }
        }

        IList<IRole> IUserWithRoles.Roles{
            get { return new ListConverter<IRole, Role>(Roles); }
        }

        [RuleRequiredField("User Name required", "Save", "The user name must not be empty")]
        [RuleUniqueValue("User Name is unique", "Save", "The login with the entered UserName was already registered within the system")]
        public string UserName{
            get { return user.UserName; }
            set{
                user.UserName = value;
                OnChanged("UserName");
            }
        }

        public bool ChangePasswordOnFirstLogon{
            get { return user.ChangePasswordAfterLogon; }
            set{
                user.ChangePasswordAfterLogon = value;
                OnChanged("ChangePasswordOnFirstLogon");
            }
        }

        public bool IsActive{
            get { return user.IsActive; }
            set{
                user.IsActive = value;
                OnChanged("IsActive");
            }
        }

        public IList<IPermission> Permissions{
            get{
                permissions.Clear();
                foreach (Role role in Roles){
                    permissions.AddRange(role.Permissions);
                }
                return permissions.AsReadOnly();
            }
        }
    }
}