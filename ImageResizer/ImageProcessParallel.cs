using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer
{
    public class ImageProcessParallel
    {
        private void LogThreadId(string message)
        {
            Console.WriteLine($"T_{Thread.CurrentThread.ManagedThreadId} : {message} , {DateTime.Now.ToString("HH:mm:ss.fff")}");
        }

       
        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public void Clean(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);

                foreach (var item in allImageFiles)
                {
                    File.Delete(item);
                }
            }
        }

        public void ResizeImagesParallel(string sourcePath, string destPath, double scale)
        {
            var allFiles = FindImages(sourcePath);
            
            ParallelOptions parallelOptions = new ParallelOptions();
            //parallelOptions.MaxDegreeOfParallelism = 100;
            //ThreadPool.SetMinThreads(100, 100);
            Parallel.ForEach(allFiles, parallelOptions, (filePath) => ResizeSingleFile(filePath, destPath, scale));
            
        }

        private void ResizeSingleFile(string filePath, string destPath, double scale)
        {
            Image imgPhoto = Image.FromFile(filePath);
            string imgName = Path.GetFileNameWithoutExtension(filePath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            int destionatonWidth = (int)(sourceWidth * scale);
            int destionatonHeight = (int)(sourceHeight * scale);

            Bitmap processedImage = processBitmap((Bitmap)imgPhoto,
                sourceWidth, sourceHeight,
                destionatonWidth, destionatonHeight);

            string destFile = Path.Combine(destPath, imgName + ".jpg");
            processedImage.Save(destFile, ImageFormat.Jpeg);
           // LogThreadId("End - ResizeSingleFile");
        }

        

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImages(string srcPath)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            return files;
        }

        /// <summary>
        /// 針對指定圖片進行縮放作業
        /// </summary>
        /// <param name="img">圖片來源</param>
        /// <param name="srcWidth">原始寬度</param>
        /// <param name="srcHeight">原始高度</param>
        /// <param name="newWidth">新圖片的寬度</param>
        /// <param name="newHeight">新圖片的高度</param>
        /// <returns></returns>
        private Bitmap processBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight)
        {
            //LogThreadId("start - ProcessBitmap");

            Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(img,
                new Rectangle(0, 0, newWidth, newHeight),
                new Rectangle(0, 0, srcWidth, srcHeight),
                GraphicsUnit.Pixel);

            //LogThreadId("End - ProcessBitmap");
            return resizedbitmap;
        }

        private async Task<Bitmap> ProcessBitmapAsync(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight)
        {
            return await Task.FromResult(processBitmap(img, srcWidth, srcHeight, newWidth, newHeight));
        }
    }
}
