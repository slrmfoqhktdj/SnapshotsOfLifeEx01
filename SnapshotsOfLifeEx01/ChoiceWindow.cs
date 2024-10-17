using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;

namespace SnapshotsOfLifeEx01
{
    public partial class ChoiceWindow : Form
    {

        private Color selectedColor = Color.FromArgb(255, 205, 18);

        private bool s_box1,s_box2,s_box3,s_box4, s_box5, s_box6, s_box7, s_box8;

        private Image img1, img2, img3, img4, img5, img6, img7, img8;

        private int penWidth = 6;

        private List<string> selectedPictures = new List<string>() { }; // 선택된 사진 리스트

        private List<string> pictures = new List<string>() { };     // 넘겨받은 사진 리스트

        private int limit_select = 4;

        private int picture_width_distance = 47;  // 사진과의 가로 거리 왼쪽

        private int picture_width_distance2 = 61;  // 사진과의 가로 거리 중앙

        private int picture_width_distance3 = 52;  // 사진과의 가로 거리 오른쪽

        private int picture_height_distance = 28;  // 사진과의 세로 거리

        private int picture_height_distance2 = 53;  // 사진과의 세로 거리 마지막

        private int first_distance = 143;    // 처음 시작부분의 틈

        private Color picture_bg_color = Color.White;   // 사진 배경 색

        private string folderPath = "";

        /*private string print_picture_name = ""; // 프린트할 사진 이름*/

        private Image bg = Image.FromFile("./ui/background.png");

        private Image uncomplete_btn_down_img = Image.FromFile("./ui/uncompletebutton_down.png");

        private Image uncomplete_btn_up_img = Image.FromFile("./ui/uncompletebutton_up.png");

        private Image complete_btn_down_img = Image.FromFile("./ui/completebutton_down.png");

        private Image complete_btn_up_img = Image.FromFile("./ui/completebutton_up.png");


        private void ChoiceWindow_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void next_MouseDown(object sender, MouseEventArgs e)
        {
           
            if (selectedPictures.Count < limit_select)
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
            
            if (selectedPictures.Count < limit_select)
            {
                next.BackgroundImage = uncomplete_btn_up_img;
            }
            else
            {
                next.BackgroundImage = complete_btn_up_img;
            }
        }

        private string CombineImageName;

        public ChoiceWindow(List<string> pictures, string folderPath)
        {
            InitializeComponent();

            this.pictures = pictures;
            this.folderPath = folderPath;

            img1 = Image.FromFile(this.pictures[0]);
            img2 = Image.FromFile(this.pictures[1]);
            img3 = Image.FromFile(this.pictures[2]);
            img4 = Image.FromFile(this.pictures[3]);
            img5 = Image.FromFile(this.pictures[4]);
            img6 = Image.FromFile(this.pictures[5]);
            img7 = Image.FromFile(this.pictures[6]);
            img8 = Image.FromFile(this.pictures[7]);

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;

            pictureBox1.Image = img1;
            pictureBox2.Image = img2;
            pictureBox3.Image = img3;
            pictureBox4.Image = img4;
            pictureBox5.Image = img5;
            pictureBox6.Image = img6;
            pictureBox7.Image = img7;
            pictureBox8.Image = img8;

            panel1.BackgroundImage = bg;
            next.BackgroundImage = uncomplete_btn_up_img;

            s_box1 = false;
            s_box2 = false;
            s_box3 = false;
            s_box4 = false;
            s_box5 = false;
            s_box6 = false;
            s_box7 = false;
            s_box8 = false;

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ClickPictureBox(ref pictureBox1, ref s_box1, img1, this.pictures[0]);
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ClickPictureBox(ref pictureBox2, ref s_box2, img2, this.pictures[1]);
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            ClickPictureBox(ref pictureBox3, ref s_box3, img3, this.pictures[2]);
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            ClickPictureBox(ref pictureBox4, ref s_box4, img4, this.pictures[3]);
        }
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            ClickPictureBox(ref pictureBox5, ref s_box5, img5, this.pictures[4]);
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            ClickPictureBox(ref pictureBox6, ref s_box6, img6, this.pictures[5]);
        }
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            ClickPictureBox(ref pictureBox7, ref s_box7, img7, this.pictures[6]);
        }
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            ClickPictureBox(ref pictureBox8, ref s_box8, img8, this.pictures[7]);
        }

        private void next_Click(object sender, EventArgs e)
        {

            if(selectedPictures.Count < limit_select)
            {
                return;
            }

            DateTime now = DateTime.Now;
            string fileName = "Com_"+now.ToString("yyyyMMddHHmmssFFF") + ".jpg";
            Bitmap bitmap = new Bitmap((img1.Width*2) + picture_width_distance+ picture_width_distance2+ picture_width_distance3, (img1.Height + picture_height_distance) * (limit_select-1) + first_distance + (img1.Height + picture_height_distance2));

            Graphics g = Graphics.FromImage(bitmap);

            g.Clear(picture_bg_color);   // 흰색 배경

            int start_x = picture_width_distance;
            int start_x2 = img1.Width + picture_width_distance+ picture_width_distance2;
            int start_y = first_distance;

            foreach (string filename in selectedPictures)
            {
                Image i = Image.FromFile(filename);
                g.DrawImage(i, start_x, start_y, i.Width, i.Height);
                g.DrawImage(i, start_x2, start_y, i.Width, i.Height);
                start_y = start_y + i.Height + picture_height_distance;
            }

            string print_picture_name = Path.Combine(folderPath, fileName);

            bitmap.Save(print_picture_name);

            g.Dispose();
            bitmap.Dispose();

            PreViewWindow pw = new PreViewWindow(print_picture_name);
            /* pw.Owner = this;*/
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { pw.ShowDialog(); }));
            }
            else
            {
                try
                {
                    pw.ShowDialog();
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1);
                }


            }

            this.Dispose();

        }

        private void cancle_Click(object sender, EventArgs e)
        {

        }

        private void ClickPictureBox(ref PictureBox box,ref bool box_bool,Image origin_img, string origin_img_name)
        {
            
            if (!box_bool)
            {
                if (selectedPictures.Count == limit_select)
                {
                    return;
                }

                OnSelectedDraw(ref box);
                selectedPictures.Add(origin_img_name);
                box_bool = !box_bool;
            }
            else
            {
                box.Image = origin_img;
                selectedPictures.Remove(origin_img_name);
                box_bool = !box_bool;
            }

            if (selectedPictures.Count < limit_select)
            {
                next.BackgroundImage = uncomplete_btn_up_img;
            }
            else
            {
                next.BackgroundImage = complete_btn_up_img;
            }


        }

        private void OnSelectedDraw(ref PictureBox box)
        {
           
            Pen pen = new Pen(selectedColor);
            pen.Width = penWidth;

            using (Graphics g = box.CreateGraphics())
            {
                g.DrawRectangle(pen, 0, 0, box.Width , box.Height );
            }
            
        }

       
    }
}
