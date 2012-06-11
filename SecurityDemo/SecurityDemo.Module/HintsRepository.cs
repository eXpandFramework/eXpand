using System;
using System.Collections.Generic;
using System.Text;

namespace DevExpress.ExpressApp.Demos {
    public class HintProvider : IHintProvider  {
        private string hint;
        public HintProvider(string hint) {
            this.hint = hint;
        }
        public string Hint {
            get { return hint; }
        }
    }

    public class HintsRepository {
        private Dictionary<string, IHintProvider> hintDictionary;
        private static HintsRepository instance;
        private HintsRepository() {
            hintDictionary = new Dictionary<string, IHintProvider>();
        }
        public void RegisterHintForView(string viewId, string hint) {
            hintDictionary[viewId] = new HintProvider(hint);
        }
        public IHintProvider FindHintProvider(View view) {
            if(hintDictionary.ContainsKey(view.Id)){
                return hintDictionary[view.Id];
            }
            return null;
        }
        public static HintsRepository Instance {
            get {
                if(instance == null) {
                    instance = new HintsRepository();
                }
                return instance;
            }
        }
    }
}
