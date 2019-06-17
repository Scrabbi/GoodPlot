using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GoodPlot
{
  /// <summary>
  /// Логика взаимодействия для ChartFormatWindow.xaml
  /// </summary>
  public partial class ChartFormatWindow : Window
  {
    /// <summary>
    /// Передаваемое окно с графиком
    /// </summary>
    System.Windows.Forms.Integration.WindowsFormsHost ChartWindow_fer;


    public ChartFormatWindow(System.Windows.Forms.Integration.WindowsFormsHost ChartWindow)
    {
      this.ChartWindow_fer = ChartWindow;
      InitializeComponent();
      WidthTextBox.Text = ChartWindow.ActualWidth.ToString();
      HeightTextBox.Text = ChartWindow.ActualHeight.ToString();
    }


        // //////////////------------------------Изменение размеров вручную
    private void WidthTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        ChartWindow_fer.Width = Convert.ToDouble(WidthTextBox.Text);
      }
      catch (Exception)
      {}
    }

    private void HeightFormatTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        ChartWindow_fer.Height = Convert.ToDouble(HeightTextBox.Text);
      }
      catch (Exception)
      { }
    }

   

                //----------------Заготовки фарматов бамаги.---------------------------
    //Книжная ориентация
    private void ButBook_Click(object sender, RoutedEventArgs e)
    {
      ChartWindow_fer.Width = ChartWindow_fer.ActualHeight/1.41;
      WidthTextBox.Text = ChartWindow_fer.Width.ToString();
    }
    //Альбомная
    private void ButAlbum_Click(object sender, RoutedEventArgs e)
    {
      //Сразу не влезает
      ChartWindow_fer.Width = ChartWindow_fer.ActualWidth / 1.5;
      WidthTextBox.Text = ChartWindow_fer.Width.ToString();
      //Ужимаем
      ChartWindow_fer.Height = ChartWindow_fer.Width / 1.45;
      HeightTextBox.Text = ChartWindow_fer.Height.ToString();
    }
    //Растянуть
    private void ButFull_Click(object sender, RoutedEventArgs e)
    {
      //BotTextBox.Text = "0";
      //TopTextBox.Text = "0";
      //LeftTextBox.Text = "40";
      //RightTextBox.Text = "0";
      //ChartWindow_fer.Margin = new System.Windows.Thickness(0, 0,0, 0);
      
      ChartWindow_fer.Width = double.NaN;
      ChartWindow_fer.Height = double.NaN;
      ChartWindow_fer.UpdateLayout();

      refreshfields();
      
    
    }
    /// <summary>
    /// Обновляет поля значений ширины и времени
    /// </summary>
    void refreshfields()
    {
      WidthTextBox.Text = ChartWindow_fer.ActualWidth.ToString();
      HeightTextBox.Text = ChartWindow_fer.ActualHeight.ToString();
    }

  }
}
