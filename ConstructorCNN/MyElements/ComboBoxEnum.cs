using LibraryCNN;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ConstructorCNN
{
    class ComboBoxEnum : ComboBox
    {
        AbLayer layerData;
        string dataName;
        public ComboBoxEnum(object data, string name, AbLayer layer) : base() 
        {
            Margin = new Thickness(0, 5, 0, 0);
            dataName = name;
            layerData = layer;
            if(data.GetType() == typeof(bool))
            {
                Items.Add(true);
                Items.Add(false);
                SelectedValue = data;
            }
            else
            {
                foreach (var item in Enum.GetNames(data.GetType()))
                {
                    Items.Add(item);
                }
                SelectedItem = data.ToString();
            }
            //layerData.ParamsChanged += LayerData_ParamsChanged;
            SelectionChanged += ComboBox_SelectionChanged;
        }

        private void LayerData_ParamsChanged(object sender, EventChangedParams e)
        {
            throw new NotImplementedException();
        }

        private T dosomething<T>(object o)
        {
            T enumVal = (T)Enum.Parse(typeof(T), o.ToString());
            return enumVal;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var field in layerData.GetType().GetProperties())
            {
                if (field.Name == dataName && field.CanWrite)
                {
                    if (field.GetValue(layerData).GetType() == typeof(bool))
                    {
                        field.SetValue(layerData, Convert.ToBoolean(SelectedValue));
                    }
                    else
                    {
                        Enum.TryParse(field.GetValue(layerData).GetType(), SelectedValue.ToString(), out object newData);
                        field.SetValue(layerData, newData);
                    }
                    break;
                }
            }
        }
/*        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var field in layerData.GetType().GetProperties())
            {
                if (field.Name == dataName && field.CanWrite)
                {
                    if (field.GetValue(layerData).GetType() == typeof(bool))
                    {
                        field.SetValue(layerData, Convert.ToBoolean(SelectedValue));
                    }
                    else
                    {
                        Enum.TryParse(field.GetValue(layerData).GetType(), SelectedValue.ToString(), out object newData);
                        field.SetValue(layerData, newData);
                    }
                    break;
                }
            }
        }*/
    }
}
