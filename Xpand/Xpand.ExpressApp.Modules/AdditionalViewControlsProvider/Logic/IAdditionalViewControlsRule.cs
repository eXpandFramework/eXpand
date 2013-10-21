using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public interface ISupportAppeareance {
        [Category("Appearance")]
        Color? BackColor { get; set; }

        [Category("Appearance")]
        Color? ForeColor { get; set; }

        [Category("Appearance")]
        FontStyle? FontStyle { get; set; }

        [Category("Appearance")]
        int? Height { get; set; }
        [Category("Appearance")]
        int? FontSize { get; set; }

        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win.v13.1", typeof(System.Drawing.Design.UITypeEditor))]
        [Category("Appearance")]
        string ImageName { get; set; }
    }

    public interface IContextAdditionalViewControlsRule:IContextLogicRule,IAdditionalViewControlsRule {
         
    }
    public interface IAdditionalViewControlsRule : ILogicRule, ISupportAppeareance {
        [Category("Data")]
        [Description("The type of the control to be added to the view")]
        [TypeConverter(typeof(StringToTypeConverter))]
        [Required]
        [DataSourceProperty("ControlTypes")]
        Type ControlType { get; set; }


        [Category("Data")]
        [Description("The type of the control that will be used to decorate the inserted control")]
        [TypeConverter(typeof(StringToTypeConverter))]
        [Required]
        [DataSourceProperty("DecoratorTypes")]
        Type DecoratorType { get; set; }


        [Category("Data")]
        [Description("The type of the control that will be used to decorate the inserted control")]
        string MessageProperty { get; set; }


        [Category("Data")]
        [Description("The type of the control that will be used to decorate the inserted control")]
        [Localizable(true)]
        string Message { get; set; }

        [Category("Behavior")]
        [Description("Specifies the position at which the control is to be inserted")]
        Position Position { get; set; }

    }
    [DomainLogic(typeof(IAdditionalViewControlsRule))]
    public class AdditionalViewControlsRuleDomainLogic {
        public static Type Get_ControlType(IAdditionalViewControlsRule additionalViewControlsRule) {
            TypeDecorator decorator = ModelAdditionalViewControlsRuleDomainLogic.GetTypeDecorator(additionalViewControlsRule.DecoratorType, additionalViewControlsRule.Position);
            return decorator != null ? decorator.DefaultType : typeof(NotAvaliableInThisPlatform);
        }

        public static Type Get_DecoratorType(IAdditionalViewControlsRule additionalViewControlsRule) {
            var decoratorType =
                ModelAdditionalViewControlsRuleDomainLogic.GetDecorators().FirstOrDefault(info => info.Type.GetCustomAttributes(typeof(TypeDecorator), true).OfType<TypeDecorator>().Count(decorator => decorator.IsDefaultDecorator) > 0);
            return decoratorType != null ? decoratorType.Type : typeof(NotAvaliableInThisPlatform);
        }
    }
    class NotAvaliableInThisPlatform {

    }
}