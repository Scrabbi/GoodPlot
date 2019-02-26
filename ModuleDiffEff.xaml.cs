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
using MathNet.Numerics;
using System.Data;

namespace GoodPlot
{
  /// <summary>
  /// Логика взаимодействия для ModuleDiffEff.xaml
  /// </summary>
  public partial class ModuleDiffEff : System.Windows.Window
  {
    DataGrid datagrid_ref;
    //КОэффициенты прямых сверху и снизу. y=A+B*x
    double A0=0;
    double A1 = 0;
    double B0 = 0;
    double B1 = 0;

    //Изменение реактивности и группы на анализируемом участке
    double dP=0;
    double dH = 0;

    /// <summary>
    /// Массив для записи 4 пар время-значение (4 точки из списка точек параметра).
    /// </summary>
    Time_and_Value[] ReactivityMassForDE = new Time_and_Value[4];
    /// <summary>
    /// Положение группы в момент середины движения.
    /// </summary>
    Time_and_Value GroupPosition;
  /// <summary>
  /// Передаваемый chart
  /// </summary>
    Chart Chart_ref;
    /// <summary>
    /// Доступ к списку параметров
    /// </summary>
    File_Acts fileacts_ref;
    Calculations MyCalc = new Calculations();
    public ModuleDiffEff(Chart chart, File_Acts FA, DataGrid datagrid)
    {
      InitializeComponent();
      Chart_ref=chart;
      fileacts_ref=FA;
      datagrid_ref = datagrid;
    }


    private void ButtonTopStart_Click(object sender, RoutedEventArgs e)
    {
      //ПРоверка что курсор нормальный, имеет значение. Если курсора вообще нет на графике.
      if (double.IsNaN(Chart_ref.ChartAreas[0].CursorX.Position))
      {
        MessageBox.Show("Перенаведите курсор!");
        return;
      }
      ////////////Вручную///////////////////////////////////////////////////////////////////////
      if (RadHand.IsChecked==true)
      {
        //Заносим точку. Запомнили точку старта.
        Parameter selectedParam =(Parameter) fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactivitiTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List,DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        ReactivityMassForDE[0] = TAV;
        ButtonTopStart.Background = Brushes.MediumSeaGreen;
        return;
      }



      
    }

    
    private void ButtonGroupPoint_Click(object sender, RoutedEventArgs e)
    {
      ButtonGroupPoint.Background = Brushes.MediumSeaGreen;
      //ПРоверка что курсор нормальный, имеет значение. Если курсора вообще нет на графике.
      if (double.IsNaN(Chart_ref.ChartAreas[0].CursorX.Position))
      {
        MessageBox.Show("Перенаведите курсор!");
        return;
      }
      ////////////Вручную///////////////////////////////////////////////////////////////////////
      if (RadHand.IsChecked == true)
      {
        //Заносим точку. Запомнили точку конца на верху.
        Parameter selectedParam = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactivitiTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        GroupPosition = TAV;
        return;
      }

    }

    private void ButtonTopEnd_Click(object sender, RoutedEventArgs e)
    {
      ButtonTopEnd.Background = Brushes.MediumSeaGreen;
      //ПРоверка что курсор нормальный, имеет значение. Если курсора вообще нет на графике.
      if (double.IsNaN(Chart_ref.ChartAreas[0].CursorX.Position))
      {
        MessageBox.Show("Перенаведите курсор!");
        return;
      }
      ////////////Вручную///////////////////////////////////////////////////////////////////////
      if (RadHand.IsChecked == true)
      {
        //Заносим точку. Запомнили точку конца на верху.
        Parameter selectedParam = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactivitiTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        ReactivityMassForDE[1] = TAV;
        return;
      }



      
    }
    

    private void ButtonBotStart_Click(object sender, RoutedEventArgs e)
    {
      ButtonBotStart.Background = Brushes.MediumSeaGreen;
      //ПРоверка что курсор нормальный, имеет значение. Если курсора вообще нет на графике.
      if (double.IsNaN(Chart_ref.ChartAreas[0].CursorX.Position))
      {
        MessageBox.Show("Перенаведите курсор!");
        return;
      }
      ////////////Вручную///////////////////////////////////////////////////////////////////////
      if (RadHand.IsChecked == true)
      {
        //Заносим точку. Запомнили точку конца на верху.
        Parameter selectedParam = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactivitiTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        ReactivityMassForDE[2] = TAV;
        return;
      }
    }

    private void ButtonBotEnd_Click(object sender, RoutedEventArgs e)
    {
      ButtonBotEnd.Background = Brushes.MediumSeaGreen;
      //ПРоверка что курсор нормальный, имеет значение. Если курсора вообще нет на графике.
      if (double.IsNaN(Chart_ref.ChartAreas[0].CursorX.Position))
      {
        MessageBox.Show("Перенаведите курсор!");
        return;
      }
      ////////////Вручную///////////////////////////////////////////////////////////////////////
      if (RadHand.IsChecked == true)
      {
        //Заносим точку. Запомнили точку конца на верху.
        Parameter selectedParam = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactivitiTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        ReactivityMassForDE[3] = TAV;
        return;
      }
    }
    /// <summary>
    /// Расчет
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
    {
    //Верх
    A0 = Fit.Line(new double[] { ReactivityMassForDE[0].Time.ToOADate(), ReactivityMassForDE[1].Time.ToOADate() }, new double[] { ReactivityMassForDE[0].Value, ReactivityMassForDE[1].Value}).Item1;
    B0 = Fit.Line(new double[] { ReactivityMassForDE[0].Time.ToOADate(), ReactivityMassForDE[1].Time.ToOADate() }, new double[] { ReactivityMassForDE[0].Value, ReactivityMassForDE[1].Value }).Item2;
    //Низ        
    A1 = Fit.Line(new double[] { ReactivityMassForDE[2].Time.ToOADate(), ReactivityMassForDE[3].Time.ToOADate() }, new double[] { ReactivityMassForDE[2].Value, ReactivityMassForDE[3].Value }).Item1;
    B1 = Fit.Line(new double[] { ReactivityMassForDE[2].Time.ToOADate(), ReactivityMassForDE[3].Time.ToOADate() }, new double[] { ReactivityMassForDE[2].Value, ReactivityMassForDE[3].Value }).Item2;

      //Вычисление приращений
      dP =Math.Abs(A0 + B0 * GroupPosition.Time.ToOADate() - (A1 + B1 * GroupPosition.Time.ToOADate()));
      Parameter ChosenGroup = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupTexBox.Text));
      double d1 = ChosenGroup.Time_and_Value_List.Find(n => n.Time.Subtract(ReactivityMassForDE[0].Time).Seconds <= 1 && n.Time.Subtract(ReactivityMassForDE[0].Time).Minutes <= 1 && n.Time.Subtract(ReactivityMassForDE[0].Time).Hours <= 1).Value;
      double d2 = ChosenGroup.Time_and_Value_List.Find(n => n.Time.Subtract(ReactivityMassForDE[2].Time).Seconds <= 1 && n.Time.Subtract(ReactivityMassForDE[2].Time).Minutes <= 1 && n.Time.Subtract(ReactivityMassForDE[2].Time).Hours <= 1).Value;
      dH = d1 - d2;

      //Заполним табличку 
      DataTable dt = new DataTable();
      dt.Columns.Add("Номер"); dt.Columns.Add("H12"); dt.Columns.Add("H11"); dt.Columns.Add("H10"); dt.Columns.Add("H9"); dt.Columns.Add("H8");
      dt.Columns.Add("dН"); dt.Columns.Add("dρ"); dt.Columns.Add("dρ/dН"); 

      
      //Файл с Бушера
      if (ChosenGroup.KKS.Contains("YVM"))
      {
        dt.Rows.Add(0,//Номер
          "-", //H12
          "-",//H11 
          fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG900")).Time_and_Value_List.Find(n => n.Time.Subtract(ReactivityMassForDE[0].Time).Seconds <= 1 && n.Time.Subtract(ReactivityMassForDE[0].Time).Minutes<= 1 && n.Time.Subtract(ReactivityMassForDE[0].Time).Hours <= 1).Value, //H10
        fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG909")).Time_and_Value_List.Find(n => n.Time.Subtract(ReactivityMassForDE[0].Time).Seconds <= 1&& n.Time.Subtract(ReactivityMassForDE[0].Time).Minutes <= 1 && n.Time.Subtract(ReactivityMassForDE[0].Time).Hours <= 1).Value, //H9
        fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG908")).Time_and_Value_List.Find(n => n.Time.Subtract(ReactivityMassForDE[0].Time).Seconds <= 1&& n.Time.Subtract(ReactivityMassForDE[0].Time).Minutes <= 1&& n.Time.Subtract(ReactivityMassForDE[0].Time).Hours <= 1).Value, //H8
        0, //dH
        0, //dP
        0);//dP / dH
        dt.Rows.Add(1,//Номер
          "-", //H12
          "-",//H11 
          fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG900")).Time_and_Value_List.Find(n => n.Time.Subtract(ReactivityMassForDE[2].Time).Seconds <= 1 && n.Time.Subtract(ReactivityMassForDE[2].Time).Minutes <= 1 && n.Time.Subtract(ReactivityMassForDE[2].Time).Hours <= 1).Value, //H10
        fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG909")).Time_and_Value_List.Find(n => n.Time.Subtract(ReactivityMassForDE[2].Time).Seconds <= 1 && n.Time.Subtract(ReactivityMassForDE[2].Time).Minutes <= 1 && n.Time.Subtract(ReactivityMassForDE[2].Time).Hours <= 1).Value, //H9
        fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG908")).Time_and_Value_List.Find(n => n.Time.Subtract(ReactivityMassForDE[2].Time).Seconds <= 1 && n.Time.Subtract(ReactivityMassForDE[2].Time).Minutes <= 1 && n.Time.Subtract(ReactivityMassForDE[2].Time).Hours <= 1).Value, //H8
        dH, //dH
        dP, //dP
        dP / dH);
        datagrid_ref.ItemsSource = dt.DefaultView;
      }


    }

    
    
  }
}
