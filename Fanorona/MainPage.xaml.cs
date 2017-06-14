using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Fanorona
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //Draw();
        }

        public void Draw()
        {
            var ellipse1 = new Ellipse();
            ellipse1.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            ellipse1.Width = 20;
            ellipse1.Height = 20;
            Container.Children.Add(ellipse1);
            var line1 = new Line();
            line1.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            line1.X2 = 400;
            
            Container.Children.Add(line1);
        }
    }
}
