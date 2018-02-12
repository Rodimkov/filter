using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace filter
{
    abstract  class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourseImage, int x, int y);

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
         public Bitmap processImage(Bitmap sourceImage,BackgroundWorker worker)
         {
             Bitmap resaultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
             for (int i = 0; i < sourceImage.Width; i++)
             {
                 worker.ReportProgress((int)((float)i / resaultImage.Width * 100));
                 if (worker.CancellationPending)
                     return null;
                 for (int j = 0; j < sourceImage.Height; j++)
                 {
                     resaultImage.SetPixel(i,j,calculateNewPixelColor(sourceImage,i,j));
                 }
             }
             return resaultImage;
         }
    }
    class invertFilters : Filters
    {
        
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {   Color sourceColor = sourseImage.GetPixel(x,y);
            Color resualColor = Color.FromArgb(255-sourceColor.R,
                                               255-sourceColor.G, 
                                               255-sourceColor.B);
            return resualColor; 
        }

    }
}
