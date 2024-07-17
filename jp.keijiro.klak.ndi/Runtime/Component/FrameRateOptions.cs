using System;

namespace Klak.Ndi
{
    public enum FrameRateOptions
    {
        Standard_NTSC_24,
        Standard_NTSC_29_97,
        Standard_NTSC_59_94,
        Standard_PAL_25,
        Standard_PAL_50,
        Common_30,
        Common_60
    }

    public static class FrameRateOptionExt
    {
        public static void GetND(this FrameRateOptions opt, out int n, out int d)
        {
            switch (opt)
            {
                case FrameRateOptions.Standard_NTSC_24:
                    n = 24000;
                    d = 1001;
                    break;
                case FrameRateOptions.Standard_NTSC_29_97:
                    n = 30000;
                    d = 1001;
                    break;
                case FrameRateOptions.Standard_NTSC_59_94:
                    n = 60000;
                    d = 1001;
                    break;
                case FrameRateOptions.Standard_PAL_25:
                    n = 30000;
                    d = 1200;
                    break;
                case FrameRateOptions.Standard_PAL_50:
                    n = 60000;
                    d = 1200;
                    break;
                case FrameRateOptions.Common_30:
                    n = 30000;
                    d = 1000;
                    break;
                case FrameRateOptions.Common_60:
                    n = 60000;
                    d = 1000;
                    break;
                default:
                    throw new NotImplementedException("Not implemented " + nameof(opt), null);
            }
        }

        public static int GetUnityFrameTarget(this FrameRateOptions opt)
        {
            switch (opt)
            {
                case FrameRateOptions.Standard_NTSC_24:
                    return 24;
                case FrameRateOptions.Standard_NTSC_29_97:
                    return 30;
                case FrameRateOptions.Standard_NTSC_59_94:
                    return 60;
                case FrameRateOptions.Standard_PAL_25:
                    return 25;
                case FrameRateOptions.Standard_PAL_50:
                    return 50;
                case FrameRateOptions.Common_30:
                    return 30;
                case FrameRateOptions.Common_60:
                    return 60;
                default:
                    throw new NotImplementedException("Not implemented " + nameof(opt), null);
            }
        }
    }
}