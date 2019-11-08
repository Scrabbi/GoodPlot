using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace GoodPlot
{
    
    class Chart_Acts
    {

    int I=0;
        /// <summary>
        /// Создает арену "по умолчанию": ось времени, значений, цвет курсора, масштабируема по все осям. Имеет меню
        /// </summary>
        /// <param name="Chart1"></param>
        public void LoadNEWArena(Chart Chart1, Int32 Arena_N)
        {
            //Все графики находятся в пределах области построения ChartArea, создадим ее
            Chart1.ChartAreas.Add(new ChartArea("Area" + "# " + Arena_N.ToString()));
            //Все же, запомним номер
            string Num_area = "Area" + "# " + Arena_N.ToString();
            //Все формат арены и курсора
            Chart1.ChartAreas[Num_area].CursorX.LineColor = Color.DarkGreen;
            Chart1.ChartAreas[Num_area].CursorX.LineDashStyle = ChartDashStyle.DashDot;
            Chart1.ChartAreas[Num_area].CursorX.LineWidth = 2;
            Chart1.ChartAreas[Num_area].CursorY.LineColor = Color.DarkGreen;
            Chart1.ChartAreas[Num_area].CursorY.LineDashStyle = ChartDashStyle.DashDot;
            Chart1.ChartAreas[Num_area].CursorY.LineWidth = 2;
            Chart1.ChartAreas[Num_area].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            Chart1.ChartAreas[Num_area].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            Chart1.ChartAreas[Num_area].AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            
            //Интервал и формат оси
            //CultureInfo ci = new CultureInfo("de-DE");
            Chart1.ChartAreas[Num_area].AxisX.IntervalType = DateTimeIntervalType.Auto;
            Chart1.ChartAreas[Num_area].AxisX.LabelStyle.Format = "HH:mm:ss";
            //Работа с курсором.
            //Добавление полосы прокрутки
            Chart1.ChartAreas[Num_area].AxisX.ScrollBar.Enabled = true;
            Chart1.ChartAreas[Num_area].AxisY.ScrollBar.Enabled = true;
            //Расположение полосы прокрутки
            Chart1.ChartAreas[Num_area].AxisX.ScrollBar.IsPositionedInside = true;
            Chart1.ChartAreas[Num_area].AxisY.ScrollBar.IsPositionedInside = true;
            //Разрешение увеличивать промежуток выделения
            Chart1.ChartAreas[Num_area].AxisX.ScaleView.Zoomable = true;
            Chart1.ChartAreas[Num_area].AxisY.ScaleView.Zoomable = true;
            //Разрешение пользоваться курсором
            Chart1.ChartAreas[Num_area].CursorX.IsUserEnabled = true;
            Chart1.ChartAreas[Num_area].CursorX.IsUserSelectionEnabled = true;
            Chart1.ChartAreas[Num_area].CursorY.IsUserEnabled = true;
            Chart1.ChartAreas[Num_area].CursorY.IsUserSelectionEnabled = true;
            //Выбор интервала для курсора. Ноль даст возможность выделять прямоугольные области.
            Chart1.ChartAreas[Num_area].CursorX.Interval = 0;
            Chart1.ChartAreas[Num_area].CursorY.Interval = 0;
            //Показывать конечное значение на оси
            //Chart1.ChartAreas[Num_area].AxisX.LabelStyle.IsStaggered=true;
            //Chart1.ChartAreas[Num_area].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Auto;
            Chart1.ChartAreas[Num_area].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //Chart1.ChartAreas[Num_area].AxisX.IntervalType = DateTimeIntervalType.Minutes;

            //Поработаем с легендой
            Legend My_Legend = new Legend(Num_area);
            

            My_Legend.Docking = Docking.Right;
            My_Legend.Alignment = StringAlignment.Far;
            My_Legend.DockedToChartArea = Num_area;
            My_Legend.IsDockedInsideChartArea = true;

            Chart1.Legends.Add(My_Legend);
            
            // Set custom chart area position
            Chart1.ChartAreas[Num_area].Position = new ElementPosition(5, 5, 90, 90);
            Chart1.ChartAreas[Num_area].InnerPlotPosition = new ElementPosition(5, 5, 90, 90);
        }
        ///// <summary>
        ///// Выровнять графики
        ///// </summary>
        //public void SetAlignment(Chart Chart1)
        //{
        //    for (int i = 1; i < Chart1.ChartAreas.Count; i++)
        //    {
        //        Chart1.ChartAreas[i].AlignWithChartArea = "Area" + "# " + 1;//1 площадка рисования
        //        Chart1.ChartAreas[i].AlignmentOrientation = AreaAlignmentOrientations.Vertical;
        //        //ChartFromForm.ChartAreas[i].AlignmentStyle = AreaAlignmentStyles.PlotPosition;
        //    }
        //}
        /// <summary>
        /// Создать ось СЛЕВА. (Создавая 2 доп. арены, 1 линию доп. невидимую, 1 перенося)
        /// </summary>
        /// <param name="chart">График</param>
        /// <param name="area">Исходная арена (для определения позиции)</param>
        /// <param name="series">Серия данных передаваема</param>
        /// <param name="axisOffset">Сдвиг оси</param>
        
        /// <summary>
        /// Создать дополнительную ось.
        /// </summary>
        /// <param name="chart">График</param>
        /// <param name="area">Исходная арена (для определения позиции)</param>
        /// <param name="series">Серия данных передаваема</param>
        /// <param name="axisOffset">Сдвиг оси</param>
        public void CreateSecondYAxis(string LeftRight, Chart chart, ChartArea area, Series series, float axisOffset, float labelsSize)
        {
          // СОздаем площадку для графика. Здесь русем график.
          ChartArea areaForSeria = new     ChartArea();
          if (LeftRight=="L")
          {
            areaForSeria = chart.ChartAreas.Add(area.Name + "1a");
          }
          else
          {
            areaForSeria = chart.ChartAreas.Add(area.Name + "2a");
          }
            
            areaForSeria.BackColor = Color.Transparent;
            areaForSeria.BorderColor = Color.Transparent;
            //Выровнять. Площадку, где рисуем и начальную.
            areaForSeria.Position.FromRectangleF(area.Position.ToRectangleF());
            areaForSeria.InnerPlotPosition.FromRectangleF(area.InnerPlotPosition.ToRectangleF());
            //Осей нет.
            areaForSeria.AxisX.Enabled = AxisEnabled.False;
            areaForSeria.AxisY.Enabled = AxisEnabled.False;
            areaForSeria.AxisY2.Enabled = AxisEnabled.False;
            //Разрешение пользоваться курсором
            areaForSeria.CursorX.IsUserEnabled = true;
            areaForSeria.CursorX.IsUserSelectionEnabled = true;
            areaForSeria.CursorY.IsUserEnabled = true;
            areaForSeria.CursorY.IsUserSelectionEnabled = true;
            //Выбор интервала для курсора. Ноль даст возможность выделять прямоугольные области.
            areaForSeria.CursorX.Interval = 0;
            areaForSeria.CursorY.Interval = 0;

            // Добавим линию, и назначим ее в созданную область.
            series.ChartArea = areaForSeria.Name;
            //Легенда линии привязывается к первой арене
            series.Legend = area.Name;
           
            // Создаем арену для размещения в ней  оси.
            // СОздаем площадку для графика. Здесь русем график.
            ChartArea areaForAxis = new ChartArea();
            if (LeftRight == "L")
            {
              areaForAxis = chart.ChartAreas.Add(area.Name + "1b");
              areaForAxis.AxisY.Name = "Y1" + "b";
            }
            else
            {
              areaForAxis = chart.ChartAreas.Add(area.Name + "2b");
              areaForAxis.AxisY2.Name = "Y2" + "b";
            }

            areaForAxis.BackColor = Color.Transparent;
            areaForAxis.BorderColor = Color.Transparent;
            areaForAxis.Position.FromRectangleF(chart.ChartAreas[series.ChartArea].Position.ToRectangleF());
            areaForAxis.InnerPlotPosition.FromRectangleF(chart.ChartAreas[series.ChartArea].Position.ToRectangleF());
            
            // Create a copy of specified series. Копия невидимая, так как надо ось настроить видимую.
            Series seriesCopy = chart.Series.Add(series.Name + "_Copy");
            seriesCopy.ChartType = series.ChartType;
            foreach (DataPoint point in series.Points)
            {
                seriesCopy.Points.AddXY(point.XValue, point.YValues[0]);
            }

            // Hide copied series
            seriesCopy.IsVisibleInLegend = false;
            seriesCopy.Color = Color.Transparent;
            seriesCopy.BorderColor = Color.Transparent;
            seriesCopy.ChartArea = areaForAxis.Name;
            
            // Настроим оси
            areaForAxis.AxisX.Enabled = AxisEnabled.False;
            if (LeftRight == "L")
            {
              areaForAxis.AxisY.Enabled = AxisEnabled.True;
              areaForAxis.AxisY2.Enabled = AxisEnabled.False;
              areaForAxis.AxisY.MajorGrid.Enabled = false;
            }
            else
            {
              areaForAxis.AxisY.Enabled = AxisEnabled.False;
              areaForAxis.AxisY2.Enabled = AxisEnabled.True;
              areaForAxis.AxisY2.MajorGrid.Enabled = false;
            }
            
            
            
            //Разрешение пользоваться курсором
            areaForAxis.CursorX.IsUserEnabled = true;
            areaForAxis.CursorX.IsUserSelectionEnabled = true;
            areaForAxis.CursorY.IsUserEnabled = true;
            areaForAxis.CursorY.IsUserSelectionEnabled = true;
            //Выбор интервала для курсора. Ноль даст возможность выделять прямоугольные области.
            areaForAxis.CursorX.Interval = 0;
            areaForAxis.CursorY.Interval = 0;

            // Adjust area position
            if (LeftRight == "L")
            {
              areaForAxis.Position.X -= axisOffset;
              areaForAxis.InnerPlotPosition.X -= labelsSize;
            }
            else
            {
              areaForAxis.Position.X += axisOffset;
              areaForAxis.InnerPlotPosition.X += labelsSize;
            }
            
        }
        /// <summary>
        /// Добавить линию для оси на данной арене.
        /// </summary>
        public void AddLine(Chart Chart1, System.Windows.Controls.ListView LW_given, System.Collections.IList tempKKSs, string chartareaName, File_Acts File_Acts_One, string lrAxis, string ISsecond)
        {
          string SerName="";
          //Можно много параметров выделить потому что.
          foreach (Parameter item in tempKKSs)
          {
            //Добавлем имя серии
            SerName=item.KKS;

            ////Если серия уже была, не уникальна, имя серии для нее назовем с добавлением номера, сколько раз уже строили дополнительные линии аналогичные, в конце.
            //РЕШИл, ЧТО ЭТО ЛИШНЕЕ
            //if (!Chart1.Series.IsUniqueName(SerName))
            //{
            //  I++;
            //  SerName+=I;
            //} 
            //Добавили серию в список графика
            Chart1.Series.Add(new Series(SerName));
            
            //Легенда видимая будет.
            Chart1.Series[SerName].IsVisibleInLegend = true;

            //Определяем, строились ли дополнительные оси.
            bool noAxis=true;
            foreach (var item1 in Chart1.ChartAreas)
            {
              if (item1.Name.Contains("1b") || item1.Name.Contains("2b"))
              {
                noAxis=false;
              }
            }
            //Если нет дополнительной оси. Легенду привязываем к указанной арене. 
            if (noAxis)
	            {
               Chart1.Series[SerName].Legend = chartareaName;
	            }
            //Если есть, то и тип графика с 4 осями, на основную арену легенду наносим.
            else
              Chart1.Series[SerName].Legend = "Area# 1";

            //Тип линии
            Chart1.Series[SerName].ChartType = SeriesChartType.Line;
            Chart1.Series[SerName].BorderDashStyle = ChartDashStyle.Solid;

            

            


            //По какой оси
            if (lrAxis == "left")
            {
              Chart1.Series[SerName].YAxisType = AxisType.Primary;
            }
            else
            {
              Chart1.Series[SerName].YAxisType = AxisType.Secondary;
            }

            
            //Пополнение серии.   
            foreach (Time_and_Value item1 in File_Acts_One.Find_Parametr(SerName).Time_and_Value_List)
            {
              Chart1.Series[SerName].Points.AddXY(item1.Time, item1.Value);
            }

            //Арена линии соответствующая
            Chart1.Series[SerName].ChartArea = chartareaName;
          
            //ЕСЛИ на допоплнительных осях строим в первый раз.
          bool IsarenaReadyLeft = false;
          foreach (var item1 in Chart1.ChartAreas)
          {
            if (item1.Name.Contains("1b"))
            {
              IsarenaReadyLeft = true;
            }
          }

          if (ISsecond == "Yes" && !IsarenaReadyLeft && lrAxis=="left")
            {
              //Теперь переносим ее.
              this.CreateSecondYAxis("L", Chart1, Chart1.ChartAreas[chartareaName], Chart1.Series[SerName], 4, 0);
            }
          if (ISsecond == "Yes" && IsarenaReadyLeft)
          {
            //Теперь переносим ее.
            // Добавим линию, и назначим ее в созданную область.
            Chart1.Series[SerName].ChartArea = Chart1.ChartAreas[chartareaName].Name + "1a";
          }


          bool IsarenaReadyRight = false;
          foreach (var item2 in Chart1.ChartAreas)
          {
            if (item2.Name.Contains("2b"))
            {
              IsarenaReadyRight = true;
            }
          }

          if (ISsecond == "Yes" && !IsarenaReadyRight && lrAxis == "right")
            {
              //Теперь переносим ее.
              this.CreateSecondYAxis("R", Chart1, Chart1.ChartAreas[chartareaName], Chart1.Series[item.KKS], 4, 0);
            }
          if (ISsecond == "Yes" && IsarenaReadyRight)
          {
            //Теперь переносим ее.
            // Добавим линию, и назначим ее в созданную область.
            Chart1.Series[SerName].ChartArea = Chart1.ChartAreas[chartareaName].Name + "2a";
          }

          //ПРи удалении единственной линии на оси дополнительной, мы ее крыли. Откроем
          if (ISsecond.Contains("Yes"))
          {
            if (lrAxis == "left")
            {
              Chart1.ChartAreas["Area# 11b"].AxisY.Enabled = AxisEnabled.True;
            }
            //ПРавая ось
            else
            {
              Chart1.ChartAreas["Area# 12b"].AxisY2.Enabled = AxisEnabled.True;
            }
          }
          }
          //LW_given.UnselectAll();
        }

        /// <summary>
        /// Добавить арену снизу.
        /// </summary>
        /// <param name="Chart1"></param>
        public void Add_Area_Bottom(Chart Chart1)
        {
          ////Сохраняем позиции по оси "Х", которые у надписей отчего-то сбиваются
          //List <float> Xpos = new List<float>(); 
          //foreach (var item in Chart1.Titles)
          //{
          // Xpos.Add(item.Position.X);
          //}

            //Добавляем арена с номером СЛЕДУЮЩИМ
            LoadNEWArena(Chart1, Chart1.ChartAreas.Count + 1);
            //Получаем теперь , уже настоящий, этот номер.
          int GraphCounter = Chart1.ChartAreas.Count;
          //Новый размер окон.
          float New_size_H = (GraphCounter - 1) * Chart1.ChartAreas[0].Position.Height / GraphCounter;
          // Disable X Axis Labels for the first chart area. Достаточно и так. Остальные оси "Х" не рисовались..
          Chart1.ChartAreas[GraphCounter - 2].AxisX.LabelStyle.Enabled = false;
          Chart1.ChartAreas[GraphCounter - 2].AxisX.MajorTickMark.Enabled = false;
          //Задать размеры всем.
          for (int i = 0; i < GraphCounter; i++)
          {
            Chart1.ChartAreas[i].Position.Height = New_size_H-1;
            //Шрифт одинаковый сделать
            Chart1.ChartAreas[i].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 10);
            Chart1.ChartAreas[i].AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 10);
            //Положение по высоте
            Chart1.ChartAreas[i].Position.Y = i * (New_size_H)+3;

          }
          //Выравнивание
          for (int i = 1; i < Chart1.ChartAreas.Count; i++)
          {
            Chart1.ChartAreas[i].AlignWithChartArea = "Area" + "# " + 1;//1 площадка рисования
            Chart1.ChartAreas[i].AlignmentOrientation = AreaAlignmentOrientations.Vertical;
              //Chart1.ChartAreas[i].AlignmentStyle=AreaAlignmentStyles.AxesView;
              //ChartFromForm.ChartAreas[i].AlignmentStyle = AreaAlignmentStyles.PlotPosition;
          }
          //Работает довольно точно. Единственно, при ломаной линии есть косяк, сложно его исправить. Начало же хорошо перемещается. Но остальное можно и руками перетащить.
          for (int i = 0; i < Chart1.Annotations.Count; i++)
          {
            Chart1.Annotations[i].Y = (GraphCounter - 1) * (Chart1.Annotations[i].Y) / GraphCounter;
            Chart1.Annotations[i].Width = Chart1.Annotations[i].Width * (GraphCounter - 1) / GraphCounter;
            Chart1.Annotations[i].Height = Chart1.Annotations[i].Height * (GraphCounter - 1) / GraphCounter;

          }
          for (int i = 0; i < Chart1.Titles.Count; i++)
          {
            Chart1.Titles[i].Position.Y = (GraphCounter - 1) * (Chart1.Titles[i].Position.Y) / GraphCounter;
            Chart1.Titles[i].Font = new System.Drawing.Font("Times New Roman", 9, System.Drawing.FontStyle.Regular);
          }

         
        }
        

    }
}
