using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.ModelAdapter {
    public interface IModelAppearanceFont:IModelNode {
        [DataSourceProperty("FontNames")]
        [Category("Font")]
        string FontName { get; set; }
        [Category("Font")]
        float? Size { get; set; }
        [Category("Font")]
        bool? Bold { get; set; }
        [Category("Font")]
        GraphicsUnit? Unit { get; set; }
        [Category("Font")]
        bool? Underline { get; set; }
        [Category("Font")]
        bool? Strikeout { get; set; }
        [Category("Font")]
        bool? Italic { get; set; }
        [Browsable(false)]
        IEnumerable<string> FontNames { get; }
    }

    [DomainLogic(typeof(IModelAppearanceFont))]
    public class ModelAppearanceFontLogic {
        public static IEnumerable<string> Get_FontNames(IModelAppearanceFont appearanceFont) {
            return FontFamily.Families.Select(family => family.Name);
        }
    }
    public interface IModelNodeEnabled : IModelNode {
        [DefaultValue(true)]
        [Category("Activation")]
        bool NodeEnabled { get; set; }
        [Browsable(false)]
        IModelObjectView ParentObjectView { get; }

    }

    [DomainLogic(typeof(IModelNodeEnabled))]
    public class ModelNodeEnabledDomainLogic{
        public static IModelObjectView Get_ParentObjectView(IModelNodeEnabled modelNodeEnabled){
            return modelNodeEnabled.GetParentNode<IModelObjectView>();
        }
    }
}
