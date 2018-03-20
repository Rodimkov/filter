using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Lab_1_Filter
{
    //базовый класс фильтров
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

        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
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

    //точечные фильтры
    //инверсия
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

    //черно-белое
    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            int Intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
            Color resualColor = Color.FromArgb(Intensity, Intensity, Intensity);

            return resualColor;
        }
    }

    //сепия
    class Sepia : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            float k = 30;
            int Intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
            Color resualColor = Color.FromArgb(Clamp((int)(Intensity + 2 * k), 0, 255),
                                               Clamp((int)(Intensity + 0.5 * k), 0, 255),
                                               Clamp((int)(Intensity - 1 * k), 0, 255));

            return resualColor;
        }
    }

    //яркость
    class Brightness : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int k = 30;
            Color sourceColor = sourseImage.GetPixel(x, y);
            Color resualColor = Color.FromArgb(Clamp(sourceColor.R + k, 0, 255),
                                               Clamp(sourceColor.G + k, 0, 255),
                                               Clamp(sourceColor.B + k, 0, 255));

            return resualColor;
        }
    }

    //стекло
    class Glass : Filters
    {
        private Random rand = new Random(DateTime.Now.Millisecond);
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int k;
            int l;
            k = Clamp((int)(x + ((rand.NextDouble() - 0.5) * 10)), 0, sourseImage.Width - 1);
            l = Clamp((int)(y + ((rand.NextDouble() - 0.5) * 10)), 0, sourseImage.Height - 1);

            Color sourceColor = sourseImage.GetPixel(k, l);
            Color resultColor = Color.FromArgb(sourceColor.R, sourceColor.G, sourceColor.B);

            return resultColor;
        }
    }

    //сдвиг
    class transference : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int k, l;

            k = Clamp(x + 50, 0, sourseImage.Width - 1);
            l = Clamp(y, 0, sourseImage.Height - 1);

            Color sourceColor = sourseImage.GetPixel(k, l);

            return sourceColor;
        }
    }

    //поворот
    class rotation : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int agl = 45;
            int x0 = (int)((sourseImage.Width - 1) / 2);
            int y0 = (int)((sourseImage.Height - 1) / 2);

            //      int k = Clamp((int)((x - x0) / Math.Sqrt(2) - (y - y0) / Math.Sqrt(2) + x0), 0, sourseImage.Width - 1);
            //      int l = Clamp((int)((x - x0) / Math.Sqrt(2) + (y - y0) / Math.Sqrt(2) + y0), 0, sourseImage.Height - 1);
            int k = Clamp((int)((x - x0) * Math.Cos(agl) - (y - y0) * Math.Sin(agl) + x0), 0, sourseImage.Width - 1);
            int l = Clamp((int)((x - x0) * Math.Sin(agl) + (y - y0) * Math.Cos(agl) + y0), 0, sourseImage.Height - 1);

            Color sourceColor = sourseImage.GetPixel(k, l);

            return sourceColor;
        }
    }
    //серый мир
    class GrayWorld : Filters
    {
        int r, g, b, avg;

        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(Clamp((int)(sourceColor.R * avg / r), 0, 255),
                                               Clamp((int)(sourceColor.G * avg / g), 0, 255),
                                               Clamp((int)(sourceColor.B * avg / b), 0, 255));

            return resultColor;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            r = g = b = avg = 0;

            Bitmap resaultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);

                    r += sourceColor.R;
                    g += sourceColor.G;
                    b += sourceColor.B;
                }
            }

            r = (int)(r / (sourceImage.Width * sourceImage.Height));
            g = (int)(g / (sourceImage.Width * sourceImage.Height));
            b = (int)(b / (sourceImage.Width * sourceImage.Height));

            avg = (r + g + b) / 3;

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

    //линейное растяжение
    class LinearStretching : Filters
    {
        float Intensity, IntensityMin, IntensityMax;

        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            Intensity = (int)(0.36f * sourceColor.R + 0.53f * sourceColor.G + 0.11f * sourceColor.B);
            Color resultColor = Color.FromArgb(
                                                Clamp((int)((Intensity - IntensityMin) * 255 / (IntensityMax - IntensityMin)), 0, 255),
                                                Clamp((int)((Intensity - IntensityMin) * 255 / (IntensityMax - IntensityMin)), 0, 255),
                                                Clamp((int)((Intensity - IntensityMin) * 255 / (IntensityMax - IntensityMin)), 0, 255));

            return resultColor;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resaultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            IntensityMax = 0;
            IntensityMin = 255;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    Intensity = (int)(0.36f * sourceColor.R + 0.53f * sourceColor.G + 0.11f * sourceColor.B);
                    if (Intensity > IntensityMax)
                        IntensityMax = Intensity;
                    if (Intensity < IntensityMin)
                        IntensityMin = Intensity;
                }
            }

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

    //базовый класс матричных фильтров
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
                                Clamp((int)resultB, 0, 255));
        }
    }

    //размытие
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

    //фильтр Гаусса
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

    //фильтр Собеля
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

            int sum = (int)Math.Sqrt(resultRx * resultRx + resultRy * resultRy) +
                      (int)Math.Sqrt(resultGx * resultGx + resultGy * resultGy) +
                      (int)Math.Sqrt(resultBx * resultBx + resultBy * resultBy);

            return Color.FromArgb(
                                Clamp(sum, 0, 255),
                                Clamp(sum, 0, 255),
                                Clamp(sum, 0, 255)
                );
        }
    }

    //случаная производная от фильтра Собеля
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

    //резкость-1
    class Sharpness : MatrixFilter
    {
        public Sharpness()
        {
            kernel = new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
        }
    }

    //резкость-2
    class Sharpness2 : MatrixFilter
    {
        public Sharpness2()
        {
            kernel = new float[3, 3] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
        }
    }

    //тиснение
    class embossing : MatrixFilter
    {
        public embossing()
        {
            kernel = new float[3, 3] { { 0, 1, 0 }, { 1, 0, -1 }, { 0, -1, 0 } };
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
                                Clamp((int)(resultR + 128), 0, 255),
                                Clamp((int)(resultG + 128), 0, 255),
                                Clamp((int)(resultB + 128), 0, 255)
                );
        }
    }

    //медианный фильтр 
    class median : MatrixFilter
    {

        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {

            int size = 5;
            int rad = size / 2;
            int s = size * size;
            Color[] sourceColor = new Color[s];

            int[] R = new int[s];
            int[] G = new int[s];
            int[] B = new int[s];

            int k = 0;

            for (int i = -rad; i <= rad; i++)
            {
                for (int j = -rad; j <= rad; j++)
                {
                    int idX = Clamp(x + i, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + j, 0, sourseImage.Height - 1);

                    sourceColor[k++] = sourseImage.GetPixel(idX, idY);
                }
            }
            
            for (int i = 0; i < s; i++)
            {
                R[i] = sourceColor[i].R;
                G[i] = sourceColor[i].G;
                B[i] = sourceColor[i].B;
            }
            
            Array.Sort(R);
            Array.Sort(G);
            Array.Sort(B);
            
            return Color.FromArgb(R[s/2], G[s/2], B[s/2]);
        }
    }

    //размытие в движение
    class MotionBlurFilter : MatrixFilter
    {
        public MotionBlurFilter()
        {
            int sizeX = 9;
            int sizeY = 9;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    if (i == j)
                        kernel[i, j] = 1.0f / (float)(sizeX);
                    else
                        kernel[i, j] = 0;

        }
    }

    //математическая морфология (содержащая расширение и сужение, как базовый класс )
    class MathMorf : Filters
    {
        bool[,] kernel;
        public bool flag;

        public MathMorf(bool[,] _kernel, bool _flag = false)
        {
            flag = _flag;
            kernel = _kernel;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int max = 0;
            int min = 10000;

            Color clr = Color.Black;

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            if (flag )
            {
                for (int l = -radiusY; l <= radiusY; l++)
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + l, 0, sourceImage.Height - 1);

                        Color sourceColor = sourceImage.GetPixel(idX, idY);

                        int intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);

                        if ((kernel[k + radiusX, l + radiusY]) && (intensity > max))
                        {
                            max = intensity;
                            clr = sourceColor;
                        }
                    }
            }
            else
            {
                for (int l = -radiusY; l <= radiusY; l++)
                    for (int k = -radiusX; k <= radiusX; k++)
                    {
                        int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + l, 0, sourceImage.Height - 1);

                        Color sourceColor = sourceImage.GetPixel(idX, idY);

                        int intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);

                        if ((kernel[k + radiusX, l + radiusY]) && intensity < min)
                        {
                            min = intensity;
                            clr = sourceColor;
                        }
                    }
            }

            return clr;
        }
    }

    //открытие
    class Opening : MathMorf
    {

        public Opening(bool[,] _kernel)
            : base(_kernel)
        {

        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap currImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            base.flag = false;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50));

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    currImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            base.flag = true;

            for (int i = 0; i < currImage.Width; i++)
            {
                worker.ReportProgress((int)(((float)i / resultImage.Width * 50) + 50));

                for (int j = 0; j < currImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(currImage, i, j));
                }
            }

            return resultImage;
        }
    }

    //закрытие
    class Closing : MathMorf
    {

        public Closing(bool[,] _kernel)
            : base(_kernel)
        {

        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap currImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            base.flag = true;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50));

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    currImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            base.flag = false;

            for (int i = 0; i < currImage.Width; i++)
            {
                worker.ReportProgress((int)(((float)i / resultImage.Width * 50) + 50));

                for (int j = 0; j < currImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(currImage, i, j));
                }
            }

            return resultImage;
        }
    } //TopHat

    class TopHat : MathMorf
    {

        public TopHat(bool[,] _kernel)
            : base(_kernel)
        {

        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap currImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap resImage = new Bitmap(sourceImage.Width, sourceImage.Height);


            base.flag = true;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50));

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    currImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            base.flag = false;

            for (int i = 0; i < currImage.Width; i++)
            {
                worker.ReportProgress((int)(((float)i / resultImage.Width * 50) + 50));

                for (int j = 0; j < currImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(currImage, i, j));
                }
            }

            for (int i = 0; i < currImage.Width; i++)
            {
                worker.ReportProgress((int)(((float)i / resultImage.Width * 50) + 50));

                for (int j = 0; j < currImage.Height; j++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    Color sColor = resultImage.GetPixel(i, j);
                  /*  Color res = Color.FromArgb(
Clamp((int)(sColor.R - sourceColor.R), 0, 255),
Clamp((int)(sColor.G - sourceColor.G), 0, 255),
Clamp((int)(sColor.B - sourceColor.B), 0, 255));
                    resImage.SetPixel(i, j, res);*/


                    int intensity = (int)(0.36 * sColor.R + 0.53 * sColor.G + 0.11 * sColor.B);

                    Color res = Color.FromArgb(
                    Clamp((int)-(sourceColor.R - intensity), 0, 255),
                    Clamp((int)-(sourceColor.G - intensity), 0, 255),
                    Clamp((int)-(sourceColor.B - intensity), 0, 255));
                    resImage.SetPixel(i, j, res);
                }
            }

            return resImage;
        }
    }

    // доп задание  (перевод близких к красному в черно-белое)
    class Invert : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            Color resualColor = sourceColor;
            int Intensity;
            if ((sourceColor.R > 200) && (sourceColor.G >= 0) && (sourceColor.G <= 100) && (sourceColor.B >= 0) && (sourceColor.B <= 100))   // && (sourceColor.G > 25) && (sourceColor.G < 75) && (sourceColor.B > 25) && (sourceColor.R < 75)
            {
                Intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
                resualColor = Color.FromArgb(Intensity, Intensity, Intensity);
            }
            return resualColor;
        }
    }

}