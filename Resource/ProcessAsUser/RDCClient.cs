using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ProcessAsUser {
    public static class RDCClient {
        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("WTSAPI32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool WTSQueryUserToken(Int32 sessionId, out IntPtr token);
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

        public enum WTSConnectstateClass {
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

        private const int WTSCurrentServerHandle = 0;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WTSSessionInfo {
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

        public struct SessionInfo {
            readonly IntPtr _intPtr;
            readonly WTSSessionInfo? _wtsSessionInfo;

            public SessionInfo(WTSSessionInfo? wtsSessionInfo, IntPtr intPtr)
                : this() {
                _wtsSessionInfo = wtsSessionInfo;
                _intPtr = intPtr;
            }

            public IntPtr IntPtr {
                get { return _intPtr; }
            }

            public WTSSessionInfo? Info {
                get { return _wtsSessionInfo; }
            }
        }

        private static WTSSessionInfo? GetWTSSessionInfo(string userName, uint sessionCount, IntPtr ppSessionInfo) {
            for (int i = 0; i < sessionCount; i++) {
                WTSSessionInfo? wtsSessionInfo = (WTSSessionInfo)Marshal.PtrToStructure(
                    ppSessionInfo + i * Marshal.SizeOf(typeof(WTSSessionInfo)),
                    typeof(WTSSessionInfo));
                if (wtsSessionInfo.Value.State == WTSConnectstateClass.WTSActive &&
                    GetUsernameBySessionId(wtsSessionInfo.Value.SessionID, false).ToLower() == userName.ToLower())
                    return wtsSessionInfo;
            }
            return null;
        }

        public static SessionInfo GetSessionInfo(string userName, string password) {
            var ppSessionInfo = IntPtr.Zero;
            UInt32 sessionCount = 0;
            var wtsEnumerateSessions = WTSEnumerateSessions((IntPtr)WTSCurrentServerHandle, 0, 1, ref ppSessionInfo, ref sessionCount);
            if (wtsEnumerateSessions) {
                var wtsSessionInfo = GetWTSSessionInfo(userName, sessionCount, ppSessionInfo);
                return wtsSessionInfo == null && Connect(userName, password)
                    ? GetSessionInfo(userName, password)
                    : new SessionInfo(wtsSessionInfo, ppSessionInfo);
            }
            return new SessionInfo(null, ppSessionInfo);
        }

        public static bool Connect(string userName, string password){
            var done = new ManualResetEventSlim();
            bool connect = false;
            var processCreationThread = new Thread(() => {
                var form = new Form1();
                connect = form.Connect(userName, password);
                done.Set();
            });
            processCreationThread.SetApartmentState(ApartmentState.STA);
            processCreationThread.Start();
            done.Wait();
            return connect;
        }

        public static IntPtr GetUserToken(WTSSessionInfo sessionInfo) {
            IntPtr logonUserToken;
            WTSQueryUserToken(sessionInfo.SessionID, out logonUserToken);
            return logonUserToken;
        }
    }
}
