using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic.Security.Improved;
using Xpand.Persistent.Base.General.ValueConverters;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security.Improved {
    [System.ComponentModel.DisplayName("AdditionalViewControls")]
    public class AdditionalViewControlsOperationPermissionData : LogicRuleOperationPermissionData, IContextAdditionalViewControlsRule {

        public AdditionalViewControlsOperationPermissionData(Session session)
            : base(session) {
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new AdditionalViewControlsPermission(this) };
        }
        #region IAdditionalViewControlsRule Members

        public string Message { get; set; }
        public string MessageProperty { get; set; }
        public Position Position { get; set; }
        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
        public FontStyle? FontStyle { get; set; }
        public int? Height { get; set; }
        public int? FontSize { get; set; }
        public string ImageName { get; set; }
        #endregion
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        public Type ControlType { get; set; }
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        public Type DecoratorType { get; set; }
    }
}