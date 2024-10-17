using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace SnapshotsOfLifeEx01
{
    public partial class PreViewWindow : Form
    {

        private string previewPath = "";
        private Timer t = new Timer();  // 창 유지 타이머
        private int timer_time; // 현재 카운트 타이머 시간
        private int timer_time_limit = 5;  // 카운트 타이머 초 수

        private Image bg = Image.FromFile("./ui/background.png");

        public PreViewWindow(string previewPath)
        {
            InitializeComponent();

            this.previewPath = previewPath;

            Image img1 = Image.FromFile(this.previewPath);

            /*pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;*/

            pictureBox1.Image = img1;

            panel1.BackgroundImage = bg;

            t.Interval = 1000;
            t.Tick += new EventHandler(timer_Event);


            // 프린트
            PrintingPicture();

        }

        private void PrintingPicture()
        {
            using (PrintDocument pd = new PrintDocument())
            {
                pd.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
                pd.DefaultPageSettings.Landscape = true;
                
                try
                {
                    pd.Print();
                    t.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    this.Dispose();
                }
                
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // 이미지 로드
            Image image = Image.FromFile(this.previewPath);

            // 이미지 크기 조정 (옵션)
            e.Graphics.DrawImage(image, e.PageBounds);

           
        }


        private void timer_Event(object sender, System.EventArgs e)
        {
            if (timer_time == timer_time_limit)    // 0초일때 사진을 찍음
            {
                timer_time = 0;   
                t.Stop();
                this.Dispose();



                return;
            }

            timer_time++;

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
