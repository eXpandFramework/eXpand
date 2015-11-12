#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
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
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
namespace Xpand.ExpressApp.NH {
	public class ObjectMemberValueCriteriaProcessor : CriteriaProcessorBase {
		private ITypesInfo typesInfo;
		private ITypeInfo objectTypeInfo;
		protected override void Process(OperandProperty operand) {
			base.Process(operand);
			IMemberInfo memberInfo = objectTypeInfo.FindMember(operand.PropertyName);
			if(memberInfo.MemberTypeInfo.IsDomainComponent) {
				operand.PropertyName = operand.PropertyName + "." + memberInfo.MemberTypeInfo.KeyMember.Name;
			}
		}
		protected override void Process(OperandValue operand) {
			base.Process(operand);
			if(operand.Value != null) {
				ITypeInfo valueTypeInfo = typesInfo.FindTypeInfo(operand.Value.GetType());
				if((valueTypeInfo != null) && valueTypeInfo.IsDomainComponent) {
					operand.Value = valueTypeInfo.KeyMember.GetValue(operand.Value);
				}
			}
		}
		protected override void Process(AggregateOperand operand) {
			base.Process(operand);
			Type collectionElementType = objectTypeInfo.FindMember(operand.CollectionProperty.PropertyName).ListElementType;
			Xpand.ExpressApp.NH.ObjectMemberValueCriteriaProcessor aggregatedCriteriaProcessor = new ObjectMemberValueCriteriaProcessor(typesInfo, collectionElementType);
			aggregatedCriteriaProcessor.Process(operand.Condition);
		}
		public ObjectMemberValueCriteriaProcessor(ITypesInfo typesInfo, Type objectType)
			: base() {
			this.typesInfo = typesInfo;
			objectTypeInfo = typesInfo.FindTypeInfo(objectType);
		}
	}
}
