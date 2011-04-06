using System;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider {
    public interface IAdditionalViewControl {
        IAdditionalViewControlsRule Rule { get; set; }
    }
}