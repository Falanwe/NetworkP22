using System;
using System.Collections.Generic;
using System.Text;

namespace Asynchronism
{
    internal struct MyBigStruct
    {
        public MyBigStruct(long value)
        {
            Value1 = value;
            Value2 = value;
            Value3 = value;
            Value4 = value;
        }

        public long Value1 { get; }
        public long Value2 { get; }
        public long Value3 { get; }
        public long Value4 { get; }
    }
       

    internal class TearingDemo
    {
        public static void Run()
        {
            var bigStruct = new MyBigStruct(0);

            void WriteIndefinitely(long i)
            {
                var localBigStruct = new MyBigStruct(i);
                while (true)
                {
                    bigStruct = localBigStruct;
                }
            }

            for(var i = 0L; i < 4; i++)
            {
                ThreadPool.QueueUserWorkItem(parameter => WriteIndefinitely((long)parameter), i);
            }

            var counter = 0;
            while(true)
            {
                counter++;

                var localBigStruct = bigStruct;

                var value1 = localBigStruct.Value1;
                var value2 = localBigStruct.Value2;
                var value3 = localBigStruct.Value3;
                var value4 = localBigStruct.Value4;
                if (value1 != value2 || value1 != value3 || value1 != value4)
                {
                    Console.WriteLine($"Tearing detected: {value1}, {value2}, {value3}, {value4} after {counter} iterations");
                    break;
                }
            }
        }
    }
}
