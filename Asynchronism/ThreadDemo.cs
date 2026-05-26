using System;
using System.Collections.Generic;
using System.Text;

namespace Asynchronism
{
    internal class ThreadDemo
    {


        void HelloWorld()
        {
                Console.WriteLine("Hello world!");
        }



        void PrintInConsole(string message)
        {
            Console.WriteLine(message);
        }

        long Increment(int number)
        {
            return number + 1;
        }

        public static void Run()
        {
            int maxExecutionTime = 2000;
            void DoLongWork (object? o)
            {
                var threadName = o as string ?? "";
                var time = Random.Shared.Next(maxExecutionTime);
                Thread.Sleep(time);
                Console.WriteLine($"Hello world from thread {threadName} after {time} ms!");
            };

            var t1 = new System.Threading.Thread(DoLongWork);
            t1.Start("1");

            var t2 = new System.Threading.Thread(DoLongWork);
            t2.Start("2");

            var t3 = new System.Threading.Thread(DoLongWork);
            t3.Start("3");

            var t4 = new System.Threading.Thread(DoLongWork);
            t4.Start("4");

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();
        }
    }
}
