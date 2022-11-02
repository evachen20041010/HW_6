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
    //public：公開的
    //partial：可在命名空間中定義類別、結構或介面的其他組件。能在需要將類別分散到不同的檔案時，可以讓很多人同時處理這個專案。
    //class MainWindow : Window -> 宣告類別 MainWindow : Window
    {
        List<Drink> drinks = new List<Drink>();     //新增一個由Drink類別定義的List清單 drinks
        List<OrderItem> order = new List<OrderItem>();     //新增一個由OrederItem類別定義的List清單 order
        string takeout;     //內用or外帶
        public MainWindow()
        //公開的類別成員 MainWindow
        {
            InitializeComponent();  //做初始化動作(初始化FROM、控制項、載入與分配資源等)

            //新增飲料品項至drinks清單內
            AddNewDrink(drinks);

            //顯示所有飲料品項
            DisplayDrinks(drinks);
        }

        private void DisplayDrinks(List<Drink> drinks)
        //私人的方法 DisplayDrinks，無回傳值，使用清單drinks為參數
        {
            foreach (Drink d in drinks)    //給陣列用的迴圈，取清單driks的值，d會從Drink的第一項開始加一項
            {
                StackPanel sp = new StackPanel(); //StackPanel：將子元素以水平或垂直方式排列   初始化 StackPanel 類別的新變數sp
                CheckBox cb = new CheckBox(); //CheckBox：有true/false的選項按鈕   初始化 CheckBox 類別的新變數cb
                //TextBox tb = new TextBox();
                Slider sl = new Slider(); //Slider：滑桿控制     初始化 Slider 類別的新變數sl
                Label lb = new Label(); //Label：視窗中顯示的文本    初始化 Label 類別的新變數lb

                sp.Orientation = Orientation.Horizontal;    //Orientation屬性設定為Horizontal(水平對齊)

                cb.Content = d.Name + d.Size + d.Price; ;     //CheckBox內容設為drinks裡的Name+Size+Price
                cb.Margin = new Thickness(5);   //設定CheckBox之間的空間，四邊套用統一長度
                cb.Width = 150;
                cb.Height = 25;

                sl.Value = 0; ;   //設定Slider的滑桿值
                sl.Width = 100;
                sl.Minimum = 0;     //設定Slider的滑桿最小值
                sl.Maximum = 20;    //設定Slider的滑桿最大值
                sl.TickPlacement = TickPlacement.TopLeft;   //設定刻度標記顯示在水平Slider的Track上方，尖端朝上
                sl.TickFrequency = 1;   //設定刻度間值差為1
                sl.IsSnapToTickEnabled = true; ;      //設定Slider會自動將Thumb(指標)移動到最近的刻度

                lb.Width = 50;

                //tb.Width = 80;
                //tb.Height = 25;
                //tb.TextAlignment = TextAlignment.Right;

                Binding myBinding = new Binding("Value");   //Binding：元素綁定，並可從源頭提取些訊息   此處提取Value訊息
                myBinding.Source = sl;
                lb.SetBinding(ContentProperty, myBinding);

                sp.Children.Add(cb);    //在sp的子區塊新增cb
                //sp.Children.Add(tb);
                sp.Children.Add(sl);    //在sp的子區塊新增sl
                sp.Children.Add(lb);    //在sp的子區塊新增lb

                stackpanel_DrinkMenu.Children.Add(sp);      //將子區塊的元素包裝起來並新增在sp的主區塊
            }
        }

        private void AddNewDrink(List<Drink> mydrink)
        //私人的方法 AddNewDrink，無回傳值，使用類別Drink的mydrink清單為使用的參數
        {
            /*mydrink.Add(new Drink() { Name = "咖啡", Size = "大杯", Price = 60 });
            mydrink.Add(new Drink() { Name = "咖啡", Size = "小杯", Price = 50 });
            mydrink.Add(new Drink() { Name = "紅茶", Size = "大杯", Price = 30 });
            mydrink.Add(new Drink() { Name = "紅茶", Size = "小杯", Price = 20 });
            mydrink.Add(new Drink() { Name = "綠茶", Size = "大杯", Price = 30 });
            mydrink.Add(new Drink() { Name = "綠茶", Size = "小杯", Price = 20 });*/

            var dialog = new Microsoft.Win32.OpenFileDialog();  //代表通用對話方塊，可讓使用者為要開啟的一個或多個檔案指定檔名
            dialog.Filter = "文字檔案|*.txt|CSV檔案|*.csv|所有檔案|*.*";  //設定篩選條件字串。可決定在開檔或讀檔時選擇的檔案類型
            dialog.DefaultExt = "*.csv";    //設定預設篩選條件字串

            if (dialog.ShowDialog() == true)    //對話框開啟時
            {
                string path = dialog.FileName;      //設path為檔案名稱

                //string content = File.ReadAllText(path);
                StreamReader sr = new StreamReader(path, Encoding.Default);     //StreamReader讀檔時，會使用指定的字元編碼方式進行讀取
                CsvReader csv = new CsvReader(sr, CultureInfo.InvariantCulture);    //CultureInfo：類別，提供有關特定文化特性的資訊
                                                                                    //取得與文化特性無關的物件
                csv.Read();     //讀取csv檔
                csv.ReadHeader();   //讀取csv檔的標頭
                while (csv.Read() == true)  //csv有讀到東西時
                {
                    //將讀取到的物件歸類並新增到清單mydrink
                    Drink d = new Drink() { Name = csv.GetField("Name"), Size = csv.GetField("Size"), Price = csv.GetField<int>("Price") };
                    mydrink.Add(d);
                }
            }
            MessageBox.Show($"偵測到{mydrink.Count}筆飲品資料");    //MessageBox顯示讀取到的筆數
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        //私人的方法 RadioButton_Checked，無回傳值，sender和e是他使用的參數
        //object sender：指的是引發該事件的來源對象是誰
        //RoutedEventArgs e：是在該事件中可以細部去使用到的細節資料
        {
            RadioButton rb = sender as RadioButton;     //設rb存取引發事件的RadioButton
            if (rb.IsChecked == true)   //如果RadioButton有被勾選
            {
                takeout = rb.Content.ToString(); ;    //takeout存取RadioButton的內容轉string
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
                total += item.SubTotal;     //變數加上清單myorder裡的SubTotal
                Drink drinkItem = drinks[item.Index];   //drinkItem存取清單drinks的指定飲品及其資訊
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
            dialog.Multiselect = false;     //不允許選取多個檔案
            if (dialog.ShowDialog() == true)
            {
                fullPath = dialog.FileName;
            }
            FileStream FileStream = new FileStream(fullPath, FileMode.Append);  //FileMode.Create 覆蓋寫入  FileMode.Append 接續寫入
            StreamWriter sw = new StreamWriter(FileStream, System.Text.Encoding.UTF8);
            sw.WriteLine(DateTime.Now);     //寫入現在時間
            sw.WriteLine(displayTextBlock.Text);    //寫入明細
            sw.WriteLine("-----------------------------------------------");
            sw.Flush();     //清除這個資料流的緩衝區，讓所有緩衝資料全部寫入檔案
            sw.Close();     //關檔
            FileStream.Close();
            displayTextBlock.Text += "\n資料已寫入write.csv\n";
        }

        private void PlaceOrder(List<OrderItem> myorder)
        {
            myorder.Clear();    //清除myorder內容
            for (int i = 0; i < stackpanel_DrinkMenu.Children.Count; i++)
            //檢查所有飲品
            {
                StackPanel sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                CheckBox cb = sp.Children[0] as CheckBox;
                Slider sl = sp.Children[1] as Slider;
                int quantity = Convert.ToInt32(sl.Value);

                if (cb.IsChecked == true && quantity != 0)
                //如果CheckBox有被勾選且滑桿數值大於0
                {
                    //添加資訊到清單myorder
                    int price = drinks[i].Price;
                    int subtotal = price * quantity;
                    myorder.Add(new OrderItem() { Index = i, Quantity = quantity, SubTotal = subtotal });
                }
            }
        }
    }
}