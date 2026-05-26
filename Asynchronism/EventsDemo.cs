using System;
using System.Collections.Generic;
using System.Text;

namespace Asynchronism
{
    internal class EventsDemo
    {
        public static event Action<int> OnWorkFinished;
        public static void Run()
        {
            OnWorkFinished += (i) => Console.WriteLine($"Work {i} finished!");
            for (var i = 0; i < 10; i++)
            {
                var j = i;
                ThreadPool.QueueUserWorkItem(argument =>
                {
                    //int myArg = (int)argument!;
                    Thread.Sleep(Random.Shared.Next(2000));

                    OnWorkFinished?.Invoke(j);
                });
            }

            Console.ReadLine();
        }
    }
}
