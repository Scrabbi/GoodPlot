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

    //Получить массив (x,y) , по которому рисовать будем сглаживающие прямые
    List<Tuple<double, double>> M6 = new List<Tuple<double, double>>();

    //Начало - конец участка для анализа.
    DateTime Start; DateTime End;
    //
     Chart_Acts chartActs=new Chart_Acts();

    /// <summary>
    /// Массив для записи 4 пар время-значение (4 точки из списка точек параметра). ПРи ручном способе.
    /// </summary>
    Time_and_Value[] ReactivityMassForDE = new Time_and_Value[4];
    /// <summary>
    /// Положение группы в момент середины движения.
    /// </summary>
    Time_and_Value GroupPosition;
  /// <summary>
  /// Передаваемый chart
  /// </summary>
   static Chart Chart_ref;
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
      MyCalc = new Calculations(FA,Chart_ref);
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

      //=====================================Автомат
      else
      {
        //Заносим точку. Запомнили точку старта.
        Start = DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position);
      }


      
    }

    /// <summary>
    /// Середина движения группы.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        //Заносим точку. 
        Parameter selectedParam = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        GroupPosition = TAV;
        return;
      }

    }

    /// <summary>
    /// Точку верх -- конец
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
    
    /// <summary>
    /// Низ--старт
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonBotStart_Click(object sender, RoutedEventArgs e)
    {
    //TimeSpan ts= new TimeSpan(2,3,15);
    //int t= ts.Hours;
    //int tg = ts.Minutes;
    //int tgggggggg = ts.Seconds;

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
    /// <summary>
    /// Низ--конец
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
      
        //==================Вручную============================Заносим точку. Запомнили точку конца на верху.
        Parameter selectedParam = (Parameter)fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactivitiTexBox.Text));
        Time_and_Value TAV = MyCalc.FindPoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        ReactivityMassForDE[3] = TAV;
        return;
      
      }

      //=================================Автомат======================================
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
      
      
      dH = Math.Abs(d1 - d2);
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
          MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG900")).Time_and_Value_List,ReactivityMassForDE[0].Time).Value, //H10
        MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG909")).Time_and_Value_List, ReactivityMassForDE[0].Time).Value, //H9
        MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG908")).Time_and_Value_List, ReactivityMassForDE[0].Time).Value, //H8
        0, //dH
        0, //dP
        0);//dP / dH

        dt.Rows.Add(1,//Номер
          "-", //H12
          "-",//H11 
          MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG900")).Time_and_Value_List, ReactivityMassForDE[2].Time).Value, //H10
        MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG909")).Time_and_Value_List, ReactivityMassForDE[2].Time).Value, //H9
        MyCalc.FindPoint(fileacts_ref.Parameters.Find(n => n.KKS.Contains("YVM10FG908")).Time_and_Value_List, ReactivityMassForDE[2].Time).Value, //H8
        dH, //dH
        dP, //dP
        dPdH);
        datagrid_ref.ItemsSource = dt.DefaultView;
        return;
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
        return;
      }
      if (!ChosenGroup.KKS.Contains("JDA") || !ChosenGroup.KKS.Contains("YVM"))
          {
            MessageBox.Show("В обработку пока не дабавлен такой файл");
            return;
          } 
      }

      //===========================================АВТОМАТ==================-----------------------==================================
      if (RadAuto.IsChecked==true)
      {
        //Найдем М1. Набор перемещений группы (стоять-опуститься, стоять-опуститься, ...)
        List<DateTime> M1 = new List<DateTime>();
        M1 = MyCalc.GiveM1(Start, End);
        //Группа иногда перемещается шажками без непрерывности. Отсеем пары значений такие, где мало времени между перемещениями.
        List<DateTime> M1_improove = new List<DateTime>();
        M1_improove = MyCalc.GiveM1_improove(M1);

        //М2. В начало и конец дабавляем точку старта и конца.
        List<DateTime> M2 = new List<DateTime>();
        M2.AddRange(M1_improove);
        M2.Insert(0,Start);
        M2.Add(End);
        //Отступить 33% от положения, когда группа опустилась только.
        List<DateTime> M3 = new List<DateTime>();
        M3 = MyCalc.GiveIndentList_M3(M2);
        //Получить коэффициенты прямых для аппроксимаций реактивности
        List<Tuple<double, double>> M4 = new List<Tuple<double,double>>();
        M4 = MyCalc.Give_ab_Koeffs_M4(M3);
        //Получить приращения
        List<Tuple<double, double>> M5 = new List<Tuple<double, double>>();
        M5 = MyCalc.GiveDelPo_M5(M4, M1_improove);
        //Получить массив (x,y) , по которому рисовать будем сглаживающие прямые
        M6 = new List<Tuple<double, double>>();
        M6 = MyCalc.GiveXYLine_M6(M2, M4);
        

        

        //Заполним табличку 
        DataTable dt = new DataTable();
        dt.Columns.Add("Номер"); dt.Columns.Add("Н12, %"); dt.Columns.Add("Н11, %"); dt.Columns.Add("Н10, %"); dt.Columns.Add("Н9, %"); dt.Columns.Add("Н8, %"); dt.Columns.Add("dН, см"); dt.Columns.Add("dρ, %"); dt.Columns.Add("dρ:dН, %:см"); dt.Columns.Add("Δρ, %");
        //Первая строчка
        dt.Rows.Add(0,//Номер
          MyCalc.FindPoint(MyCalc.H12.Time_and_Value_List, M1_improove[0]).Value, //H12
          MyCalc.FindPoint(MyCalc.H11.Time_and_Value_List, M1_improove[0]).Value,//H11 
          MyCalc.FindPoint(MyCalc.H10.Time_and_Value_List, M1_improove[0]).Value, //H10
          MyCalc.FindPoint(MyCalc.H9.Time_and_Value_List, M1_improove[0]).Value, //H9
          MyCalc.FindPoint(MyCalc.H8.Time_and_Value_List, M1_improove[0]).Value, //H8
        0, //dH
        0, //dP
        0,//dP / dH
        0);
        double PO=0;
        //Остальные
        for (int i = 0; i < M5.Count; i++)
        {
        
          dt.Rows.Add(i+1,//Номер
          MyCalc.FindPoint(MyCalc.H12.Time_and_Value_List, M1_improove[i * 2 + 1]).Value, //H12
          MyCalc.FindPoint(MyCalc.H11.Time_and_Value_List, M1_improove[i * 2 + 1]).Value,//H11 
          MyCalc.FindPoint(MyCalc.H10.Time_and_Value_List, M1_improove[i * 2 + 1]).Value, //H10
          MyCalc.FindPoint(MyCalc.H9.Time_and_Value_List, M1_improove[i * 2 + 1]).Value, //H9
          MyCalc.FindPoint(MyCalc.H8.Time_and_Value_List, M1_improove[i * 2 + 1]).Value, //H8
          M5[i].Item2,//dH
          M5[i].Item1, //dP
          M5[i].Item1 / ( M5[i].Item2),//dP / dH
          PO+=M5[i].Item1);//Суммарная реактивность
        }
          datagrid_ref.ItemsSource = dt.DefaultView;
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
    /// <summary>
    /// Строим график. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (M6==null)
      {
        MessageBox.Show("Не было расчета");
        return;
      }
      //Построить кривую изменений реактивности
      Chart_ref.Series.Add("InterLine");
      Chart_ref.Series["InterLine"].ChartType = SeriesChartType.Line;
      Chart_ref.Series["InterLine"].ChartArea = "Area# 1";
      foreach (var item in M6)
      {
        Chart_ref.Series["InterLine"].Points.AddXY(item.Item1, item.Item2);
      }
    }
    
    
    
  }
}
