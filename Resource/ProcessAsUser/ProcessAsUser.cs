using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ProcessAsUser{
    class ProcessAsUser{
        const UInt32 Infinite = 0xFFFFFFFF;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint exitCode);

        public struct SessionInfo{
            readonly IntPtr _intPtr;
            readonly WTSSessionInfo? _wtsSessionInfo;

            public SessionInfo(WTSSessionInfo? wtsSessionInfo, IntPtr intPtr) : this(){
                _wtsSessionInfo = wtsSessionInfo;
                _intPtr = intPtr;
            }

            public IntPtr IntPtr{
                get { return _intPtr; }
            }

            public WTSSessionInfo? Info{
                get { return _wtsSessionInfo; }
            }
        }

        static SessionInfo GetSessionInfo(string userName, string password) {
            var ppSessionInfo = IntPtr.Zero;
            UInt32 sessionCount = 0;
            var wtsEnumerateSessions = WTSEnumerateSessions((IntPtr) WTSCurrentServerHandle, 0, 1, ref ppSessionInfo,ref sessionCount);
            if (wtsEnumerateSessions){
                var wtsSessionInfo = GetWTSSessionInfo(userName, sessionCount, ppSessionInfo);
                return wtsSessionInfo == null && RDCClient.Connect(userName,password)
                    ? GetSessionInfo(userName, password)
                    : new SessionInfo(wtsSessionInfo, ppSessionInfo);
            }
            return new SessionInfo(null, ppSessionInfo);
        }


        private static WTSSessionInfo? GetWTSSessionInfo(string userName, uint sessionCount, IntPtr ppSessionInfo){
            for (int i = 0; i < sessionCount; i++){
                WTSSessionInfo? wtsSessionInfo = (WTSSessionInfo) Marshal.PtrToStructure(
                    ppSessionInfo + i*Marshal.SizeOf(typeof (WTSSessionInfo)),
                    typeof (WTSSessionInfo));
                if (wtsSessionInfo.Value.State == WTSConnectstateClass.WTSActive &&
                    GetUsernameBySessionId(wtsSessionInfo.Value.SessionID, false).ToLower() == userName.ToLower())
                    return wtsSessionInfo;
            }
            return null;
        }

        private static ProcessInformation? CreateProcess(string childProcName, IntPtr logonUserToken, string arguments) {
            ProcessInformation processInformation;
            var tStartUpInfo = new Startupinfo { cb = Marshal.SizeOf(typeof(Startupinfo)) };

            bool childProcStarted = CreateProcessAsUser(
                logonUserToken,             
                childProcName,      
                Path.GetFileName(childProcName) + " " + arguments,               
                IntPtr.Zero,        
                IntPtr.Zero,        
                false,              // Does NOT inherit parent's handles.
                0,                  // No any specific creation flag.
                null,               // Default environment path.
                null,               // Default current directory.
                ref tStartUpInfo,   // Process Startup Info. 
                out processInformation    // Process information to be returned.
                );
            return childProcStarted ? (ProcessInformation?) processInformation : null;
        }

        static IntPtr GetUserToken(int sessionID){
            IntPtr logonUserToken;
            WTSQueryUserToken(sessionID, out logonUserToken);
            return logonUserToken;
        }

        public static void Launch(string userName, string password, string processPath, string arguments,Action<int> exitCode) {
            var sessionInfo = GetSessionInfo(userName,password);
            if (sessionInfo.Info != null) {
                var wtsSessionInfo = sessionInfo.Info.Value;
                var userToken = GetUserToken(wtsSessionInfo.SessionID);
                var processInformation = CreateProcess(processPath, userToken, arguments);
                if (processInformation != null) {
                    var tProcessInfo = processInformation.Value;
                    WaitForSingleObject(tProcessInfo.hProcess, Infinite);
                    uint code;
                    var exitCodeProcess = GetExitCodeProcess(processInformation.Value.hProcess, out code);
                    if (exitCodeProcess)
                        exitCode((int) code);
                    CloseHandle(tProcessInfo.hThread);
                    CloseHandle(tProcessInfo.hProcess);
                }
                CloseHandle(userToken);
            }
            else{
                throw new Exception("Login");
            }
            WTSFreeMemory(sessionInfo.IntPtr);
        }


        #region P/Invoke WTS APIs
        /// <summary>
        /// Struct, Enum and P/Invoke Declarations of WTS APIs.
        /// </summary>
        /// 

        private const int WTSCurrentServerHandle = 0;

        public enum WTSConnectstateClass
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }
        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

        /// <summary>
        /// Contains values that indicate the type of session information to retrieve in a call to the <see cref="WTSQuerySessionInformation"/> function.
        /// </summary>
        public enum WtsInfoClass {
            WTSInitialProgram,


            WTSApplicationName,


            WTSWorkingDirectory,


            WTSOEMId,


            WTSSessionId,


            WTSUserName,


            WTSWinStationName,

            WTSDomainName,


            WTSConnectState,

            WTSClientBuildNumber,


            WTSClientName,


            WTSClientDirectory,


            WTSClientProductId,


            WTSClientHardwareId,


            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,

            WTSIdleTime,

            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo
        }

        public static string GetUsernameBySessionId(int sessionId, bool prependDomain) {
            IntPtr buffer;
            int strLen;
            string username = "SYSTEM";
            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSUserName, out buffer, out strLen) && strLen > 1) {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
                if (prependDomain) {
                    if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSDomainName, out buffer, out strLen) && strLen > 1) {
                        username = Marshal.PtrToStringAnsi(buffer) + "\\" + username;
                        WTSFreeMemory(buffer);
                    }
                }
            }
            return username;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WTSSessionInfo
        {
            public Int32 SessionID;
            public string pWinStationName;
            public WTSConnectstateClass State;
        }

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] UInt32 reserved,
            [MarshalAs(UnmanagedType.U4)] UInt32 version,
            ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref UInt32 pSessionInfoCount
            );

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSQueryUserToken(Int32 sessionId, out IntPtr token);
        #endregion


        #region P/Invoke CreateProcessAsUser
        /// <summary>
        /// Struct, Enum and P/Invoke Declarations for CreateProcessAsUser.
        /// </summary>
        /// 

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct Startupinfo
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ProcessInformation{
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("ADVAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            string lpEnvironment,
            string lpCurrentDirectory,
            ref Startupinfo lpStartupInfo,
            out ProcessInformation lpProcessInformation
            );
//        [DllImport("kernel32.dll", SetLastError = true)]
//        static extern bool CreateProcess(string lpApplicationName,
//           string lpCommandLine, ref SecurityAttributes lpProcessAttributes,
//           ref SecurityAttributes lpThreadAttributes, bool bInheritHandles,
//           uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
//           [In] ref Startupinfo lpStartupInfo,
//           out ProcessInformation lpProcessInformation);
        [StructLayout(LayoutKind.Sequential)]
        public struct SecurityAttributes {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }
        [DllImport("KERNEL32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CloseHandle(IntPtr hHandle);
        #endregion
    }
}
