using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace SnapshotsOfLifeEx01
{
    public partial class MainWindow : Form, IObserver
    {
        private CameraController _controller = null;
        private IObserver formAs = null;
        private ActionSource _actionSource = new ActionSource();
        private List<ActionListener> _actionListenerList = new List<ActionListener>();
        private IntPtr[] _volumes = new IntPtr[2];
        private int _volume_count = 0;
        private List<string> pictures = new List<string>(){ };
        private int TakePicture = 8;  //사진찍는 횟수
        private Timer t = new Timer();  // 카운트 타이머
        private int timer_time; // 현재 카운트 타이머 시간
        private int timer_time_limit = 5;  // 카운트 타이머 초 수
        private Timer s_t = new Timer();  // 셔터 타이머
        private int shutter_animation_interval = 10;
        private List<IObserver> _observerList = new List<IObserver>();

        private string folderPath = "./pictures";

        private Image intro = Image.FromFile("./ui/intro.png");
        private Image bg = Image.FromFile("./ui/main.png");
        private Image start_up_btn = Image.FromFile("./ui/startbutton_up.png");
        private Image start_down_btn = Image.FromFile("./ui/startbutton_down.png");
        private Image picture_up_btn = Image.FromFile("./ui/picturebutton_up.png");
        private Image picture_down_btn = Image.FromFile("./ui/picturebutton_down.png");

        private Image[] flash_pictures = { Image.FromFile("./ui/flash01.png") , Image.FromFile("./ui/flash02.png"), Image.FromFile("./ui/flash03.png"), Image.FromFile("./ui/flash04.png"), Image.FromFile("./ui/flash05.png"),
                                            Image.FromFile("./ui/flash06.png") ,Image.FromFile("./ui/flash07.png") ,Image.FromFile("./ui/flash08.png") ,Image.FromFile("./ui/flash09.png") ,};
        private int flash_pictures_count = 0;

       /* private Image picture_animation;*/

        private long maxSizeByte = 1024 * 1024 * 500; // 최고 용량 (500MB)
        private int deleteFilesCount = 9;   // 용량 초과시 지울 파일 개수




        public MainWindow(ref CameraController controller)
        {
            uint err = EDSDKLib.EDSDK.EDS_ERR_OK;
            // Get initial volume
            EDSDKLib.EDSDK.EdsVolumeInfo outVolumeInfo;

            formAs = this;
            InitializeComponent();

            // 화면 및 픽쳐박스 버벅임 줄이기
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();


            _controller = controller;
            err = EDSDKLib.EDSDK.EdsGetChildCount(_controller.GetModel().Camera, out _volume_count);

            _actionListenerList.Add((ActionListener)_controller);

            _actionListenerList.ForEach(actionListener => _actionSource.AddActionListener(ref actionListener));








            _observerList.Add((IObserver)evfPictureBox1);

            System.Threading.Thread.Sleep(1000);
            
            _observerList.ForEach(observer => _controller.GetModel().Add(ref observer));

            evfPictureBox1.SetActionSource(ref _actionSource);

            takePictureButton.SetActionSource(ref _actionSource);
            /*takePictureButton.Command = ActionEvent.Command.TAKE_PICTURE;*/

            actionButton5.SetActionSource(ref _actionSource);
            actionButton5.Command = ActionEvent.Command.START_EVF;
            
            

            /*actionButton6.SetActionSource(ref _actionSource);
            actionButton6.Command = ActionEvent.Command.END_EVF;*/

            /*actionButton7.SetActionSource(ref _actionSource);
            actionButton7.Command = ActionEvent.Command.DOWNLOAD;*/

            t.Interval = 1000;
            t.Tick += new EventHandler(timer_Event);
            timer_time = timer_time_limit;

            s_t.Interval = shutter_animation_interval;
            s_t.Tick += new EventHandler(shutter_timer_Event);

            

            /*evfPictureBox1.Parent = panel1;*/
            /*evfPictureBox1.BackColor = Color.Transparent;*/

            label1.Parent = pictureBox1;
           /* label1.BackColor = Color.Transparent;*/
            label1.Location = new Point((pictureBox1.Width / 2) - (label1.Width / 2), (pictureBox1.Height / 2) - (label1.Height / 2));
            label1.Text = " ";

            label2.Parent = panel1;
            /*label2.BackColor = Color.Transparent;*/
            /*label2.Location = new Point((evfPictureBox1.Width) - ((label3.Width * 2 / 3) + (label2.Width *4 / 5)), (0));*/
            label2.Text = " ";

            label3.Parent = panel1;
            /*label3.BackColor = Color.Transparent;*/
            /*label3.Location = new Point((evfPictureBox1.Width ) - (label3.Width *2 / 3), (0));*/
            label3.Text = " ";

            pictureBox1.Parent = evfPictureBox1;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Size = new Size(evfPictureBox1.Size.Width, evfPictureBox1.Size.Height);

            /* pictureBox1.BackColor = Color.Transparent;*/



            panel1.BackgroundImage = intro;
            actionButton5.BackgroundImage = start_up_btn;
            takePictureButton.BackgroundImage = picture_up_btn;

            /* evfPictureBox1.Image = Properties.Resources.intro;*/

            // 사진 저장 폴더 생성
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            /*_controller.EndREC();   // 기본이 REC 모드인데 REC모드에서는 사진이 저장안됨.. */

            /*_controller.StartEvf();*/

            _controller.EndEvf();   // 시작시 초기화


            // DS버전이 아닐시 스타트를 따로 해줘야함
            if (!_controller.GetModel().isTypeDS)
            {
                _actionSource.FireEvent(ActionEvent.Command.REMOTESHOOTING_START, IntPtr.Zero);
            }












            






            // 예제
            /* ChoiceWindow cw = new ChoiceWindow(new List<string>() { "./pictures/20241008140422764.jpg", "./pictures/20241008140422764.jpg", "./pictures/20241008140422764.jpg", "./pictures/20241008140422764.jpg", "./pictures/20241008140422764.jpg", "./pictures/20241008140422764.jpg", "./pictures/20241008140422764.jpg", "./pictures/20241008140422764.jpg" }, folderPath);
             *//* cw.Owner = this;*//*
             if (this.InvokeRequired)
             {
                 this.Invoke(new MethodInvoker(delegate { cw.ShowDialog(); }));
             }
             else
             {
                 cw.ShowDialog();

             }*/
            // 예제
          /*  PreViewWindow pw = new PreViewWindow("./pictures/Com_20241011155802965.jpg");
            pw.Owner = this;
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


            }*/






        }

      

       

        private void takePictureButton_Click(object sender, EventArgs e)
        {


            takePictureButton.Visible = false;
            takePictureButton.Enabled = false;
            t.Start();
           


            /* evfPictureBox1.ChangeBackImage();*/       // 찍은후 배경 바꾸기
        }

       /* private void DrawAnimationsPaintEvent(object sender, System.EventArgs e)
        {
            ImageAnimator.UpdateFrames(picture_animation);
            pictureBox1.Image = picture_animation;
            
        }*/

        private void startButton_Click(object sender, EventArgs e)
        {

            PicturesDelete();

            panel1.BackgroundImage = bg;
            actionButton5.Visible = false;
            actionButton5.Enabled = false;

            label2.Text = "0";
            label3.Text = "/8";

            /*SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            ImageAnimator.Animate(picture_animation, this.OnFrameChangedHandler);

            s_t.Start();*/

        }

        /*        private void AnimateStart()
                {
                    if (!currentlyAnimating)
                    {
                        ImageAnimator.Animate(picture_animation_Bit, new EventHandler(this.OnFrameChangedHandler));
                        currentlyAnimating = true;
                    }

                }

                private void OnFrameChangedHandler(object sender, EventArgs e)
                {
                    this.Invalidate();
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                    AnimateStart();
                    ImageAnimator.UpdateFrames();
                    e.Graphics.DrawImage(picture_animation_Bit, new Point(0, 0));

                }*/


        

        private void MainWindow_Load(object sender, EventArgs e)
        {
           
           
        }
        

     

        public void Update(Observable from, CameraEvent e)
        {
            CameraEvent.Type eventType = e.GetEventType();

            switch (eventType)
            {
                case CameraEvent.Type.SHUT_DOWN:

                    MessageBox.Show("Camera is disconnected");

                    this.Close();

                    break;

                case CameraEvent.Type.PROPERTY_CHANGED:

                    break;

                case CameraEvent.Type.ANGLEINFO:

                    break;

                case CameraEvent.Type.MOUSE_CURSOR:

                    break;

                case CameraEvent.Type.DOWNLOAD_COMPLETE:
                    Console.WriteLine("CameraEvent.Type.DOWNLOAD_COMPLETE");
                    /*bool result = evfPictureBox1.ChangeBackImage();
                    if (result)
                    {
                        ChoiceWindow cw = new ChoiceWindow();
                        cw.Owner = this;
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new MethodInvoker(delegate { cw.ShowDialog(); }));
                        }
                        else
                        {
                            cw.ShowDialog();
                        }

                        this.DialogResult = DialogResult.OK;
                        

                        *//* Program.cw.Show();
                         Program.mw.Hide();*//*
                    }*/

                    break;

                default:
                    break;
            }


        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {

            _controller.EndEvf();

            Application.DoEvents();
            _actionSource.FireEvent(ActionEvent.Command.END_ROLLPITCH, IntPtr.Zero);
            _actionSource.FireEvent(ActionEvent.Command.END_EVF, IntPtr.Zero);
            if (!_controller.GetModel().isTypeDS)
            {
                _actionSource.FireEvent(ActionEvent.Command.REMOTESHOOTING_STOP, IntPtr.Zero);
            }
            _actionSource.FireEvent(ActionEvent.Command.PRESS_OFF, IntPtr.Zero);
            _actionSource.FireEvent(ActionEvent.Command.EVF_AF_OFF, IntPtr.Zero);
            _observerList.ForEach(observer => _controller.GetModel().Remove(ref observer));

            t.Stop();
            s_t.Stop();

            _actionSource.FireEvent(ActionEvent.Command.CLOSING, IntPtr.Zero);
            _actionListenerList.ForEach(actionListener => _actionSource.RemoveActionListener(ref actionListener));
        }

        

        /*private Bitmap ModifyImage(Bitmap original, int width, int height, float dpi)
        {
            // 해상도와 크기 변경
            Bitmap newBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, width, height);
            }

            *//*// 해상도 설정
            newBitmap.SetResolution(dpi, dpi);*//*



            // 비트 깊이 변경 (예: 32비트에서 8비트로 변환)
           *//* return ConvertTo8Bit(newBitmap);*//*
            return newBitmap;
        }*/

        private Bitmap CropAndResizeBitmap(Bitmap sourceBitmap, int targetWidth, int targetHeight)
        {
            double targetAspectRatio = (double)targetWidth / targetHeight;
            double sourceAspectRatio = (double)sourceBitmap.Width / sourceBitmap.Height;

            Rectangle cropArea;

            // 비율에 맞게 잘라낼 영역 계산
            if (sourceAspectRatio > targetAspectRatio)
            {
                // 원본 이미지가 더 넓은 경우
                int newWidth = (int)(sourceBitmap.Height * targetAspectRatio);
                int x = (sourceBitmap.Width - newWidth) / 2; // 중앙 정렬
                cropArea = new Rectangle(x, 0, newWidth, sourceBitmap.Height);
            }
            else
            {
                // 원본 이미지가 더 높은 경우
                int newHeight = (int)(sourceBitmap.Width / targetAspectRatio);
                int y = (sourceBitmap.Height - newHeight) / 2; // 중앙 정렬
                cropArea = new Rectangle(0, y, sourceBitmap.Width, newHeight);
            }

            // 잘라낸 비트맵 생성
            Bitmap croppedBitmap = new Bitmap(targetWidth, targetHeight);
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourceBitmap, new Rectangle(0, 0, targetWidth, targetHeight), cropArea, GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }

        /*private Bitmap ConvertTo8Bit(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);

            // 색상 팔레트를 설정
            ColorPalette palette = newBitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i); // 그레이스케일 팔레트
            }
            newBitmap.Palette = palette;

            // 픽셀 변경
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11); // 그레이스케일 변환
                    newBitmap.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            return newBitmap;
        }*/

        private void timer_Event(object sender, System.EventArgs e)
        {
            if (timer_time == 0)    // 0초일때 사진을 찍음
            {
                /*Console.WriteLine("is 0");*/
                s_t.Start();

                timer_time = timer_time_limit;
                label1.Text = " ";
                t.Stop();
                picture_downHandler();

                
                
                return;
            }



            
            label1.Text = timer_time.ToString();
            timer_time--;
            
        }

        private void picture_downHandler()
        {
            DateTime now = DateTime.Now;
            string fileName = now.ToString("yyyyMMddHHmmssFFF") + ".jpg";

            fileName = Path.Combine(folderPath,fileName);


            // 사이즈 변환가능
            /*Bitmap bitmap = ModifyImage((Bitmap)evfPictureBox1.Image, 520, 380, 72f);*/
            using (Bitmap bitmap = CropAndResizeBitmap((Bitmap)evfPictureBox1.Image,520,380))
            {
                bitmap.Save(fileName, ImageFormat.Jpeg);
                
            }
               
            /*evfPictureBox1.Image.Save(fileName);*/

            pictures.Add(fileName);

            label2.Text = pictures.Count.ToString();    // 현재 찍은 사진 갯수

            
            if (pictures.Count == TakePicture)
            {
                ChoiceWindow cw = new ChoiceWindow(pictures, folderPath);
                /* cw.Owner = this;*/
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { cw.ShowDialog(); }));
                }
                else
                {
                    cw.ShowDialog();

                }

                this.DialogResult = DialogResult.OK;
                pictures.Clear();
                _controller.EndEvf();

                takePictureButton.Visible = true;
                takePictureButton.Enabled = true;
                actionButton5.Visible = true; 
                actionButton5.Enabled = true;

                label2.Text = " ";
                label3.Text = " ";

                panel1.BackgroundImage = intro;

            }
            else
            {
                t.Start();
            }



        }

        private void inShutter()
        {

            /*Bitmap canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            int iWidth = canvas.Width;
            int iHeight = canvas.Height;

            using (Graphics g = Graphics.FromImage(canvas))
            {

                SolidBrush black = new SolidBrush(Color.Black);

                Rectangle rectLeft = new Rectangle(0, 0, iWidth / 2 - 100, iHeight);
                Rectangle rectRight = new Rectangle(iWidth / 2 + 100, 0, iWidth, iHeight);
                g.FillRectangle(black, rectLeft);
                g.FillRectangle(black, rectRight);

                pictureBox1.Image = canvas;
            }*/

            pictureBox1.Image = flash_pictures[flash_pictures_count];
            flash_pictures_count++;
        }


        private void outShutter()
        {
            pictureBox1.Image = null;
            flash_pictures_count = 0;
        }


        private void shutter_timer_Event(object sender, System.EventArgs e)
        {
            if (flash_pictures_count < flash_pictures.Length)
            {
                inShutter();
            }
            else
            {
                s_t.Stop();
                outShutter();

            }

        }

        private void PicturesDelete()
        {
            List<FileInfo> files = Directory.GetFiles(folderPath)
            .Select(f => new FileInfo(f))
            .OrderBy(f => f.CreationTime) // 생성일 기준으로 정렬
            .ToList();

            long currentSize = files.Sum(f => f.Length);

            Console.WriteLine("lenght = " + currentSize);

            if (currentSize < maxSizeByte)
            {
                return;
            }


            for (int i = 0; i < deleteFilesCount; i++)
            {
                FileInfo oldestFile = files.First();
                files.RemoveAt(0);

                try
                {
                    File.Delete(oldestFile.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("파일 삭제 오류 발생 : " + ex.Message);
                }
            }
        }

        

        private void evfPictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void actionButton5_MouseDown(object sender, MouseEventArgs e)
        {
            actionButton5.BackgroundImage = start_down_btn;
        }

        private void actionButton5_MouseUp(object sender, MouseEventArgs e)
        {
            actionButton5.BackgroundImage = start_up_btn;
        }

        private void takePictureButton_MouseDown(object sender, MouseEventArgs e)
        {
            takePictureButton.BackgroundImage = picture_down_btn;
        }

        private void takePictureButton_MouseUp(object sender, MouseEventArgs e)
        {
            takePictureButton.BackgroundImage = picture_up_btn;
        }

       
    }
}
