using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class RibbonFromModelController : WindowController, IModelExtender {
        IModelActionToContainerMapping _mapping;

        void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            var template = e.Template as IClassicToRibbonTransformerHolder;
            if (template != null && template.RibbonTransformer != null) {
                template.RibbonTransformer.BarItemAdding += RibbonTransformer_BarItemAdding;
                template.RibbonTransformer.Form.Disposed += Form_Disposed;
            }
        }

        void Form_Disposed(object sender, EventArgs e) {
            var template = sender as IClassicToRibbonTransformerHolder;
            if (template != null) {
                var classicToRibbonTransformer = template.RibbonTransformer;
                if (classicToRibbonTransformer != null) {
                    classicToRibbonTransformer.BarItemAdding -= RibbonTransformer_BarItemAdding;
                    classicToRibbonTransformer.Form.Disposed -= Form_Disposed;
                }
            }
        }

        RibbonPage AddPage(RibbonControl ribbon, string pageName) {
            var page = new RibbonPage(pageName) { Name = pageName };
            ribbon.Pages.Add(page);
            return page;
        }

        RibbonPageGroup AddPageGroup(RibbonPage page, string groupName) {
            RibbonPageGroup pageGroup = new ActionContainersRibbonPageGroup(groupName);
            pageGroup.MergeOrder = page.Groups.Count;
            page.Groups.Add(pageGroup);
            return pageGroup;
        }


        void RibbonTransformer_BarItemAdding(object sender, BarItemAddingEventArgs e) {
            var transformer = (ClassicToRibbonTransformer)sender;
            if (e.Action != null) {
                var findRibbonActionLink = FindRibbonActionLink(e.Action);
                var modelActionToContainerMapping = ((IModelActionDesignContainerMapping)Application.Model.ActionDesign).ActionToContainerMapping;
                foreach (IModelActionContainer v in modelActionToContainerMapping) {
                    foreach (IModelActionLink actionLink in v) {
                        if (actionLink.ActionId == e.Action.Id) {
                            findRibbonActionLink = ((IModelActionLinkRibbon)actionLink).Ribbon;
                        }
                    }
                }
                var mapping = modelActionToContainerMapping.SelectMany(container => container).OfType<IModelActionLinkRibbon>();
                foreach (var map in mapping) {
                    if (map.ActionId == e.Action.Id)
                        findRibbonActionLink = map.Ribbon;
                }

                if (findRibbonActionLink != null) {
                    BarItemSetup(findRibbonActionLink, e.Item);

                    RibbonPage page = e.Group.Page;
                    if (!string.IsNullOrEmpty(findRibbonActionLink.TargetRibbonPage)) {
                        page = transformer.FindPageByName(findRibbonActionLink.TargetRibbonPage) ??
                               AddPage(transformer.Ribbon, findRibbonActionLink.TargetRibbonPage);
                    }
                    RibbonPageGroup pageGroup = e.Group;
                    if (!string.IsNullOrEmpty(findRibbonActionLink.TargetRibbonGroup)) {
                        pageGroup = transformer.FindGroupByName(page, findRibbonActionLink.TargetRibbonGroup) ??
                                    AddPageGroup(page, findRibbonActionLink.TargetRibbonGroup);
                    }
                    e.Group = pageGroup;
                }
            }
        }

        void BarItemSetup(IModelRibbonActionLink actionLink, BarItem barItem) {
            if (actionLink.RibbonStyle.HasValue)
                barItem.RibbonStyle = actionLink.RibbonStyle.Value;
            if (!string.IsNullOrEmpty(actionLink.ShortcutKeyDisplayString))
                barItem.ShortcutKeyDisplayString = actionLink.ShortcutKeyDisplayString;
            if (actionLink.ShowInCustomizationForm.HasValue)
                barItem.ShowInCustomizationForm = actionLink.ShowInCustomizationForm.Value;
            if (actionLink.SmallWithTextWidth.HasValue)
                barItem.SmallWithTextWidth = actionLink.SmallWithTextWidth.Value;
            if (actionLink.SmallWithoutTextWidth.HasValue)
                barItem.SmallWithoutTextWidth = actionLink.SmallWithoutTextWidth.Value;
            if (actionLink.Visibility.HasValue)
                barItem.Visibility = actionLink.Visibility.Value;
            if (actionLink.VisibleWhenVertical.HasValue)
                barItem.VisibleWhenVertical = actionLink.VisibleWhenVertical.Value;
            if (actionLink.Width.HasValue)
                barItem.Width = actionLink.Width.Value;
        }

        IModelRibbonActionLink FindRibbonActionLink(ActionBase actionBase) {
            var link = _mapping.SelectMany(actionContainer => actionContainer).Cast<IModelActionLinkRibbon>().FirstOrDefault(actionLink => actionLink.ActionId == actionBase.Id);
            if (link != null) {
                return link.Ribbon;
            }
            return null;
        }

        protected override void OnActivated() {
            base.OnActivated();
            _mapping = ((IModelActionDesignContainerMapping)Application.Model.ActionDesign).ActionToContainerMapping;
            Application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        protected override void OnDeactivated() {
            Application.CustomizeTemplate -= Application_CustomizeTemplate;
            _mapping = null;
            base.OnDeactivated();
        }
        #region IModelExtender Members
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelActionLink, IModelActionLinkRibbon>();
        }
        #endregion
    }

    [ModelAbstractClass]
    public interface IModelActionLinkRibbon : IModelActionLink {
        IModelRibbonActionLink Ribbon { get; }
    }

    public interface IModelRibbonActionLink : IModelNode {
        [Localizable(true)]
        string TargetRibbonPage { get; set; }

        [Localizable(true)]
        string TargetRibbonGroup { get; set; }

        RibbonItemStyles? RibbonStyle { get; set; }
        string ShortcutKeyDisplayString { get; set; }
        bool? ShowInCustomizationForm { get; set; }
        int? SmallWithTextWidth { get; set; }
        int? SmallWithoutTextWidth { get; set; }
        BarItemVisibility? Visibility { get; set; }
        bool? VisibleWhenVertical { get; set; }
        int? Width { get; set; }
    }

}