using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using tcp_module;

namespace game01
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        double orginalWidth, originalHeight;
        ScaleTransform scale = new ScaleTransform();
        Boolean timer_flag = true;
        Boolean manu_flag1 = true;
        Boolean manu_flag2 = false;

        //server_variable
        Boolean server_connected = false;
        static string serverIp = "127.0.0.1";
        static int serverPort = 7979;
        static string return_From_Server = null;

        TcpModule tcpm = new TcpModule(serverIp, serverPort);

        public MainWindow()
        {
            InitializeComponent();
            InitializeNui();
            tcpm.initModule();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        KinectSensor nui = null;
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        void InitializeNui()
        {
            nui = KinectSensor.KinectSensors[0];

            nui.ColorStream.Enable();
            nui.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(nui_ColorFrameReady);

            nui.DepthStream.Enable();
            nui.SkeletonStream.Enable();
            //AllFrameReady: 컬러스트림, 뎁스스트림, 스캘러톤스트림의 모든 프레임이 준비되었을 때 발생하는 이벤트 
            nui.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(nui_AllFramesReady);

            nui.Start();
        }

        void nui_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
                  
            ColorImageFrame ImageParam = e.OpenColorImageFrame();

            if (ImageParam == null) return;

            byte[] ImageBits = new byte[ImageParam.PixelDataLength];
            ImageParam.CopyPixelDataTo(ImageBits);
            
            BitmapSource src = null;

            src = BitmapSource.Create(ImageParam.Width,     //비트맵 소스를 생성
                ImageParam.Height,
                96, 96,
                PixelFormats.Bgr32, //Bgra32 픽셀형식, r g, b, a 각 8비트
                null,
                ImageBits, // 실제영상데이터
                ImageParam.Width * ImageParam.BytesPerPixel);

            image1.Source = src; //정지영상
        }

        void nui_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            SkeletonFrame sf = e.OpenSkeletonFrame();

            if (sf == null) return;

            Skeleton[] skeltonData = new Skeleton[sf.SkeletonArrayLength];
            System.Diagnostics.Debug.WriteLine("SkeletonArrayLength: {0}", sf.SkeletonArrayLength);
            sf.CopySkeletonDataTo(skeltonData);

            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {
                    foreach (Skeleton sd in skeltonData) //java의 inhence문
                    {
                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            Joint joint = sd.Joints[JointType.HandRight];
                            /*좌표계 변환*/
                            CoordinateMapper coordMapper = new CoordinateMapper(nui);
                            DepthImagePoint depthPoint = coordMapper.MapSkeletonPointToDepthPoint(joint.Position, depthImageFrame.Format);

                            Point point = new Point((int)(image1.Width * depthPoint.X / depthImageFrame.Width),
                                (int)(image1.Height * depthPoint.Y / depthImageFrame.Height));
                            Canvas.SetLeft(Image6, point.X);
                            Canvas.SetTop(Image6, point.Y);


                            //////////////////button1
                            if ((point.X > 100 && point.X < 500) && (point.Y > 100 && point.Y < 200) && manu_flag1)
                            {
                                if (timer_flag)
                                {
                                    myTimer.Tick += new EventHandler(myTimer_Tick_Start);
                                    myTimer.Stop();
                                    timer_flag = false;

                                    myTimer.Interval = 3000;
                                    myTimer.Start();
                                }
                            }
                        
                            //////////////////button2
                            else if ((point.X > 100 && point.X < 500) && (point.Y > 300 && point.Y < 400) && manu_flag1)
                            {
                                if (timer_flag)
                                {
                                    myTimer.Tick += new EventHandler(myTimer_Tick_Finish);
                                    myTimer.Stop();
                                    timer_flag = false;

                                    myTimer.Interval = 3000;
                                    myTimer.Start();
                                }
                            }

                            //////////////////manu2
 
                            else if((point.X > 75 && point.X < 200) && (point.Y >100 && point.Y < 250) && manu_flag2)
                            {
                                if (timer_flag)
                                {
                                    myTimer.Tick += new EventHandler(myTimer_Tick_Create_Room);
                                    myTimer.Stop();
                                    timer_flag = false;

                                    myTimer.Interval = 3000;
                                    myTimer.Start();
                                }
                            }

                            else if((point.X > 237.5 && point.X < 362.5) && (point.Y > 100 && point.Y < 250) && manu_flag2)
                            {
                                if (timer_flag)
                                {
                                    myTimer.Tick += new EventHandler(myTimer_Tick_Matching);
                                    myTimer.Stop();
                                    timer_flag = false;

                                    myTimer.Interval = 3000;
                                    myTimer.Start();
                                }
                            }

                            //////////////////button3
                            else if ((point.X > 400 && point.X < 525) && (point.Y > 100 && point.Y < 250) && manu_flag2)
                            {
                                if (timer_flag)
                                {
                                    myTimer.Tick += new EventHandler(myTimer_Tick_Return_Menu1);
                                    myTimer.Stop();
                                    timer_flag = false;

                                    myTimer.Interval = 3000;
                                    myTimer.Start();
                                }
                            }

                            else
                            {
                                myTimer.Stop();
                                myTimer.Enabled = true;
                                timer_flag = true;
                            }
                        }
                    }
                }
            }
        }

        void myTimer_Tick_Matching(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void myTimer_Tick_Create_Room(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            return_From_Server = tcpm.requestAndWaiting("#Create");
            System.Diagnostics.Debug.WriteLine(return_From_Server);
            if(return_From_Server.Equals("#Create"))
            {
                this.Image3.Visibility = Visibility.Hidden;
                this.Image4.Visibility = Visibility.Hidden;
                this.Image5.Visibility = Visibility.Hidden;
                manu_flag2 = false;
            }
        }

        void myTimer_Tick_Start(object sender, EventArgs e)
        {
            if (!server_connected)
            {
                server_connected = tcpm.connectToServer();
                System.Diagnostics.Debug.WriteLine("success1\n");
            }
            if (server_connected)
            {
                //throw new NotImplementedException();
                this.Image1.Visibility = Visibility.Hidden;
                this.Image2.Visibility = Visibility.Hidden;
                manu_flag1 = false;

                this.Image3.Visibility = Visibility.Visible;
                this.Image4.Visibility = Visibility.Visible;
                this.Image5.Visibility = Visibility.Visible;
                manu_flag2 = true;
                System.Diagnostics.Debug.WriteLine("success2\n");
            }
        }

        void myTimer_Tick_Finish(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Application.Current.Shutdown();
        }

        void myTimer_Tick_Return_Menu1(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.Image1.Visibility = Visibility.Visible;
            this.Image2.Visibility = Visibility.Visible;
            manu_flag1 = true;

            this.Image3.Visibility = Visibility.Hidden;
            this.Image4.Visibility = Visibility.Hidden;
            this.Image5.Visibility = Visibility.Hidden;
            manu_flag2 = false;
            tcpm.closeModule();
        }

        void Window1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeSize(e.NewSize.Width, e.NewSize.Height);
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            orginalWidth = this.Width;
            originalHeight = this.Height;
            this.SizeChanged += new SizeChangedEventHandler(Window1_SizeChanged);
            this.WindowState = WindowState.Maximized;
        }

        private void ChangeSize(double width, double height)
        {
            scale.ScaleX = width / orginalWidth;
            scale.ScaleY = height / originalHeight;
            FrameworkElement rootElement = this.Content as FrameworkElement;
            rootElement.LayoutTransform = scale;
        }
    }
}
