#region Copyright (c) 2000-2010 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                 }
{                                                                   }
{       Copyright (c) 2000-2010 Developer Express Inc.              }
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
#endregion Copyright (c) 2000-2010 Developer Express Inc.

using System;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;
namespace eXpand.ExpressApp.Core
{
	public class ServerCollectionSource : CollectionSourceBase {
		private ITypeInfo objectTypeInfo;
		private XPServerCollectionSource serverCollection;
		private string additionalDisplayableProperties;
		private void SetDisplayablePropertiesToCollection() {
			if((serverCollection != null) && !String.IsNullOrEmpty(displayableProperties)) {
				serverCollection.DisplayableProperties =
					CombineDisplayableProperties(displayableProperties, serverCollection.DisplayableProperties);
			}
		}
		
		protected override String GetDisplayableProperties() {
			if(serverCollection != null) {
				return serverCollection.DisplayableProperties;
			}
			else {
				return base.GetDisplayableProperties();
			}
		}
		protected override void SetDisplayableProperties(String displayableProperties) {
			if(additionalDisplayableProperties != displayableProperties) {
				additionalDisplayableProperties = displayableProperties;
				SetDisplayablePropertiesToCollection();
				ResetCollection();
			}
		}

		protected override Object RecreateCollection(CriteriaOperator criteria, SortingCollection sortings) {
            
            XPClassInfo objectClassInfo = null;

            if (XafTypesInfo.XpoTypeInfoSource.TypeIsKnown(ObjectTypeInfo.Type))
            {
                objectClassInfo = XafTypesInfo.XpoTypeInfoSource.GetEntityClassInfo(ObjectTypeInfo.Type);
            }

            if (objectClassInfo != null)
            {
                serverCollection = new XPServerCollectionSource(this.ObjectSpace.Session, objectClassInfo, criteria);
            }
            else
            {
                serverCollection = new XPServerCollectionSource(this.ObjectSpace.Session, this.ObjectTypeInfo.Type, criteria);
            }

			SetDisplayablePropertiesToCollection();
			return serverCollection;
		}
		protected override void ApplyCriteriaCore(CriteriaOperator criteria) {
			if(serverCollection != null) {
				CriteriaOperator prevCriteria = serverCollection.FixedFilterCriteria;
				try {
					if(serverCollection != null) {
						serverCollection.FixedFilterCriteria = criteria;
					}
				}
				catch {
					serverCollection.FixedFilterCriteria = prevCriteria;
					throw;
				}
			}
		}
		protected override bool DefaultAllowAdd(out string diagnosticInfo) {
			diagnosticInfo = "Always is true";
			return true;
		}
		protected override bool DefaultAllowRemove(out string diagnosticInfo) {
			diagnosticInfo = "Always is true";
			return true;
		}
		public ServerCollectionSource(ObjectSpace objectSpace, Type objectType)
			: base(objectSpace) {
			if(objectType == null) {
				throw new ArgumentNullException("objectType");
			}
			objectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
			if(!objectTypeInfo.IsPersistent) {
				throw new ArgumentException(String.Format("The '{0}' type is not persistent.", objectTypeInfo.FullName));
			}
            objectSpace.Committed += new EventHandler(objectSpace_Committed);
		}
        private void objectSpace_Committed(object sender, EventArgs e)
        {
            //Reload();
        }
		public override void Reload() {
			if(serverCollection != null) {
				OnCollectionReloading();
				serverCollection.Reload();
				OnCollectionReloaded();
			}
		}
		public override void Add(object obj) {
			Reload();
		}
		public override void Remove(object obj) {
			Reload();
		}
		public override Boolean? IsObjectFitForCollection(Object obj) {
			if(serverCollection != null) {
				return ObjectSpace.IsObjectFitForCriteria(objectTypeInfo.Type, obj, serverCollection.FixedFilterCriteria);
			}
			return null;
		}
		public override CriteriaOperator GetIntegratedFilter() {
			CriteriaOperator result = null;
			if(serverCollection != null) {
				result = serverCollection.FixedFilterCriteria;
			}
			return result;
		}
		public override ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
		}
	}
}
