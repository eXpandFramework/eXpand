using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Logic.Conditional.Security.Improved;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security.Improved {
    public class AdditionalViewControlsOperationPermissionData : ConditionalLogicOperationPermissionData, IAdditionalViewControlsRule {

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
        #endregion
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        public Type ControlType { get; set; }
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        public Type DecoratorType { get; set; }
    }
}