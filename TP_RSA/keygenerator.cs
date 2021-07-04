using System;
using System.Collections.Generic;
using System.Text;
using static TP_RSA.ClassExtensionRandom;

namespace TP_RSA
{
    class keygenerator
    {
        public long Generator10char()
        {
            int i, flag = 0;
            long number, reducNum;
            Random random = new Random();
            number = ClassExtensionRandom.LongRandom(random, 1000000000, 9999999999);
            
            reducNum = number / 10000;
            for (i = 2; i <= reducNum; i++)
            {
                if (number % i == 0)
                {                    
                    flag = 1;
                    break;
                }
            }
            if (flag == 0)
                return number;
            else return 0;
        }
    }
}
