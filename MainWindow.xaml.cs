using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Configuration;
using System.Globalization;

namespace MapExanima
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    ///    
    /// </summary>
    /// 
    ///  The important stuff happens in ReadMemoryOfExanima()
    /// <see cref="ReadMemoryOfExanima"/>
    /// 


    public partial class MainWindow : Window
    {
        //Important addresses from the Memory Exanima
        private uint OFFSET_X_Ptr = 0x2A7FF4;
        private uint OFFSET_Y_Ptr = 0x2A7FFC;
        private uint OFFSET_LVL_Ptr = 0x2137C8;

        //The small map window size
        private const short SMWindow = 300;
        //if the window is big rezised
        bool isBig = false;
        //check for doubleclick on map
        static bool isDoubleClick = false;
      //  float[] posXY;
        byte mapLVL = 2;
        //MAIN Thread for getting information and refreshing map
        Thread workThread;
        bool runThread = true;

        //Maps offsets and speeds
        int offsetX = 10;
        int offsetY = 10;
        float scaleXY = 0.05f;
        public MainWindow()
        {
           // posXY = new float[2];
            InitializeComponent();
            checkDebugMode();
           
            

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
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed)
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
        }
        private void window_MouseLeave(object s, MouseEventArgs e)
        {
            this.DropDownMap.Visibility = Visibility.Hidden;
            this.close_btn.Visibility = Visibility.Hidden;        
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


            /*
             * Those are the pointer to X/Y and lvl -> with CheatEngine tryanderror
             *  
             */
            uint XPtr = OFFSET_X_Ptr + (uint)baseaddr;
            uint YPtr = OFFSET_Y_Ptr + (uint)baseaddr;
            uint LVLPtr = OFFSET_LVL_Ptr + (uint)baseaddr;

            //offsets: some areas in the game are placed off
            //TODO: Determine the areas "Position and Size" to get rid of the scaling and the offsets, there has to be an optimal Map.png
                                    
            float X = .0f;
            float Y = .0f;
            byte mapLVL_tmp = 0;
            mapLVL = mapLVL_tmp;

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
                X = ReadMemoryValueFloat(process, XPtr);
                Y = ReadMemoryValueFloat(process, YPtr);
                mapLVL_tmp = ReadMemoryValueByte(process, LVLPtr);

                if(mapLVL_tmp != mapLVL)
                {
                    mapLVL = mapLVL_tmp;
                    offsetX = Int32.Parse(getConfigValue("mapID_" + mapLVL + "_offsetX"));
                    offsetY = Int32.Parse(getConfigValue("mapID_" + mapLVL + "_offsetY"));
                    scaleXY = float.Parse(getConfigValue("mapID_" + mapLVL + "_scaleXY"), CultureInfo.InvariantCulture);
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        XValueSlider.Value = offsetX;
                        YValueSlider.Value = offsetY;
                        ScaleSlider.Value = scaleXY;
                    }));


                }
                
                //refresh MAP and Location

                this.Dispatcher.Invoke(new Action(() =>
                    {
                        Thickness thicknessMap = this.MapImageElement.Margin;

                        this.DropDownMap.SelectedIndex = Math.Max(mapLVL - 2, 0);

                        if (isBig)
                        {
                            this.CPosition.Margin = new Thickness((X + offsetX) * scaleXY + thicknessMap.Left, (Y + offsetY) * scaleXY + thicknessMap.Top, 0, 0);
                        }
                        else
                        {
                            this.CPosition.Margin = new Thickness((X + offsetX) * scaleXY + thicknessMap.Left, (Y + offsetY) * scaleXY + thicknessMap.Top, 0, 0);
                            Thickness thicknessCPos = this.CPosition.Margin;
                            this.MapImageElement.Margin = new Thickness(-thicknessCPos.Left + SMWindow / 2 + thicknessMap.Left, -thicknessCPos.Top + SMWindow / 2 + thicknessMap.Top, 0, 0);
                        }
                        this.cordinate_txt.Text = "(" + offsetX + "/" + offsetY + ")*"+scaleXY+" -" + this.DropDownMap.SelectedValue;
                        
                    }));

                Thread.Sleep(150);
            }

        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
           IntPtr hProcess,
           uint lpBaseAddress,
           byte[] lpBuffer,
           int nSize,
           IntPtr lpNumberOfBytesRead);

        private float ReadMemoryValueFloat(Process process, uint addr){
           
            byte[] data = new byte[(uint)Marshal.SizeOf(typeof(float))];
            ReadProcessMemory(process.Handle, addr, data, data.Length, IntPtr.Zero);         
            return System.BitConverter.ToSingle(data, 0);
        }
        private byte ReadMemoryValueByte(Process process, uint addr)
        {

            byte[] data = new byte[(uint)Marshal.SizeOf(typeof(byte))];
            ReadProcessMemory(process.Handle, addr, data, data.Length, IntPtr.Zero);
            return data[0];
        }

        private void XValue_Changed(object sender, TextChangedEventArgs e)
        {
            offsetX = Int32.Parse(XValue.Text);            
        }
        private void YValue_Changed(object sender, TextChangedEventArgs e)
        {
            offsetY = Int32.Parse(YValue.Text);
        }
        private void Scale_Changed(object sender, TextChangedEventArgs e)
        {
            scaleXY = float.Parse(Scale.Text);
        }

        private void checkDebugMode()
        {
            if (getConfigValue("debug") == "true")
            {
                cordinate_txt.Visibility = Visibility.Visible;
                XValue.Visibility = Visibility.Visible;
                YValue.Visibility = Visibility.Visible;
                Scale.Visibility = Visibility.Visible;
                savecal_btn.Visibility = Visibility.Visible;    
            } else
            {
                cordinate_txt.Visibility = Visibility.Collapsed;
                XValue.Visibility = Visibility.Collapsed;
                YValue.Visibility = Visibility.Collapsed;
                Scale.Visibility = Visibility.Collapsed;
                savecal_btn.Visibility = Visibility.Collapsed;
            }
        }

        private void debug_w(String s)
        {
            System.Diagnostics.Debug.WriteLine(s);  
        }

        private String getConfigValue(String key)
        {           
          return ConfigurationManager.AppSettings[key];
        }
        private void setConfigValue(String key, String value)
        {
            debug_w(key+" <- KEY"+" VALUE-> "+value);   
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                debug_w("Error writing app settings");
            }            
        }

        private void XValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            XValue.Text = XValueSlider.Value+"";
        }

        private void YValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            YValue.Text = YValueSlider.Value + "";
        }

        private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Scale.Text = ScaleSlider.Value + "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            setConfigValue("mapID_" + mapLVL + "_offsetX",offsetX+"");
            setConfigValue("mapID_" + mapLVL + "_offsetY", offsetY + "");
            setConfigValue("mapID_" + mapLVL + "_scaleXY", scaleXY + "");           
        }
    }



}
