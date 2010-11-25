using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation {
    public class SystemAutomation {
        public static void Run(string applicationPath) {
            Process.Start(applicationPath);
            Application.DoEvents();
        }

        public static bool ChangeScreenResolution(int width, int height) {
            int iWidth = width;
            int iHeight = height;


            var dm = new Win32Types.DEVMODE {
                dmDeviceName = new String(new char[32]),
                dmFormName = new String(new char[32])
            };
            dm.dmSize = (short)Marshal.SizeOf(dm);

            if (0 !=
                Win32Declares.Display.EnumDisplaySettings(null, Win32Constants.EnumDisplaySettings.ENUM_CURRENT_SETTINGS,
                                                          ref dm)) {
                dm.dmPelsWidth = iWidth;
                dm.dmPelsHeight = iHeight;

                Win32Constants.ChangeDisplaySettingsResult iRet = Win32Declares.Display.ChangeDisplaySettings(ref dm,
                                                                                                              Win32Constants
                                                                                                                  .
                                                                                                                  ChangeDisplaySettingsFlags
                                                                                                                  .
                                                                                                                  CDS_TEST);

                if (iRet == Win32Constants.ChangeDisplaySettingsResult.DISP_CHANGE_FAILED) {
                    throw new CannotChangeScreenResolution(iWidth, iHeight);
                }
                iRet = Win32Declares.Display.ChangeDisplaySettings(ref dm,
                                                                   Win32Constants.ChangeDisplaySettingsFlags.
                                                                       CDS_UPDATEREGISTRY);

                switch (iRet) {
                    case Win32Constants.ChangeDisplaySettingsResult.DISP_CHANGE_RESTART: {
                            MessageBox.Show(
                                "Description: You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.",
                                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    case Win32Constants.ChangeDisplaySettingsResult.DISP_CHANGE_SUCCESSFUL: {
                            return true;
                        }
                }
            }
            return false;
        }
    }

    public class CannotChangeScreenResolution : Exception {
        readonly int _width;
        readonly int _height;

        public CannotChangeScreenResolution(int width, int height) {
            _width = width;
            _height = height;
        }

        public int Width {
            get { return _width; }
        }

        public int Height {
            get { return _height; }
        }
    }
}