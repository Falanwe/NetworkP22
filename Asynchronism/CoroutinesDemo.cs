using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Asynchronism
{
    internal class CoroutinesDemo
    {
        private static bool _isRunning = false;

        private static HashSet<IEnumerator> _coroutines = new HashSet<IEnumerator>();

        public static void StartCoroutineEngine()
        {
            if (_isRunning) return;
            _isRunning = true;
            Task.Run(async () =>
            {
                while (_isRunning)
                {
                    IEnumerator[] currentCoroutines;
                    lock (_coroutines)
                    {
                        currentCoroutines = _coroutines.ToArray();
                    }

                    foreach (var coroutine in currentCoroutines)
                    {
                        if (!coroutine.MoveNext())
                        {
                            _coroutines.Remove(coroutine);
                        }
                    }
                    await Task.Delay(200);
                }
            });
        }

        public static void StopCoroutineEngine()
        {
            _isRunning = false;
            lock (_coroutines)
            {
                _coroutines.Clear();
            }
        }

        public static void StartCoroutine(IEnumerator coroutine)
        {
            lock (_coroutines)
            {
                _coroutines.Add(coroutine);
            }
        }

        public static IEnumerator ExampleCoroutine()
        {
            Console.WriteLine("Coroutine started");
            yield return 1;
            yield return 1;
            yield return 1;
            yield return 1;
            yield return 1;
            Console.WriteLine("Coroutine resumed after 1 second");
            yield return 1;
            yield return 1;
            yield return 1;
            yield return 1;
            yield return 1;
            Console.WriteLine("Coroutine resumed after 2 seconds");
        }

        public static void Run()
        {
            StartCoroutineEngine();
            StartCoroutine(ExampleCoroutine());
            Console.ReadLine();
            StopCoroutineEngine();
        }
    }
}
