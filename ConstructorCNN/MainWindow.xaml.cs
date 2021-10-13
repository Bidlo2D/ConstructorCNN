using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
        private IList<DataPoint> PointsTest = new List<DataPoint>();
        private IList<DataPoint> PointsTrain = new List<DataPoint>();
        private bool statusWin = true, stoped;
        private int conv, fully, percent, batchSize;
        class MyClass
        {
            public int Pop { get; set; } = 5;
        }
        public MainWindow()
        {
            InitializeComponent();
            ChannelComboBox.ItemsSource = Enum.GetValues(typeof(TypeChannel)).Cast<TypeChannel>();
            ChannelComboBox.SelectedItem = Network.Channel;
            OnCreateLayerAdd(new FullyConnectInput(), StackFully, InfoGridFully, ref fully, false);
            OnCreateLayerAdd(new FullyConnectClassifier(), StackFully, InfoGridFully, ref fully, false);
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
            OnCreateLayerAdd(new ConvalutionLayerBias(), StackConv, InfoGridConv, ref conv);
        }
        private void Button_Click_ConvNoBias(object sender, RoutedEventArgs e)
        {
            OnCreateLayerAdd(new ConvalutionLayer(), StackConv, InfoGridConv, ref conv);
        }
        private void Button_Click_Pooling(object sender, RoutedEventArgs e)
        {
            OnCreateLayerAdd(new PoolingLayer(), StackConv, InfoGridConv, ref conv);
        }
        private void Button_Click_FullyBias(object sender, RoutedEventArgs e)
        {
            OnCreateLayerIndex(new FullyConnectBias(), StackFully, InfoGridFully, ref fully);
        }
        private void Button_Click_FullyNoBias(object sender, RoutedEventArgs e)
        {
            OnCreateLayerIndex(new FullyConnectLayer(), StackFully, InfoGridFully, ref fully);
        }
        private void OnCreateLayerIndex(AbLayer layer, StackPanel AddStack, Grid info, ref int counter, int index = 1, bool Fdelete = true)
        {
            LayerButton element = new LayerButton(layer, info, AddStack, counter, Fdelete);//Element
            AddStack.Children.Insert(index, element);//Add elemet to stack
            Network.AddIndex(layer, index);
            counter++;
        }
        private void OnCreateLayerAdd(AbLayer layer, StackPanel AddStack, Grid info, ref int counter, bool Fdelete = true)
        {
            LayerButton element = new LayerButton(layer, info, AddStack, counter, Fdelete);//Element
            AddStack.Children.Add(element);//Add elemet to stack
            Network.Add(layer);
            counter++;
        }
        private void OnCreateLayerLoad(AbLayer layer, StackPanel AddStack, Grid info, ref int counter, bool Fdelete = true)
        {
            LayerButton element = new LayerButton(layer, info, AddStack, counter, Fdelete);//Element
            AddStack.Children.Add(element);//Add elemet to stack
            Network.Load(layer);
            counter++;
        }
        private void Button_Start(object sender, RoutedEventArgs e)
        {
            On_Off(false, StartB, ConstructNetGrid, TabData, EpothsBox, ProbabilityBox, LearnRatioBox, RatioBoxA, SaveB, LoadB);
            stoped = false;
            StatusBox.Clear();
            PointsTrain.Clear();
            PointsTest.Clear();
            Thread train = new Thread(TheardTrain);
            if (Network.CountLayers != 0 && Network.SelectionData != null)
            {
                EpothProgress.Value = EpothProgress.Minimum;
                EpothProgress.Maximum = Network.Epoth;
                BatchBar.Value = BatchBar.Minimum;
                BatchBar.Maximum = Network.SelectionData.Batches.Count;
                train.Start();
            }
            else
            {
                StatusBox.AppendText("Error - No data!");
                On_Off(true, StartB, ConstructNetGrid, TabData, EpothsBox, ProbabilityBox, LearnRatioBox, RatioBoxA, SaveB, LoadB);
            }
        }
        private void TheardTrain()
        {
            //try
            //{
                for (int i = 0; i < Network.Epoth; i++)
                {
                    (int, double) result = Train();
                    //Status by one epoch train
                    Dispatcher.Invoke(new Action(() =>
                    {
                        StatusBox.AppendText($"Epoth - {i + 1}, Loss - {Math.Round(result.Item2, 6)}, Count right - {result.Item1}\n");
                        PointsTrain.Add(new DataPoint(i + 1, Convert.ToDouble(Network.Loss)));
                        ChartsLoss.Series[0].ItemsSource = PointsTrain;
                        ChartsLoss.InvalidatePlot();
                        EpothProgress.Value++;
                        StatusBox.ScrollToEnd();
                    }));
                    double lossTest = Test();
                    //Statistic test
                    Dispatcher.Invoke(new Action(() =>
                    {
                        PointsTest.Add(new DataPoint(i + 1, Convert.ToDouble(lossTest)));
                        ChartsLoss.Series[1].ItemsSource = PointsTest;
                        ChartsLoss.InvalidatePlot();
                    }));
                    if (stoped) { break; }
                    Thread.Sleep(5);
                }
/*            }
            catch (Exception e)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    StatusBox.Clear();
                    StatusBox.AppendText($"Error! - {e.Message}\n");
                    StatusBox.ScrollToEnd();
                }));
            }*/
            //End
            Dispatcher.Invoke(new Action(() =>
            {
                EpothProgress.Value = EpothProgress.Minimum;
                BatchBar.Value = BatchBar.Minimum;
                MinibatchBar.Value = MinibatchBar.Minimum;
                On_Off(true, StartB, ConstructNetGrid, TabData, EpothsBox, ProbabilityBox, LearnRatioBox, RatioBoxA, SaveB, LoadB);
            }));
        }
        private double Test()
        {
            //Test
            double AvgLossTest = 0;
            Dispatcher.Invoke(new Action(() => {
                MinibatchBar.Maximum = Network.SelectionData.Test.Count;
                MinibatchBar.Value = MinibatchBar.Minimum;
            }));
            foreach (var image in Network.SelectionData.Test)
            {
                Network.ForwardNet(image);
                Network.BackNoRefresh(image);
                AvgLossTest += Network.Loss;
                Dispatcher.Invoke(new Action(() =>
                { MinibatchBar.Value++; }));
            }
            return AvgLossTest / Network.SelectionData.Test.Count;
        }
        private (int, double) Train()
        {
            //Train
            int CountRight = 0;
            double AvgLossTrain = 0;
            Dispatcher.Invoke(new Action(() =>
            { BatchBar.Value = BatchBar.Minimum; }));
            foreach (var batch in Network.SelectionData.Batches)
            {
                Dispatcher.Invoke(new Action(() => {
                    MinibatchBar.Maximum = batch.Length;
                    MinibatchBar.Value = MinibatchBar.Minimum;
                    if ((bool)CheckDrop.IsChecked) { Network.DropOut(); }
                }));
                foreach (var image in batch)
                {
                    Network.ForwardNet(image);
                    Network.BackNet(image);
                    AvgLossTrain += Network.Loss;
                    if (Network.Answer == image.Right) { CountRight++; }
                    Dispatcher.Invoke(new Action(() =>
                    { MinibatchBar.Value++; }));
                }
                //AvgLossTrain /= batch.Length;
                Dispatcher.Invoke(new Action(() =>
                { 
                    BatchBar.Value++;
                    if ((bool)CheckDrop.IsChecked) { Network.DropIn(); }
                }));
            }
            return (CountRight, AvgLossTrain / Network.SelectionData.Batches.Count);
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
            string PathData = "";
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PathData = dialog.SelectedPath.ToString();
                    IsEnabled = false;
                }
            }
            new Thread(() =>
            {
                if (!String.IsNullOrEmpty(PathData))
                { BrowseImages(PathData, StackImagesTest, StackImagesTrain); }
            }).Start();
        }
        private void BrowseImages(string PathData, StackPanel panelTrain, StackPanel panelTest)
        {
            Dispatcher.Invoke(new Action(() =>
            { panelTrain.Children.Clear(); panelTest.Children.Clear(); }));
            Network.SelectionData = new Batch(PathData, batchSize, percent);//DirImagesToTensor(PathData);
            //DataBar.Maximum = Network.SelectionData.dataSet.Count;
            foreach (var mass in Network.SelectionData.Batches)
            {
                foreach (var image in mass)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Image im = new Image();
                        im.Source = new BitmapImage(new Uri(image.Path));
                        panelTrain.Children.Add(im);
                    }));
                }
            }
            //Test
            foreach (var image in Network.SelectionData.Test)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Image im = new Image();
                    im.Source = new BitmapImage(new Uri(image.Path));
                    panelTest.Children.Add(im);
                }));
            }
            Dispatcher.Invoke(new Action(() =>
            { IsEnabled = true; }));
        }
        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            Network.SelectionData = null;
            StackImagesTest.Children.Clear();
            StackImagesTrain.Children.Clear();
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
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
                                OnCreateLayerLoad(item, StackConv, InfoGridConv, ref conv);
                            }
                            else
                            {
                                OnCreateLayerLoad(item, StackFully, InfoGridFully, ref fully);
                            }
                        }
                        Network.CheckLoad();
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
        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            if (Network.SelectionData != null)
            {
                BatchData.Maximum = Network.SelectionData.Test.Count;
                new Thread(() =>
                {
                    int CountRight = 0;
                    double AvgLossTest = 0;
                    Network.InitializationConv();
                    Network.InitializationFully();
                    foreach (var image in Network.SelectionData.Test)
                    {
                        Network.ForwardNet(image);
                        Network.BackNoRefresh(image);
                        AvgLossTest += Network.Loss;
                        if (Network.Answer == image.Right) { CountRight++; }
                        Dispatcher.Invoke(new Action(() =>
                        {
                            BatchData.Value++;
                        }));
                    }
                    Dispatcher.Invoke(new Action(() =>
                    {
                        BatchData.Value = BatchData.Minimum;
                        StatusReset.AppendText($"Loss - {AvgLossTest / Network.SelectionData.Test.Count},Count right - {CountRight}\n");
                        StatusReset.ScrollToEnd();
                        IsEnabled = true;
                    }));
                }).Start();
            }
            else
            {
                StatusReset.Clear();
                StatusReset.AppendText($"No data!\n");
                StatusReset.ScrollToEnd();
                IsEnabled = true;
            }
        }
        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            StackImagesTest.Children.Clear(); StackImagesTrain.Children.Clear();
            Network.SelectionData = null;
            //MenuItem a = (MenuItem)MenuCreateLayers.Items[1];
            //a.IsEnabled = false;
        }
        private void ComboBoxChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)sender;
            Network.Channel = (TypeChannel)a.SelectedItem;
        }
        private void TextBox_EpothChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox a = (TextBox)sender;
            if (!String.IsNullOrWhiteSpace(EpothsBox.Text))
            {
                EpothsBox.Text = EpothsBox.Text.Replace(" ", "");
                Network.Epoth = Convert.ToInt32(EpothsBox.Text);
                EpothsBox.CaretIndex = EpothsBox.Text.Length;
            }
        }
        private void TextBox_LearnRChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox a = (TextBox)sender;
            if (!String.IsNullOrWhiteSpace(EpothsBox.Text))
            {
                EpothsBox.Text = EpothsBox.Text.Replace(" ", "");
                Network.Epoth = Convert.ToInt32(EpothsBox.Text);
                EpothsBox.CaretIndex = EpothsBox.Text.Length;
            }
        }
        private void TextBox_RatioLearnTextChanged(object sender, TextChangedEventArgs e)
        {
            Network.LRate = (double)TextBoxChanged((TextBox)sender, Network.LRate);
        }
        private void TextBox_RatioATextChanged(object sender, TextChangedEventArgs e)
        {
            Network.ARatio = (double)TextBoxChanged((TextBox)sender, Network.ARatio);
        }
        private void TextBox_ProbabilityChanged(object sender, TextChangedEventArgs e)
        {
            Network.Probability = (double)TextBoxChanged((TextBox)sender, Network.Probability);
        }
        private void TextBox_PercentChanged(object sender, TextChangedEventArgs e)
        {
            percent = (int)TextBoxChanged((TextBox)sender, percent);
        }
        private void TextBox_BatchSizeChanged(object sender, TextChangedEventArgs e)
        {
            batchSize = (int)TextBoxChanged((TextBox)sender, batchSize);
        }
        private void TextBox_EpothTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            char number = Convert.ToChar(e.Text);
            if (!Char.IsDigit(number)) // цифры 
            {
                e.Handled = true;
            }
        }
        private void TextBox_RatioTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox box = (TextBox)sender;
            char number = Convert.ToChar(e.Text);
            if (!Char.IsDigit(number) && number != 44) // цифры и ,
            {
                e.Handled = true;
            }
            if (number == 44 && box.Text.Contains(number)) { e.Handled = true; }
        }
        private object TextBoxChanged(TextBox box, object data)
        {
            if (!String.IsNullOrWhiteSpace(box.Text))
            {
                box.Text = box.Text.Replace(" ", "");
                data = Convert.ChangeType(box.Text, data.GetType());
                box.CaretIndex = box.Text.Length;
            }
            return data;
        }
    }
}
