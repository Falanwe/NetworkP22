using System;
using System.Collections.Generic;
using System.Text;

namespace Asynchronism
{
    internal class ThreadPoolDemo
    {
        public static void Run()
        {
            int maxExecutionTime = 2000;
            void DoLongWork(object? o)
            {
                var threadName = o as string ?? "";
                var time = Random.Shared.Next(maxExecutionTime);
                Thread.Sleep(time);
                Console.WriteLine($"Hello world from thread {threadName} after {time} ms!");
            }
            ;
            for (int i = 1; i <= 4; i++)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(DoLongWork, i.ToString());
            }
            // Wait for user input to prevent the application from exiting immediately
            Console.ReadLine();
        }
    }
}
