﻿using LibraryCNN;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ConstructorCNN
{
    class ParamsTextBox : TextBox
    {
        AbLayer layerData;
        string dataName;
        public ParamsTextBox(object data, string name, AbLayer layer) : base() 
        {
            Margin = new Thickness(0, 5, 0, 0);
            dataName = name;
            layerData = layer;
            Text = data.ToString();
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
/*            if (!String.IsNullOrWhiteSpace(Text))
            {
                Text = Text.Replace(" ", "");
                Network.Epoth = Convert.ToInt32(Text);
                CaretIndex = Text.Length;
            }*/
            if (!String.IsNullOrEmpty(Text)) {
                foreach (var field in layerData.GetType().GetProperties())
                {
                    if(field.Name == dataName && field.CanWrite)
                    {
                        object newData = Convert.ChangeType(Text, field.GetValue(layerData).GetType());
                        field.SetValue(layerData, newData);
                        break;
                    }
                }
            }
        }
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            char number = Convert.ToChar(e.Text);
            if (Char.IsDigit(number))
            {
                Text += number;
                CaretIndex = Text.Length;
            }
        }
    }
}
