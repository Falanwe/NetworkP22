using System;
using System.Collections.Generic;
using System.Text;

namespace Asynchronism
{
    internal class Threads
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

        public void Run()
        {
            ParameterizedThreadStart myThreadDelegate = (object? o) =>
            {
                var threadName = o as string ?? "";
                var time = Random.Shared.Next(2000);
                Thread.Sleep(time);
                Console.WriteLine($"Hello world from thread {threadName} after {time} ms!");
            };

            var t1 = new System.Threading.Thread(myThreadDelegate);
            t1.Start("1");

            var t2 = new System.Threading.Thread(myThreadDelegate);
            t2.Start("2");

            var t3 = new System.Threading.Thread(myThreadDelegate);
            t3.Start("3");

            var t4 = new System.Threading.Thread(myThreadDelegate);
            t4.Start("4");

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();
        }
    }
}
