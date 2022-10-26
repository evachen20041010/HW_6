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
using System.Windows.Controls.Primitives;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Formats.Asn1;
using System.Windows.Markup;
using Microsoft.Win32;

namespace HW_6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Drink> drinks = new List<Drink>();
        List<OrderItem> order = new List<OrderItem>();
        string takeout;
        public MainWindow()
        {
            InitializeComponent();

            //新增飲料品項至drinks清單內
            AddNewDrink(drinks);

            //顯示所有飲料品項
            DisplayDrinks(drinks);
        }

        private void DisplayDrinks(List<Drink> drinks)
        {
            foreach (Drink d in drinks)
            {
                StackPanel sp = new StackPanel();
                CheckBox cb = new CheckBox();
                //TextBox tb = new TextBox();
                Slider sl = new Slider();
                Label lb = new Label();

                sp.Orientation = Orientation.Horizontal;

                cb.Content = d.Name + d.Size + d.Price;
                cb.Margin = new Thickness(5);
                cb.Width = 150;
                cb.Height = 25;

                sl.Value = 0;
                sl.Width = 100;
                sl.Minimum = 0;
                sl.Maximum = 20;
                sl.TickPlacement = TickPlacement.TopLeft;
                sl.TickFrequency = 1;
                sl.IsSnapToTickEnabled = true;

                lb.Width = 50;

                //tb.Width = 80;
                //tb.Height = 25;
                //tb.TextAlignment = TextAlignment.Right;

                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb.SetBinding(ContentProperty, myBinding);

                sp.Children.Add(cb);
                //sp.Children.Add(tb);
                sp.Children.Add(sl);
                sp.Children.Add(lb);

                stackpanel_DrinkMenu.Children.Add(sp);
            }
        }

        private void AddNewDrink(List<Drink> mydrink)
        {
            /*mydrink.Add(new Drink() { Name = "咖啡", Size = "大杯", Price = 60 });
            mydrink.Add(new Drink() { Name = "咖啡", Size = "小杯", Price = 50 });
            mydrink.Add(new Drink() { Name = "紅茶", Size = "大杯", Price = 30 });
            mydrink.Add(new Drink() { Name = "紅茶", Size = "小杯", Price = 20 });
            mydrink.Add(new Drink() { Name = "綠茶", Size = "大杯", Price = 30 });
            mydrink.Add(new Drink() { Name = "綠茶", Size = "小杯", Price = 20 });*/

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "文字檔案|*.txt|CSV檔案|*.csv|所有檔案|*.*";
            dialog.DefaultExt = "*.csv";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                //string content = File.ReadAllText(path);
                StreamReader sr = new StreamReader(path, Encoding.Default);
                CsvReader csv = new CsvReader(sr, CultureInfo.InvariantCulture);

                csv.Read();
                csv.ReadHeader();
                while (csv.Read() == true)
                {
                    Drink d = new Drink() { Name = csv.GetField("Name"), Size = csv.GetField("Size"), Price = csv.GetField<int>("Price") };
                    mydrink.Add(d);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.IsChecked == true)
            {
                takeout = rb.Content.ToString();
            }
        }

        private void orderButton_Click(object sender, RoutedEventArgs e)
        {
            displayTextBlock.Text = "";
            PlaceOrder(order);
            DisplayOrderDetail(order);
        }

        private void DisplayOrderDetail(List<OrderItem> myorder)
        {
            int total = 0;
            displayTextBlock.Text = $"您的用餐方式為{takeout}，訂購明細如下：\n";
            int i = 1, sum = 0, end_sum = 0;

            foreach (OrderItem item in myorder)
            {
                total += item.SubTotal;
                Drink drinkItem = drinks[item.Index];
                displayTextBlock.Text += $"訂購品項{i}：{drinkItem.Name}{drinkItem.Size}，單價{drinkItem.Price}元 X {item.Quantity}，小計{item.SubTotal}元。\n";
                i++;
                sum += item.SubTotal;
            }
            displayTextBlock.Text += $"\n總價{sum}元";
            if (sum >= 500)
            {
                end_sum = (int)(sum * 0.8);
                displayTextBlock.Text += $"，訂單滿500以上打8折，折扣後總價為{end_sum}元\n";
                displayTextBlock.Background = Brushes.MediumSlateBlue;  //背景改紫藍色
                displayTextBlock.Foreground = Brushes.White;    //字體改白色
                displayTextBlock.FontStyle = FontStyles.Italic;     //斜體字
            }
            else if (sum >= 300)
            {
                end_sum = (int)(sum * 0.85);
                displayTextBlock.Text += $"，訂單滿300以上打85折，折扣後總價為{end_sum}元\n";
                displayTextBlock.Background = Brushes.PaleVioletRed; //背景改灰紫紅色
                displayTextBlock.Foreground = Brushes.White;    //字體改白色
                displayTextBlock.FontWeight = FontWeights.UltraBold;    //粗體字
            }
            else if (sum >= 200)
            {
                end_sum = (int)(sum * 0.9);
                displayTextBlock.Text += $"，訂單滿200以上打9折，折扣後總價為{end_sum}元\n";
                displayTextBlock.Background = Brushes.Teal; //背景改青綠色
                displayTextBlock.Foreground = Brushes.White;    //字體改白色
                displayTextBlock.FontWeight = FontWeights.UltraLight;   //細體字
            }
            else
            {
                displayTextBlock.Background = Brushes.IndianRed; //背景改印度紅色
                displayTextBlock.Foreground = Brushes.White;    //字體改白色
                displayTextBlock.FontWeight = FontWeights.Normal;   //不改體字
            }

            string fullPath = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == true)
            {
                fullPath = dialog.FileName;
            }
            FileStream FileStream = new FileStream(fullPath, FileMode.Append);  //FileMode.Create 覆蓋寫入  FileMode.Append 接續寫入
            StreamWriter sw = new StreamWriter(FileStream, System.Text.Encoding.UTF8);
            sw.WriteLine(DateTime.Now);
            sw.WriteLine(displayTextBlock.Text);
            sw.WriteLine("-----------------------------------------------");
            sw.Flush();
            sw.Close();
            FileStream.Close();
            displayTextBlock.Text += "\n資料已寫入write.csv\n";

        }

        private void PlaceOrder(List<OrderItem> myorder)
        {
            myorder.Clear();
            for (int i = 0; i < stackpanel_DrinkMenu.Children.Count; i++)
            {
                StackPanel sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                CheckBox cb = sp.Children[0] as CheckBox;
                Slider sl = sp.Children[1] as Slider;
                int quantity = Convert.ToInt32(sl.Value);

                if (cb.IsChecked == true && quantity != 0)
                {
                    int price = drinks[i].Price;
                    int subtotal = price * quantity;
                    myorder.Add(new OrderItem() { Index = i, Quantity = quantity, SubTotal = subtotal });
                }
            }
        }
    }
}