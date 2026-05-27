using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Asynchronism
{
    internal class AdditionGame
    {
        private static int _yellowValue = 0;
        private static int _redValue = 0;

#if RED_AND_YELLOW
        private static readonly object _synchRoot = new object();


        private static async Task UpdateYellowValue(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _yellowValue = Random.Shared.Next(100);
                lock (_synchRoot)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(_yellowValue);
                    Console.ResetColor();
                }
                try
                {
                    await Task.Delay(Random.Shared.Next(4000, 10_000), token);
                }
                catch (TaskCanceledException )
                {
                    // ignore
                }
            }
            Console.WriteLine("UpdateYellowValue stopped");
        }

        private static async Task UpdateRedValue(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _redValue = Random.Shared.Next(100);
                lock (_synchRoot)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(_redValue);
                    Console.ResetColor();
                }
                try
                {
                    await Task.Delay(Random.Shared.Next(4000, 10_000), token);
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }
            }
            Console.WriteLine("UpdateRedValue stopped");
        }
#endif

        public static async Task Run()
        {
            using var cts = new CancellationTokenSource();

            Console.WriteLine("Press Enter to start the addition game...");

            Console.ReadLine();

            Console.WriteLine("Game started!");

            await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(1000, 3000)));

#if RED_AND_YELLOW
            Console.WriteLine($"What is red + yellow?");

            _ = UpdateRedValue(cts.Token);
            _ = UpdateYellowValue(cts.Token);
#else
            _redValue = Random.Shared.Next(100);
            _yellowValue = Random.Shared.Next(100);
            Console.WriteLine($"{_redValue} + {_yellowValue} = ?");
#endif

            var watch = new Stopwatch();
            watch.Start();

            while (true)
            {
                var attempt = Console.ReadLine();
                if (attempt == (_redValue + _yellowValue).ToString())
                {
                    watch.Stop();
                    Console.WriteLine($"Correct! You took {watch.ElapsedMilliseconds} ms");

                    cts.Cancel();
                    break;
                }
                else
                {
                    Console.WriteLine($"Wrong!");
                }
            }

            Console.ReadLine();
        }
    }
}
