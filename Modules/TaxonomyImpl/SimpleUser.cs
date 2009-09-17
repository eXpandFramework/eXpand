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
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [DefaultProperty("UserName")]
    [ImageName("BO_User"), System.ComponentModel.DisplayName("User")]
    public class SimpleUser : eXpandLiteObject, ISimpleUser, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser
    {
        private UserImpl user;

        public SimpleUser(Session session) : base(session){
            user = new UserImpl(this);
        }

        public string FullName{
            get { return user.FullName; }
            set{
                user.FullName = value;
                OnChanged("FullName");
            }
        }

        [Persistent]
        protected string Password{
            get { return user.StoredPassword; }
            set{
                user.StoredPassword = value;
                OnChanged("StoredPassword");
            }
        }
        #region IAuthenticationStandardUser Members
        public bool ComparePassword(string password){
            return user.ComparePassword(password);
        }

        public void SetPassword(string password){
            user.SetPassword(password);
        }

        public bool ChangePasswordOnFirstLogon{
            get { return user.ChangePasswordAfterLogon; }
            set{
                user.ChangePasswordAfterLogon = value;
                OnChanged("ChangePasswordAfterLogon");
            }
        }
        #endregion
        #region ISimpleUser Members
        public string UserName{
            get { return user.UserName; }
            set{
                user.UserName = value;
                OnChanged("UserName");
            }
        }

        public bool IsActive{
            get { return user.IsActive; }
            set{
                user.IsActive = value;
                OnChanged("IsActive");
            }
        }

        public bool IsAdministrator{
            get { return user.IsAdministrator; }
            set{
                user.IsAdministrator = value;
                OnChanged("IsAdministrator");
            }
        }
        #endregion
    }

    [DefaultProperty("UserName")]
    [ImageName("BO_User"), System.ComponentModel.DisplayName("User")]
    public class BasicUser : XPObject, ISimpleUser, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser{
        private UserImpl user;

        public BasicUser(Session session)
            : base(session){
            user = new UserImpl(this);
        }

        public bool ComparePassword(string password){
            return user.ComparePassword(password);
        }

        public void SetPassword(string password){
            user.SetPassword(password);
        }

        public string UserName{
            get { return user.UserName; }
            set{
                user.UserName = value;
                OnChanged("UserName");
            }
        }

        public string FullName{
            get { return user.FullName; }
            set{
                user.FullName = value;
                OnChanged("FullName");
            }
        }

        public bool IsActive{
            get { return user.IsActive; }
            set{
                user.IsActive = value;
                OnChanged("IsActive");
            }
        }

        public bool IsAdministrator{
            get { return user.IsAdministrator; }
            set{
                user.IsAdministrator = value;
                OnChanged("IsAdministrator");
            }
        }

        public bool ChangePasswordOnFirstLogon{
            get { return user.ChangePasswordAfterLogon; }
            set{
                user.ChangePasswordAfterLogon = value;
                OnChanged("ChangePasswordAfterLogon");
            }
        }

#if MediumTrust
		[Persistent]
		public string Password {
			get { return user.StoredPassword; }
			set {
				user.StoredPassword = value;
				OnChanged("StoredPassword");
			}
		}
#else
        [Persistent]
        protected string Password{
            get { return user.StoredPassword; }
            set{
                user.StoredPassword = value;
                OnChanged("StoredPassword");
            }
        }
#endif
    }
}