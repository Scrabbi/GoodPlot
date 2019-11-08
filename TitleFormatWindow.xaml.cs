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
using System.Windows.Shapes;
using System.Windows.Forms.DataVisualization.Charting;


namespace GoodPlot
{
  /// <summary>
  /// Логика взаимодействия для TitleFormatWindow.xaml
  /// </summary>
  public partial class TitleFormatWindow : Window
  {
  /// <summary>
  /// Ссылка на переданный заголовок
  /// </summary>
    Title TitleGiven;
    public TitleFormatWindow(Title TitleFrom)
    {
      InitializeComponent();
      //Передаваяемый заголовок
      TitleGiven=TitleFrom;

      //Текущий текст ЗАГОЛОВКА
      Title_Text.Text = TitleGiven.Text;
      //Обработчик изменения текста
      Title_Text.TextChanged += Title_Text_TextChanged;

      //Текущая ОРИЕТАЦИЯ
      OrientationBox.SelectedIndex = 0;
      //Изменение
      OrientationBox.SelectionChanged += OrientationBox_SelectionChanged;
      
      //Теккущий ЦВЕТ
      ColorBox.SelectedColor = System.Windows.Media.Color.FromArgb(TitleGiven.BackColor.A, TitleGiven.BackColor.R, TitleGiven.BackColor.G, TitleGiven.BackColor.B);
      //Обработчик изменения цвета
      ColorBox.SelectedColorChanged += ColorBox_SelectedColorChanged;

      //Текущий ШРИФТ
      FontSizeBox.Text=TitleGiven.Font.Size.ToString();
      //Обработкчик
      FontSizeBox.TextChanged += FontSizeBox_TextChanged;
    }

    void FontSizeBox_TextChanged(object sender, TextChangedEventArgs e)
    {
    double d=0;
    double.TryParse(FontSizeBox.Text,out d);
    if (d>0)
    {
      TitleGiven.Font = new System.Drawing.Font("Times New Roman", (float)Convert.ToDouble(FontSizeBox.Text), System.Drawing.FontStyle.Regular); 
    }
      
    }

    void ColorBox_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
    {
      TitleGiven.BackColor = System.Drawing.Color.FromArgb(ColorBox.SelectedColor.Value.A, ColorBox.SelectedColor.Value.R, ColorBox.SelectedColor.Value.G, ColorBox.SelectedColor.Value.B);
    }

    void OrientationBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    if (OrientationBox.SelectedItem.ToString().Contains("Вертикально"))
	    TitleGiven.TextOrientation = TextOrientation.Rotated90;
    if (OrientationBox.SelectedItem.ToString().Contains("Вертикально 180"))
      TitleGiven.TextOrientation = TextOrientation.Rotated270;
    if (OrientationBox.SelectedItem.ToString().Contains("Горизонтально"))
      TitleGiven.TextOrientation = TextOrientation.Horizontal;
    }

    //Изменение названия текста
    void Title_Text_TextChanged(object sender, TextChangedEventArgs e)
    {
      TitleGiven.Text = Title_Text.Text;
    }

    private void ComboBoxStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
  }
}
