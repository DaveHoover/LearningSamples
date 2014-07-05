using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelAndTasksLearning
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ParallelExamples
    {

#if DEBUG
        const int SIZE = 200;
#else
  const int SIZE = 768;
#endif
        static int[,] m1 = new int[SIZE, SIZE];
        static int[,] m2 = new int[SIZE, SIZE];
        static int[,] res1 = new int[SIZE, SIZE]; // seq
        static int[,] res2 = new int[SIZE, SIZE]; // pfor
        static int[,] res3 = new int[SIZE, SIZE]; // tasks
        static int[,] res4 = new int[SIZE, SIZE]; // pool

        public static async Task<List<string>> ParallelStart()
        {
            List<string> sumary = new List<string>();
            // Populate arrays
            Random rgen = new Random(69);

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    m1[i, j] = rgen.Next();
                }
            }

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    m2[i, j] = rgen.Next();
                }
            }

            Stopwatch sw;
            #region Sequential
            Console.Write("Sequential: ");
            sw = Stopwatch.StartNew();
            MulSeq(SIZE, m1, m2, res1);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString());
            sumary.Add("Sequential: " + sw.ElapsedMilliseconds.ToString());
            #endregion

            #region ParalleFor
            Console.Write("ParalleFor: ");
            sw = Stopwatch.StartNew();
            MulPFor(SIZE, m1, m2, res2);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString());
            sumary.Add("ParalleFor: " + sw.ElapsedMilliseconds.ToString());
            #endregion

            #region Tasks
            Console.Write("Tasks wait all     : ");
            sw = Stopwatch.StartNew();
            MulTask(SIZE, m1, m2, res3);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString());
            sumary.Add("Tasks wait all     : " + sw.ElapsedMilliseconds.ToString()); 
            #endregion

            #region Tasks
            Console.Write("Async Tasks when all     : ");
            sw = Stopwatch.StartNew();
            await MulTaskAwait(SIZE, m1, m2, res3);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString());
            sumary.Add("Async Tasks when all     : " + sw.ElapsedMilliseconds.ToString()); 
            #endregion


            #region ThreadPool
            Console.Write("ThreadPool: ");
            sw = Stopwatch.StartNew();
            MulPool(SIZE, m1, m2, res4);
            Console.WriteLine(sw.ElapsedMilliseconds.ToString());
            sumary.Add("ThreadPool: " + sw.ElapsedMilliseconds.ToString()); 
            #endregion
            return sumary;
        }

        public static void MulSeq(int size, int[,] m1, int[,] m2, int[,] result)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int tmp = 0;
                    for (int k = 0; k < size; k++)
                    {
                        tmp += m1[i, k] * m2[k, j];
                    }
                    result[i, j] = tmp;
                }
            }
        }
        public static void MulPFor(int size, int[,] m1, int[,] m2, int[,] result)
        {
            Parallel.For(0, size, (int i) =>
            {
                for (int j = 0; j < size; j++)
                {
                    int tmp = 0;
                    for (int k = 0; k < size; k++)
                    {
                        tmp += m1[i, k] * m2[k, j];
                    }
                    result[i, j] = tmp;
                }
            });
        }

        public static void MulTask(int size, int[,] m1, int[,] m2, int[,] result)
        {
            Task[] tasks = new Task[size];
            for (int n = 0; n < size; n++)
            {
                int i = n;
                tasks[n] = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < size; j++)
                    {
                        int tmp = 0;
                        for (int k = 0; k < size; k++)
                        {
                            tmp += m1[i, k] * m2[k, j];
                        }
                        result[i, j] = tmp;
                    }
                    Debug.WriteLine("done " + Task.CurrentId.ToString());
                });
            }
            Task.WaitAll(tasks);
        }

        public async static Task MulTaskAwait(int size, int[,] m1, int[,] m2, int[,] result)
        {
            Task[] tasks = new Task[size];
            for (int n = 0; n < size; n++)
            {
                int i = n;
                tasks[n] = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < size; j++)
                    {
                        int tmp = 0;
                        for (int k = 0; k < size; k++)
                        {
                            tmp += m1[i, k] * m2[k, j];
                        }
                        result[i, j] = tmp;
                    }
                    Debug.WriteLine("done " + Task.CurrentId.ToString());
                });
            }
            await Task.WhenAll(tasks);
        }


        public static void MulPool(int size, int[,] m1, int[,] m2, int[,] result)
        {
            ManualResetEvent signal = new ManualResetEvent(false);
            long iters = size;
            for (int n = 0; n < size; n++)
            {
                int i = n;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (int j = 0; j < size; j++)
                    {
                        int tmp = 0;
                        for (int k = 0; k < size; k++)
                        {
                            tmp += m1[i, k] * m2[k, j];
                        }
                        result[i, j] = tmp;
                    }

                    if (Interlocked.Decrement(ref iters) == 0)
                        signal.Set();
                });
            }
            signal.WaitOne();
        }
    }
}
