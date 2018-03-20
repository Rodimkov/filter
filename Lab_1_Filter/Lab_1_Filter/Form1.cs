using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_1_Filter
{
    public partial class Form1 : Form
    {
        Bitmap image;
        bool[,] kernel;
        public Form1()
        {

            InitializeComponent();
            NewPosition();
        }

        Stack<Bitmap> stimage = new Stack<Bitmap>();

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files | *.png; *.jpg; *.bmp; | ALL Files (*.*) | *.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
            {
                stimage.Push(image);
                image = newImage;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {

                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        //отмена фильтрации
        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        //сохранение изображения 
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog savedialog = new SaveFileDialog();

            savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        //обновление формы при изменение ее размера
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            NewPosition();
        }

        //вычисление новых позиций в форме для ее элементов
        void NewPosition()
        {
            int k = 400;
            int l = 270;

            button1.Location = new Point(this.Width - 150, this.Height - 100);
            button2.Location = new Point(this.Width - 250, this.Height - 100);

            progressBar1.Width = this.Width - k;
            progressBar1.Location = new Point(50, this.Height - 100);

            pictureBox1.Width = this.Width - k;
            pictureBox1.Height = this.Height - l;
            pictureBox1.Location = new Point(30, 70);

            textBox1.Location = new Point(this.Width - 100, 80);
            textBox2.Location = new Point(this.Width - 150, 80);
            textBox3.Location = new Point(this.Width - 200, 80);
            textBox4.Location = new Point(this.Width - 100, 120);
            textBox5.Location = new Point(this.Width - 150, 120);
            textBox6.Location = new Point(this.Width - 200, 120);
            textBox7.Location = new Point(this.Width - 100, 160);
            textBox8.Location = new Point(this.Width - 150, 160);
            textBox9.Location = new Point(this.Width - 200, 160);
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
                {
                    image = stimage.Pop();
                    pictureBox1.Image = image;
                    pictureBox1.Refresh();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        void KernelInsert()
        {
            kernel = new bool[3, 3] { {false,false,false} , {false,false,false},{false,false,false}};

            if ( (Convert.ToInt32(textBox1.Text) == 1) )
                kernel[0, 0] = true;
            if ((Convert.ToInt32(textBox2.Text) == 1))
                kernel[0, 1] = true;
            if ((Convert.ToInt32(textBox3.Text) == 1))
                kernel[0, 2] = true;
            if ((Convert.ToInt32(textBox4.Text) == 1))
                kernel[1, 0] = true;
            if ((Convert.ToInt32(textBox5.Text) == 1))
                kernel[1, 1] = true;
            if ((Convert.ToInt32(textBox6.Text) == 1))
                kernel[1, 2] = true;
            if ((Convert.ToInt32(textBox7.Text) == 1))
                kernel[2, 0] = true;
            if ((Convert.ToInt32(textBox8.Text) == 1))
                kernel[2, 1] = true;
            if ((Convert.ToInt32(textBox9.Text) == 1))
                kernel[2, 2] = true;
        }

        //вызов фильтров
        //точечные фильтры
        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*InvertFilter filter = new InvertFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();*/
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void чернобелоеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sepia();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void яркостьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Filters filter = new Brightness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void стеклоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Glass();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сдвигToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new transference();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void поворотToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new rotation();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayWorld();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void линейноеРастяжениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new LinearStretching();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        // матричные фильтры
        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрГауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void собельToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sobel();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void лСДToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelLSD();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void резкость2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness2();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new embossing();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void медианныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new median();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void размытиеВДвижениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MotionBlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        //Математическая морфмология
        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KernelInsert();

            Filters filter = new MathMorf(kernel, true);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KernelInsert();

            Filters filter = new MathMorf(kernel, false);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void открытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KernelInsert();

            Filters filter = new Opening(kernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KernelInsert();

            Filters filter = new Closing(kernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void topHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KernelInsert();

            Filters filter = new TopHat(kernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Invert();
            backgroundWorker1.RunWorkerAsync(filter);
        }

    }
}
