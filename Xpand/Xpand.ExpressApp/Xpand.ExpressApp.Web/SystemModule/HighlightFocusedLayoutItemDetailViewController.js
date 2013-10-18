window.lastFocusedElement = undefined;

window.isLayoutItemRow = function (obj) {
    return obj.className == "Item" && obj.tagName == "DIV";
}
window.isValid = function (obj) {
    return obj != null && obj != undefined;
}
window.assignStyle = function (element, color) {
    var curr = element;
    while (window.isValid(curr) && !window.isLayoutItemRow(curr)) {
        curr = curr.parentNode;
    };
    if (window.isValid(curr)) {
        curr.style.backgroundColor = color;
    }
}
window.initEditor = function (s, e) {
    if (s.focused) {
        window.assignStyle(window.lastFocusedElement = s.GetMainElement(), "#C9E4F0");
    }
}
window.gotFocusEditor = function (s, e) {
    window.assignStyle(window.lastFocusedElement, "");
    window.assignStyle(window.lastFocusedElement = s.GetMainElement(), "#C9E4F0");
}