using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace eXpand.Utils.BackingFieldResolver {
    public sealed class Instruction {
        private OpCode opcode;
        private object operand;

        internal Instruction(int offset, OpCode opCode, object operand)
            : this(offset, opCode) {
            this.operand = operand;
        }

        internal Instruction(int offset, OpCode opCode) {
            Offset = offset;
            opcode = opCode;
        }

        internal Instruction(OpCode opCode, object operand)
            : this(0, opCode, operand) {
        }

        internal Instruction(OpCode opCode)
            : this(0, opCode) {
        }

        public int Offset { get; set; }

        public OpCode OpCode {
            get { return opcode; }
            set { opcode = value; }
        }

        public object Operand {
            get { return operand; }
            set { operand = value; }
        }

        public Instruction Previous { get; set; }

        public Instruction Next { get; set; }

        public int GetSize() {
            int size = opcode.Size;

            switch (opcode.OperandType) {
                case OperandType.InlineSwitch:
                    size += (1 + ((int[]) operand).Length)*4;
                    break;
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    size += 8;
                    break;
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    size += 4;
                    break;
                case OperandType.InlineVar:
                    size += 2;
                    break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    size += 1;
                    break;
            }

            return size;
        }

        public override string ToString() {
            return opcode.Name;
        }
    }

    internal class MethodBodyReader {
        private static readonly OpCode[] one_byte_opcodes;
        private static readonly OpCode[] two_bytes_opcodes;

        private readonly MethodBody body;
        private Instruction instruction;
        private readonly List<Instruction> instructions = new List<Instruction>();
        private readonly IList<LocalVariableInfo> locals;
        private readonly MethodBase method;
        private readonly Type[] method_arguments;
        private readonly Module module;
        private readonly ParameterInfo[] parameters;
        private readonly ByteBuffer raw_il;
        private readonly Type[] type_arguments;

        static MethodBodyReader() {
            one_byte_opcodes = new OpCode[0xe1];
            two_bytes_opcodes = new OpCode[0x1f];

            foreach (FieldInfo field in GetOpCodeFields()) {
                var opcode = (OpCode) field.GetValue(null);
                if (opcode.OpCodeType == OpCodeType.Nternal)
                    continue;

                if (opcode.Size == 1)
                    one_byte_opcodes[opcode.Value] = opcode;
                else
                    two_bytes_opcodes[opcode.Value & 0xff] = opcode;
            }
        }

        private MethodBodyReader(MethodBase method) {
            this.method = method;

            body = method.GetMethodBody();
            if (body == null)
                throw new ArgumentException();

            byte[] bytes = body.GetILAsByteArray();
            if (bytes == null)
                throw new ArgumentException();

            if (!(method is ConstructorInfo))
                method_arguments = method.GetGenericArguments();

            if (method.DeclaringType != null)
                type_arguments = method.DeclaringType.GetGenericArguments();

            parameters = method.GetParameters();
            locals = body.LocalVariables;
            module = method.Module;
            raw_il = new ByteBuffer(bytes);
        }

        private static IEnumerable<FieldInfo> GetOpCodeFields() {
            return typeof (OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
        }

        private void ReadInstructions() {
            while (raw_il.position < raw_il.buffer.Length) {
                CreateInstruction();
                ReadInstruction();
                instructions.Add(instruction);
            }
        }

        private void CreateInstruction() {
            Instruction previous = instruction;
            instruction = new Instruction(raw_il.position, ReadOpCode());

            if (previous != null) {
                instruction.Previous = previous;
                previous.Next = instruction;
            }
        }

        private void ReadInstruction() {
            switch (instruction.OpCode.OperandType) {
                case OperandType.InlineNone:
                    break;
                case OperandType.InlineSwitch:
                    int length = raw_il.ReadInt32();
                    var branches = new int[length];
                    var offsets = new int[length];
                    for (int i = 0; i < length; i++)
                        offsets[i] = raw_il.ReadInt32();
                    for (int i = 0; i < length; i++)
                        branches[i] = raw_il.position + offsets[i];

                    instruction.Operand = branches;
                    break;
                case OperandType.ShortInlineBrTarget:
                    instruction.Operand = raw_il.position - (sbyte) raw_il.ReadByte();
                    break;
                case OperandType.InlineBrTarget:
                    instruction.Operand = raw_il.position - raw_il.ReadInt32();
                    break;
                case OperandType.ShortInlineI:
                    if (instruction.OpCode == OpCodes.Ldc_I4_S)
                        instruction.Operand = (sbyte) raw_il.ReadByte();
                    else
                        instruction.Operand = raw_il.ReadByte();
                    break;
                case OperandType.InlineI:
                    instruction.Operand = raw_il.ReadInt32();
                    break;
                case OperandType.ShortInlineR:
                    instruction.Operand = raw_il.ReadSingle();
                    break;
                case OperandType.InlineR:
                    instruction.Operand = raw_il.ReadDouble();
                    break;
                case OperandType.InlineI8:
                    instruction.Operand = raw_il.ReadInt64();
                    break;
                case OperandType.InlineSig:
                    instruction.Operand = module.ResolveSignature(raw_il.ReadInt32());
                    break;
                case OperandType.InlineString:
                    instruction.Operand = module.ResolveString(raw_il.ReadInt32());
                    break;
                case OperandType.InlineTok:
                    instruction.Operand = module.ResolveMember(raw_il.ReadInt32(), type_arguments, method_arguments);
                    break;
                case OperandType.InlineType:
                    instruction.Operand = module.ResolveType(raw_il.ReadInt32(), type_arguments, method_arguments);
                    break;
                case OperandType.InlineMethod:
                    instruction.Operand = module.ResolveMethod(raw_il.ReadInt32(), type_arguments, method_arguments);
                    break;
                case OperandType.InlineField:
                    instruction.Operand = module.ResolveField(raw_il.ReadInt32(), type_arguments, method_arguments);
                    break;
                case OperandType.ShortInlineVar:
                    instruction.Operand = GetVariable(raw_il.ReadByte());
                    break;
                case OperandType.InlineVar:
                    instruction.Operand = GetVariable(raw_il.ReadInt16());
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private object GetVariable(int index) {
            if (TargetsLocalVariable(instruction.OpCode))
                return GetLocalVariable(index);
            return GetParameter(index);
        }

        private static bool TargetsLocalVariable(OpCode opcode) {
            return opcode.Name.Contains("loc");
        }

        private LocalVariableInfo GetLocalVariable(int index) {
            return locals[index];
        }

        private ParameterInfo GetParameter(int index) {
            if (!method.IsStatic)
                index--;

            return parameters[index];
        }

        private OpCode ReadOpCode() {
            byte il_opcode = raw_il.ReadByte();
            return il_opcode != 0xfe
                       ? one_byte_opcodes[il_opcode]
                       : two_bytes_opcodes[raw_il.ReadByte()];
        }

        public static List<Instruction> GetInstructions(MethodBase method) {
            var reader = new MethodBodyReader(method);
            reader.ReadInstructions();
            return reader.instructions;
        }
        #region Nested type: ByteBuffer
        private class ByteBuffer {
            internal readonly byte[] buffer;
            internal int position;

            public ByteBuffer(byte[] buffer) {
                this.buffer = buffer;
            }

            public byte ReadByte() {
                CheckCanRead(1);
                return buffer[position++];
            }

            private byte[] ReadBytes(int length) {
                CheckCanRead(length);
                var bytes = new byte[length];
                Buffer.BlockCopy(buffer, position, bytes, 0, length);
                position += length;
                return bytes;
            }

            public short ReadInt16() {
                CheckCanRead(2);
                var @short = (short) (buffer[position]
                                      + (buffer[position + 1] << 8));
                position += 2;
                return @short;
            }

            public int ReadInt32() {
                CheckCanRead(4);
                int @int = buffer[position]
                           + (buffer[position + 1] << 8)
                           + (buffer[position + 2] << 16)
                           + (buffer[position + 3] << 24);
                position += 4;
                return @int;
            }

            public long ReadInt64() {
                CheckCanRead(8);
                long @long = buffer[position]
                             + (buffer[position + 1] << 8)
                             + (buffer[position + 2] << 16)
                             + (buffer[position + 3] << 24)
                             + (buffer[position + 4] << 32)
                             + (buffer[position + 5] << 40)
                             + (buffer[position + 6] << 48)
                             + (buffer[position + 7] << 56);
                position += 8;
                return @long;
            }

            public float ReadSingle() {
                if (!BitConverter.IsLittleEndian) {
                    byte[] bytes = ReadBytes(4);
                    Array.Reverse(bytes);
                    return BitConverter.ToSingle(bytes, 0);
                }

                CheckCanRead(4);
                float value = BitConverter.ToSingle(buffer, position);
                position += 4;
                return value;
            }

            public double ReadDouble() {
                if (!BitConverter.IsLittleEndian) {
                    byte[] bytes = ReadBytes(8);
                    Array.Reverse(bytes);
                    return BitConverter.ToDouble(bytes, 0);
                }

                CheckCanRead(8);
                double value = BitConverter.ToDouble(buffer, position);
                position += 8;
                return value;
            }

            private void CheckCanRead(int count) {
                if (position + count > buffer.Length)
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }

    public static class MethodBaseRocks {
        public static IList<Instruction> GetInstructions(this MethodBase self) {
            return MethodBodyReader.GetInstructions(self).AsReadOnly();
        }
    }
}