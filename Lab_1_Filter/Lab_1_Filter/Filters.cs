using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Lab_1_Filter
{
    abstract class Filters
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

        public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resaultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resaultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resaultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resaultImage;
        }
    }

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            Color resualColor = Color.FromArgb(255 - sourceColor.R,
                                               255 - sourceColor.G,
                                               255 - sourceColor.B);
            return resualColor;
        }
    }

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighbotColor = sourseImage.GetPixel(idX, idY);
                    resultR += neighbotColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighbotColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighbotColor.B * kernel[k + radiusX, l + radiusY];
                }
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }
    }


    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            int Intensity = (int)(0.36f * sourceColor.R + 0.53f * sourceColor.G + 0.11f * sourceColor.B);
            Color resualColor = Color.FromArgb(Intensity, Intensity, Intensity);
            return resualColor;    
        }
    }

    class Sepia : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            float k = 30f;
            int Intensity = (int)(0.36f * sourceColor.R + 0.53f * sourceColor.G + 0.11f * sourceColor.B);
            Color resualColor = Color.FromArgb(Clamp((int)(Intensity + 2 * k),0,255),
                                               Clamp((int)(Intensity+0.5*k),0,255),
                                               Clamp((int)(Intensity-1*k),0,255));
            return resualColor;
        }
    }
    class Brightness : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            int k = 30;
            Color resualColor = Color.FromArgb(Clamp(sourceColor.R + k, 0, 255),
                                               Clamp(sourceColor.G + k, 0, 255),
                                               Clamp(sourceColor.B + k, 0, 255));
            return resualColor;
        }
    }



    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    }
    class GaussianFilter : MatrixFilter
    {
        public GaussianFilter()
        {
            creatGaussianKernel(3, 2);
        }
        public void creatGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
    }
    class Sobel : MatrixFilter
    {
        public float[,] SobelX;
        public float[,] SobelY;

        public Sobel()
        {
            SobelX = new float[3, 3] { {-1 , 0 , 1 },
                                     {-2 , 0 , 2 },
                                     {-1 , 0 , 1 }
                                           };

            SobelY = new float[3, 3] { {-1 ,-2 ,-1 },
                                     { 0 , 0 , 0 },
                                     { 1 , 2 , 1 }
                                           };
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int radiusX = 3 / 2;
            int radiusY = 3 / 2;
            float resultRx = 0;
            float resultGx = 0;
            float resultBx = 0;
            float resultRy = 0;
            float resultGy = 0;
            float resultBy = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighbotColor = sourseImage.GetPixel(idX, idY);
                    resultRx += neighbotColor.R * SobelX[k + radiusX, l + radiusY];
                    resultGx += neighbotColor.G * SobelX[k + radiusX, l + radiusY];
                    resultBx += neighbotColor.B * SobelX[k + radiusX, l + radiusY];
                }
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighbotColor = sourseImage.GetPixel(idX, idY);
                    resultRy += neighbotColor.R * SobelY[k + radiusX, l + radiusY];
                    resultGy += neighbotColor.G * SobelY[k + radiusX, l + radiusY];
                    resultBy += neighbotColor.B * SobelY[k + radiusX, l + radiusY];
                }
            int sum = (int)Math.Sqrt(resultRx * resultRx + resultRy * resultRy) + (int)Math.Sqrt(resultGx * resultGx + resultGy * resultGy) + (int)Math.Sqrt(resultBx * resultBx + resultBy * resultBy);
            return Color.FromArgb(
                Clamp(sum, 0, 255),
                Clamp(sum, 0, 255),
                Clamp(sum, 0, 255)
                );
        }
    }

        class SobelLSD : MatrixFilter
        {
            public float[,] SobelX;
            public float[,] SobelY;

            public SobelLSD()
            {
                SobelX = new float[3, 3] { {-1 , 0 , 1 },
                                              {-2 , 0 , 2 },
                                              {-1 , 0 , 1 }
                                           };

               SobelY = new float[3, 3] { {-1 ,-2 ,-1 },
                                         { 0 , 0 , 0 },
                                         { 1 , 2 , 1 }
                                           };
            }

            protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
            {
                int radiusX = 3 / 2;
                int radiusY = 3 / 2;
                float resultRx = 0;
                float resultGx = 0;
                float resultBx = 0;
                float resultRy = 0;
                float resultGy = 0;
                float resultBy = 0;
                for (int l = -radiusY; l <= radiusY; l++)
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                        int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                        Color neighbotColor = sourseImage.GetPixel(idX, idY);
                        resultRx += neighbotColor.R * SobelX[k + radiusX, l + radiusY];
                        resultGx += neighbotColor.G * SobelX[k + radiusX, l + radiusY];
                        resultBx += neighbotColor.B * SobelX[k + radiusX, l + radiusY];
                    }
                for (int l = -radiusY; l <= radiusY; l++)
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                        int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                        Color neighbotColor = sourseImage.GetPixel(idX, idY);
                        resultRy += neighbotColor.R * SobelY[k + radiusX, l + radiusY];
                        resultGy += neighbotColor.G * SobelY[k + radiusX, l + radiusY];
                        resultBy += neighbotColor.B * SobelY[k + radiusX, l + radiusY];
                    }
                return Color.FromArgb(
                    Clamp((int)Math.Sqrt(resultRx * resultRx + resultRy * resultRy), 0, 255),
                    Clamp((int)Math.Sqrt(resultGx * resultGx + resultGy * resultGy), 0, 255),
                    Clamp((int)Math.Sqrt(resultBx * resultBx + resultBy * resultBy), 0, 255)
                    );
            }
         }

    class Sharpness : MatrixFilter
    {
        public Sharpness()
        {
            kernel = new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
        }
    }
}

