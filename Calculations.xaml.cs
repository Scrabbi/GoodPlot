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

namespace GoodPlot
{
  /// <summary>
  /// Логика взаимодействия для Calculations.xaml
  /// </summary>
  public partial class Calculations : System.Windows.Window
  {
    public double Step_h = 3.75;
    public double B_eff = 0.74;
    public string ReactKKSMainPart = "247.";
    public string GroupKKSMainPart = "YVM";
    public string GroupKKSMainPart12 = "-----";
    public string GroupKKSMainPart11 = "-----";
    public string GroupKKSMainPart10 = "YVM10FG900";
    public string GroupKKSMainPart9 = "YVM10FG909";
    public string GroupKKSMainPart8 = "YVM10FG908";

    //Выделим параметры для групп ОР СУЗ и реактивности.
    Parameter H12 = null;
    Parameter H11 = null;
    Parameter H10 = null;
    Parameter H9 = null;
    Parameter H8 = null;
    Parameter Reactivity = null;
    /// <summary>
    /// Доступ к списку параметров
    /// </summary>
    static File_Acts Fileacts_ref;
    public Calculations(File_Acts FA)
    {
      InitializeComponent();
      Fileacts_ref=FA;
    }
    /// <summary>
    /// Найти из списка точек точку с заданным временем
    /// </summary>
    /// <param name="TadList">Список точек</param>
    /// <param name="Dt">Заданное время</param>
    /// <returns></returns>
    public Time_and_Value FindPoint(List<Time_and_Value> TadList, DateTime Dt)
    {
      foreach (var item in TadList)
      {
        if (item.Time.Hour == Dt.Hour && item.Time.Minute == Dt.Minute && Math.Abs(item.Time.Second - Dt.Second) <= 1)
          return item;
      }
      MessageBox.Show("Не найдено совпадение времени с курсором");
      return new Time_and_Value();
    }
    /// <summary>
    /// Найти индекс элемента в списке значений времён с указанным временем
    /// </summary>
    /// <param name="TadList"></param>
    /// <param name="Dt"></param>
    /// <returns></returns>
    public int FindIndex(List<Time_and_Value> TadList, DateTime Dt)
    {
      for (int i = 0; i < TadList.Count; i++)
      {
        if (TadList[i].Time.Hour==Dt.Hour && TadList[i].Time.Minute == Dt.Minute && Math.Abs(TadList[i].Time.Second - Dt.Second) <= 1)
        return i;
      }

    return 0;
     }

    /// <summary>
    /// Изменение значения бета эфф
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BetaTexBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        B_eff = Convert.ToDouble(BetaTexBox.Text);
      }
      catch (FormatException)
      {}
      
    }
    /// <summary>
    /// Изменение значения шага группы в см
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ShagTexBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        Step_h = Convert.ToDouble(ShagTexBox.Text);
      }
      catch (FormatException)
      { }
    }
    /// <summary>
    /// Изменение признака реактивности
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReactivitiTexBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        ReactKKSMainPart = ReactivitiTexBox.Text;
      }
      catch (FormatException)
      { }
    }
    /// <summary>
    /// Изменение признака группы, а так же признака для групп 12-8
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GroupTexBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      try
      {
        GroupKKSMainPart = GroupTexBox.Text;
      }
      catch (FormatException)
      { }
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
    {}
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
        &&Math.Abs(H8.Time_and_Value_List[i].Value - H8.Time_and_Value_List[i + 3].Value) < 0.05)
        {
        i++;
        }
        if (i < end)//а то последняя точка залезает
        M1.Add(H10.Time_and_Value_List[i].Time);//Запомнили точку спуска
        
            
        while (Math.Abs(H12.Time_and_Value_List[i].Value - H12.Time_and_Value_List[i + 5].Value) > 0.03 
        || Math.Abs(H11.Time_and_Value_List[i].Value - H11.Time_and_Value_List[i + 5].Value) > 0.03 
        ||Math.Abs(H10.Time_and_Value_List[i].Value - H10.Time_and_Value_List[i + 5].Value) > 0.03 
        ||Math.Abs(H9.Time_and_Value_List[i].Value - H9.Time_and_Value_List[i + 5].Value) > 0.03 
        ||Math.Abs(H8.Time_and_Value_List[i].Value - H8.Time_and_Value_List[i + 5].Value) > 0.03)
        {
        i++;  
        }
        if (i < end)//а то последняя точка залезает
        M1.Add(H10.Time_and_Value_List[i].Time);//Точка низа

        i++;
      }  
    

      return M1;
    }
    /// <summary>
    /// Получить лист с отступами в 33% у каждой точки через одну, начиная с 3-й точки.
    /// </summary>
    /// <param name="M"></param>
    /// <returns></returns>
    public List<DateTime> GiveIndentList(List<DateTime> M)
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
    public List<Tuple<double,double>> Give_ab_Koeffs(List<DateTime> M)
    {
    List<Tuple<double,double>> AB_Koeffs= new List<Tuple<double,double>>();
    for (int i = 0; i < M.Count; i+=2)
			    {
        //AB_Koeffs.Add(Fit.Line(
        //new double[] { M[i].ToOADate(), M[i + 1].ToOADate() },
        //new double[] { FindPoint(Reactivity.Time_and_Value_List, M[i]).Value, FindPoint(Reactivity.Time_and_Value_List, M[i+1]).Value }));
        List<double> Times = new List<double>();
        List<double> Values = new List<double>();
        GiveIntervalBtw(M[i], M[i + 1], out Values, out Times);

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

    for (int i = this.FindIndex(Reactivity.Time_and_Value_List, p_Start); i < this.FindIndex(Reactivity.Time_and_Value_List, p_End); i++)
      {
        Times.Add(Reactivity.Time_and_Value_List[i].Time.ToOADate());
        Values.Add(Reactivity.Time_and_Value_List[i].Value);
      }
    }
    /// <summary>
    /// Получить все значения изменений реактивности.
    /// </summary>
    /// <returns></returns>
    public List<double> GiveDelPo()
    {
      

    }
  }
}
