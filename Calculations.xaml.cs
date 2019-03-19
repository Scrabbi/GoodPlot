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
using MathNet.Numerics;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.ObjectModel;

namespace GoodPlot
{
  /// <summary>
  /// Логика взаимодействия для Calculations.xaml
  /// </summary>
  public partial class Calculations : System.Windows.Window
  {
    public  double Step_h = 3.52;
    public double B_eff = 0.74;
    public static string ReactKKSMainPart = "247.";
    public static string GroupKKSMainPart = "YVM";
    public static string GroupKKSMainPart12 = "-----";
    public static string GroupKKSMainPart11 = "-----";
    public static string GroupKKSMainPart10 = "YVM10FG900";
    public static string GroupKKSMainPart9 = "YVM10FG909";
    public static string GroupKKSMainPart8 = "YVM10FG908";

    //Выделим параметры для групп ОР СУЗ и реактивности.
    public Parameter H12 = null;
    public Parameter H11 = null;
    public Parameter H10 = null;
    public Parameter H9 = null;
    public Parameter H8 = null;
    public Parameter Reactivity = null;
    /// <summary>
    /// Доступ к списку параметров
    /// </summary>
    static File_Acts Fileacts_ref;

    static Chart chart_ref;

    public ObservableCollection<Line_for_Table> TableList = new ObservableCollection<Line_for_Table>();
    public Calculations(File_Acts FA, Chart chart)
    {
      InitializeComponent();
      Fileacts_ref=FA;
      chart_ref=chart;
      
      //Считать состояние
      try
      {
        StreamReader FileRead = new StreamReader("Options.txt");
        //Считать 1 линию из файла.
        GroupKKSMainPart = FileRead.ReadLine();
        //Считать 2 линию из файла.
        ReactKKSMainPart = FileRead.ReadLine();
        //Считать 3 линию из файла.
        Step_h = Convert.ToDouble(FileRead.ReadLine());
        //Считать 4 линию из файла.
        B_eff = Convert.ToDouble(FileRead.ReadLine());
        FileRead.Close();
      }
      catch (Exception)
      {}
      GroupTexBox.Text=GroupKKSMainPart;
      ReactivitiTexBox.Text=ReactKKSMainPart;
      ShagTexBox.Text=Step_h.ToString();
      BetaTexBox.Text=B_eff.ToString();
        
      //Бушер
      if (GroupKKSMainPart=="YVM")
      {
        GroupKKSMainPart12 = "--------";
        GroupKKSMainPart11 = "------";
        GroupKKSMainPart10 = "YVM10FG900";
        GroupKKSMainPart9 = "YVM10FG909";
        GroupKKSMainPart8 = "YVM10FG908";
      }
      //НВАЭС-2
      if (GroupKKSMainPart == "JDA")
      {
        GroupKKSMainPart12 = "JDA00FG912";
        GroupKKSMainPart11 = "JDA00FG911";
        GroupKKSMainPart10 = "JDA00FG910";
        GroupKKSMainPart9 = "JDA00FG909";
        GroupKKSMainPart8 = "JDA00FG908";
      }
    }
    /// <summary>
    /// Найти из списка точек точку с заданным временем из списка значений параметра
    /// </summary>
    /// <param name="TadList">Список точек</param>
    /// <param name="Dt">Заданное время</param>
    /// <returns></returns>
    public Time_and_Value FindPoint(List<Time_and_Value> TadList, DateTime Dt)
    {
      double FrecvencyRegistration = TadList[1].Time.Subtract(TadList[0].Time).TotalSeconds;
      foreach (var item in TadList)
      {
        if (item.Time.Hour == Dt.Hour && item.Time.Minute == Dt.Minute && Math.Abs(item.Time.Second - Dt.Second) <= FrecvencyRegistration/2)
          return item;
      }
      MessageBox.Show("Не найдено совпадение времени с курсором");
      return new Time_and_Value();
    }
    /// <summary>
    /// Тоже найти точку, но уже из списка точек серии
    /// </summary>
    /// <param name="PointList"></param>
    /// <param name="Dt"></param>
    /// <returns></returns>
    public static DataPoint FindPoint(DataPointCollection PointList, DateTime Dt)
    {
      double FrecvencyRegistration = DateTime.FromOADate(PointList[1].XValue).Subtract(DateTime.FromOADate(PointList[0].XValue)).TotalSeconds;
      foreach (var item in PointList)
      {
        if (DateTime.FromOADate(item.XValue).Hour == Dt.Hour && DateTime.FromOADate(item.XValue).Minute == Dt.Minute && Math.Abs(DateTime.FromOADate(item.XValue).Second - Dt.Second) <= FrecvencyRegistration/2)
          return item;
      }
      MessageBox.Show("Не найдено совпадение времени с курсором");
      return new DataPoint();
    }
    /// <summary>
    /// Среднее вправо-лево от указанного количества времени.
    /// </summary>
    /// <param name="PointList"></param>
    /// <param name="Dat"></param>
    /// <param name="SecCount_for_Aver"></param>
    /// <returns></returns>
    double FindAverage(DataPointCollection PointList, DateTime Dat, int SecCount_for_Aver)
    {
      double aver_Start = 0;
      double aver_End = 0;
      double aver=0;
      int pointcount=0;
      TimeSpan ts= new TimeSpan(0,0,SecCount_for_Aver);
      aver_Start = FindPoint(PointList, Dat.Subtract(ts)).XValue;
      aver_End = FindPoint(PointList, Dat.Add(ts)).XValue;
      foreach (var item in PointList)
      {
        if (item.XValue>aver_Start&&item.XValue<aver_End)
        {
        pointcount++;
          aver+=item.YValues[0];
        }
      }
      aver=aver/pointcount;



      return aver;
    }
    /// <summary>
    /// Перегрузка для поиска среднего по 2 точкам
    /// </summary>
    /// <param name="PointList"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    double FindAverage(DataPointCollection PointList, DateTime start, DateTime end)
    {
      double aver_Start = 0;
      double aver_End = 0;
      double aver=0;
      int pointcount=0;

      aver_Start = FindPoint(PointList, start).XValue;
      aver_End = FindPoint(PointList, end).XValue;
      foreach (var item in PointList)
      {
        if (item.XValue>aver_Start&&item.XValue<aver_End)
        {
        pointcount++;
          aver+=item.YValues[0];
        }
      }
      aver=aver/pointcount;



      return aver;
    }
    /// <summary>
    /// Найти индекс элемента в списке значений времён с указанным временем
    /// </summary>
    /// <param name="TadList"></param>
    /// <param name="Dt"></param>
    /// <returns></returns>
    public int FindIndex(List<Time_and_Value> TadList, DateTime Dt)
    {
    double FrecvencyRegistration=TadList[1].Time.Subtract(TadList[0].Time).TotalSeconds;
    
      for (int i = 0; i < TadList.Count; i++)
      {
        if (TadList[i].Time.Hour == Dt.Hour && TadList[i].Time.Minute == Dt.Minute && Math.Abs(TadList[i].Time.Second - Dt.Second) < FrecvencyRegistration)
        return i;
      }
     

     return 0;
    }
    public struct Line_for_Table
    {
      /// <summary>
      /// Название линии
      /// </summary>
      public string KKS { get; set; }
      /// <summary>
      /// Значение точки.
      /// </summary>
      public double Value { get; set; }
      /// <summary>
      /// Среднее за указанное количество точек
      /// </summary>
      public double AverageValue { get; set; }
      /// <summary>
      /// Среднее между 2 точками
      /// </summary>
      public double AverageValueBTW { get; set; }
    }
    /// <summary>
    /// Список легенд с графика. Значения в месте курсора.
    /// </summary>
    /// <returns></returns>
    public void ParametrsOnGraph_Values(int secforaver)
    {
      ObservableCollection<Line_for_Table> Rezult = new ObservableCollection<Line_for_Table>();
      Line_for_Table line = new Line_for_Table(); 
      
      foreach (var item in chart_ref.Series)
      {
        try
        {
          line.KKS = item.Name;
          line.Value = FindPoint(item.Points, DateTime.FromOADate(chart_ref.ChartAreas[item.ChartArea].CursorX.Position)).YValues[0];
          line.AverageValue=FindAverage(item.Points,DateTime.FromOADate(chart_ref.ChartAreas[item.ChartArea].CursorX.Position),secforaver);
          Rezult.Add(line);
        }
        catch (Exception)
        {}
        
      }



      TableList=Rezult;
     // return Rezult;
    }

    public void ParametrsOnGraph_Values(int secforaver, DateTime pstart, DateTime pend)
    {
      ObservableCollection<Line_for_Table> Rezult = new ObservableCollection<Line_for_Table>();
      Line_for_Table line = new Line_for_Table();

      foreach (var item in chart_ref.Series)
      {
        try
        {
          line.KKS = item.Name;
          line.Value = FindPoint(item.Points, DateTime.FromOADate(chart_ref.ChartAreas[item.ChartArea].CursorX.Position)).YValues[0];
          line.AverageValue = FindAverage(item.Points, DateTime.FromOADate(chart_ref.ChartAreas[item.ChartArea].CursorX.Position), secforaver);
          line.AverageValueBTW=FindAverage(item.Points,pstart,pend);
          Rezult.Add(line);
        }
        catch (Exception)
        { }

      }



      TableList = Rezult;
      // return Rezult;
    }
    
    
    
    
    /// <summary>
    /// Получить времена перемещения группы.
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public List<DateTime> GiveM1(DateTime Start, DateTime End)
    {
    List<DateTime> M1=new List<DateTime>();

    try
    {
      H12 = Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart12));
      H11 = Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart11));
      H10 = Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart10));
      H9 = Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart9));
      H8 = Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart8));
      Reactivity = Fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactKKSMainPart));
    }
    catch (Exception)
    { throw; }
    if (H10==null)
    {
      MessageBox.Show("Наверное, забыли изменить характерные KKS");
      return null;
    }
   int start = FindIndex(H10.Time_and_Value_List,Start);
   int end = FindIndex(H10.Time_and_Value_List, End);
    if (H12==null)
    {
    //Чтобы алгоритм работал при любых файлах
    H11=H10;
    H12=H11;
    }

    int i = start;
      while ( i < end)
      {
        while (Math.Abs( H12.Time_and_Value_List[i].Value - H12.Time_and_Value_List[i+3].Value) < 0.05
        && Math.Abs(H11.Time_and_Value_List[i].Value - H11.Time_and_Value_List[i + 3].Value) < 0.05
        &&Math.Abs(H10.Time_and_Value_List[i].Value - H10.Time_and_Value_List[i + 3].Value) < 0.05 
        &&Math.Abs(H9.Time_and_Value_List[i].Value - H9.Time_and_Value_List[i + 3].Value) < 0.05 
        &&Math.Abs(H8.Time_and_Value_List[i].Value - H8.Time_and_Value_List[i + 3].Value) < 0.05
        && i < end)
        {
        i++;
        }
        if (i < end)//переход к следующему движению группы
        M1.Add(H10.Time_and_Value_List[i+2].Time);//Запомнили точку спуска
        
            
        while (Math.Abs(H12.Time_and_Value_List[i].Value - H12.Time_and_Value_List[i + 4].Value) > 0.03 
        || Math.Abs(H11.Time_and_Value_List[i].Value - H11.Time_and_Value_List[i + 4].Value) > 0.03 
        ||Math.Abs(H10.Time_and_Value_List[i].Value - H10.Time_and_Value_List[i + 4].Value) > 0.03 
        ||Math.Abs(H9.Time_and_Value_List[i].Value - H9.Time_and_Value_List[i + 4].Value) > 0.03 
        ||Math.Abs(H8.Time_and_Value_List[i].Value - H8.Time_and_Value_List[i + 4].Value) > 0.03
        && i < end)
        {
        i++;  
        }
        if (i < end)//переход к следующему движению группы
        M1.Add(H10.Time_and_Value_List[i].Time);//Точка низа

        i+=1;
      }  
    

      return M1;
    }
    /// <summary>
    /// Улучшает в том смысле, что убирает лишние маленькие перемещения, непригодные для обработки
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    public List<DateTime> GiveM1_improove(List<DateTime> m1)
    {
    List<DateTime> M1_improove = new List<DateTime>();
    M1_improove.Add(m1[0]);
    
    for (int i = 0; i < m1.Count - 1; i += 1)
    {
      
      if (m1[i + 1].Subtract(m1[i]).TotalSeconds> Convert.ToInt32(MinTimeTexBox.Text))
      {
        M1_improove.Add(m1[i]);
        M1_improove.Add(m1[i+1]);
      }
    }
    M1_improove.Add(m1[m1.Count-1]);


    return M1_improove;
    }
    /// <summary>
    /// Получить лист с отступами в 33% у каждой точки через одну, начиная с 3-й точки.
    /// </summary>
    /// <param name="M"></param>
    /// <returns></returns>
    public List<DateTime> GiveIndentList_M3(List<DateTime> M)
    {
    List<DateTime> Mout=new List<DateTime>();
    Mout.AddRange(M);
      for (int i = 2; i < M.Count; i+=2)
      {
        Mout[i] = Give30PercentPointBetween(Mout[i], Mout[i + 1]);
      }
      return Mout;
    }
    /// <summary>
    /// Вернуть точку на 33% вперед (по отношению к следующей точке) от точку старта
    /// </summary>
    /// <param name="p_Start"></param>
    /// <param name="p_End"></param>
    /// <returns></returns>
    private DateTime Give30PercentPointBetween(DateTime p_Start, DateTime p_End)
    {
      return p_Start.AddSeconds((int)p_End.Subtract(p_Start).TotalSeconds / 3);
    }
    /// <summary>
    /// Получить наборы кофэффициентов для прямых.
    /// </summary>
    /// <param name="M"></param>
    /// <returns></returns>
    public List<Tuple<double,double>> Give_ab_Koeffs_M4(List<DateTime> M3)
    {
    List<Tuple<double,double>> AB_Koeffs= new List<Tuple<double,double>>();
    for (int i = 0; i < M3.Count; i+=2)
			    {
        //AB_Koeffs.Add(Fit.Line(
        //new double[] { M[i].ToOADate(), M[i + 1].ToOADate() },
        //new double[] { FindPoint(Reactivity.Time_and_Value_List, M[i]).Value, FindPoint(Reactivity.Time_and_Value_List, M[i+1]).Value }));
        List<double> Times = new List<double>();
        List<double> Values = new List<double>();
        GiveIntervalBtw(M3[i], M3[i + 1], out Values, out Times);

        AB_Koeffs.Add(Fit.Line(Times.ToArray(), Values.ToArray()));
       }

        return AB_Koeffs;
    }
    /// <summary>
    /// Получить интервал значений времени и значения между двумя точками времени
    /// </summary>
    /// <param name="p_Start"></param>
    /// <param name="p_End"></param>
    /// <returns></returns>
    private void GiveIntervalBtw(DateTime p_Start, DateTime p_End, out List<double> Values, out List<double> Times)
    {
    Times = new List<double>();
    Values = new List<double>();

    for (int i = this.FindIndex(Reactivity.Time_and_Value_List, p_Start); i <= this.FindIndex(Reactivity.Time_and_Value_List, p_End); i++)
      {
        Times.Add(Reactivity.Time_and_Value_List[i].Time.ToOADate());
        Values.Add(Reactivity.Time_and_Value_List[i].Value);
      }
    }
    /// <summary>
    /// Получить все значения изменений реактивности. И перемещений группы.
    /// </summary>
    /// <returns></returns>
    public List<Tuple<double, double>> GiveDelPo_M5(List<Tuple<double, double>> M4, List<DateTime> M1)
    {
      List<Tuple<double, double>> M5 = new List<Tuple<double, double>>();

      double DelPo=0;
      for (int i = 0; i < M4.Count-1; i++)
      {
      //точка по центру
        DateTime PointMiddle =   M1[i*2].AddSeconds(M1[i*2+1].Subtract(M1[i*2]).TotalSeconds / 2);

        DelPo = Math.Abs( M4[i].Item1 + M4[i].Item2 * PointMiddle.ToOADate() -
          (M4[i + 1].Item1 + M4[i + 1].Item2 * PointMiddle.ToOADate()));
        double dH12 = Math.Abs( Step_h*( FindPoint(H12.Time_and_Value_List, M1[i * 2]).Value - FindPoint(H12.Time_and_Value_List, M1[i * 2 + 1]).Value));
        double dH11 = Math.Abs( Step_h*(FindPoint(H11.Time_and_Value_List, M1[i * 2]).Value - FindPoint(H11.Time_and_Value_List, M1[i * 2 + 1]).Value));
        double dH10 = Math.Abs( Step_h*(FindPoint(H10.Time_and_Value_List, M1[i * 2]).Value - FindPoint(H10.Time_and_Value_List, M1[i * 2 + 1]).Value));
        double dH9 = Math.Abs( Step_h*(FindPoint(H9.Time_and_Value_List, M1[i * 2]).Value - FindPoint(H9.Time_and_Value_List, M1[i * 2 + 1]).Value));
        double dH8 = Math.Abs( Step_h*(FindPoint(H8.Time_and_Value_List, M1[i * 2]).Value - FindPoint(H8.Time_and_Value_List, M1[i * 2 + 1]).Value));
        if (dH12 > 1)
        {
          M5.Add(new Tuple<double,double> (DelPo * B_eff,dH12)); 
        }
        if (dH12 <1 && dH11>1)
        {
          M5.Add(new Tuple<double, double>(DelPo * B_eff, dH11));
        }
        if (dH12 < 1 && dH11 < 1 && dH10 > 1)
        {
          M5.Add(new Tuple<double, double>(DelPo * B_eff, dH10));
        }
        if (dH12 < 1 && dH11 < 1 && dH10 < 1 && dH9 > 1)
        {
          M5.Add(new Tuple<double, double>(DelPo * B_eff, dH9));
        }
        if (dH12 < 1 && dH11 < 1 && dH10 < 1 && dH9 < 1 && dH8 > 1)
        {
          M5.Add(new Tuple<double, double>(DelPo * B_eff, dH8));
        }

      }

      return M5;
    }
    /// <summary>
    /// Получить массив (x,y) , по которому рисовать будем сглаживающие прямые
    /// </summary>
    /// <param name="M1"></param>
    /// <param name="M4"></param>
    /// <returns></returns>
    public List<Tuple<double, double>> GiveXYLine_M6(List<DateTime> M2, List<Tuple<double, double>> M4)
    {
      List<Tuple<double, double>> M6 = new List<Tuple<double, double>>();
      //Промежуточный массив с серединными временами у пар точек изменения положения группы.
      List <DateTime> M2_half=new List<DateTime>();
      M2_half.Add(M2[0]);
      for (int i = 1; i < M2.Count-1; i+=2)
      {
        M2_half.Add(M2[i].AddSeconds(M2[i  + 1].Subtract(M2[i]).TotalSeconds / 2));
        M2_half.Add(M2[i].AddSeconds(M2[i + 1].Subtract(M2[i]).TotalSeconds / 2));
      }
      M2_half.Add(M2[M2.Count-1]);

      DateTime time;
      double value;
      for (int i = 0; i < M2_half.Count; i++)
      {
        time = M2_half[i];

        value = M4[(int)i / 2].Item1 + M4[(int)i / 2].Item2 * M2_half[i].ToOADate();
        M6.Add(new Tuple<double, double>(time.ToOADate(), value));
      }

      return M6;

    }
    /// <summary>
    /// Записать параметры
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonAccept_Click(object sender, RoutedEventArgs e)
    {
       
        //GroupKKSMainPart = GroupTexBox.Text;
        StreamWriter streamWriter = new StreamWriter("Options.txt");
        streamWriter.WriteLine(GroupTexBox.Text);//Главный признак группы.
        streamWriter.WriteLine(ReactivitiTexBox.Text);//Главный признак реактивности.
        streamWriter.WriteLine(ShagTexBox.Text);
        streamWriter.WriteLine(BetaTexBox.Text);
        streamWriter.Close();
    }
    

  }
}
