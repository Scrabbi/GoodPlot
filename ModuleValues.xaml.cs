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
    ListView MylistV;
    File_Acts MyFileActs;
    /// <summary>
    /// Точка начала усреднения
    /// </summary>
    DateTime pStart;
    public ModuleValues(File_Acts FA, Chart chart, ListView listVgiven)
    {
      InitializeComponent();
      //График, с которым работаем и прочее, ссылаемся тут.
      Mychart=chart;
      MyCalc=new Calculations(FA,chart);
      MylistV=listVgiven;
      MyFileActs = FA;

      


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

    private void ButtonMulty_Click(object sender, RoutedEventArgs e)
    {
    //ПРоверка числа вводимого
    double rez=0;
      double.TryParse(TBNumber.Text,out rez);
      if (rez==0)
	      {
		        return;
	      }

      //Сюда умноженный параметр сохраним
      Parameter NewParametrSubstr = new Parameter();
      //Выделенный , по его значениям идем. Проверим, что выделен параметр.
      if (MylistV.SelectedItems.Count==0)
      {
        MessageBox.Show("Выделите параметр");
        return;
      }
      Parameter selectedParametr = (Parameter)MylistV.SelectedItem;
      
      //Пошли/ Получили параметр умноженный
      for (int i = 0; i < selectedParametr.Time_and_Value_List.Count; i++)
      {
        //Создадим точку данных по разности
        Time_and_Value TaV = new Time_and_Value();
        TaV.Time = selectedParametr.Time_and_Value_List[i].Time;
        TaV.Value = selectedParametr.Time_and_Value_List[i].Value * rez;
        //Добавим точечку
        NewParametrSubstr.Time_and_Value_List.Add(TaV);
      }
      NewParametrSubstr.KKS = "*" + TBNumber.Text + selectedParametr.KKS;
        //Описываем, что получили.
        NewParametrSubstr.Description = selectedParametr.KKS;


        MyFileActs.Parameters.Add(NewParametrSubstr);


      //Привязка
      //List_Parameters.ItemsSource = File_Acts_One.Parameters;
        MylistV.Items.Refresh();
    }
  }
}
