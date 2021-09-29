using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LibraryCNN;
using LibraryCNN.Other;
using OxyPlot;

namespace ConstructorCNN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IList<DataPoint> Points = new List<DataPoint>();
        private string PathData;
        private bool statusWin = true, stoped;
        private int conv, fully;
        public MainWindow()
        {
            InitializeComponent();
            OnCreateLayer(new FullyConnectClassifier(), StackFully, InfoGridFully, ref fully, 0, false);
            OnCreateLayer(new FullyConnectInput(), StackFully, InfoGridFully, ref fully, 0, false);
            Converter.DirImagesToTensor(@"C:\Games\Programs\Fonts\alphabet");
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
            Network.Add(layer, index);
            counter++;
        }
        private void Button_Start(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            On_Off(false, b, ConstructNetGrid);
            stoped = false;
            StatusBox.Clear();
            Points.Clear();
            Thread train = new Thread(new ParameterizedThreadStart(TheardTrain));
            if (Network.CountLayer != 0 && Converter.Images.Count != 0)
            {
                EpothProgress.Value = EpothProgress.Minimum;
                DataProgress.Value = DataProgress.Minimum;
                DataProgress.Maximum = Converter.Images.Count;
                EpothProgress.Maximum = Network.Epoth;
                train.Start(sender);
            }
            else
            {
                StatusBox.AppendText("Error - No data!");
                On_Off(true, b, ConstructNetGrid);
            }
        }
        private void TheardTrain(object obj)
        {
            Button c = (Button)obj;
            try
            {
                for (int i = 0; i < Network.Epoth; i++)
                {
                    int CountRight = 0;
                    Dispatcher.Invoke(new Action(() =>
                    { DataProgress.Value = DataProgress.Minimum; }));
                    foreach (var image in Converter.Images)
                    {
                        Network.ForwardNet(image);
                        Network.BackNet(image);
                        if (Network.Answer == image.Right) { CountRight++; }
                        Dispatcher.Invoke(new Action(() =>
                        { DataProgress.Value++; }));
                    }
                    //Status by one epoch
                    Dispatcher.Invoke(new Action(() =>
                    {
                        StatusBox.AppendText($"Epoth - {i + 1}, Loss - {Math.Round(Network.Loss, 6)}, Count right - {CountRight}\n");
                        Points.Add(new DataPoint(i + 1, Network.Loss));
                        ChartsLoss.Series[0].ItemsSource = Points;
                        ChartsLoss.InvalidatePlot();// InvalidateVisual();
                        EpothProgress.Value++;
                        StatusBox.ScrollToEnd();
                    }));
                    if (stoped) { break; }
                    Thread.Sleep(5);
                }
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    StatusBox.Clear();
                    StatusBox.AppendText($"Error! - {e.Message}\n");
                    StatusBox.ScrollToEnd();
                }));
            }
            //End
            Dispatcher.Invoke(new Action(() =>
            {
                EpothProgress.Value = EpothProgress.Minimum;
                DataProgress.Value = DataProgress.Minimum;
                On_Off(true, c, ConstructNetGrid);
            }));
        }
        private void On_Off( bool OffOn, params FrameworkElement[] controls)
        {
            if (OffOn)
            {
                foreach(var i in controls)
                {
                    i.IsEnabled = true; 
                }
            }
            else
            {
                foreach (var i in controls)
                {
                    i.IsEnabled = false;
                }
            }
        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            stoped = true;
        }
        private void Button_Browse(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PathData = dialog.SelectedPath.ToString();
                    Converter.DirImagesToTensor(PathData);
                    foreach (var image in Converter.ImagesPath)
                    {
                        Image im = new Image();
                        im.Source = new BitmapImage(new Uri(image));
                        StackImages.Children.Add(im);
                    }
                }
            }
        }
        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            Converter.ClearDate();
            StackImages.Children.Clear();
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            //new Thread(() => { }).Start();
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "bin files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StackConv.Children.Clear();
                StackFully.Children.Clear();
                string name = openFileDialog.FileName;
                using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate))
                {
                    try
                    {
                        var layers = Converter.LoadNet(fs);
                        foreach (AbLayer item in layers.Layer)
                        {
                            if (item.GetType() == typeof(ConvalutionLayer)
                                || item.GetType().BaseType == typeof(ConvalutionLayer))
                            {
                                OnCreateLayer(item, StackConv, InfoGridConv, ref conv, 0);
                            }
                            else
                            {
                                OnCreateLayer(item, StackFully, InfoGridFully, ref fully, 0);
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        StatusBox.Clear();
                        StatusBox.AppendText($"Error! - {error.Message}\n");
                        StatusBox.ScrollToEnd();
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog openFileDialog = new System.Windows.Forms.SaveFileDialog();
            openFileDialog.Filter = "bin files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string name = openFileDialog.FileName;
                using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate))
                {
                    Converter.SaveNet(fs);
                }
            }
        }

        private void TextBox_EpothChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox a = (TextBox)sender;
            if (!String.IsNullOrWhiteSpace(EpothsText.Text))
            {
                EpothsText.Text = EpothsText.Text.Replace(" ", "");
                Network.Epoth = Convert.ToInt32(EpothsText.Text);
                EpothsText.CaretIndex = EpothsText.Text.Length;
            }
        }
        private void TextBox_EpothTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            char number = Convert.ToChar(e.Text);
            if (!Char.IsDigit(number) && number != 8 && number != 48) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
    }
}
