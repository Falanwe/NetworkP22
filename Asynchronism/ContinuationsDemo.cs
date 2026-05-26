using System;
using System.Collections.Generic;
using System.Text;

namespace Asynchronism
{
    internal class ContinuationsDemo
    {
        public static void DoSomeAsyncWork(int workId, Action<int, int> continuation)
        {
            var delay = Random.Shared.Next(2000);
            Task.Run(async () =>
            {
                await Task.Delay(delay);
                continuation(workId, delay);
            }).ContinueWith(t => { });
        }

        public static void Run()
        {
            void Continuation(int workId, int delay)
            {
                Console.WriteLine($"Work {workId} completed after {delay} ms");
            }
            for (var i = 0; i < 10; i++)
            {
                DoSomeAsyncWork(i, Continuation);
            }
            
            Console.ReadLine();
        }
    }
}
