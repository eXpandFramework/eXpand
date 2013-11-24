using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.ModelAdapter {
    public class ModelSynchronizerList : List<IModelSynchronizable>, IModelSynchronizable, IDisposable {
        public void ApplyModel() {
            foreach (IModelSynchronizable synchronizable in this) {
                synchronizable.ApplyModel();
            }
        }

        public virtual void Dispose() {
            foreach (IModelSynchronizable synchronizable in this) {
                var disposable = synchronizable as IDisposable;
                if (disposable != null) {
                    disposable.Dispose();
                }
            }
        }

        public void SynchronizeModel() {
            foreach (IModelSynchronizable synchronizable in this) {
                synchronizable.SynchronizeModel();
            }
        }
    }


}
