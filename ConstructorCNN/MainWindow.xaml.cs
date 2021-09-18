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
using LibraryCNN.Other;
using LibraryCNN;

namespace ConstructorCNN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool statusWin = true;
        Network Net = new Network();
        int cvB, cv, po, fuB, fu;
        public MainWindow()
        {
            InitializeComponent();
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
            ConvalutionLayerBias layer = new ConvalutionLayerBias();//Layer
            LayerButton element = new LayerButton(layer, InfoGridConv, fuB);//Element
            StackConv.Children.Add(element);//Add elemet to stack
            cvB++;
        }
        private void Button_Click_ConvNoBias(object sender, RoutedEventArgs e)
        {
            ConvalutionLayer layer = new ConvalutionLayer();//Layer
            LayerButton element = new LayerButton(layer, InfoGridConv, fuB);//Element
            StackConv.Children.Add(element);//Add elemet to stack
            cv++;
        }
        private void Button_Click_Pooling(object sender, RoutedEventArgs e)
        {
            PoolingLayer layer = new PoolingLayer();//Layer
            LayerButton element = new LayerButton(layer, InfoGridConv, fuB);//Element
            StackConv.Children.Add(element);//Add elemet to stack
            po++;
        }
        private void Button_Click_FullyBias(object sender, RoutedEventArgs e)
        {
            FullyConnectBias layer = new FullyConnectBias();//Layer
            LayerButton element = new LayerButton(layer, InfoGridFully, fuB);//Element
            StackFully.Children.Add(element);//Add elemet to stack
            fuB++;
        }
        private void Button_Click_FullyNoBias(object sender, RoutedEventArgs e)
        {
            FullyConnectLayer layer = new FullyConnectLayer();//Layer
            LayerButton element = new LayerButton(layer, InfoGridFully, fuB);//Element
            StackFully.Children.Add(element);//Add elemet to stack
            fu++;
        }
    }
}
