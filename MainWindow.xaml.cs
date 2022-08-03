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
using System.Threading;
using System.Diagnostics;
using Memory.Win32;
using Memory.Utils;
using System.Runtime.InteropServices;
using Memory.Win64;

namespace MapExanima
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowSinker sinker;
        bool isBig = false;
        static bool isDoubleClick = false;
        public MainWindow()
        {
            //sinker = new WindowSinker(this);
            //sinker.Sink();
            InitializeComponent(); 

            this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map1Exanima.png"));
            new Thread(new ThreadStart(ReadMemoryOfExanima)).Start(); 
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DropDownMap.SelectedIndex);
            switch (DropDownMap.SelectedIndex)
            {
                case 0: 
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map1Exanima.png"));
                    break;
                case 1: 
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map2Exanima.png"));
                    break;
                case 2:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map2-1Exanima.png"));
                    break;
                case 3:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map3Exanima.png"));
                    break;
                case 4:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map4Exanima.png"));
                    break;
                case 5:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map5Exanima.png"));
                    break;
                case 6:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map5-1Exanima.png"));
                    break;
                case 7:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map6Exanima.png"));
                    break;
                case 8:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map7Exanima.png"));
                    break;
                case 9:
                    this.MapImageElement.Source = new BitmapImage(new Uri("pack://application:,,,/MapExanima;component/Map7-1Exanima.png"));
                    break;
            }
        }
        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
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
            
            if (e.Key == Key.Up) { 
                this.MapImageElement.Margin = new Thickness(thicknessMap.Left, thicknessMap.Top + 10, thicknessMap.Right, thicknessMap.Bottom);                
            }
            if (e.Key == Key.Down)
            {
                this.MapImageElement.Margin = new Thickness(thicknessMap.Left, thicknessMap.Top - 10, thicknessMap.Right, thicknessMap.Bottom);              
            }
            if (e.Key == Key.Left)
            {
                this.MapImageElement.Margin = new Thickness(thicknessMap.Left+10, thicknessMap.Top, thicknessMap.Right, thicknessMap.Bottom);              
            }
            if (e.Key == Key.Right)
            {
                this.MapImageElement.Margin = new Thickness(thicknessMap.Left-10, thicknessMap.Top, thicknessMap.Right, thicknessMap.Bottom);             
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
                    this.MaxHeight = 200;
                    this.MaxWidth = 200;
                    this.MinHeight = 200;
                    this.MinWidth = 200;
                    
                    isBig = false;
                }
                else
                {
                    this.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight-200;
                    this.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth-400;
                    this.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight-200;
                    this.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth-400;
                    isBig = true;                    
                    this.MapImageElement.Margin = new Thickness(0, 0, 0, 0);
                }
            } else
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

        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
          IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        private void ReadMemoryOfExanima()
        {
            Process process = Process.GetProcessesByName("Exanima")[0];
            if (process == null) return;

            IntPtr baseaddr = process.MainModule.BaseAddress;
            System.Diagnostics.Debug.WriteLine(process.MainModule.BaseAddress.ToString("X"));

            MemoryHelper32 helper = new MemoryHelper32(process);

            uint XPtr = (uint)(0x2A7FF4) + (uint)baseaddr;
            uint YPtr = (uint)(0x2A7FFC) + (uint)baseaddr; 

            

            //System.Diagnostics.Debug.WriteLine("DAS IST DER X POINTER -> "+XPtr.ToString("X"));
            //System.Diagnostics.Debug.WriteLine("DAS IST DER Y POINTER -> " + YPtr.ToString("X"));
            
            while (true)
            {
               
                //System.Diagnostics.Debug.WriteLine("Y: " + helper.ReadMemory<float>(0x6A7FFC));
                //System.Diagnostics.Debug.WriteLine("X: " + helper.ReadMemory<float>(0x6A7FF4));


                float X = helper.ReadMemory<float>(XPtr);
                float Y = helper.ReadMemory<float>(YPtr);

                //System.Diagnostics.Debug.WriteLine("(X/Y): (" + X+ "/"+ Y +")");

                this.Dispatcher.Invoke(new Action(() =>
                {
                    Thickness thicknessMap = this.MapImageElement.Margin;
                    Thickness thicknessCPos = this.CPosition.Margin;
                    //System.Diagnostics.Debug.WriteLine("(X/Y) CIRCLE: (" + thicknessCPos.Left+"/"+thicknessCPos.Top+")");
                   
                    this.CPosition.Margin = new Thickness((X+10850) / 20 + thicknessMap.Left, (Y+10750) / 20 + thicknessMap.Top, 0, 0);                    

                }));
                

                Thread.Sleep(200);
            }

           
         
        }
    }
}
