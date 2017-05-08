using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TextManager.Interop;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands{
    public class DuplicateLineCommand:VSCommand {
        private DuplicateLineCommand(IVsTextManager vsTextManager) : base((sender, args) => DuplicateLine(vsTextManager),
            new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidDouplicateLine)){
            BindCommand("Text Editor::Ctrl+D");
            this.EnableForActiveFile();
        }

        public static void Init(){
            var vsTextManager = VSPackage.VSPackage.Instance.GetService(typeof(SVsTextManager));
            new DuplicateLineCommand((IVsTextManager) vsTextManager);
        }

        static void DuplicateLine(IVsTextManager vsTextManager) {
            if (vsTextManager==null)
                return;
            IVsTextView ppView;
            vsTextManager.GetActiveView(1, null, out ppView);
            if (ppView != null) {
                int line;
                int bottomCol;
                string newText;
                int anchorLine;
                int anchorCol;
                int endLine;
                int endCol;
                ppView.GetSelection(out anchorLine, out anchorCol, out endLine, out endCol);
                if ((anchorLine == endLine) && (anchorCol == endCol)) {
                    IVsTextLines lines;
                    int num7;
                    ppView.GetBuffer(out lines);
                    lines.GetLineCount(out num7);
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