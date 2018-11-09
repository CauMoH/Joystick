using System;

namespace Joystick.Utils
{
    internal static class AppUtils
    {
        public const int SCAN_SECONDS_DEFAULT = 10;
        public const int SCAN_SECONDS_MAX = 30;
        public const int CONNECTION_TIMEOUT_SECONDS = 15;

        public static double ClampSeconds(double seconds)
        {
            return Math.Max(Math.Min(seconds, SCAN_SECONDS_MAX), 0);
        }
    }
}
