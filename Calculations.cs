using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using MathNet.Numerics;

namespace GoodPlot
{
 public class Calculations
  {
  
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
        if (item.Time.Hour == Dt.Hour && item.Time.Minute == Dt.Minute && (item.Time.Second - Dt.Second)<=1)
        return item;
      }
      MessageBox.Show("Не найдено совпадение времени с курсором");
      return new Time_and_Value();
    }

  }
}
