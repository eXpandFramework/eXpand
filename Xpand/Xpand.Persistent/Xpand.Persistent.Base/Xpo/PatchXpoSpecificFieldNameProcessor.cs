using DevExpress.Data.Filtering;

namespace Xpand.Persistent.Base.Xpo {
    public class PatchXpoSpecificFieldNameProcessor : DevExpress.Persistent.Base.CriteriaProcessorBase {
        readonly System.Collections.Generic.List<string> existingLookupFieldNames;
        readonly bool _remove=true;

        public PatchXpoSpecificFieldNameProcessor() {
        }

        public PatchXpoSpecificFieldNameProcessor(System.Collections.Generic.List<string> existingLookupFieldNames) {
            this.existingLookupFieldNames = existingLookupFieldNames;
            _remove = false;
        }

        protected override void Process(OperandProperty theOperand) {
            if (!_remove) {
                if (AggregateLevel == 0 && !theOperand.PropertyName.EndsWith("!")) {
                    string probeLookupFieldName = theOperand.PropertyName + '!';
                    if (existingLookupFieldNames.Contains(probeLookupFieldName)) {
                        theOperand.PropertyName = probeLookupFieldName;
                    }
                }
                base.Process(theOperand);
            }
            else {
                if (AggregateLevel == 0 && theOperand.PropertyName.EndsWith("!")) {
                    theOperand.PropertyName = theOperand.PropertyName.TrimEnd('!');
                }                
            }
        }
    }
}