using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using VSLangProj;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Process = System.Diagnostics.Process;

namespace Xpand.VSIX.Services {
    class ExternalToolsService {
        private static ReferencesEvents _referencesEvents;
        private static SolutionEvents _solutionEvents;
        private static bool _referencesAdd;
        private static bool _referencesRemoved;
        private static Events _dteEvents;
        private static BuildEvents _buildEvents;
        private static DocumentEvents _documentEvents;
        private static bool _referencesChanged;

        public static void Init(){
            _dteEvents = DteExtensions.DTE.Events;
            _documentEvents = _dteEvents.DocumentEvents;
            _documentEvents.DocumentSaved+=DocumentEventsOnDocumentSaved;
            _buildEvents = _dteEvents.BuildEvents;
            _buildEvents.OnBuildBegin+=BuildEventsOnOnBuildBegin;
            _solutionEvents = _dteEvents.SolutionEvents;
            _solutionEvents.Opened+=SolutionEventsOnOpened;
            _solutionEvents.AfterClosing+=SolutionEventsOnAfterClosing;
            _referencesEvents =((ReferencesEvents)_dteEvents.GetObject("CSharpReferencesEvents"));
            
        }

        private static void BuildEventsOnOnBuildBegin(vsBuildScope scope, vsBuildAction action){
            InvokePendingOnSave();
            InvokeTool(DTEEvent.BuildBegin);
        }

        private static void DocumentEventsOnDocumentSaved(Document document){
            InvokePendingOnSave();
            InvokeTool(DTEEvent.DocumentSaved);
        }

        private static void InvokePendingOnSave(){
            if (_referencesAdd){
                _referencesAdd = false;
                InvokeTool(DTEEvent.ReferenceAdded);
            }
            if (_referencesRemoved){
                _referencesRemoved = false;
                InvokeTool(DTEEvent.ReferenceRemoved);
            }
            if (_referencesChanged){
                _referencesChanged = false;
                InvokeTool(DTEEvent.ReferenceChanged);
            }
        }

        private static void SolutionEventsOnAfterClosing(){
            InvokePendingOnSave();
            if (_referencesEvents != null) {
                _referencesEvents.ReferenceAdded -= DTEEventsOnReferenceAdded;
                _referencesEvents.ReferenceChanged -= DTEEventsOnReferenceChanged;
                _referencesEvents.ReferenceRemoved -= ReferencesEventsOnReferenceRemoved;
            }

            InvokeTool(DTEEvent.SolutionAfterClosing);
        }

        private static void SolutionEventsOnOpened(){
            _referencesEvents.ReferenceAdded += DTEEventsOnReferenceAdded;
            _referencesEvents.ReferenceChanged += DTEEventsOnReferenceChanged;
            _referencesEvents.ReferenceRemoved += ReferencesEventsOnReferenceRemoved;
            InvokeTool(DTEEvent.SolutionOpen);
        }

        private static void ReferencesEventsOnReferenceRemoved(Reference pReference){
            _referencesRemoved = true;
        }

        private static void DTEEventsOnReferenceChanged(Reference pReference){
            _referencesChanged = true;
        }

        private static void DTEEventsOnReferenceAdded(Reference pReference){
            _referencesAdd = true;
        }

        private static void InvokeTool(DTEEvent dteEvent){
            foreach (var externalTool in OptionClass.Instance.ExternalTools.Where(tools => tools.DTEEvent==dteEvent).Where(SolutionMatch)){
                try{
                    DteExtensions.DTE.WriteToOutput($"Invoking on {dteEvent} {externalTool.Path} {externalTool.Arguments}");
                    Process.Start(externalTool.Path, externalTool.Arguments);
                }
                catch (Exception e){
                    DteExtensions.DTE.LogError(e.ToString());
                    DteExtensions.DTE.WriteToOutput(e.ToString());
                }
            }
        }


        private static bool SolutionMatch(ExternalTools externalTools){
            return Regex.IsMatch(Path.GetFileNameWithoutExtension(DteExtensions.DTE.Solution.FileName) + "",
                externalTools.SolutionRegex);
        }
    }
}
