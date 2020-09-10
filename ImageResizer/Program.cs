using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output"); ;

            ImageProcess imageProcess = new ImageProcess();
            ImageProcessAdvance imageProcessAdvance = new ImageProcessAdvance();
            ImageProcessParallel imageProcessParallel = new ImageProcessParallel();
            Stopwatch sw = new Stopwatch();

            #region Original Code

            imageProcess.Clean(destinationPath);
            sw.Start();
            imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
            sw.Stop();
            var syncTime = sw.ElapsedMilliseconds;

            #endregion

            #region Async Version
           
            imageProcessAdvance.Clean(destinationPath);
            sw.Reset();
            sw.Start();
            // 
            var asyncTask = imageProcessAdvance.ResizeImagesAsync(sourcePath, destinationPath, 2.0);
            await asyncTask;

            sw.Stop();
            var asyncTime = sw.ElapsedMilliseconds;
            double performanceRatio = ((double)(syncTime - asyncTime)/(double)syncTime) * 100;
            #endregion

            #region Parallel - only for self experiment

            //imageProcessParallel.Clean(destinationPath);
            //sw.Reset();
            //sw.Start();
            //imageProcessParallel.ResizeImagesParallel(sourcePath, destinationPath, 2.0);
            
            //sw.Stop();
            //var parallelTime = sw.ElapsedMilliseconds;

            #endregion

            Console.WriteLine($" [Sync] - 花費時間: {syncTime} ms");
            Console.WriteLine($" [Async] - 花費時間: {asyncTime} ms");
            Console.WriteLine($" 提升效率: { performanceRatio.ToString("N2") } %");

            //Console.WriteLine($" [Parallel] - 花費時間: {parallelTime} ms");
        }
    }
}
