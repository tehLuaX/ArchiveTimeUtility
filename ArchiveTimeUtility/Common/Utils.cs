using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveTimeUtility.Common
{
    public static class Utils
    {
        public static long ToUnixEpoch(DateTime time)
        {
            return (long) time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
