var DXagent = navigator.userAgent.toLowerCase();
var DXopera = (DXagent.indexOf("opera") > -1);
var DXsafari = DXagent.indexOf("safari") > -1;
var DXie = (DXagent.indexOf("msie") > -1 && !DXopera);
var DXns = (DXagent.indexOf("mozilla") > -1 || DXagent.indexOf("netscape") > -1 || DXagent.indexOf("firefox") > -1) && !DXsafari && !DXie && !DXopera;

function DXattachEventToElement(element, eventName, func) {
    if (element) {
        if (DXns || DXsafari)
            element.addEventListener(eventName, func, true);
        else {
            if (eventName.toLowerCase().indexOf("on") != 0)
                eventName = "on" + eventName;
            element.attachEvent(eventName, func);
        }
    }
}
function DXGetElement(id) {
    if (document.getElementById != null) {
        return document.getElementById(id);
    }
    if (document.all != null) {
        return document.all[id];
    }
    if (document.layers != null) {
        return document.layers[id];
    }
    return null;
}
function DXGetElementHeight(id) {
    var el = DXGetElement(id);
    if (el) {
        return parseInt(el.offsetHeight);
    }
    return 0;
}
function DXGetWindowHeight() {
    var height = 0;
    if (typeof (window.innerHeight) == 'number') {
        height = window.innerHeight;
    } else if (document.documentElement && document.documentElement.clientHeight) {
        height = document.documentElement.clientHeight;
    } else if (document.body && document.body.clientHeight) {
        height = document.body.clientHeight;
    }
    return parseInt(height);
}
function DXMoveFooter() {
    var spacer = DXGetElement('Spacer');
    spacerHeight = DXGetElementHeight('Spacer');
    var newheight = DXGetWindowHeight() - (DXGetElementHeight('PageContent') - spacerHeight);
    if (newheight < 1) newheight = 1;
    if (spacerHeight != newheight || newheight == 1) {
        spacer.style.height = newheight + 'px';
        DXUpdateSplitterSize();
    }
}
function DXUpdateSplitterSize() {
    if (window.splitter) {
        splitter.width = "100%";
        var leftHeight = DXGetElementHeight('LeftPane');
        var rightHeight = DXGetElementHeight('ContentPane');
        var vAddition = 4;
        var hAddition = 0;
        var newHeight = (leftHeight > rightHeight ? leftHeight : rightHeight) + vAddition;
        var currentHeight = splitter.GetHeight();
        splitter.SetHeight(newHeight);
        var paneElement = splitter.GetPaneByName('Content').helper.GetContentContainerElement();
        var scrollWidth = paneElement.scrollWidth;
        var offsetWidth = paneElement.offsetWidth;
        if (scrollWidth != offsetWidth) {
            splitter.SetWidth(splitter.GetWidth() + scrollWidth - offsetWidth + hAddition);
        }
    }
}
function DXWindowOnResize(evt) {
    DXMoveFooter();
}

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
    middleRowContent.style.overflow = "hidden";
    var middleRowParent = document.getElementById("MRC");
    var mainTable = document.getElementById("MT");

    var getHeight = function (id) {
        var element = document.getElementById(id);
        if (element) {
            return element.offsetHeight;
        }
        else {
            return 0;
        }
    };

    var getParentTagHeight = function (id, parentTagName) {
        var element = document.getElementById(id);
        parentTagName = parentTagName.toUpperCase();
        while (element && element.tagName.toUpperCase() != parentTagName) {
            element = element.parentNode;
        }

        if (element) {
            return element.offsetHeight;
        }
        else {
            return 0;
        }

    }
    var mainTableHeight;

    mainTableHeight = mainTable.offsetHeight;

    var windowHeight = GetWindowHeight();
    var footer = document.getElementById("Footer");
    var newHeight = windowHeight - mainTableHeight - 20;
    var controlToResize = window.MasterDetailSplitter;
    var footerTableHeight = footer != null ? footer.getElementsByTagName("table")[0].offsetHeight : 0;

    var middleRowHeight = windowHeight - mainTableHeight + footerTableHeight + middleRowParent.offsetHeight - getHeight("UPVH") -
        getHeight("TB_Menu") - getParentTagHeight("UPQC", "td");

    if (controlToResize) {
        var elementToResize = controlToResize.GetMainElement();
        if (controlToResize && controlToResize.GetMainElement()) {
            controlToResize.SetWidth(window.innerWidth - 60 - document.getElementById("LPcell").offsetWidth);
            controlToResize.SetHeight(middleRowHeight - 20);
            if (elementToResize.parentNode.offsetHeight > elementToResize.offsetHeight)
                controlToResize.SetHeight(elementToResize.parentNode.offsetHeight);

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


