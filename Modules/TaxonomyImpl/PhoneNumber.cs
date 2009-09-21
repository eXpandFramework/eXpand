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
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;

namespace eXpand.Persistent.TaxonomyImpl {
	[DefaultProperty("Number")]
	public class PhoneNumber : BaseObject, IPhoneNumber {
		private PhoneNumberImpl phone = new PhoneNumberImpl();
		private Party party = null;
		public PhoneNumber(Session session) : base(session) { }
		public override string ToString() {
			return Number;
		}
		[Persistent]
		public string Number {
			get { return phone.Number; }
			set {
				phone.Number = value;
				OnChanged("Number");
			}
		}
		[Association("Party-PhoneNumbers")]
		public Party Party {
			get { return party; }
			set {
				party = value;
				OnChanged("Party");
			}
		}
		public string PhoneType {
			get { return phone.PhoneType; }
			set {
				phone.PhoneType = value;
				OnChanged("PhoneType");
			}
		}
	}
	public class PhoneType : BaseObject {
		public PhoneType(Session session) : base(session) { }
		private string typeName;
		public string TypeName {
			get { return typeName; }
			set {
				typeName = value;
				OnChanged("TypeName");
			}
		}
	}
}
