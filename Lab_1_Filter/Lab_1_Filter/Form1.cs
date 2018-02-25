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
                //stimage.Push(image);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
                pictureBox2.Image = image;
                pictureBox2.Refresh();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog savedialog = new SaveFileDialog();

            savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }


        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*InvertFilter filter = new InvertFilter();
            Bitmap resultImage = filter.processImage(image);
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();*/
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);

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

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

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

        private void яркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Brightness();
            backgroundWorker1.RunWorkerAsync(filter);        
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness();
            backgroundWorker1.RunWorkerAsync(filter);  
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

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

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new embossing();
            backgroundWorker1.RunWorkerAsync(filter); 
        }

        private void резкость2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness2();
            backgroundWorker1.RunWorkerAsync(filter); 
        }

        private void медианToolStripMenuItem_Click(object sender, EventArgs e)
        {
             Filters filter = new median();
            backgroundWorker1.RunWorkerAsync(filter); 
        }

        private void переносToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new transference();
            backgroundWorker1.RunWorkerAsync(filter); 
        }

        private void стеклоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Glass();
            backgroundWorker1.RunWorkerAsync(filter); 
        }


        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayWorld();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void линейноеРастяжениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new line();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            NewPosition();
        }

        void NewPosition()
        {
            button1.Location = new Point(this.Width  - 150, this.Height  - 100);
            button2.Location = new Point(this.Width - 250, this.Height - 100);
            progressBar1.Width = this.Width - 400;
            progressBar1.Location = new Point(20, this.Height - 100);
            int k = 150;
            int l = -130;
            pictureBox1.Width = this.Width/2 - k ;
            pictureBox1.Height = this.Height/2 - l;

            pictureBox1.Location = new Point( 50 , 70);
            //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Width = this.Width/2 - k;
            pictureBox2.Height = this.Height/2 - l;

            pictureBox2.Location = new Point(this.Width / 2 + 70, 70);
            //pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;


            label1.Text = Convert.ToString(this.Height) + ' ' +  Convert.ToString(this.Width);
            label2.Text = Convert.ToString(button1.Location.X) + ' ' + Convert.ToString(button1.Location.Y);
            label3.Text = Convert.ToString(button2.Location.X) + ' ' + Convert.ToString(button2.Location.Y);


        }

        private void menuStrip1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void menuStrip1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
            {
                label1.Text = "hello";
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.Modifiers == Keys.Alt)//alt+D
            {
                MessageBox.Show("ALT+D");
            }
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

        private void ужасToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Dilation();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void хмToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Erosion();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
