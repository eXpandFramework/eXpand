using System;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.AOP.DataStore
{
    public class StatementArgs : EventArgs
    {
        public BaseStatement[] Statements { get; set; }

        public StatementArgs(BaseStatement[] statements)
        {
            Statements = statements;
        }

//        public ModificationStatement[] ModificationStatements { get; set; }
    }
}