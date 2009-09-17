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
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace eXpand.Persistent.TaxonomyImpl{
    [DefaultProperty("Subject")]
    public class Task : TaxonomyBaseObject, ITask{
        private readonly TaskImpl task = new TaskImpl();
        private Party assignedTo;
#if MediumTrust
		[Persistent("DateCompleted"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime dateCompleted {
			get { return task.DateCompleted; }
			set { task.DateCompleted = value; }
		}
#else
        [Persistent("DateCompleted")]
        private DateTime dateCompleted{
            get { return task.DateCompleted; }
            set { task.DateCompleted = value; }
        }
#endif
        public Task(Session session) : base(session) {}

        protected override void OnLoading(){
            task.IsLoading = true;
            base.OnLoading();
        }

        protected override void OnLoaded(){
            base.OnLoaded();
            task.DateCompleted = dateCompleted;
            task.IsLoading = false;
        }

        [Action]
        public void MarkCompleted(){
            task.MarkCompleted();
            OnChanged("Status");
        }

        public string Subject{
            get { return task.Subject; }
            set{
                task.Subject = value;
                OnChanged("Subject");
            }
        }

        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))]
        public string Description{
            get { return task.Description; }
            set{
                task.Description = value;
                OnChanged("Description");
            }
        }

        public DateTime DueDate{
            get { return task.DueDate; }
            set{
                task.DueDate = value;
                OnChanged("DueDate");
            }
        }

        public DateTime StartDate{
            get { return task.StartDate; }
            set{
                task.StartDate = value;
                OnChanged("StartDate");
            }
        }

        public Party AssignedTo{
            get { return assignedTo; }
            set{
                assignedTo = value;
                OnChanged("AssignedTo");
            }
        }

        public TaskStatus Status{
            get { return task.Status; }
            set{
                task.Status = value;
                OnChanged("Status");
            }
        }

        public Int32 PercentCompleted{
            get { return task.PercentCompleted; }
            set{
                task.PercentCompleted = value;
                OnChanged("PercentCompleted");
            }
        }

        public DateTime DateCompleted{
            get { return dateCompleted; }
        }
    }
}