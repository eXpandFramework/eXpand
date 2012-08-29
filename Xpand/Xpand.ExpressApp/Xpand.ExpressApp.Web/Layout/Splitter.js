function AdjustSplitterSize(middleRowContent, newHeight) {
    var middleRowHeight = middleRowContent.offsetHeight + newHeight;
    var elementToResize = document.getElementById("MasterDetailSplitter");

    if (elementToResize) {
        var controlToResize = elementToResize.ClientControl;
        if (controlToResize) {
            controlToResize.SetWidth(window.innerWidth - 50);
            controlToResize.SetHeight(middleRowHeight - 130);
        }
    }
}
