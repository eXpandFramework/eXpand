function (s, e) {
    window.DetailUpdatePanelControl = s; s.GetMainElement().ClientControl = s;
    if (!window.AdjustSizeOverriden) {
        window.AdjustSizeCore = function () {
            var middleRowContent = document.getElementById("CP");
            
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

            var getWidth = function (id) {
                var element = document.getElementById(id);
                if (element) {
                    return element.offsetWidth;
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

            var middleRowHeight = windowHeight - mainTableHeight + footerTableHeight + middleRowParent.offsetHeight - getHeight("Horizontal_UPVH") -
				getHeight("Horizontal_TB_Menu") - getParentTagHeight("UPQC", "td") - getHeight("Vertical_UPVH") -
				getHeight("Vertial_TB_Menu");

            if (controlToResize) {
                middleRowContent.style.overflow = "hidden";
                var elementToResize = controlToResize.GetMainElement();
                if (elementToResize) {
					
                    var width = (window.innerWidth || document.documentElement.clientWidth) - 60 - getWidth("LPcell");
                    if (width > 0)
                        controlToResize.SetWidth(width);
						
                    controlToResize.SetHeight(middleRowHeight - 20);
                    if (elementToResize.parentNode.offsetHeight > elementToResize.offsetHeight)
                        controlToResize.SetHeight(elementToResize.parentNode.offsetHeight);

                    middleRowContent.style.height = middleRowHeight + "px";
                }
                else {
                    middleRowContent.style.overflow = "auto";
                }
				
            }
            else {
                middleRowContent.style.overflow = "auto";
                if (windowHeight > mainTable.offsetHeight) {
                    middleRowContent.style.height = middleRowHeight + "px";
                }
                else {
                    middleRowContent.style.height = "100%";
                }
            }

            window.isAdjusting = false;

        }
        
        var dxo = aspxGetGlobalEvents();
        dxo.EndCallback.AddHandler(window.AdjustSize);
        window.AdjustSizeOverriden = true;
        window.AdjustSizeCore();
    }
}
