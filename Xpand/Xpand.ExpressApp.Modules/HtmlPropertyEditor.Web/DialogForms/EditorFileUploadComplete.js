function (e, s) {
    var editor = window.ASPxDialog.GetOwnerControl();
    var split = s.callbackData.toString().split(',');
    editor.InsertLink(split[0], split[1], "", "");
    window.aspxDialogComplete(1, s);
}
