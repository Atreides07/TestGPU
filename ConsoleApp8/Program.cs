using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alea;
using Alea.CSharp;
using Alea.Parallel;

namespace ConsoleApp8
{
    class Program
    {
        static void Main(string[] args)
        {

            //for (var i = 0; i < 10; i++)
            //{
            //    var sw = new Stopwatch();
            //    sw.Start();
            //    var s1 = TestCPU();
            //    sw.Stop();
            //    Console.WriteLine($"GPU1: {sw.Elapsed.Milliseconds} Summ: {s1}");
            //    Console.WriteLine();



            //    //var sw = new Stopwatch();
            //    //sw.Start();
            //    //var s=TestGPU();
            //    //sw.Stop();
            //    //Console.WriteLine($"GPU2: {sw.Elapsed.Milliseconds} Summ: {s}");
            //    //Console.WriteLine();
            //}
            var s1 = TestCPU();
            var s2 = TestGPU();

            //Enumerable.Re
        }

        [GpuManaged]
        private static int TestGPU()
        {
            var length = 32_000_000;
            var gpu = Gpu.Default;
            var a1 = Enumerable.Repeat(1, length).ToArray();
            var a2 = Enumerable.Repeat(1, length).ToArray();
            var r = new int[length];


            int[] arg1 = gpu.Allocate<int>(a1);
            int[] arg2 = gpu.Allocate<int>(a2);
            int[] result = gpu.Allocate<int>(r);

            for (var i = 0; i < 10; i++)
            {
                var sw = new Stopwatch();
                sw.Start();
                var s = TestSummGPU(gpu, length, arg1, arg2, result);
                sw.Stop();
                Console.WriteLine($"GPU1: {sw.Elapsed.Milliseconds} Summ: {s}");
                Console.WriteLine();
            }

            
            Gpu.Free(arg1);
            Gpu.Free(arg2);
            Gpu.Free(result);

            return 0;
        }

        private static int TestSummGPU(Gpu gpu, int length, int[] arg1, int[] arg2, int[] result)
        {
            //gpu.For(0, length, i => arg1[i] = 1);
            //gpu.For(0, length, i => arg2[i] = 1);
            gpu.For(0, length, i => result[i] = arg1[i] + arg2[i]);
            var s = gpu.Sum(result);
            //gpu.Synchronize();
            return s;
        }


        private static int TestCPU()
        {
            var Length = 32_000_000;
            //var gpu = Gpu.Default;
            var arg1 = Enumerable.Repeat(1, Length).ToArray();
            var arg2 = Enumerable.Repeat(1, Length).ToArray();
            var result = new int[Length];

            //for (var k = 0; k < 10; k++)
            //{
            //    var sw = new Stopwatch();
            //    sw.Start();
            //    Gpu.Default.For(0, result.Length, i => result[i] = arg1[i] + arg2[i]);
            //    var s = Gpu.Default.Sum(result);
            //    //var s = result.Sum();
            //    sw.Stop();
            //    Console.WriteLine($"GPU2: {sw.Elapsed.Milliseconds} Summ: {s}");
            //    Console.WriteLine();
            //}

            for (var k = 0; k < 10; k++)
            {
                var sw = new Stopwatch();
                sw.Start();
                Parallel.For(0, Length, i => result[i] = arg2[i] + arg1[i]);
                
                var s = Gpu.Default.Sum(result);
                sw.Stop();
                Console.WriteLine($"CPU2: {sw.Elapsed.Milliseconds} Summ: {s}");
                Console.WriteLine();
            }


            //sw.Stop();
            //Console.WriteLine(sw.Elapsed.Milliseconds);
            //sw.Restart();
            //for (var i = 0; i < result.Length; i++)
            //{
            //    result[i] = arg1[i] + arg2[i];
            //}


            return 0;
        }
    }
}
