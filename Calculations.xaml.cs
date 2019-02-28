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
  /// Логика взаимодействия для Calculations.xaml
  /// </summary>
  public partial class Calculations : Window
  {
    public double Step_h = 3.75;
    public double B_eff = 0.74;
    public string ReactKKSMainPart = "247.";
    public string GroupKKSMainPart = "YVM";
    public string GroupKKSMainPart12 = "-";
    public string GroupKKSMainPart11 = "-";
    public string GroupKKSMainPart10 = "YVM10FG900";
    public string GroupKKSMainPart9 = "YVM10FG909";
    public string GroupKKSMainPart8 = "YVM10FG908";
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
        if (item.Time.Hour == Dt.Hour && item.Time.Minute == Dt.Minute && (item.Time.Second - Dt.Second) <= 1)
          return item;
      }
      MessageBox.Show("Не найдено совпадение времени с курсором");
      return new Time_and_Value();
    }
    /// <summary>
    /// Найти индекс элемента с указанным временем
    /// </summary>
    /// <param name="TadList"></param>
    /// <param name="Dt"></param>
    /// <returns></returns>
    public int FindIndex(List<Time_and_Value> TadList, DateTime Dt)
    {
      for (int i = 0; i < TadList.Count; i++)
      {
        if (TadList[i].Time.Hour==Dt.Hour && TadList[i].Time.Minute == Dt.Minute && (TadList[i].Time.Second - Dt.Second) <= 1)
        {
          return i;
        }
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
        GroupKKSMainPart12 = "-";
        GroupKKSMainPart11 = "-";
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

    //Выделим параметры для групп ОР СУЗ и реактивности.
    Parameter H12= null;
    Parameter H11 = null;
    Parameter H10 = null;
    Parameter H9 = null;
    Parameter H8 = null;
    Parameter Reactivity = null;
    try
    {
      H12 = (Parameter)Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart12));
      H11 = (Parameter)Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart11));
      H10 = (Parameter)Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart10));
      H9 = (Parameter)Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart9));
      H8 = (Parameter)Fileacts_ref.Parameters.Find(n => n.KKS.Contains(GroupKKSMainPart8));
      Reactivity = (Parameter)Fileacts_ref.Parameters.Find(n => n.KKS.Contains(ReactKKSMainPart));
    }
    catch (Exception)
    {}
   int start = FindIndex(H10.Time_and_Value_List,Start);
   int end = FindIndex(H10.Time_and_Value_List, End);
    if (H12!=null)
    {
      for (int i = start; i < end; i++)
      {
        if ( Math.Abs( H12.Time_and_Value_List[i].Value - H12.Time_and_Value_List[i+5].Value)>1 
        || Math.Abs(H11.Time_and_Value_List[i].Value - H11.Time_and_Value_List[i+5].Value)>1 || 
        Math.Abs(H10.Time_and_Value_List[i].Value - H10.Time_and_Value_List[i+5].Value)>1 || 
        Math.Abs(H9.Time_and_Value_List[i].Value - H9.Time_and_Value_List[i+5].Value)>1||
        Math.Abs(H8.Time_and_Value_List[i].Value - H8.Time_and_Value_List[i+5].Value)>1 )
        {
            M1.Add(H12.Time_and_Value_List[i].Time);//Запомнили точку опуска
            for (int j = 0; j < 1000; j++)
            {
              if (Math.Abs(H12.Time_and_Value_List[i].Value - H12.Time_and_Value_List[i + 5].Value) < 0.02 &&
         Math.Abs(H11.Time_and_Value_List[i].Value - H11.Time_and_Value_List[i + 5].Value) < 0.02 &&
        Math.Abs(H10.Time_and_Value_List[i].Value - H10.Time_and_Value_List[i + 5].Value) < 0.02 &&
        Math.Abs(H9.Time_and_Value_List[i].Value - H9.Time_and_Value_List[i + 5].Value) < 0.02 &&
        Math.Abs(H8.Time_and_Value_List[i].Value - H8.Time_and_Value_List[i + 5].Value) < 0.02)
              {
                M1.Add(H12.Time_and_Value_List[i].Time);//Точка низа
                continue;
              }
            }
        }
      }  
    }

      return M1;
    }

  }
}
