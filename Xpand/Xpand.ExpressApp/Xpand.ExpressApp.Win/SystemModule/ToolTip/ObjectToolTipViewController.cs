using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.Win.SystemModule.ToolTip {
    public interface IModelToolTipController : IModelNode {
        [DataSourceProperty("ToolTipControllers")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        [Required]
        Type ToolTipController { get; set; }

        [Browsable(false)]
        IEnumerable<Type> ToolTipControllers { get; }
    }
    [DomainLogic(typeof(IModelToolTipController))]
    public class IModelToolTipControllerDomainLogic : BaseDomainLogic {
        public static IEnumerable<Type> Get_ToolTipControllers(IModelToolTipController modelToolTipController) {
            return FindTypeDescenants(typeof(ObjectToolTipController));
        }
    }
    public interface IObjectToolTipController {
        void ShowHint(object editObject, Point location, IObjectSpace objectSpace);
        void HideHint(bool clearCurrentObject);
        bool HintIsShown { get; }
    }

    public abstract class ObjectToolTipController : IDisposable, IObjectToolTipController {
        readonly ToolTipController _toolTipController;
        readonly Control parent;
        object _editObject;
        bool _hintIsShown;
        IObjectSpace _objectSpace;

        protected ObjectToolTipController(Control parent) {
            this.parent = parent;
            this.parent.Disposed += delegate { Dispose(); };
            _toolTipController = new ToolTipController();
            _toolTipController.ReshowDelay = _toolTipController.InitialDelay;
            _toolTipController.AllowHtmlText = true;
            _toolTipController.ToolTipType = ToolTipType.SuperTip;
            _toolTipController.AutoPopDelay = 10000;
            parent.MouseDown += delegate { HideHint(false); };
            parent.MouseLeave += delegate { HideHint(true); };

        }

        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        public object EditObject {
            get { return _editObject; }
        }
        #region IDisposable Members
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        public void ShowHint(object editObject, Point location, IObjectSpace objectSpace, ToolTipController toolTipController) {
            if (Equals(editObject, _editObject)) return;
            _objectSpace = objectSpace;
            _editObject = editObject;
            var info = new ToolTipControlInfo();
            var item = new ToolTipItem();
            InitToolTipItem(item);
            item.ImageToTextDistance = 10;
            info.Object = DateTime.Now.Ticks;
            info.SuperTip = new SuperToolTip();
            info.SuperTip.Items.Add(item);
            info.ToolTipPosition = parent.PointToScreen(location);
            toolTipController.ShowHint(info);
            _hintIsShown = true;
        }

        public void ShowHint(object editObject, Point location, IObjectSpace objectSpace) {
            ShowHint(editObject, location, objectSpace, _toolTipController);
        }

        protected abstract void InitToolTipItem(ToolTipItem item);

        public void HideHint(bool clearCurrentObject, ToolTipController toolTipController) {
            if (clearCurrentObject) _editObject = null;
            toolTipController.HideHint();
            _hintIsShown = false;
        }

        public void HideHint(bool clearCurrentObject) {
            HideHint(clearCurrentObject, _toolTipController);
        }

        public bool HintIsShown {
            get { return _hintIsShown; }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _toolTipController.Dispose();
            }
        }

        ~ObjectToolTipController() {
            Dispose(false);
        }
    }
}