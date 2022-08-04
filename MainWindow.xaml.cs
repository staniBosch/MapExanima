using Memory.Win32;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MapExanima
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    ///    
    /// </summary>
    /// 
    ///  The important stuff happens in ReadMemoryOfExanima()
    /// <see cref="ReadMemoryOfExanima"/>
    public partial class MainWindow : Window
    {
        //The small map window size
        private const short SMWindow = 300;
        //if the window is big rezised
        bool isBig = false;
        //check for doubleclick on map
        static bool isDoubleClick = false;
        float[] posXY;
        //MAIN Thread for getting information and refreshing map
        Thread workThread;
        bool runThread = true;
        public MainWindow()
        {
            posXY = new float[2];
            InitializeComponent();
            workThread = new Thread(new ThreadStart(ReadMemoryOfExanima));
            workThread.Start();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(DropDownMap.SelectedIndex);
            switch (DropDownMap.SelectedIndex)
            {
                case 0:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID2]Map_LVL1.png"));
                    break;
                case 1:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID3]Map_LVL2.png"));
                    break;
                case 2:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID4]Map_LVL3.png"));
                    break;
                case 3:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID5]Map_Catacombs.png"));
                    break;
                case 4:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID6]Map_Archive.png"));
                    break;
                case 5:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID7]Map_Crossroads.png"));
                    break;
                case 6:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID8]Map_Golems.png"));
                    break;
                case 7:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID9]Map_CrossroadsSewers.png"));
                    break;
                case 8:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID10]Map_Market.png"));
                    break;
                case 9:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/Maps/[ID11]Map_MarketSewer.png"));
                    break;
            }
        }
        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            runThread = false;
            this.Close();
        }
        private void window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void KeyDown_Event(object s, KeyEventArgs e)
        {
            Thickness thicknessMap = this.MapImageElement.Margin;


            if (e.Key == Key.Up)
            {
                this.MapImageElement.Margin = new Thickness(thicknessMap.Left, thicknessMap.Top + 10, thicknessMap.Right, thicknessMap.Bottom);
            }
            if (e.Key == Key.Down)
            {
                this.MapImageElement.Margin = new Thickness(thicknessMap.Left, thicknessMap.Top - 10, thicknessMap.Right, thicknessMap.Bottom);
            }
            if (e.Key == Key.Left)
            {
                this.MapImageElement.Margin = new Thickness(thicknessMap.Left + 10, thicknessMap.Top, thicknessMap.Right, thicknessMap.Bottom);
            }
            if (e.Key == Key.Right)
            {
                this.MapImageElement.Margin = new Thickness(thicknessMap.Left - 10, thicknessMap.Top, thicknessMap.Right, thicknessMap.Bottom);
            }
        }
        private void window_MouseEnter(object s, MouseEventArgs e)
        {
            this.DropDownMap.Visibility = Visibility.Visible;
            this.close_btn.Visibility = Visibility.Visible;
            this.cordinate_txt.Visibility = Visibility.Visible;
        }
        private void window_MouseLeave(object s, MouseEventArgs e)
        {
            this.DropDownMap.Visibility = Visibility.Hidden;
            this.close_btn.Visibility = Visibility.Hidden;
            this.cordinate_txt.Visibility= Visibility.Hidden;
        }
        private void MouseClick_Event(object s, MouseEventArgs e)
        {
            if (isDoubleClick)
            {
                if (isBig)
                {
                    this.MaxHeight = SMWindow;
                    this.MaxWidth = SMWindow;
                    this.MinHeight = SMWindow;
                    this.MinWidth = SMWindow;
                    isBig = false;
                }
                else
                {
                    this.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                    this.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth - 600;
                    this.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                    this.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth - 600;
                    isBig = true;
                    this.MapImageElement.Margin = new Thickness(0, 0, 0, 0);
                }
            }
            else
            {
                isDoubleClick = true;
                new Thread(new ThreadStart(ThreadCheckDoubleClick)).Start();
            }

        }
        public static void ThreadCheckDoubleClick()
        {
            Thread.Sleep(250);
            isDoubleClick = false;
        }

        private void ReadMemoryOfExanima()
        {
            /*
             * Get the pointer to the memory of Exanima
             */
            Process process;
            IntPtr baseaddr;
            try
            {
                process = Process.GetProcessesByName("Exanima")[0];
                baseaddr = process.MainModule.BaseAddress;

            }
            catch
            {
                MessageBox.Show("No game named Exanima found. Pls start the game first! And thx for testing :)!", "Big Problem", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Dispatcher.Invoke(new Action(() => { this.Close(); }));
                return;
            };

            //System.Diagnostics.Debug.WriteLine(process.MainModule.BaseAddress.ToString("X"));

            MemoryHelper32 helper = new MemoryHelper32(process);

            /*
             * Those are the pointer to X/Y and lvl -> with CheatEngine tryanderror
             *  
             */
            uint XPtr = (uint)(0x2A7FF4) + (uint)baseaddr;
            uint YPtr = (uint)(0x2A7FFC) + (uint)baseaddr;
            uint LVLPtr = (uint)(0x2137C8) + (uint)baseaddr;

            //offsets: some areas in the game are placed off
            //TODO: Determine the areas "Position and Size" to get rid of the scaling and the offsets, there has to be an optimal Map.png
            int offsetX = 0;
            int offsetY = 0;
            float scaleF = 20;

            while (runThread)
            {
                /**
                 * READ FROM MEMORY ALL INFORMATION THAT ARE NEEDED
                 * POSITION, LVL, ITEMS
                 *
                 * POSTION
                 *
                 * Fetching up X/Y Coordinates [as float]
                 * The values are in their Engine format. Values between (-50000.0f) to (+50000.0f). 
                 * For the map(ImageObject) that is scaled 1150/1150 there has to be a mapping/tranformation
                 *
                 * Map LVL starts with value:2[as byte] on first floor 
                 *  
                 *   2      3       4        5          6         7          8           9            10         11
                 *  LVL 1 LVL 2   LVL 3   Catacombs   Archive  Crossroads  Golems Crossroads Sewer  Market  Market Sewer
                 */

                float X = helper.ReadMemory<float>(XPtr);
                float Y = helper.ReadMemory<float>(YPtr);
                byte mapLvl = helper.ReadMemory<byte>(LVLPtr);
                //System.Diagnostics.Debug.WriteLine("(X/Y): (" + X+ "/"+ Y +")");
                //System.Diagnostics.Debug.WriteLine(mapLvl);


                /*
                 * SetOffsets
                 */
                if (mapLvl < 5)
                {
                    offsetX = 11350;
                    offsetY = 10900;
                    scaleF = 20;
                }
                else if (mapLvl == 5)
                {
                    scaleF = 20;
                }
                else if (mapLvl == 6)
                {
                    offsetX = 13550;
                    offsetY = 11300;
                    scaleF = 22;
                }
                else if (mapLvl == 7)
                {
                    offsetX = 20000;
                    offsetY = 13600;
                    scaleF = 25;
                }
                else if (mapLvl == 8)
                {
                    offsetX = 17000;
                    offsetY = 15700;
                    scaleF = 27;
                }
                else if (mapLvl == 9)
                {

                }
                else if (mapLvl == 10)
                {
                    offsetX = 16000;
                    offsetY = 17200;
                    scaleF = 27.9f;

                }
                else if (mapLvl == 11)
                {

                }


                //refresh MAP and Location

                this.Dispatcher.Invoke(new Action(() =>
                    {
                        Thickness thicknessMap = this.MapImageElement.Margin;

                        posXY[0] = (X + offsetX) / 20;
                        posXY[1] = (Y + offsetY) / 20;

                        this.DropDownMap.SelectedIndex = Math.Max(mapLvl - 2,0);

                        if (isBig)
                        {
                            this.CPosition.Margin = new Thickness((X + offsetX) / scaleF + thicknessMap.Left, (Y + offsetY) / scaleF + thicknessMap.Top, 0, 0);
                        }
                        else
                        {
                            this.CPosition.Margin = new Thickness((X + offsetX) / scaleF + thicknessMap.Left, (Y + offsetY) / scaleF + thicknessMap.Top, 0, 0);
                            Thickness thicknessCPos = this.CPosition.Margin;
                            this.MapImageElement.Margin = new Thickness(-thicknessCPos.Left + SMWindow / 2 + thicknessMap.Left, -thicknessCPos.Top + SMWindow / 2 + thicknessMap.Top, 0, 0);

                        }
                        this.cordinate_txt.Text = "(" + posXY[0] + "/" + posXY[1] + ")" + this.DropDownMap.SelectedValue;
                        //this.cordinate_txt.Visibility = Visibility.Collapsed;
                    }));

                Thread.Sleep(200);
            }



        }

    }
}
