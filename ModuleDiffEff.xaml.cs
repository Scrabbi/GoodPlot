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
    double dPdH;

    

    //Начало - конец участка для анализа.
    DateTime Start; DateTime End;

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

    Calculations MyCalc; 
    public ModuleDiffEff(Chart chart, File_Acts FA, DataGrid datagrid)
    {
      InitializeComponent();
      Chart_ref=chart;
      fileacts_ref=FA;
      datagrid_ref = datagrid;
      MyCalc = new Calculations(FA);
    }


    private void ButtonTopStart_Click(object sender, RoutedEventArgs e)
    {
      ButtonTopStart.Background = Brushes.MediumSeaGreen;
      //ПРоверка что курсор нормальный, имеет значение. Если курсора вообще нет на графике.
      if (double.IsNaN(Chart_ref.ChartAreas[0].CursorX.Position))
      {
        MessageBox.Show("Перенаведите курсор!");
        return;
      }

      if (RadHand.IsChecked == true)
      {
      ////////////Вручную///////////////////////////////////////////////////////////////////////
      if (RadHand.IsChecked==true)
      {
        //Заносим точку. Запомнили точку старта.
        Parameter selectedParam =(Parameter) fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactivitiTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        ReactivityMassForDE[0] = TAV;
        return;
      }
      }
      //Автомат
      else
      {
        //Заносим точку. Запомнили точку старта.
        Start = DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position);
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
      if (RadHand.IsChecked==true)
      {
       
        //ПРоверка что курсор нормальный, имеет значение. Если курсора вообще нет на графике.
        if (double.IsNaN(Chart_ref.ChartAreas[0].CursorX.Position))
        {
          MessageBox.Show("Перенаведите курсор!");
          return;
        }
      
        //Заносим точку. Запомнили точку конца на верху.
        Parameter selectedParam = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactivitiTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        ReactivityMassForDE[3] = TAV;
        return;
      
      }
      else
      {
        //Заносим точку. Запомнили точку конца.
        End = DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position);
      }
    }
    /// <summary>
    /// Расчет
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
    {
      if (RadHand.IsChecked == true)
      {
          //Верх
    A0 = Fit.Line(new double[] { ReactivityMassForDE[0].Time.ToOADate(), ReactivityMassForDE[1].Time.ToOADate() }, new double[] { ReactivityMassForDE[0].Value, ReactivityMassForDE[1].Value}).Item1;
    B0 = Fit.Line(new double[] { ReactivityMassForDE[0].Time.ToOADate(), ReactivityMassForDE[1].Time.ToOADate() }, new double[] { ReactivityMassForDE[0].Value, ReactivityMassForDE[1].Value }).Item2;
    //Низ        
    A1 = Fit.Line(new double[] { ReactivityMassForDE[2].Time.ToOADate(), ReactivityMassForDE[3].Time.ToOADate() }, new double[] { ReactivityMassForDE[2].Value, ReactivityMassForDE[3].Value }).Item1;
    B1 = Fit.Line(new double[] { ReactivityMassForDE[2].Time.ToOADate(), ReactivityMassForDE[3].Time.ToOADate() }, new double[] { ReactivityMassForDE[2].Value, ReactivityMassForDE[3].Value }).Item2;

      //Вычисление приращений
      dP = Math.Abs(A0 + B0 * GroupPosition.Time.ToOADate() - (A1 + B1 * GroupPosition.Time.ToOADate()));
      dP *= MyCalc.B_eff;

      Parameter ChosenGroup = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupTexBox.Text));
      double d1 = MyCalc.FindPoint(ChosenGroup.Time_and_Value_List, ReactivityMassForDE[0].Time).Value;
      double d2 = MyCalc.FindPoint(ChosenGroup.Time_and_Value_List, ReactivityMassForDE[2].Time).Value;
      
      
      dH = d1 - d2;
      dH *= MyCalc.Step_h;
      dPdH = dP/dH;

      //Заполним табличку 
      DataTable dt = new DataTable();
      dt.Columns.Add("Номер"); dt.Columns.Add("Н12, %"); dt.Columns.Add("Н11, %"); dt.Columns.Add("Н10, %"); dt.Columns.Add("Н9, %"); dt.Columns.Add("Н8, %"); dt.Columns.Add("dН, см"); dt.Columns.Add("dρ, %"); dt.Columns.Add("dρ:dН, %:см"); 

      
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
        dPdH);
        datagrid_ref.ItemsSource = dt.DefaultView;
      }

      //Файл НВАЭС-2
      if (ChosenGroup.KKS.Contains("JDA"))
      {
        dt.Rows.Add(0,//Номер
          MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG912")).Time_and_Value_List, ReactivityMassForDE[0].Time).Value.ToString("F"), //H12
          MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG911")).Time_and_Value_List, ReactivityMassForDE[0].Time).Value.ToString("F"),//H11 
          MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG910")).Time_and_Value_List, ReactivityMassForDE[0].Time).Value.ToString("F"), //H10
        MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG909")).Time_and_Value_List, ReactivityMassForDE[0].Time).Value.ToString("F"), //H9
        MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG908")).Time_and_Value_List, ReactivityMassForDE[0].Time).Value.ToString("F"), //H8
        0, //dH
        0, //dP
        0);//dP / dH

        dt.Rows.Add(1,//Номер
          MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG912")).Time_and_Value_List, ReactivityMassForDE[2].Time).Value.ToString("F"), //H12
          MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG911")).Time_and_Value_List, ReactivityMassForDE[2].Time).Value.ToString("F"),//H11 
          MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG910")).Time_and_Value_List, ReactivityMassForDE[2].Time).Value.ToString("F"), //H10
        MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG909")).Time_and_Value_List, ReactivityMassForDE[2].Time).Value.ToString("F"), //H9
        MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("JDA00FG908")).Time_and_Value_List, ReactivityMassForDE[2].Time).Value.ToString("F"),//H8
        dH.ToString("F"), //dH
        dP.ToString("G2"), //dP
        dPdH.ToString("G2"));
        datagrid_ref.ItemsSource = dt.DefaultView;
      }
      else MessageBox.Show("В обработку пока не дабавлен такой файл");
      }


      if (RadAuto.IsChecked==true)
      {
        //Найдем М1. Набор перемещений группы (стоять-опуститься, стоять-опуститься, ...)
        List<DateTime> M1 = new List<DateTime>();
        M1 = MyCalc.GiveM1(Start, End);
        //М2. В начало и конец дабавляем точку старта и конца.
        List<DateTime> M2 = new List<DateTime>();
        M2.AddRange(M1);
        M2.Insert(0,Start);
        M2.Add(End);
        //Отступить 33% от положения, когда группа опустилась только.
        List<DateTime> M3 = new List<DateTime>();
        M3 = MyCalc.GiveIndentList_M3(M2);
        //Получить коэффициенты прямых для аппроксимаций реактивности
        List<Tuple<double, double>> M4 = new List<Tuple<double,double>>();
        M4 = MyCalc.Give_ab_Koeffs_M4(M3);

        
        List<double> M5 = new List<double>();
        M5 = MyCalc.GiveDelPo_M5(M4,M1);

      }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      Chart_ref.Focus();
    }

    private void RadAuto_Checked(object sender, RoutedEventArgs e)
    {
        ButtonTopEnd.IsEnabled=false; ButtonBotStart.IsEnabled=false;ButtonGroupPoint.IsEnabled=false;
    }

    
    
  }
}
