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
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
namespace eXpand.Persistent.TaxonomyImpl {
	[MemberDesignTimeVisibility(false)]
	public class ServerPrefix : XPObject {
		private string prefix;
		public ServerPrefix(Session session) : base(session) { }
		public string Prefix {
			get { return prefix; }
			set { prefix = value; }
		}
	}
	[Persistent("IDGeneratorTable")]
	[MemberDesignTimeVisibility(false)]
	public class OidGenerator : XPBaseObject {
		private Guid id;
		private string type;
		private string prefix;
		private int oid;
		public OidGenerator(Session session) : base(session) { }
		[Persistent("ID"), Key(true)]
		public Guid ID {
			get { return id; }
			set { id = value; }
		}
		[Size(254)]
		public string Type {
			get { return type; }
			set { type = value; }
		}
		public string Prefix {
			get { return prefix; }
			set { prefix = value; }
		}
		public int Oid {
			get { return oid; }
			set { oid = value; }
		}
	}
	public static class DistributedIdGeneratorHelper {
		public const int MaxIdGenerationAttemptsCounter = 7;
		public static int Generate(IDataLayer idGeneratorDataLayer, string seqType, string serverPrefix) {
			for(int attempt = 1; ; ++attempt) {
				try {
					using(Session generatorSession = new Session(idGeneratorDataLayer)) {
						OidGenerator generator = generatorSession.FindObject<OidGenerator>(
							new OperandProperty("Type") == seqType &
							new OperandProperty("Prefix") == serverPrefix);
						if(generator == null) {
							generator = new OidGenerator(generatorSession);
							generator.Type = seqType;
							generator.Prefix = serverPrefix;
						}
						generator.Oid++;
						generator.Save();
						return generator.Oid;
					}
				}
				catch(LockingException) {
					if(attempt >= MaxIdGenerationAttemptsCounter)
						throw;
				}
			}
		}
		[ThreadStatic]
		static string cachedServerPrefix = null;
		public static int Generate(IDataLayer idGeneratorDataLayer, string seqType) {
				using(Session sess = new Session(idGeneratorDataLayer)) {
					ServerPrefix prefixRecord = sess.FindObject<ServerPrefix>(null);
					if(prefixRecord == null) {
						prefixRecord = new ServerPrefix(sess);
						prefixRecord.Prefix = "A";
						prefixRecord.Save();
					}
				}
			return Generate(idGeneratorDataLayer, seqType, cachedServerPrefix);
		}
	}
}
