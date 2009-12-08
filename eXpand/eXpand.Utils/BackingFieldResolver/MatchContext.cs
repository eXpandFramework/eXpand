using System.Reflection;

namespace eXpand.Utils.BackingFieldResolver {
    internal class MatchContext {
        internal FieldInfo field;
        internal Instruction _instruction;
        internal bool success;

        public MatchContext(Instruction instruction) {
            Reset(instruction);
        }

        public void Reset(Instruction instruction) {
            _instruction = instruction;
            success = true;
        }

        public void Advance() {
            _instruction = _instruction.Next;
        }
    }
}