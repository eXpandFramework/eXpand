﻿using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model {
    [ModelAbstractClass]
    public interface IModelListViewLinq : IModelListView {
        [Category("eXpand")]
        string XPQueryMethod { get; set; }
    }

    [ModelAbstractClass]
    public interface IModelStaticTextEx : IModelStaticText {
        [Category("eXpand")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        new string Text { get; set; }
    }

}
