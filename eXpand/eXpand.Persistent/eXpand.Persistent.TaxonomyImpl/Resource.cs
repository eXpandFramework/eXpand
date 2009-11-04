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
using System.Drawing;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [DefaultProperty("Caption")]
    public class Resource : BaseObject, IResource{
#if MediumTrust
		[Persistent("Color")]
		[Browsable(false)]
		public Int32 color;
#else
        [Persistent("Color")] private Int32 color;
#endif
        private string caption;
        public Resource(Session session) : base(session) {}

        public override void AfterConstruction(){
            base.AfterConstruction();
            color = Color.White.ToArgb();
        }

        [NonPersistent, Browsable(false)]
        public object Id{
            get { return Oid; }
        }

        public string Caption{
            get { return caption; }
            set{
                caption = value;
                OnChanged("Caption");
            }
        }

        [NonPersistent, Browsable(false)]
        public Int32 OleColor{
            get { return ColorTranslator.ToOle(Color.FromArgb(color)); }
        }

        [Association("Event-Resource", typeof (Event))]
        public XPCollection Events{
            get { return GetCollection("Events"); }
        }

        [NonPersistent]
        public Color Color{
            get { return Color.FromArgb(color); }
            set{
                color = value.ToArgb();
                OnChanged("Color");
            }
        }
    }
}