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
        ///// <summary>
        ///// Передавайемый из MainWindow чарт
        ///// </summary>
        //Chart Chart1 = new Chart();
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
        public void AddLine(Chart Chart1, string tempKKS, string chartareaName, File_Acts File_Acts_One, string lrAxis, string What_Axis)
        {
          try
          {
            Chart1.Series.Add(new Series(tempKKS));
          }
          catch (ArgumentException)
          {
            System.Windows.MessageBox.Show("Данная линия уже имеется.");
            return;
          }
          //Легенда.
          if (!chartareaName.Contains("1a") && !chartareaName.Contains("2a"))
          {
            Chart1.Series[tempKKS].Legend = chartareaName;
          }
          else
          Chart1.Series[tempKKS].Legend = "Area# 1";
          //Отображение легенды
          Chart1.Series[tempKKS].IsVisibleInLegend = true;
          //Тип линии
          Chart1.Series[tempKKS].ChartType = SeriesChartType.Line;
          Chart1.Series[tempKKS].BorderDashStyle = ChartDashStyle.Solid;

          //ПРи удалении единственной линии на оси дополнительной, мы ее крыли. Откроем
          if (chartareaName.Contains("1a") || chartareaName.Contains("2a"))
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
          
            //Арена линии соответствующая
            Chart1.Series[tempKKS].ChartArea = chartareaName;

            
            //По первой оси
            if (lrAxis=="left")
            {
                Chart1.Series[tempKKS].YAxisType = AxisType.Primary;   
            }
            else
            {
                Chart1.Series[tempKKS].YAxisType = AxisType.Secondary;   
            }
            
            //Chart1.Series[tempKKS].Color = System.Drawing.Color.Red; Задавайемый так цвет уже отразится не нулевым.
            //Пополнение серии.   
            foreach (Time_and_Value item in File_Acts_One.Find_Parametr(tempKKS).Time_and_Value_List)
            {
                Chart1.Series[tempKKS].Points.AddXY(item.Time, item.Value);
            }

            //ЕСЛИ на допоплнительных осях строим в первый раз.
            if (What_Axis=="L")
            {
              //Теперь переносим ее.
              this.CreateSecondYAxis("L", Chart1, Chart1.ChartAreas[chartareaName], Chart1.Series[tempKKS], 4, 0);
            }
            if (What_Axis=="R")
            {
             //Теперь переносим ее.
              this.CreateSecondYAxis("R", Chart1, Chart1.ChartAreas[chartareaName], Chart1.Series[tempKKS], 4, 0);
            }
        }
        /// <summary>
        /// Добавить арену снизу.
        /// </summary>
        /// <param name="Chart1"></param>
        public void Add_Area_Bottom(Chart Chart1)
        {
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
            Chart1.ChartAreas[i].Position.Height = New_size_H;
            //Шрифт одинаковый сделать
            Chart1.ChartAreas[i].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 10);
            Chart1.ChartAreas[i].AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 10);
            //Ширину
            Chart1.ChartAreas[i].Position.Y = i * New_size_H;
          }
            //Выравнивание
          for (int i = 1; i < Chart1.ChartAreas.Count; i++)
          {
            Chart1.ChartAreas[i].AlignWithChartArea = "Area" + "# " + 1;//1 площадка рисования
            Chart1.ChartAreas[i].AlignmentOrientation = AreaAlignmentOrientations.Vertical;
              //Chart1.ChartAreas[i].AlignmentStyle=AreaAlignmentStyles.AxesView;
              //ChartFromForm.ChartAreas[i].AlignmentStyle = AreaAlignmentStyles.PlotPosition;
          }
        }

        }
}
