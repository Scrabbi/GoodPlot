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
using System.IO;

namespace GoodPlot
{
  /// <summary>
  /// Логика взаимодействия для ModuleDiffEff.xaml
  /// </summary>
  public partial class ModuleDiffEff : System.Windows.Window
  {
    DataGrid datagrid_ref;
    ListView listRezult_ref;
      ListView ListCalculation_KI_ref;
      ListView List_PolinomRezult_ref;
      

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
    public ModuleDiffEff(Chart chart, File_Acts FA, DataGrid datagrid, ListView List_Rezult, ListView ListCalculation_KI, ListView List_PolinomRezult)
    {
      InitializeComponent();
      Chart_ref=chart;
      fileacts_ref=FA;
      datagrid_ref = datagrid;
      listRezult_ref=List_Rezult;
      ListCalculation_KI_ref = ListCalculation_KI;
      List_PolinomRezult_ref = List_PolinomRezult;
     

      MyCalc = new Calculations(FA,Chart_ref);
    }

      /// <summary>
      /// Начальная точка для расчета.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
    private void ButtonTopStart_Click(object sender, RoutedEventArgs e)
    {
      ButtonTopStart.Background = Brushes.MediumSeaGreen;
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
        Time_and_Value TAV = MyCalc.FindClosePoint(selectedParam.Time_and_Value_List, DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position));

        //Заносим параметр. 
        ReactivityMassForDE[0] = TAV;
        return;
      }
      

      //=====================================Автомат
      else
      {
        //Заносим точку. Запомнили точку старта.
        Start = DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position);
          int i=10;
          i++;
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
        Time_and_Value TAV = MyCalc . FindClosePoint ( selectedParam . Time_and_Value_List , DateTime . FromOADate ( Chart_ref . ChartAreas [ 0 ] . CursorX . Position ) );

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
        Time_and_Value TAV = MyCalc . FindClosePoint ( selectedParam . Time_and_Value_List , DateTime . FromOADate ( Chart_ref . ChartAreas [ 0 ] . CursorX . Position ) );

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
        Time_and_Value TAV = MyCalc . FindClosePoint ( selectedParam . Time_and_Value_List , DateTime . FromOADate ( Chart_ref . ChartAreas [ 0 ] . CursorX . Position ) );

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
        Time_and_Value TAV = MyCalc . FindClosePoint ( selectedParam . Time_and_Value_List , DateTime . FromOADate ( Chart_ref . ChartAreas [ 0 ] . CursorX . Position ) );

        //Заносим параметр. 
        ReactivityMassForDE[3] = TAV;
        return;
      
      }

      //=================================Автомат======================================
      else
      {
        //Заносим точку. Запомнили точку конца.
        End = DateTime.FromOADate(Chart_ref.ChartAreas[0].CursorX.Position);
        int i = 10;
        i++;
      }
    }
    /// <summary>
    /// Расчет
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
    {
        //Вручную
        #region
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
          double d1 = MyCalc . FindClosePoint ( ChosenGroup . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value;
          double d2 = MyCalc . FindClosePoint ( ChosenGroup . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value;
      
      
          dH = Math.Abs(d1 - d2);
          dH *= Calculations.Step_h;
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
              MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "YVM10FG900" ) ) . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value , //H10
            MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "YVM10FG909" ) ) . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value , //H9
            MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "YVM10FG908" ) ) . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value , //H8
            0, //dH
            0, //dP
            0);//dP / dH

            dt.Rows.Add(1,//Номер
              "-", //H12
              "-",//H11 
              MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "YVM10FG900" ) ) . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value , //H10
            MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "YVM10FG909" ) ) . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value , //H9
            MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "YVM10FG908" ) ) . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value , //H8
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
                  MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG912" ) ) . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value . ToString ( "F" ) , //H12
                  MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG911" ) ) . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value . ToString ( "F" ) ,//H11 
                  MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG910" ) ) . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value . ToString ( "F" ) , //H10
                MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG909" ) ) . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value . ToString ( "F" ) , //H9
                MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG908" ) ) . Time_and_Value_List , ReactivityMassForDE [ 0 ] . Time ) . Value . ToString ( "F" ) , //H8
                0, //dH
                0, //dP
                0);//dP / dH

                dt.Rows.Add(1,//Номер
                  MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG912" ) ) . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value . ToString ( "F" ) , //H12
                  MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG911" ) ) . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value . ToString ( "F" ) ,//H11 
                  MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG910" ) ) . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value . ToString ( "F" ) , //H10
                MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG909" ) ) . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value . ToString ( "F" ) , //H9
                MyCalc . FindClosePoint ( fileacts_ref . Parameters . Find ( n => n . KKS . Contains ( "JDA00FG908" ) ) . Time_and_Value_List , ReactivityMassForDE [ 2 ] . Time ) . Value . ToString ( "F" ) ,//H8
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
        #endregion
        //===========================================АВТОМАТ==================-----------------------
        if (RadAuto.IsChecked==true)
        {
            //Массивы значений вычислить
            #region
            //Найдем М1. Набор перемещений группы (стоять-опуститься, стоять-опуститься, ...)
        List<DateTime> groupStepsTimes = new List<DateTime>();
        groupStepsTimes = MyCalc.GiveGroupSteps(Start, End);

            //Группа иногда перемещается шажками очень малыми. Отсеем пары значений такие, где мало времени между перемещениями.
        List<DateTime> improoveGroupStepsTimes_m1 = new List<DateTime> ( );
        //improoveGroupStepsTimes_m1 = MyCalc . GiveM1_improove ( groupStepsTimes );
        improoveGroupStepsTimes_m1=groupStepsTimes;

            //М2. В начало и конец дабавляем точку старта и конца.
        List<DateTime> M2 = new List<DateTime>();
        M2 . AddRange ( improoveGroupStepsTimes_m1 );
        M2.Insert(0,Start);
        M2.Add(End);

            //Отступить 33% от положения, когда группа опустилась только.
        List<DateTime> M3 = new List<DateTime>();
        M3 = MyCalc . GiveIndentList_M3 ( M2 );

            //Получить коэффициенты прямых для аппроксимаций реактивности
        List<Tuple<double, double>> m4_KoeffForLinearApprox = new List<Tuple<double,double>>();
        m4_KoeffForLinearApprox = MyCalc.Give_ab_Koeffs_M4(M3);

            //Получить приращения/ Тут и DelPo и dH.
        List<Tuple<double, double>> delPo_dH_M5 = new List<Tuple<double, double>>();
        delPo_dH_M5 = MyCalc . GiveDelPo_M5 ( m4_KoeffForLinearApprox , improoveGroupStepsTimes_m1 );

            //Получить массив (x,y) , по которому рисовать будем сглаживающие прямые
        M6 = new List<Tuple<double, double>>();
        M6 = MyCalc.GiveXYLine_M6(M2, m4_KoeffForLinearApprox);
        
        #endregion
        
        //Заполним табличку 
        DataTable dt = new DataTable();
        RezultTable rezultTable= new RezultTable();

            //Табличка
        #region
        //Табличка ;Класс есть
        dt.Columns.Add("Номер"); dt.Columns.Add("Н12, %"); dt.Columns.Add("Н11, %"); dt.Columns.Add("Н10, %"); dt.Columns.Add("Н9, %"); dt.Columns.Add("Н8, %"); dt.Columns.Add("dН, см"); dt.Columns.Add("dρ, %"); dt.Columns.Add("dρ:dН, %:см"); dt.Columns.Add("Δρ, %");
        
                //Первая строчка
        dt.Rows.Add(0,//Номер
          MyCalc . FindClosePoint ( MyCalc . H12 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value , //H12
          MyCalc . FindClosePoint ( MyCalc . H11 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value ,//H11 
          MyCalc . FindClosePoint ( MyCalc . H10 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value , //H10
          MyCalc . FindClosePoint ( MyCalc . H9 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value , //H9
          MyCalc . FindClosePoint ( MyCalc . H8 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value , //H8
        0, //dH
        0, //dP
        0,//dP / dH
        0);
        double PO=0;

        //Остальные
        for (int i = 0; i < delPo_dH_M5.Count; i++)
        {
            dt.Rows.Add(i + 1,//Номер
            MyCalc.FindClosePoint(MyCalc.H12.Time_and_Value_List, improoveGroupStepsTimes_m1[i * 2 + 1]).Value, //H12
            MyCalc.FindClosePoint(MyCalc.H11.Time_and_Value_List, improoveGroupStepsTimes_m1[i * 2 + 1]).Value,//H11 
            MyCalc.FindClosePoint(MyCalc.H10.Time_and_Value_List, improoveGroupStepsTimes_m1[i * 2 + 1]).Value, //H10
            MyCalc.FindClosePoint(MyCalc.H9.Time_and_Value_List, improoveGroupStepsTimes_m1[i * 2 + 1]).Value, //H9
            MyCalc.FindClosePoint(MyCalc.H8.Time_and_Value_List, improoveGroupStepsTimes_m1[i * 2 + 1]).Value, //H8
            delPo_dH_M5[i].Item2,//dH
            delPo_dH_M5[i].Item1, //dP
            delPo_dH_M5[i].Item1 / (delPo_dH_M5[i].Item2),//dP / dH
            PO += delPo_dH_M5[i].Item1);//Суммарная реактивность
        }
        #endregion

            //В классе
        #region
        rezultTable.Rows.Add(new Row
                {
                Number=0,
                Indication = 1000 - (100 - MyCalc.FindClosePoint(MyCalc.H12.Time_and_Value_List, improoveGroupStepsTimes_m1[0]).Value),
                H12=MyCalc . FindClosePoint ( MyCalc . H12 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value,
                H11=MyCalc . FindClosePoint ( MyCalc . H11 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value ,
                H10=MyCalc . FindClosePoint ( MyCalc . H10 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value ,
                H9=MyCalc . FindClosePoint ( MyCalc . H9 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value ,
                H8=MyCalc . FindClosePoint ( MyCalc . H8 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ 0 ] ) . Value ,
                Dh=0,
                Dρ=0,
                Dρ_Dh=0,
                Δρ=0
                }  );
                double P1 = 0;
                double indication1000 = 1000 - (100 - MyCalc.FindClosePoint(MyCalc.H12.Time_and_Value_List, improoveGroupStepsTimes_m1[0]).Value);
        //Остальные
        for ( int i = 0 ; i < delPo_dH_M5 . Count ; i++ )
        {
            P1+=delPo_dH_M5[i].Item1;
            indication1000 -= delPo_dH_M5[i].Item2 / Calculations.Step_h;

            rezultTable.Rows.Add(new Row
                {
                Number= i+1 ,
                Indication = indication1000,
                H12 =  MyCalc . FindClosePoint ( MyCalc . H12 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ i * 2 + 1 ] ) . Value , 
                H11=   MyCalc . FindClosePoint ( MyCalc . H11 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ i * 2 + 1 ] ) . Value , 
                H10=  MyCalc . FindClosePoint ( MyCalc . H10 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ i * 2 + 1 ] ) . Value ,
                 H9=   MyCalc . FindClosePoint ( MyCalc . H9 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ i * 2 + 1 ] ) . Value , 
                 H8=   MyCalc . FindClosePoint ( MyCalc . H8 . Time_and_Value_List , improoveGroupStepsTimes_m1 [ i * 2 + 1 ] ) . Value , 
                 Dh=  delPo_dH_M5 [ i ] . Item2 ,
                 Dρ=   delPo_dH_M5 [ i ] . Item1 , 
                 Dρ_Dh=  delPo_dH_M5 [ i ] . Item1 / ( delPo_dH_M5 [ i ] . Item2 ) ,
                 Δρ= P1
            });
        }
        #endregion

        datagrid_ref.ItemsSource = dt.DefaultView;
        listRezult_ref . ItemsSource=rezultTable.Rows;

                    //Заполнение расчета КИ
        #region

        //Определяем тип эксперимента
        string filepath = "";

        if (RezultTable.TypeExpeariment(rezultTable) == "AllDown")
        {
            //filepath= System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SRC_KI_data\\All_down.txt");
            filepath = "SRC_KI_data\\All_down.txt";
        }

        else
        {
            if (RezultTable.TypeExpeariment(rezultTable) == "H12")
            {
                filepath = "SRC_KI_data\\H12.txt";
            }
            if (RezultTable.TypeExpeariment(rezultTable) == "H11")
            {
                filepath = "SRC_KI_data\\H11.txt";
            }
            if (RezultTable.TypeExpeariment(rezultTable) == "H10")
            {
                filepath = "SRC_KI_data\\H10.txt";
            }
            if (RezultTable.TypeExpeariment(rezultTable) == "H9")
            {
                filepath = "SRC_KI_data\\H9.txt";
            }
        }
        #endregion
        RezultTable calculationNICKITable = RezultTable.GetTable(filepath);
        ListCalculation_KI_ref.ItemsSource = calculationNICKITable.Rows;

        //Результирующая таблица (а ней 2 вписанные таблицы для сравнений)
        #region
                    //Всегда на погружение сориентируем результаты эксперимента
            RezultTable rezultTable_reverse = RezultTable.DirectToDownTable(rezultTable);



            List<double> stepsEperiment = new List<double>();
            List<double> stepsCalculate = new List<double>();

                    //Значения расчета одинаково добываются
            List<double> valuesCalculate_Po = new List<double>();
            List<double> valuesCalculate_dPdH = new List<double>();
            valuesCalculate_Po = RezultTable.ThisToList(calculationNICKITable, "Po");
            valuesCalculate_dPdH = RezultTable.ThisToList(calculationNICKITable, "dPdH");

            var Spline_dPdH = Interpolate.CubicSpline(new List<double> { 1, 2 }, new List<double> { 1, 2 });
            var Spline_Po = Interpolate.CubicSpline(new List<double> { 1, 2 }, new List<double> { 1, 2 });

            //Добавачная интегральная эффективность
            ////double PoAdditional=0;

            if (filepath.Contains("All"))
            {
                //stepsEperiment = RezultTable.ReturnGroupIndication(rezultTable_reverse);
                stepsEperiment = RezultTable.ReturnGroupIndication(rezultTable);
                stepsCalculate = RezultTable.ReturnGroupIndication(calculationNICKITable);

                Spline_dPdH = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_dPdH);
                Spline_Po = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_Po);

               // PoAdditional = Math.Abs(Spline_Po.Interpolate(stepsEperiment[0])); 
            }
            if (filepath.Contains("H12"))
            {
                stepsEperiment = RezultTable.ThisToList(rezultTable, "H12");
                stepsCalculate =  RezultTable.ThisToList(calculationNICKITable, "H12");

                
                Spline_dPdH = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_dPdH);
                Spline_Po = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_Po);

                
            }
            if (filepath.Contains("H11"))
            {
                stepsEperiment = RezultTable.ThisToList(rezultTable, "H11");
                stepsCalculate = RezultTable.ThisToList(calculationNICKITable, "H11");

                
                Spline_dPdH = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_dPdH);
                Spline_Po = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_Po);
            }
            if (filepath.Contains("H10"))
            {
                stepsEperiment = RezultTable.ThisToList(rezultTable, "H10");
                stepsCalculate = RezultTable.ThisToList(calculationNICKITable, "H10");

                Spline_dPdH = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_dPdH);
                Spline_Po = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_Po);
            }
            if (filepath.Contains("H9"))
            {
                stepsEperiment = RezultTable.ThisToList(rezultTable_reverse, "H9");
                stepsCalculate = RezultTable.ThisToList(calculationNICKITable, "H9");

                
                Spline_dPdH = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_dPdH);
                Spline_Po = Interpolate.CubicSpline(stepsCalculate, valuesCalculate_Po);
            }

            //if (stepsEperiment[0]<stepsEperiment[stepsEperiment.Count-1])
            //    PoAdditional = Math.Abs(Spline_Po.Interpolate(stepsEperiment[0]));    
            //else
            //    PoAdditional = Math.Abs(Spline_Po.Interpolate(stepsEperiment[stepsEperiment.Count-1])); 


                    //Финальная табличка
            //for (int i = 0; i < rezultTable_reverse.Rows.Count; i++)
            //{
            //    rezultTable_reverse.Rows[i].Dρ_Dh_Calculate = Spline_Po.Interpolate(stepsEperiment[0]);
                
            //}
            
            //if (PoAdditional!=0)
            //    {
            //        for (int i = 0; i < rezultTable_reverse.Rows.Count; i++)
            //        {
            //            row = rezultTable_reverse.Rows[i];
            //            row.Dρ_Dh_Calculate = Spline_dPdH.Interpolate(stepsEperiment[i]);
            //            row.Δρ_Calculate = Spline_Po.Interpolate(stepsEperiment[i]);
            //            row.Δρ = (rezultTable_reverse.Rows[i].Δρ + PoAdditional);
            //            //Отклонения
            //            row.Dρ_Dh_Deviation = 100 * (row.Dρ_Dh - row.Dρ_Dh_Calculate) / row.Dρ_Dh;
            //            row.Δρ_Deviation = 100 * Math.Abs(row.Δρ - row.Δρ_Calculate) / (row.Δρ);

            //            poliniomTable.Rows.Add(row);
            //        }
            //    }
            //else
            //    {
            RezultTable poliniomTable = new RezultTable();
            Row row = new Row();
            double Δρ_Calculate = 0;

                    for (int i = 1; i < rezultTable.Rows.Count; i++)
                    {
                       Δρ_Calculate += Math.Abs( Spline_Po.Interpolate(stepsEperiment[i ]) - Spline_Po.Interpolate(stepsEperiment[i-1])  );
                       // Δρ_Calculate = Math . Abs ( Spline_Po . Interpolate ( stepsEperiment [ i ] )  );

                        row = rezultTable.Rows[i];
                        row.Dρ_Dh_Calculate = Spline_dPdH.Interpolate(stepsEperiment[i]);

                        row.Δρ_Calculate = Δρ_Calculate;
                        row.Δρ = rezultTable.Rows[i].Δρ;
                        //Отклонения
                        row.Dρ_Dh_Deviation = 100 * (row.Dρ_Dh - row.Dρ_Dh_Calculate) / row.Dρ_Dh;
                        row.Δρ_Deviation = 100 * Math.Abs(row.Δρ - row.Δρ_Calculate) / (row.Δρ);

                        poliniomTable.Rows.Add(row);
                    }
            //}


            #endregion
            List_PolinomRezult_ref.ItemsSource = poliniomTable.Rows;
            


        }//что автомат
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
