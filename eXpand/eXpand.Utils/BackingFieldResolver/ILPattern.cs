using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace eXpand.Utils.BackingFieldResolver {
    internal abstract class ILPattern {
        public static ILPattern Optional(OpCode opcode) {
            return Optional(OpCode(opcode));
        }

        public static ILPattern Optional(ILPattern pattern) {
            return new OptionalPattern(pattern);
        }

        public static ILPattern Sequence(params ILPattern[] patterns) {
            return new SequencePattern(patterns);
        }

        public static ILPattern OpCode(OpCode opcode) {
            return new OpCodePattern(opcode);
        }

        public static ILPattern Either(ILPattern a, ILPattern b) {
            return new EitherPattern(a, b);
        }

        public static ILPattern Field(OpCode opcode) {
            return new FieldPattern(new OpCodePattern(opcode));
        }

        internal abstract void Match(MatchContext context);

        internal bool TryMatch(MatchContext context) {
            Instruction instruction = context._instruction;
            Match(context);

            if (context.success)
                return true;

            context.Reset(instruction);
            return false;
        }

        public static MatchContext Match(MethodBase method, ILPattern pattern) {
            IList<Instruction> instructions = method.GetInstructions();
            if (instructions.Count == 0)
                throw new ArgumentException();

            var context = new MatchContext(instructions[0]);
            pattern.Match(context);
            return context;
        }
        #region Nested type: EitherPattern
        private class EitherPattern : ILPattern {
            private readonly ILPattern a;
            private readonly ILPattern b;

            public EitherPattern(ILPattern a, ILPattern b) {
                this.a = a;
                this.b = b;
            }

            internal override void Match(MatchContext context) {
                if (!a.TryMatch(context))
                    b.Match(context);
            }
        }
        #endregion
        #region Nested type: FieldPattern
        private class FieldPattern : ILPattern {
            private readonly ILPattern pattern;

            public FieldPattern(ILPattern pattern) {
                this.pattern = pattern;
            }

            internal override void Match(MatchContext context) {
                if (!pattern.TryMatch(context)) {
                    context.success = false;
                    return;
                }

                context.field = (FieldInfo) context._instruction.Previous.Operand;
            }
        }
        #endregion
        #region Nested type: OpCodePattern
        private class OpCodePattern : ILPattern {
            private readonly OpCode opcode;

            public OpCodePattern(OpCode opcode) {
                this.opcode = opcode;
            }

            internal override void Match(MatchContext context) {
                if (context._instruction == null) {
                    context.success = false;
                    return;
                }

                context.success = context._instruction.OpCode == opcode;
                context.Advance();
            }
        }
        #endregion
        #region Nested type: OptionalPattern
        private class OptionalPattern : ILPattern {
            private readonly ILPattern pattern;

            public OptionalPattern(ILPattern optional) {
                pattern = optional;
            }

            internal override void Match(MatchContext context) {
                pattern.TryMatch(context);
            }
        }
        #endregion
        #region Nested type: SequencePattern
        private class SequencePattern : ILPattern {
            private readonly ILPattern[] patterns;

            public SequencePattern(ILPattern[] patterns) {
                this.patterns = patterns;
            }

            internal override void Match(MatchContext context) {
                foreach (ILPattern pattern in patterns) {
                    pattern.Match(context);

                    if (!context.success)
                        break;
                }
            }
        }
        #endregion
    }
}