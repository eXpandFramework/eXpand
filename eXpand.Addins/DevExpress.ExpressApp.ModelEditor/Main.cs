

//ModelEditor 9.1.2



//#region Copyright (c) 2000-2008 Developer Express Inc.
///*
//{*******************************************************************}
//{                                                                   }
//{       Developer Express .NET Component Library                    }
//{       eXpressApp Framework                                 }
//{                                                                   }
//{       Copyright (c) 2000-2008 Developer Express Inc.              }
//{       ALL RIGHTS RESERVED                                         }
//{                                                                   }
//{   The entire contents of this file is protected by U.S. and       }
//{   International Copyright Laws. Unauthorized reproduction,        }
//{   reverse-engineering, and distribution of all or any portion of  }
//{   the code contained in this file is strictly prohibited and may  }
//{   result in severe civil and criminal penalties and will be       }
//{   prosecuted to the maximum extent possible under the law.        }
//{                                                                   }
//{   RESTRICTIONS                                                    }
//{                                                                   }
//{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
//{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
//{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
//{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
//{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
//{                                                                   }
//{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
//{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
//{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
//{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
//{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
//{                                                                   }
//{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
//{   ADDITIONAL RESTRICTIONS.                                        }
//{                                                                   }
//{*******************************************************************}
//*/
//#endregion Copyright (c) 2000-2008 Developer Express Inc.

using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.ModelEditor {
	public class MainClass {
		private static ModelEditorForm modelEditorForm;
		static private void HandleException(Exception e) {
			Tracing.Tracer.LogError(e);
			Messaging.GetMessaging(null).Show(ModelEditorForm.Title, e);
		}
		static private void OnException(object sender, ThreadExceptionEventArgs e) {
			HandleException(e.Exception);
		}
		[STAThread]
		static void Main(string[] args) {
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += new ThreadExceptionEventHandler(OnException);
		    

			if (args.Length == 0 ||args.Length > 2) {
                MessageBox.Show(string.Format("Usage: {0}'{1}' <Path_to_app_config_file> {0} '{1}' <Path_to_dll_file> <Path_to_diff_file>", 
                                                Environment.NewLine, Environment.GetCommandLineArgs()[0]), ModelEditorForm.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else {
				try {
				    args[0] = args[0].TrimStart('"').TrimEnd('"');
				    args[1] = args[1].TrimStart('"').TrimEnd('"');
					string targetPath = args[0];
                    string diffsPath = (args.Length == 2 ? args[1] : string.Empty);
                    if (Path.GetExtension(targetPath).ToLower() != ".config" && Path.GetExtension(targetPath).ToLower() != ".dll") {
                        throw new Exception("This file can be executed with a configuration or an assebly file as a parameter.");
                    }

					if(!File.Exists(targetPath)) {
						targetPath = Path.Combine(System.Environment.CurrentDirectory, targetPath);
						if(!File.Exists(targetPath)) {
							throw new Exception(String.Format("The config file '{0}' couldn't be found.", targetPath));
						}
					}
                    if (diffsPath == string.Empty) {
                        diffsPath = Path.GetDirectoryName(targetPath);
                    }
					if(diffsPath == string.Empty) { 
                        diffsPath = System.Environment.CurrentDirectory; 
                    } 
					DesignerModelFactory dmf = new DesignerModelFactory();
					ApplicationModulesManager mgr = dmf.CreateModelManager(targetPath, string.Empty);
					mgr.Load();
                    ApplicationModelsManager modelManager = new ApplicationModelsManager(mgr.Modules, mgr.ControllersManager, mgr.DomainComponents);
                    FileModelStore fileModelStore = dmf.CreateModuleModelStore(diffsPath);
                    
                    IModelApplication modelApplication = modelManager.CreateModelApplication(null, fileModelStore, false);
				    var controller = new ModelEditorViewController(modelApplication, fileModelStore, mgr.Modules);
				    modelEditorForm = new ModelEditorForm(controller, new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor"));
					modelEditorForm.SetCaption(Path.GetFileName(targetPath));
                    
					Application.Run(modelEditorForm);
				} catch(Exception exception) {
					HandleException(exception);
				}
			}
		}
	}
}
