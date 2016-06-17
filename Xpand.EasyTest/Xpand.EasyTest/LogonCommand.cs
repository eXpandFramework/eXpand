using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest {
    public class LogonCommand:Commands.LogonCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            base.InternalExecute(adapter);
        }
    }
}
