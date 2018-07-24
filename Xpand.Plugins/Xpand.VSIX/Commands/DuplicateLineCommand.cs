using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.TextManager.Interop;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands{
    public class DuplicateLineCommand:VSCommand {
        private DuplicateLineCommand(IVsTextManager vsTextManager) : base((sender, args) => DuplicateLine(vsTextManager),
            new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidDouplicateLine)){
            var dteCommand = Options.OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == GetType().Name);
            BindCommand(dteCommand);
            this.EnableForActiveFile();
        }

        public static void Init(){
            var vsTextManager = VSPackage.VSPackage.Instance.GetService(typeof(SVsTextManager));
            var unused = new DuplicateLineCommand((IVsTextManager) vsTextManager);
        }

        static void DuplicateLine(IVsTextManager vsTextManager) {
            if (vsTextManager==null)
                return;
            vsTextManager.GetActiveView(1, null, out var ppView);
            if (ppView != null) {
                int line;
                int bottomCol;
                string newText;
                ppView.GetSelection(out var anchorLine, out var anchorCol, out var endLine, out var endCol);
                if ((anchorLine == endLine) && (anchorCol == endCol)) {
                    ppView.GetBuffer(out var lines);
                    lines.GetLineCount(out var num7);
                    if (anchorLine < (num7 - 1)) {
                        line = anchorLine + 1;
                        bottomCol = 0;
                        ppView.GetTextStream(anchorLine, 0, anchorLine + 1, 0, out newText);
                        anchorLine = endLine = line;
                    }
                    else {
                        line = anchorLine;
                        lines.GetLengthOfLine(anchorLine, out bottomCol);
                        ppView.GetTextStream(anchorLine, 0, anchorLine, bottomCol, out newText);
                        newText = Environment.NewLine + newText;
                        anchorLine = endLine = line + 1;
                    }
                    endCol = anchorCol;
                }
                else {
                    line = endLine;
                    bottomCol = endCol;
                    if ((anchorLine > endLine) || ((anchorLine == endLine) && (anchorCol > endCol))) {
                        endLine = anchorLine;
                        endCol = anchorCol;
                        anchorLine = line;
                        anchorCol = bottomCol;
                        line = endLine;
                        bottomCol = endCol;
                    }
                    ppView.GetSelectedText(out newText);
                    if (anchorLine == endLine) {
                        anchorCol = endCol = Math.Max(anchorCol, endCol);
                        endCol += newText.Length;
                    }
                    else {
                        endLine += Math.Abs(endLine - anchorLine);
                        anchorLine = line;
                        anchorCol = endCol;
                    }
                }
                ppView.ReplaceTextOnLine(line, bottomCol, 0, newText, newText.Length);
                ppView.SetSelection(anchorLine, anchorCol, endLine, endCol);
            }
        }
    }
}