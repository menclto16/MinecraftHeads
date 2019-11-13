using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MinecraftHeads
{
    class DrawingHandler
    {
        public BitmapImage ConvertImage(Bitmap inputImage)
        {
            Bitmap outputImage = new Bitmap(16, 32);
            CopyRegionIntoImage(inputImage, new Rectangle(8, 8, 8, 8), ref outputImage, new Rectangle(4, 0, 8, 8));
            CopyRegionIntoImage(inputImage, new Rectangle(44, 20, 4, 12), ref outputImage, new Rectangle(0, 8, 4, 12));
            CopyRegionIntoImage(inputImage, new Rectangle(20, 20, 8, 12), ref outputImage, new Rectangle(4, 8, 8, 12));
            CopyRegionIntoImage(inputImage, new Rectangle(36, 52, 4, 12), ref outputImage, new Rectangle(12, 8, 4, 12));
            CopyRegionIntoImage(inputImage, new Rectangle(4, 20, 4, 12), ref outputImage, new Rectangle(4, 20, 4, 12));
            CopyRegionIntoImage(inputImage, new Rectangle(20, 52, 4, 12), ref outputImage, new Rectangle(8, 20, 4, 12));

            CopyRegionIntoImage(inputImage, new Rectangle(40, 8, 8, 8), ref outputImage, new Rectangle(4, 0, 8, 8));
            CopyRegionIntoImage(inputImage, new Rectangle(44, 36, 4, 12), ref outputImage, new Rectangle(0, 8, 4, 12));
            CopyRegionIntoImage(inputImage, new Rectangle(20, 36, 8, 12), ref outputImage, new Rectangle(4, 8, 8, 12));
            CopyRegionIntoImage(inputImage, new Rectangle(52, 52, 4, 12), ref outputImage, new Rectangle(12, 8, 4, 12));
            CopyRegionIntoImage(inputImage, new Rectangle(4, 36, 4, 12), ref outputImage, new Rectangle(4, 20, 4, 12));
            CopyRegionIntoImage(inputImage, new Rectangle(4, 52, 4, 12), ref outputImage, new Rectangle(8, 20, 4, 12));

            using (MemoryStream memory = new MemoryStream())
            {
                outputImage.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }

        }
        public void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
            using (Graphics grD = Graphics.FromImage(destBitmap))
            {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
        }
    }
}
