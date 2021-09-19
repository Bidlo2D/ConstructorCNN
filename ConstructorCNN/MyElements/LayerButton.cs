using LibraryCNN;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Markup;
using System.Windows;

namespace ConstructorCNN
{
    class LayerButton : Button
    {
        AbLayer Layer { get; set; }
        Panel controlInfo { get; set; }
        StackPanel Add { get; set; }
        bool Delete { get; set; }

        public LayerButton(AbLayer layer, Grid control, StackPanel add, int count, bool delete) : base() 
        {
            Content = $"{layer.GetType().Name}#{count}";
            Height = 50;
            Layer = layer;
            Delete = delete;
            Add = add;
            controlInfo = control;
            //Delete button
            if (Delete)
            {
                ContextMenu = new ContextMenu();
                MenuItem mi = new MenuItem();
                mi.Header = "Delete";
                mi.Click += Delete_Click;
                ContextMenu.Items.Add(mi);
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Network.Remove(Layer);
            Add.Children.Remove(this);
        }
        protected override void OnClick()
        {
            StackPanel infoPanel = (StackPanel)controlInfo.Children[0];
            StackPanel paramsPanel = (StackPanel)controlInfo.Children[1];
            infoPanel.Children.Clear();
            paramsPanel.Children.Clear();
            //Header layer
            Label name = new Label();
            name.Content = "Name layer:";
            infoPanel.Children.Add(name);
            //Name layer
            Label nameValue = new Label();
            nameValue.Content = Content;
            paramsPanel.Children.Add(nameValue);
            foreach (var field in Layer.GetType().GetProperties())
            {
                if (!field.CanRead) { continue; }
                //Header
                Label nameProp = new Label(); 
                nameProp.Content = field.Name;
                infoPanel.Children.Add(nameProp);
                //Value
                if (field.GetValue(Layer).GetType().IsEnum 
                    || field.GetValue(Layer).GetType() == typeof(bool))
                {
                    ComboBoxEnum comboValue = new ComboBoxEnum(field.GetValue(Layer), field.Name, Layer);
                    comboValue.IsEnabled = field.CanWrite ? true : false;
                    paramsPanel.Children.Add(comboValue);
                }
                else
                {
                    ParamsTextBox textValue = new ParamsTextBox(field.GetValue(Layer), field.Name, Layer);
                    textValue.IsEnabled = field.CanWrite ? true : false;
                    paramsPanel.Children.Add(textValue);
                }
            }
        }
    }
}
