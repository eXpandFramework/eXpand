using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandProcessRecordCommand : ProcessRecordCommand{
        private ICommandAdapter _adapter;
        public const string Name = "XpandProcessRecord";

        protected override void InternalExecute(ICommandAdapter adapter){
            if (!adapter.IsWinAdapter()&&Parameters["Action"]==null){
                Parameters.Add(new Parameter("Action","Edit",true,StartPosition));
            }
            base.InternalExecute(adapter);
            _adapter = adapter;
        }

        protected override bool IsActionParameterName(string parameterName){
            bool isActionParameterName = base.IsActionParameterName(parameterName);
            if (isActionParameterName&&_adapter.IsWinAdapter()){
                var parameter = Parameters[Parameters.Count - 1];
                if (parameter.Value == "Edit"){
                    Parameters.Remove(parameter);
                    return false;
                }
            }
            return isActionParameterName;
        }
    }


}