using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryCNN.Other;
using LibraryCNN;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Reflection;

namespace ConstructorCNN
{
    class LayerButton : Button
    {
        AbLayer Layer { get; set; }
        Panel controlInfo { get; set; }

        public LayerButton(AbLayer layer, Grid control, int count) : base() 
        {
            Content = $"{layer.GetType().Name}#{count}";
            Height = 50;
            Layer = layer;
            controlInfo = control;
        }
        protected override void OnClick()
        {
            StackPanel infoPanel = (StackPanel)controlInfo.Children[0];
            StackPanel paramsPanel = (StackPanel)controlInfo.Children[1];
            infoPanel.Children.Clear();
            paramsPanel.Children.Clear();
            foreach (var field in Layer.GetType().GetProperties())
            {
                //Header
                Label nameProp = new Label(); 
                nameProp.Content = field.Name;
                infoPanel.Children.Add(nameProp);
                //Value
                TextBox textValue = new TextBox();
                textValue.Text = field.GetValue(Layer).ToString();
                paramsPanel.Children.Add(textValue);
            }
        }
        private T OnCreateElem<T>(string text, T control) where T : ContentControl
        {
            control.FontFamily = new FontFamily("Comic Sans MS");
            control.FontSize = 18;
            control.Content = text;
            return control;
        }
    }
}
