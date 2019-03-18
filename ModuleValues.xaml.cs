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
using System.Windows.Forms.DataVisualization.Charting;

namespace GoodPlot
{
  /// <summary>
  /// Логика взаимодействия для ModuleValues.xaml
  /// </summary>
  public partial class ModuleValues : Window
  {
    Calculations MyCalc;
    Chart Mychart;
    /// <summary>
    /// Точка начала усреднения
    /// </summary>
    DateTime pStart;
    public ModuleValues(File_Acts FA, Chart chart)
    {
      InitializeComponent();
      //График, с которым работаем
      Mychart=chart;
      MyCalc=new Calculations(FA,chart);
      
      chart.CursorPositionChanged += chart_CursorPositionChanged;
      //Values_List.ItemsSource =   File_Acts_One.Find_Parametr(tempKKS).Time_and_Value_List;
    }

    void chart_CursorPositionChanged(object sender, CursorEventArgs e)
    {
    int Myint=0;
      try
      {
      Myint=Convert.ToInt32(IntervalSecTexBox.Text);

      }
      catch (Exception)
      {
       
      MessageBox.Show("Необходимо число");
      }
      

      MyCalc.ParametrsOnGraph_Values(Myint);
      
      Values_List.ItemsSource = MyCalc.TableList;
      
      
    }
    /// <summary>
    /// Усреднение по 2 точкам
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonStart_Click(object sender, RoutedEventArgs e)
    {
      ButtonStart.Background = Brushes.MediumSeaGreen;
      pStart= DateTime.FromOADate(Mychart.ChartAreas[0].CursorX.Position);
       
      
          
    }

    private void ButtonEnd_Click(object sender, RoutedEventArgs e)
    {
      ButtonEnd.Background = Brushes.MediumSeaGreen;
      int Myint = 0;
      try
      {
        Myint = Convert.ToInt32(IntervalSecTexBox.Text);

      }
      catch (Exception)
      {

        MessageBox.Show("Необходимо число");
      }
      MyCalc.ParametrsOnGraph_Values(Myint, pStart, DateTime.FromOADate(Mychart.ChartAreas[0].CursorX.Position));
      Values_List.ItemsSource = MyCalc.TableList;     
    }
  }
}
