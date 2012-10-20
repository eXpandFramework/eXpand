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
    var middleRowContent = document.getElementById("MRC");
    var mainTable = document.getElementById("MT");
    if (!__aspxIE) {
        middleRowContent.style.height = "0px"
    }
    var windowHeight = GetWindowHeight();
    var newHeight = windowHeight - mainTable.offsetHeight - 20;
    if (windowHeight > mainTable.offsetHeight) {
        middleRowContent.style.height = middleRowContent.offsetHeight + newHeight + "px";
    }
    else {
        middleRowContent.style.height = "0px"
    }
}