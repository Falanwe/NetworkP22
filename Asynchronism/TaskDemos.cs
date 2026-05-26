using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Asynchronism
{
    internal class TaskDemos
    {
        public static async Task<int> WorkForARandomTime()
        {
            var delay = Random.Shared.Next(2000);
            await Task.Delay(delay);
            return delay;
        }

        public static void Run()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var delay = await WorkForARandomTime();
                    Console.WriteLine($"I waited for {delay} ms");
                    if (delay > 1900)
                    {
                        break;
                    }
                }
                Console.WriteLine("done");
            });
            Console.ReadLine();
        }

        public static void RunWithoutAsync()
        {
            Task Impl()
            {
                return WorkForARandomTime().ContinueWith(t =>
                {
                    var delay = t.Result;
                    Console.WriteLine($"I waited for {delay} ms");
                    if (delay > 1900)
                    {
                        Console.WriteLine("done");
                        return Task.CompletedTask;
                    }
                    else
                    {
                        return Impl();
                    }
                });
            }

            Impl();
            Console.ReadLine();
        }
    }
}
