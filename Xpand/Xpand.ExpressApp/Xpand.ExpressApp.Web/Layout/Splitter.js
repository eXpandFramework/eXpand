function GetWindowHeight() {
    var height = 0;
    if (typeof (window.innerHeight) == 'number') {
        height = window.innerHeight;
    } else if (document.documentElement && document.documentElement.clientHeight) {
        height = document.documentElement.clientHeight;
    } else if (document.body && document.body.clientHeight) {
        height = document.body.clientHeight;
    }
    var margin = 0;
    if (document.body.currentStyle) {
        margin = parseInt(document.body.currentStyle.margin);
    }
    return parseInt(height) - (margin * 2);
}
function AdjustSize() {
    var middleRowContent = document.getElementById("CP");
    var mainTable = document.getElementById("MT");
    if (!window.__aspxIE) {
        middleRowContent.style.height = "0px";
    }
    var windowHeight = GetWindowHeight();
    var newHeight = windowHeight - mainTable.offsetHeight - 20;
    var elementToResize = document.getElementById("MasterDetailSplitter");
    var middleRowHeight = middleRowContent.offsetHeight + newHeight;
    if (elementToResize) {
        var controlToResize = elementToResize.ClientControl;
        if (controlToResize) {
            controlToResize.SetWidth(window.innerWidth - 50);
            controlToResize.SetHeight(middleRowHeight - 15);
            middleRowContent.style.height = middleRowHeight + "px";
        }
    }
    else {
        if (windowHeight > mainTable.offsetHeight) {
            middleRowContent.style.height = middleRowHeight + "px";
        }
        else {
            middleRowContent.style.height = "100%";
        }
    }



}
