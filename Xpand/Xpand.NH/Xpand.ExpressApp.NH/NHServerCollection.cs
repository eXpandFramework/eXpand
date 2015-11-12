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
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Data.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
namespace Xpand.ExpressApp.NH
{
    public class NHServerCollection : ILinqServerModeFrontEndOwner, IListSource, INHCollection
    {
        private NHObjectSpace objectSpace;
        private Type objectType;
        private CriteriaOperator criteria;
        private String defaultSorting;
        private String keyExpression;
        private IQueryable queryableSource;
        private EntityServerModeFrontEnd entityServerModeFrontEnd;
        private NHServerModeSourceAdderRemover serverModeSourceAdderRemover;
        private void InitQueryableSource()
        {
            IList<String> memberNames = serverModeSourceAdderRemover.DisplayableProperties.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            queryableSource = objectSpace.GetObjectQuery(objectType, memberNames, criteria, null);
        }
        public NHServerCollection(NHObjectSpace objectSpace, Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting)
        {
            this.objectSpace = objectSpace;
            this.objectType = objectType;
            this.criteria = criteria;
            this.defaultSorting = BaseObjectSpace.ConvertSortingToString(sorting);
            keyExpression = objectSpace.GetKeyPropertyName(objectType);
            ITypeInfo typeInfo = objectSpace.TypesInfo.FindTypeInfo(objectType);
            entityServerModeFrontEnd = new EntityServerModeFrontEnd(this);
            serverModeSourceAdderRemover = new NHServerModeSourceAdderRemover(entityServerModeFrontEnd, objectSpace, objectType);
            InitQueryableSource();
            entityServerModeFrontEnd.CatchUp();
        }
        public IList<SortProperty> GetSorting()
        {
            return BaseObjectSpace.ConvertStringToSorting(defaultSorting);
        }
        public void SetSorting(IList<SortProperty> sorting)
        {
            defaultSorting = BaseObjectSpace.ConvertSortingToString(sorting);
            entityServerModeFrontEnd.CatchUp();
        }
        public void Reload()
        {
            entityServerModeFrontEnd.Refresh();
        }
        public Type ObjectType
        {
            get { return objectType; }
            set
            {
                objectType = value;
                InitQueryableSource();
                entityServerModeFrontEnd.CatchUp();
            }
        }
        public CriteriaOperator Criteria
        {
            get { return criteria; }
            set
            {
                criteria = value;
                InitQueryableSource();
                entityServerModeFrontEnd.CatchUp();
            }
        }
        public String DisplayableProperties
        {
            get { return serverModeSourceAdderRemover.DisplayableProperties; }
            set
            {
                serverModeSourceAdderRemover.DisplayableProperties = value;
                InitQueryableSource();
                entityServerModeFrontEnd.CatchUp();
            }
        }
        public Boolean IsReadyForTakeOff()
        {
            return !String.IsNullOrWhiteSpace(keyExpression) && (queryableSource != null);
        }
        public String DefaultSorting
        {
            get { return defaultSorting; }
            set
            {
                defaultSorting = value;
                entityServerModeFrontEnd.CatchUp();
            }
        }
        public Type ElementType
        {
            get { return objectType; }
        }
        public String KeyExpression
        {
            get { return keyExpression; }
        }
        public IQueryable QueryableSource
        {
            get { return queryableSource; }
        }
        IList IListSource.GetList()
        {
            return serverModeSourceAdderRemover;
        }
        Boolean IListSource.ContainsListCollection
        {
            get { return false; }
        }
    }
}
