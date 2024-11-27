using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace SnapshotsOfLifeEx01
{

   
    public partial class FrameWindow : Form
    {

        private Image bg = Image.FromFile("./ui/background.png");
        private Image back_up_btn = Image.FromFile("./ui/back_arrow_up.png");
        private Image back_down_btn = Image.FromFile("./ui/back_arrow_down.png");
        private Image uncomplete_btn_down_img = Image.FromFile("./ui/uncompletebutton_down.png");
        private Image uncomplete_btn_up_img = Image.FromFile("./ui/uncompletebutton_up.png");
        private Image complete_btn_down_img = Image.FromFile("./ui/completebutton_down.png");
        private Image complete_btn_up_img = Image.FromFile("./ui/completebutton_up.png");
        private Image shadow_fram_img = Image.FromFile("./ui/shadow_frame.png");

        private int frame_cnt = 0;      // 테마를 불러올때 각각의 id를 대신할 카운트값

        private Color selectedColor = Color.FromArgb(255, 205, 18); // 선택될시 테두리 색

        private int penWidth = 6;   // 테두리 두께

        /*private int selectedLineWidthDistance = 10;
        private int selectedLineHigthDistance = 10;*/

        private List<PictureBox> frame_list = new List<PictureBox>() { };   // 생성된 picturebox 리스트

        private Bitmap bitmap;
        private Graphics _g;


        //켤때 깜빡임 방지 
        /*protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }*/

        /*public Image selected_picture { get; set; }         // 리턴값*/
        public string selected_picture_name { get; set; }         // 리턴값

        public FrameWindow()
        {

            InitializeComponent();


            panel1.BackgroundImage = bg;
            button1.BackgroundImage = back_up_btn;
            next.BackgroundImage = uncomplete_btn_up_img;

        }

        private void FrameWindow_Load(object sender, EventArgs e)
        {
           

            string[] files = Directory.GetFiles("./ui/", "frame*.png").ToArray();       // ui 파일에 있는 테마 이미지 이름 불러오기


            int title = 200;        // 윗 타이틀 공간
            int buttonTerm = 300;        // 아랫 공간
            int pictureboxTop = 50;       // 윗 공간
            int pictureboxWidth = this.Width / 2;
            int pictureboxHeight = (int)((this.Height - title - buttonTerm) / (Math.Ceiling((double)files.Length / 2)) - (pictureboxTop * 2));

            

            foreach (string img in files)
            {


                Image img_p1 = Image.FromFile(img);


                bitmap = new Bitmap(shadow_fram_img);
                _g = Graphics.FromImage(bitmap);

                _g.DrawImage(img_p1, (shadow_fram_img.Width - img_p1.Width) / 2, (shadow_fram_img.Height - img_p1.Height)/2, img_p1.Width, img_p1.Height);



                PictureBox p1 = new PictureBox
                {
                    Name = img,
                    Tag = new PictureBoxData
                    {
                        dataimg = bitmap,
                        databool = false

                    },

                };
                p1.Top = ((int)(frame_cnt / 2) * (pictureboxHeight + pictureboxTop)) + pictureboxTop + title;
                p1.Left = (int)(this.Width / 2) * (frame_cnt % 2) + ((this.Width / 2 - pictureboxWidth) / 2);
                p1.Width = pictureboxWidth;
                p1.Height = pictureboxHeight;
                p1.SizeMode = PictureBoxSizeMode.Zoom;

                
                p1.Image = bitmap;
                
                panel1.Controls.Add(p1);
                p1.Parent = panel1;

                bool s_b = false;

                p1.Click += (s, ea) => PictureBoxClickEvent(s, ea);

                frame_cnt += 1;

                frame_list.Add(p1);

                
                
            }
        }

        private void PictureBoxClickEvent(object sender, EventArgs ea)
        {
            /*Console.WriteLine($"left = {((PictureBox)sender).Image.ToString()}");*/
            /*Console.WriteLine($"left = {((PictureBox)sender).Left}");*/

            PictureBoxData data = ((PictureBox)sender).Tag as PictureBoxData;

            if (!data.databool)
            {

                // 다른 picturebox 체크 해제
                foreach (PictureBox other in frame_list)
                {
                    if (other != ((PictureBox)sender))
                    {
                        PictureBoxData data_other = other.Tag as PictureBoxData;
                        other.Image = data_other.dataimg;
                        data_other.databool = false;
                    }

                }

                next.BackgroundImage = complete_btn_up_img;


                /*selected_picture = data.dataimg;*/
                selected_picture_name = ((PictureBox)sender).Name;

                Pen pen = new Pen(selectedColor);
                pen.Width = penWidth;

                using (Graphics g = ((PictureBox)sender).CreateGraphics())
                {
                    // 이미지의 원래 크기
                    int originalWidth = ((PictureBox)sender).Image.Width;
                    int originalHeight = ((PictureBox)sender).Image.Height;

                    // 이미지 비율 계산
                    float originalAspectRatio = (float)originalWidth / originalHeight;
                    float pictureBoxAspectRatio = (float)((PictureBox)sender).Width / ((PictureBox)sender).Height;

                    int zoomedWidth, zoomedHeight;
                    int startx, starty;

                    if (originalAspectRatio > pictureBoxAspectRatio)
                    {
                        // 너비가 더 긴 경우
                        zoomedWidth = ((PictureBox)sender).Width;
                        zoomedHeight = (int)(((PictureBox)sender).Width / originalAspectRatio);
                        startx = 0;
                        starty = (((PictureBox)sender).Height - zoomedHeight) / 2;
                    }
                    else
                    {
                        // 높이가 더 긴 경우
                        zoomedHeight = ((PictureBox)sender).Height;
                        zoomedWidth = (int)(((PictureBox)sender).Height * originalAspectRatio);
                        startx = (((PictureBox)sender).Width - zoomedWidth) / 2;
                        starty = 0;
                    }




                    /*drawSelectedLine(pen, startx, starty, zoomedWidth, zoomedHeight);*/




                   /* g.DrawRectangle(pen, startx - selectedLineWidthDistance / 2, starty - selectedLineHigthDistance / 2, zoomedWidth + selectedLineWidthDistance, zoomedHeight + selectedLineHigthDistance);*/
                    g.DrawRectangle(pen, startx , starty , zoomedWidth, zoomedHeight );

                }
            }
            else
            {
                ((PictureBox)sender).Image = data.dataimg;

                if (selected_picture_name == ((PictureBox)sender).Name)
                {
                    /*selected_picture = null;*/
                    selected_picture_name = null;
                    next.BackgroundImage = uncomplete_btn_up_img;
                }
            }


            data.databool = !data.databool;

        }

     



        private void next_Click(object sender, EventArgs e)
        {
            if (selected_picture_name != null)
            {
                this.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*selected_picture = null;*/
            selected_picture_name = null;
            this.Dispose();
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            button1.BackgroundImage = back_down_btn;
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            button1.BackgroundImage = back_up_btn;
        }

        private void next_MouseDown(object sender, MouseEventArgs e)
        {
            if (selected_picture_name == null)
            {
                next.BackgroundImage = uncomplete_btn_down_img;
            }
            else
            {
                next.BackgroundImage = complete_btn_down_img;
            }
        }

        private void next_MouseUp(object sender, MouseEventArgs e)
        {
            if (selected_picture_name == null)
            {
                next.BackgroundImage = uncomplete_btn_up_img;
            }
            else
            {
                next.BackgroundImage = complete_btn_up_img;
            }
        }


        private void FrameWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (PictureBox p in frame_list)
            {
                p.Dispose();
            }
            frame_list.Clear();
            _g.Dispose();
            bitmap.Dispose();

            _g = null;
            bitmap = null;

            bg = null;
            back_up_btn = null;
            back_down_btn = null;
            uncomplete_btn_down_img = null;
            uncomplete_btn_up_img = null;
            complete_btn_down_img = null;
            complete_btn_up_img = null;
            shadow_fram_img = null;

        }

        
    }


    public class PictureBoxData
    {
        public Image dataimg { get; set; }
        public bool databool { get; set; }
    }

}
