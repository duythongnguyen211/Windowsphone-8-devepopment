using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Personal_Notes_Management_System.Storage
{
    public class StorageManager
    {
        private static IsolatedStorageFile isf;

        private static void OpenStorage()
        {
            isf = IsolatedStorageFile.GetUserStoreForApplication();
        }
        public static void CreateDirectory(string direct)
        {
            OpenStorage();
            if (!isf.DirectoryExists(direct))
                isf.CreateDirectory(direct);
        }

        public static void DeleteDirectory(string direct)
        {
            OpenStorage();
            if (isf.DirectoryExists(direct))
                isf.DeleteDirectory(direct);
        }

        public static void DeleteFile(string path)
        {
            OpenStorage();
            if (isf.FileExists(path))
                isf.DeleteFile(path);
        }

        public static BitmapImage GetImage(string path)
        {
            OpenStorage();
            BitmapImage img = new BitmapImage();
            try
            {
                using (isf)
                {
                    using (IsolatedStorageFileStream fileStream = isf.OpenFile(path, FileMode.Open, FileAccess.Read))
                    {
                        img.SetSource(fileStream);
                        fileStream.Close();
                    }
                }
            }
            catch { }
            return img;
        }

        public static void SaveImage(BitmapImage img, string fname)
        {
            OpenStorage();
            if (img != null)
            {
                WriteableBitmap wbmp = new WriteableBitmap(img);
                using (isf)
                {
                    if (isf.FileExists(fname)) { isf.DeleteFile(fname); }
                    using (var stream = isf.OpenFile(fname, System.IO.FileMode.OpenOrCreate))
                    {
                        wbmp.SaveJpeg(stream, img.PixelWidth, img.PixelHeight, 0, 100);
                        stream.Close();
                    }
                }
            }
        }

        //public static void SaveImageAsPNG(BitmapImage img, string fname)
        //{
        //    using (var writer = new StreamWriter(new IsolatedStorageFileStream(fname, FileMode.Create, FileAccess.Write, isf)))
        //    {
        //        var encoder = new PngEncoder();
        //        WriteableBitmap wb = new WriteableBitmap(img);
        //        encoder.Encode(wb.ToImage(), writer.BaseStream);
        //    }
        //}
    }
}