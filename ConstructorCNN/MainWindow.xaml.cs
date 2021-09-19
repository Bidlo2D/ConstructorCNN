using System.Windows;
using System.Windows.Controls;
using LibraryCNN;
using OxyPlot;
using OxyPlot.Series;

namespace ConstructorCNN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool statusWin = true;
        int conv, fully;
        public MainWindow()
        {
            InitializeComponent();
            OnCreateLayer(new FullyConnectClassifier(), StackFully, InfoGridFully, ref fully, 0, false);
            OnCreateLayer(new FullyConnectInput(), StackFully, InfoGridFully, ref fully, 0, false);
            
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (statusWin) { WindowState = WindowState.Maximized; statusWin = false; }
            else { WindowState = WindowState.Normal; statusWin = true; }
        }
        private void Button_Click_ConvBias(object sender, RoutedEventArgs e)
        {
            OnCreateLayer(new ConvalutionLayerBias(), StackConv, InfoGridConv, ref conv, 0);
        }
        private void Button_Click_ConvNoBias(object sender, RoutedEventArgs e)
        {
            OnCreateLayer(new ConvalutionLayer(), StackConv, InfoGridConv, ref conv, 0);
        }
        private void Button_Click_Pooling(object sender, RoutedEventArgs e)
        {
            OnCreateLayer(new PoolingLayer(), StackConv, InfoGridConv, ref conv, 0);
        }
        private void Button_Click_FullyBias(object sender, RoutedEventArgs e)
        {
            OnCreateLayer(new FullyConnectBias(), StackFully, InfoGridFully, ref fully);
        }
        private void Button_Click_FullyNoBias(object sender, RoutedEventArgs e)
        {
            OnCreateLayer(new FullyConnectLayer(), StackFully, InfoGridFully, ref fully);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void OnCreateLayer(AbLayer layer, StackPanel AddStack, Grid info, ref int counter, int index = 1, bool Fdelete = true)
        {
            LayerButton element = new LayerButton(layer, info, AddStack, counter, Fdelete);//Element
            AddStack.Children.Insert(index, element);//Add elemet to stack
            Network.Add(layer);
            counter++;
        }
    }
}
