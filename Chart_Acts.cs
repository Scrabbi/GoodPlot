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
                                    //Работа с графиком
    class Chart_Acts
    {
                  /// <summary>
         /// Счетчик главных арен
         /// </summary>
        int mainArenaCounter=0;
                  /// <summary>
        /// Счетчик повторно строящихся линий
        /// </summary>
        int slisesCounter=0;
                  /// <summary>
        /// Счетчик дополнительных осей слева и справа.
        /// </summary>
        int addAxisesCounter_L = 0, addAxisesCounter_R = 0;
                  /// <summary>
        /// Постоянные , на сколько сдвинута ось, место под отметки шкалы
        /// </summary>
        float axisOffset=7,  labelsSize=4;
              /// <summary>
        /// Создать начальную (главную) арену: ось времени, значений, масштабируема по все осям. Имеет меню
        /// </summary>
        /// <param name="Chart1">Передаваемый из MainWindow объект график.</param>
        public void LoadFirstArena(Chart myChart)
        {
                  //Увеличиваем счетчик главных арен, так как добавляем 1.
            mainArenaCounter++;
                  //Все графики находятся в пределах области построения ChartArea, создадим ее.
            string meinAreaName = "Area" + "# " + mainArenaCounter;     //Запомним номер
            myChart.ChartAreas.Add( new ChartArea(meinAreaName) );
                  //Формат линий сетки
            myChart.ChartAreas[meinAreaName].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            myChart.ChartAreas[meinAreaName].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            myChart.ChartAreas[meinAreaName].AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                  //Интервал и формат оси
            myChart.ChartAreas[meinAreaName].AxisX.IntervalType = DateTimeIntervalType.Auto;
            myChart.ChartAreas[meinAreaName].AxisX.LabelStyle.Format = "HH:mm:ss";
                  //Добавление полосы прокрутки.
            myChart.ChartAreas[meinAreaName].AxisX.ScrollBar.Enabled = true;
            myChart.ChartAreas[meinAreaName].AxisY.ScrollBar.Enabled = true;
            myChart.ChartAreas[meinAreaName].AxisY2.ScrollBar.Enabled = true;
                  //Разрешение увеличивать промежуток выделения
            myChart.ChartAreas[meinAreaName].AxisX.ScaleView.Zoomable = true;
            myChart.ChartAreas[meinAreaName].AxisY.ScaleView.Zoomable = true;
            myChart.ChartAreas[meinAreaName].AxisY2.ScaleView.Zoomable = true;
                  //Разрешение пользоваться курсором
            myChart.ChartAreas[meinAreaName].CursorX.IsUserEnabled = true;
            myChart.ChartAreas[meinAreaName].CursorX.IsUserSelectionEnabled = true;
            myChart.ChartAreas[meinAreaName].CursorY.IsUserEnabled = true;
            myChart.ChartAreas[meinAreaName].CursorY.IsUserSelectionEnabled = true;
                  //Выбор интервала для курсора. Ноль даст возможность выделять прямоугольные области нормально. Иначе коряво работает
            myChart.ChartAreas[meinAreaName].CursorX.Interval = 0;
            myChart.ChartAreas[meinAreaName].CursorY.Interval = 0;
                  //Автомасштаб оси
            myChart.ChartAreas[meinAreaName].AxisY.IsStartedFromZero=false;
            myChart.ChartAreas[meinAreaName].AxisY2.IsStartedFromZero = false;
                  //Задаем, как интервалы считаются
            myChart.ChartAreas[meinAreaName].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                  //Поработаем с легендой
            Legend My_Legend = new Legend(meinAreaName);
            My_Legend.Docking = Docking.Right;
            My_Legend.Alignment = StringAlignment.Far;
            My_Legend.IsDockedInsideChartArea = true;
                 // Set custom chart area position. 
            myChart.ChartAreas[meinAreaName].Position = new ElementPosition(1, 1, 90, 90);
            myChart.ChartAreas[meinAreaName].InnerPlotPosition = new ElementPosition(5, 1, 90, 95);
        }
              /// <summary>
        /// Добавить линию на основной арене. ПРи этом, один и тот же параметр можно несколько раз рисовать. Сделано, чтобы иметь возможность сшивать графики по времени.
        /// </summary>
        /// <param name="chart1">График, с которым работаем</param>
        /// <param name="selectedKKSs">Выбранная пользователем группа параметров</param>
        /// <param name="chartareaName"> Выбранное пользователем имя главной арены, на которой рисуем.</param>
        /// <param name="myFileActs">Передает данные</param>
        /// <param name="r_L">Сообщает сторону оси</param>
        public void AddMainLine(Chart myChart, System.Collections.IList selectedKKSs, string chartareaName, File_Acts myFileActs, string r_L)
        {
                //Сейчас обрабатываемый KKS из списка переданного.
          string serName = "";
                //Цикл по выдленным параметрам.
          foreach (Parameter item in selectedKKSs)
          {
                //Проверка, была ли уже серия с таким именем. (Если сшивали по времени.) Счетчик увеличиваем, имя с номером повторно добавленной серии называем. Если единственная линия с таким именем, как обычно, по KKS называем.
            bool isAttend=false; // ПРисутствует ли добавляемое имя линии
            foreach (var item0 in myChart.Series)
            {
              if (item0.Name==item.KKS)
              {
              isAttend=true;
              }
            }
            if (isAttend)
            {
              slisesCounter++;
              serName = item.KKS + "_" + slisesCounter;
            }
            else
              serName = item.KKS;
                  //Добавляем серию на график
            myChart.Series.Add(new Series(serName));
                  //Легенда видимая будет.
            myChart.Series[serName].IsVisibleInLegend = true;
                  //Тип линии
            myChart.Series[serName].ChartType = SeriesChartType.Line;
            myChart.Series[serName].BorderDashStyle = ChartDashStyle.Solid;
                  //Арена линии соответствующая
            myChart.Series[serName].ChartArea = chartareaName;
                  //По какой оси
            if (r_L == "left")
            {
              myChart.Series[serName].YAxisType = AxisType.Primary;
            }
            else
            {
              myChart.Series[serName].YAxisType = AxisType.Secondary;
            }
                  //Пополнение серии.   
            foreach (Time_and_Value item1 in myFileActs.Find_Parametr(serName).Time_and_Value_List)
            {
              myChart.Series[serName].Points.AddXY(item1.Time, item1.Value);
            }
          }
        }
              /// <summary>
        /// Добавить линию на дополнительной оси (на дополнительной арене). ПРи этом, один и тот же параметр можно по-прежнему несколько раз рисовать.  
        /// Насчет положения арены: есть положение самой арены. Есть положение пространства для рисования графиков. Оно внутри чартарены. Таким образом, надо и то и то задать. Схема, как это работает -- в описании. Кода много, но все просто. Хотя логику придется смотреть в описании.
        /// </summary>
        /// <param name="myChart">График, с которым работаем</param>
        /// <param name="chartareaName"> Выбранное пользователем имя главной арены, на которой рисуем. Здесь для привязки к этой арене</param>
        /// <param name="selectedKKSs">Выбранная пользователем группа параметров</param>
        /// <param name="myFileActs">Передает данные</param>
        /// <param name="r_L">Сообщает сторону оси</param>
        /// <param name="axisOffset">На сколько сдвигать арену с дополнительной осью</param>
        /// <param name="labelsSize">На сколько сдвигать область рисования арены с дополнительной осью</param>
        public void AddAdditional_AxisAndlLine(Chart myChart, string chartareaName, System.Collections.IList selectedKKSs, File_Acts myFileActs, string r_L)
        {
                  //Цикл по выделенным параметрам пользователем.
          foreach (Parameter item in selectedKKSs)
          {
                         //1) Создаем незаметную арену для видимой серии на графике. Она точно совпадает с основной ареной.
            ChartArea areaSeria = myChart.ChartAreas.Add("invisible_" + chartareaName + "_#" ); //Имя дополним (для вновь добавляемых арен было бы повторяющимся)
          areaSeria.BackColor = Color.Transparent;
          areaSeria.BorderColor = Color.Transparent;
          areaSeria.Position.FromRectangleF(myChart.ChartAreas[chartareaName].Position.ToRectangleF());
          areaSeria.InnerPlotPosition.FromRectangleF(myChart.ChartAreas[chartareaName].InnerPlotPosition.ToRectangleF());
          areaSeria.AxisX.MajorGrid.Enabled = false;
          areaSeria.AxisX.MajorTickMark.Enabled = false;
          areaSeria.AxisX.LabelStyle.Enabled = false;
          areaSeria.AxisY.MajorGrid.Enabled = false;
          areaSeria.AxisY.MajorTickMark.Enabled = false;
          areaSeria.AxisY.LabelStyle.Enabled = false;
          areaSeria.AxisY.IsStartedFromZero = myChart.ChartAreas[chartareaName].AxisY.IsStartedFromZero;
                    //Добавляем на главную арену серию, переносим ее на эту невидимую арену. Первые 2 строки ниже -- для подстановки в метод добавления по главной оси параметра единственного. 
          List<Parameter> itemAsCollection   =  new List<Parameter>();
          itemAsCollection.Add(item);
          AddMainLine(myChart, itemAsCollection, chartareaName, myFileActs, r_L);
          myChart.Series[item.KKS].ChartArea = areaSeria.Name;//сам перенос на невидимую арену.
                    //Определение слева/справа строить
          if (r_L == "left")
          {
            myChart.Series[item.KKS].YAxisType = AxisType.Primary;
          }
          else
          {
            myChart.Series[item.KKS].YAxisType = AxisType.Secondary;
          }
                     // 2) Теперь создаем арену для оси. (Кроме оси дополнительной, на ней все остальное незаметное.) Делаем совпадающей с основной сейчас.
          ChartArea areaAxis = myChart.ChartAreas.Add("arenaForAxisYin_" + chartareaName + "_#" );
          areaAxis.BackColor = Color.Transparent;
          areaAxis.BorderColor = Color.Transparent;
          areaAxis.Position.FromRectangleF(myChart.ChartAreas[chartareaName].Position.ToRectangleF());
          areaAxis.InnerPlotPosition.FromRectangleF(myChart.ChartAreas[chartareaName].InnerPlotPosition.ToRectangleF());
                    // Создаем копию видимой серии. (Копия невидима.)
          Series seriesCopy = myChart.Series.Add(myChart.Series[item.KKS].Name + "_invisibleCopy");
          seriesCopy.ChartType = myChart.Series[item.KKS].ChartType;
          foreach (DataPoint point in myChart.Series[item.KKS].Points)
          {
            seriesCopy.Points.AddXY(point.XValue, point.YValues[0]);
          }
          seriesCopy.ChartArea = areaAxis.Name;
                      //Определение слева/справа строить
          if (r_L == "left")
          {
            seriesCopy.YAxisType = AxisType.Primary;
          }
          else
          {
            seriesCopy.YAxisType = AxisType.Secondary;
          }
                    // Скрываем копию серии
          seriesCopy.IsVisibleInLegend = false;
          seriesCopy.Color = Color.Transparent;
          seriesCopy.BorderColor = Color.Transparent;
                    // Disable grid lines & tickmarks
          areaAxis.AxisX.LineWidth = 0;
          areaAxis.AxisX.MajorGrid.Enabled = false;
          areaAxis.AxisX.MajorTickMark.Enabled = false;
          areaAxis.AxisX.LabelStyle.Enabled = false;
          areaAxis.AxisY.MajorGrid.Enabled = false;
          areaAxis.AxisY.IsStartedFromZero = myChart.ChartAreas[chartareaName].AxisY.IsStartedFromZero;
                    // 3) Задаем позицию оси. Смотрится сложно, ведь R_l вначале (определить сторону). Потом сдвинуть по этой стороне.
                    //switch выбирает сторону
          switch (r_L)
          {
            case "left":
                      //Счетчик количества арен слева увеличим, созданные арены правильно назовем.
              addAxisesCounter_L++;
              areaSeria.Name = "invisible_" + chartareaName + "_#" + addAxisesCounter_L;
              areaAxis.Name = "arenaForAxisYin_" + chartareaName + "_#" + addAxisesCounter_L;
                        //Cжимаем все арены, кроме нашей с осью, последней, на axisOffset. А позицию -- вперед.
              for (int i = 0; i < myChart.ChartAreas.Count - 1; i++)
              {
              myChart.ChartAreas[i].Position.Width -= axisOffset;
              myChart.ChartAreas[i].Position.X += axisOffset;
              }
                        //Последнюю (которую добавляем) тоже сожмем. Двигаем ее назад на axisOffset*(addAxisesCounter-1) . Её площадку с графиком, который связан с осью, от начала на labelsSize отодвигаем. МОжет показаться странным множитель (addAxisesCounter - 1). Он тут поскольку положение последней мы задавали при добавлении наравне с основной. И если добавлялись еще оси, то сдвигать следует соответственно.
              areaAxis.Position.Width -= axisOffset;
              areaAxis.Position.X -= axisOffset * (addAxisesCounter_L - 1);
              areaAxis.InnerPlotPosition.X += labelsSize;
            break;

                      //Сдвигаем в другую сторону
            case "right":
                       //Счетчик количества арен слева увеличим, созданные арены правильно назовем.
            addAxisesCounter_R++;
            areaSeria.Name = "invisible_" + chartareaName + "_#" + addAxisesCounter_R;
            areaAxis.Name = "arenaForAxisYin_" + chartareaName + "_#" + addAxisesCounter_R;
                        //Cжимаем все арены, кроме нашей с осью, последней, на axisOffset. А позицию -- без изменений.
            for (int i = 0; i < myChart.ChartAreas.Count - 1; i++)
            {
              myChart.ChartAreas[i].Position.Width -= axisOffset;
            }
                      //Последнюю (которую добавляем) тоже сожмем. Двигаем ее вперед на axisOffset*(addAxisesCounter-1) . Её площадку с графиком, который связан с осью, от начала на labelsSize отодвигаем влево. 
            areaAxis.Position.Width -= axisOffset;
            areaAxis.Position.X += axisOffset * addAxisesCounter_R;
            areaAxis.InnerPlotPosition.X -= labelsSize;
            break;

            default:
              System.Windows.MessageBox.Show("Это никогда не выводтся. ПРосто для заполнения default");
                break;
          }//Конец switch
         }//Закрыли обход по списку выделенных KKS.
       }//Закрыли тело метода
        
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
            LoadFirstArena(Chart1);
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
        }//Тело метода
     }//Класс
}//Пространство имен
