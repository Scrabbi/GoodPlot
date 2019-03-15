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
    public ModuleValues(File_Acts FA, Chart chart)
    {
      InitializeComponent();

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
  }
}
