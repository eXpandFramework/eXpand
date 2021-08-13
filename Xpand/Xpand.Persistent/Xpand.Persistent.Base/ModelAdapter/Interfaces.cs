using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.ModelAdapter {
    public enum GraphicsUnit {
        /// <summary>
        /// Specifies the world unit as the unit of measure.
        /// </summary>
        World = 0,
        /// <summary>
        /// Specifies 1/75 inch as the unit of measure.
        /// </summary>
        Display = 1,
        /// <summary>
        /// Specifies a device pixel as the unit of measure.
        /// </summary>
        Pixel = 2,
        /// <summary>
        /// Specifies a printer's point (1/72 inch) as the unit of measure.
        /// </summary>
        Point = 3,
        /// <summary>
        /// Specifies the inch as the unit of measure.
        /// </summary>
        Inch = 4,
        /// <summary>
        /// Specifies the document unit (1/300 inch) as the unit of measure.
        /// </summary>
        Document = 5,
        /// <summary>
        /// Specifies the millimeter as the unit of measure.
        /// </summary>
        Millimeter = 6
    }

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
            return modelNodeEnabled.GetParent<IModelObjectView>();
        }
    }
}
