using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace ProcessAsUser{
    public class UserSpecificProcess : Process{
        private const int StdInputHandle = -10;
        private const int StdErrorHandle = -12;
        private const int StartfUsestdhandles = 256;
        private static readonly IntPtr _invalidHandleValue = (IntPtr) (-1);
        public static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool CloseHandle(HandleRef handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool
            CreateProcess([MarshalAs(UnmanagedType.LPTStr)] string lpApplicationName, StringBuilder lpCommandLine,
                SecurityAttributes lpProcessAttributes, SecurityAttributes lpThreadAttributes, bool bInheritHandles,
                int dwCreationFlags, IntPtr lpEnvironment, [MarshalAs(UnmanagedType.LPTStr)] string lpCurrentDirectory,
                CreateProcessStartupInfo lpStartupInfo, CreateProcessProcessInformation lpProcessInformation);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcessAsUserW(IntPtr token,
            [MarshalAs(UnmanagedType.LPTStr)] string lpApplicationName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpCommandLine, SecurityAttributes lpProcessAttributes,
            SecurityAttributes lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment,
            [MarshalAs(UnmanagedType.LPTStr)] string lpCurrentDirectory, CreateProcessStartupInfo lpStartupInfo,
            CreateProcessProcessInformation lpProcessInformation);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetStdHandle(int whichHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
            SecurityAttributes lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes,
            HandleRef hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateNamedPipe(string name, int openMode, int pipeMode, int maxInstances,
            int outBufSize, int inBufSize, int timeout, SecurityAttributes lpPipeAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetConsoleOutputCP();

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DuplicateTokenEx(HandleRef hToken, int access, SecurityAttributes tokenAttributes,
            int impersonationLevel, int tokenType, ref IntPtr hNewToken);

        public void StartAsUser(IntPtr userToken) {
            if (StartInfo.UseShellExecute){
                throw new InvalidOperationException("can't call this with shell execute");
            }

            IntPtr primayUserToken = userToken;

            var startupInfo = new CreateProcessStartupInfo();
            var processInformation = new CreateProcessProcessInformation();

            IntPtr stdoutReadHandle = IntPtr.Zero;
            IntPtr stdoutWriteHandle = IntPtr.Zero;
            string commandLine = GetCommandLine();
            try{

                IntPtr stdinHandle = GetStdHandle(StdInputHandle);
                MyCreatePipe(out stdoutReadHandle, out stdoutWriteHandle, false);
                IntPtr stderrHandle = GetStdHandle(StdErrorHandle);

                startupInfo.dwFlags = StartfUsestdhandles;
                startupInfo.hStdInput = stdinHandle;
                startupInfo.hStdOutput = stdoutWriteHandle;
                startupInfo.hStdError = stderrHandle;

                const int creationFlags = 0;
                IntPtr environment = IntPtr.Zero;
                string workingDirectory = GetWorkingDirectory();

                if (!CreateProcessAsUserW(primayUserToken,null,commandLine,null,null,true,creationFlags,environment,workingDirectory,startupInfo,processInformation)){
                    throw new Win32Exception();
                }
                Trace.TraceInformation("-----Process Created-------");
                Trace.TraceInformation("cmd=" + commandLine);
                Trace.TraceInformation("hprocess=" + processInformation.hProcess);
                var processById = GetProcessById(processInformation.dwProcessId);
                processById.WaitForExit();
                Trace.TraceInformation("Process finished");
            }
            catch (Exception e){
                Trace.TraceError(e.ToString());
            }
            finally{

                if (processInformation.hThread != _invalidHandleValue){
                    CloseHandle(new HandleRef(this, processInformation.hThread));
                }


                CloseHandle(new HandleRef(this, stdoutWriteHandle));
            }


            if (processInformation.hProcess == IntPtr.Zero){
                throw new Exception("failed to create process");
            }
            Encoding encoding = Encoding.GetEncoding(GetConsoleOutputCP());
            var fileStream = new FileStream(new SafeFileHandle(stdoutReadHandle,true), FileAccess.Read,  4096, true);
            var standardOutput = new StreamReader(fileStream, encoding);
            Trace.TraceInformation("----------------------------------stdOutput----------------------------------");
            Trace.TraceInformation(standardOutput.ReadToEnd());
        }



        /// <summary>
        ///     Gets the appropriate commandLine from the process.
        /// </summary>
        /// <returns></returns>
        private string GetCommandLine(){
            var builder1 = new StringBuilder();
            string text1 = StartInfo.FileName.Trim();
            string text2 = StartInfo.Arguments;
            bool flag1 = text1.StartsWith("\"") && text1.EndsWith("\"");
            if (!flag1){
                builder1.Append("\"");
            }
            builder1.Append(text1);
            if (!flag1){
                builder1.Append("\"");
            }
            if ((text2.Length > 0)){
                builder1.Append(" ");
                builder1.Append(text2);
            }
            return builder1.ToString();
        }

        /// <summary>
        ///     Gets the working directory or returns null if an empty string was found.
        /// </summary>
        /// <returns></returns>
        private string GetWorkingDirectory(){
            return (StartInfo.WorkingDirectory != string.Empty)
                ? StartInfo.WorkingDirectory
                : null;
        }

        /// <summary>
        ///     A clone of Process.CreatePipe. This is only implemented because reflection with
        ///     out parameters are a pain.
        ///     Note: This is only finished for w2k and higher machines.
        /// </summary>
        /// <param name="parentHandle"></param>
        /// <param name="childHandle"></param>
        /// <param name="parentInputs">Specifies whether the parent will be performing the writes.</param>
        public static void MyCreatePipe(out IntPtr parentHandle, out IntPtr childHandle, bool parentInputs){
            string pipename = @"\\.\pipe\Global\" + Guid.NewGuid();

            var attributes2 = new SecurityAttributes{bInheritHandle = false};

            parentHandle = CreateNamedPipe(pipename, 0x40000003, 0, 0xff, 0x1000, 0x1000, 0, attributes2);
            if (parentHandle == _invalidHandleValue){
                throw new Win32Exception();
            }

            var attributes3 = new SecurityAttributes{bInheritHandle = true};
            int num1 = 0x40000000;
            if (parentInputs){
                num1 = -2147483648;
            }
            childHandle = CreateFile(pipename, num1, 3, attributes3, 3, 0x40000080, NullHandleRef);
            if (childHandle == _invalidHandleValue){
                throw new Win32Exception();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CreateProcessProcessInformation{
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;

            public CreateProcessProcessInformation(){
                hProcess = IntPtr.Zero;
                hThread = IntPtr.Zero;
                dwProcessId = 0;
                dwThreadId = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CreateProcessStartupInfo{
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;

            public CreateProcessStartupInfo(){
                cb = Marshal.SizeOf(typeof (CreateProcessStartupInfo));
                lpReserved = null;
                lpDesktop = null;
                lpTitle = null;
                dwX = 0;
                dwY = 0;
                dwXSize = 0;
                dwYSize = 0;
                dwXCountChars = 0;
                dwYCountChars = 0;
                dwFillAttribute = 0;
                dwFlags = 0;
                wShowWindow = 0;
                cbReserved2 = 0;
                lpReserved2 = IntPtr.Zero;
                hStdInput = IntPtr.Zero;
                hStdOutput = IntPtr.Zero;
                hStdError = IntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SecurityAttributes{
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;

            public SecurityAttributes(){
                nLength = Marshal.SizeOf(typeof (SecurityAttributes));
            }
        }
    }
}

