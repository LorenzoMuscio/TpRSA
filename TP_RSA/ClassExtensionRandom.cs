using System;
using System.Collections.Generic;
using System.Text;

namespace TP_RSA
{
    public static class ClassExtensionRandom
    {
        public static long LongRandom(this Random rand, long min, long max)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}
