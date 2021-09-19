﻿using System;
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

        private void OnCreateLayer(AbLayer layer, StackPanel AddStack, Grid info, ref int counter, int index = 1, bool Fdelete = true)
        {
            LayerButton element = new LayerButton(layer, info, AddStack, counter, Fdelete);//Element
            AddStack.Children.Insert(index, element);//Add elemet to stack
            counter++;
        }
    }
}