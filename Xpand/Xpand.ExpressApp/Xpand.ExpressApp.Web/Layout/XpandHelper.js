var XpandHelper = {

    GetParentControl: function (childControl) {
        return this.GetElementParentControl(childControl.GetMainElement());
    },

    GetElementParentControl: function (element) {
        for (var c = element.parentNode; c; c = c.parentNode) {
            var parentControl = ASPxClientControl.GetControlCollection().GetByName(c.id);
            if (parentControl) {
                return parentControl;
            }
        }
    },
    
    GetFirstChildControl: function (element) {
        for (var i = 0; i < element.childNodes.length; i++) {
            var control = ASPxClientControl.GetControlCollection().GetByName(element.childNodes[i].id);
            if (control) {
                return control;
            }
        }
    },

    IsRootSplitter: function (splitter) {
        for (var c = splitter.GetMainElement().parentNode; c; c = c.parentNode) {
            var parentControl = ASPxClientControl.GetControlCollection().GetByName(c.id);
            if (parentControl && parentControl.GetPane)
                return false;
        }

        return true;

    }
}

