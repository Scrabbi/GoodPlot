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
         /// Счетчик главных арен. Они пронумерованы сверху вниз.
         /// </summary>
        public int mainArenaCounter=0;
                  /// <summary>
        /// Счетчик повторно строящихся линий. Для их нумерации. К имени присоединяется номер и становится уникальным имя.
        /// </summary>
        int slisesCounter=0;
                  /// <summary>
        /// Постоянные , на сколько сдвинута ось, место под отметки шкалы.
        /// </summary>
        float axisOffset=4,  labelsSize=5;
              /// <summary>
        /// Создать начальную (главную) арену: ось времени, значений, масштабируема по всем осям.
        /// </summary>
        /// <param name="Chart1">Передаваемый из MainWindow объект график.</param>
        public void LoadFirstArena(Chart myChart)
        {
                  //Увеличиваем счетчик главных арен, так как добавляем 1.
            mainArenaCounter++;
                  //Все графики находятся в пределах области построения ChartArea, создадим ее. Номер идентифицицирует: 1 - выше, 2-ниже и так далее.
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
            Legend My_Legend = new Legend(meinAreaName + "_Legend");
            myChart.Legends.Add(My_Legend);
            My_Legend.Enabled = true;
            My_Legend.LegendStyle = LegendStyle.Table;
            My_Legend.TableStyle = LegendTableStyle.Auto;
            My_Legend.Docking = Docking.Right;
            My_Legend.Alignment = StringAlignment.Far;
            My_Legend.InsideChartArea = meinAreaName;
            
                  //В верхнем левом углу начало координат.
            myChart.ChartAreas[meinAreaName].Position = new ElementPosition(1, 1, 99, 99);
                  //Позиция полотна. 
                  //Автоположение выставляется некорректно, когда много арен добавляется (как у нас будет) 
                  //Ширину оставить 100- 2*labelsSize. Так как оси будут и справа, и слева.
                  // Высоту -- по удобству отображения, но место для меток оставить.
                  // По "У" не отступаем вообще.
                  //По "Х" слева labelsSize для отметок делений. Справа этого не нужно, место справа расчитается по формуле: ширина-место слева=место справа.
            myChart.ChartAreas[meinAreaName].InnerPlotPosition = new ElementPosition(labelsSize, 0, 90, 93);
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
          foreach (Parameter selectedKKS_i in selectedKKSs)
          {
                //Была ли построена серия с таким именем. (Если сшивали по времени.) Тогда, в имя номер повторно добавленной серии. Или не меняем KKS.
            bool isNameIn=false; // ПРисутствует ли добавляемое имя линии
            foreach (var Seria_i in myChart.Series)
            {
              if (Seria_i.Name==selectedKKS_i.KKS)
              {
                isNameIn = true;
              }
            }
            if (isNameIn)
            {
              slisesCounter++;
              serName = selectedKKS_i.KKS + "_" + slisesCounter;
            }
            else
              serName = selectedKKS_i.KKS;

                    //Добавляем серию на график
            myChart.Series.Add(new Series(serName));
                    //Выбор стороны
            if (r_L == "left")
            {
              myChart.Series[serName].YAxisType = AxisType.Primary;
            }
            else
            {
              myChart.Series[serName].YAxisType = AxisType.Secondary;
            }

                  //Легенда видимая будет.
            myChart.Series[serName].IsVisibleInLegend = true;
           

                  //Тип линии
            myChart.Series[serName].ChartType = SeriesChartType.Line;
            myChart.Series[serName].BorderDashStyle = ChartDashStyle.Solid;
                  //Арена линии соответствующая
            myChart.Series[serName].ChartArea = chartareaName;
                  //Пополнение серии.   
            foreach (Time_and_Value item1 in myFileActs.Find_Parametr(serName).Time_and_Value_List)
            {
              myChart.Series[serName].Points.AddXY(item1.Time, item1.Value);
            }
          }
        }
              /// <summary>
        /// Добавить линию на дополнительной оси. 
        /// Для построения 1 доп. оси создается 2 дополнительные арены: 1-ая содержит график; 2-ая содержит ось.
        /// Насчет положения арены: есть положение самой арены. Есть положение пространства для рисования графиков. Оно внутри арены. Таким образом, надо и то и то задать. Схема, как это работает -- в описании. Кода много, но все просто. Самое сложное -- с положением дополнительных арен, арен для рисования внутри них.
        /// </summary>
        /// <param name="myChart">График, с которым работаем</param>
        /// <param name="chartareaName"> Выбранное пользователем имя главной арены, на которой рисуем. Здесь для привязки к этой арене</param>
        /// <param name="selectedKKSs">Выбранная пользователем группа параметров</param>
        /// <param name="myFileActs">Передает данные</param>
        /// <param name="r_L">Сообщает сторону оси</param>
        public List<ChartArea> AddAdditional_AxisAndlLine(Chart myChart, string chartareaName, System.Collections.IList selectedKKSs, File_Acts myFileActs, string r_L)
        {
                    //Что возвращать будем. Это лист арен с видимым графиком линий строящихся.
          List<ChartArea> ListAreas = new List<ChartArea>();
                          //Цикл по выделенным параметрам пользователем.
          foreach (Parameter selectedParameters_i in selectedKKSs)
          {
                         //1) Создаем незаметную арену для видимой серии на графике. Она точно совпадает с основной ареной. В том и смысл, что начало графика будет таким образом совпадать с началом всех других и начинаться от основной оси.
          ChartArea areaSeria = CreateArenaSeria(myChart,chartareaName);
                          //Добавляем на главную арену серию, переносим ее на эту невидимую арену. Так нам проще, ибо метод добавления на глав. арену уже есть.
          List<Parameter> itemAsCollection   =  new List<Parameter>();
          itemAsCollection.Add(selectedParameters_i);
          AddMainLine(myChart, itemAsCollection, chartareaName, myFileActs, r_L);
          myChart.Series[selectedParameters_i.KKS].ChartArea = areaSeria.Name;//сам перенос на невидимую арену.
                    //Определение слева/справа строить
          if (r_L == "left")
          {
            myChart.Series[selectedParameters_i.KKS].YAxisType = AxisType.Primary;
          }
          else
          {
            myChart.Series[selectedParameters_i.KKS].YAxisType = AxisType.Secondary;
          }

                     // 2) Теперь создаем арену для оси. (Кроме оси дополнительной, на ней все остальное незаметное.) 
          ChartArea areaAxis = CreateArenaAxis(myChart,chartareaName);
                      // Создаем копию видимой серии. (Копия невидима.) Служит для масштабирования оси.
          Series seriesCopy = myChart.Series.Add(myChart.Series[selectedParameters_i.KKS].Name + "_invisibleCopy");
          seriesCopy.ChartType = myChart.Series[selectedParameters_i.KKS].ChartType;
                      // Скрываем копию серии
          seriesCopy.IsVisibleInLegend = false;
          seriesCopy.Color = Color.Transparent;
          seriesCopy.BorderColor = Color.Transparent;
                      //ПОполняем серию
          foreach (DataPoint point in myChart.Series[selectedParameters_i.KKS].Points)
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

          //3)ПОзиции арен задаем: 2 новых арены совпадают с начальной позицеей. Задается позиция арены с серией, чтобы не была авто (тогда её нельзя уменьшить, если она ноль. И эта ошибка выскакивает реально)
          areaSeria.Position.FromRectangleF(myChart.ChartAreas[chartareaName].Position.ToRectangleF());
          areaSeria.InnerPlotPosition.FromRectangleF(myChart.ChartAreas[chartareaName].InnerPlotPosition.ToRectangleF());

          areaAxis.Position.FromRectangleF(myChart.ChartAreas[chartareaName].Position.ToRectangleF());
          areaAxis.InnerPlotPosition.FromRectangleF(myChart.ChartAreas[chartareaName].InnerPlotPosition.ToRectangleF());
          
                    // 4) Задаем позицию оси. Смотрится сложно, поскольку предполагается неоднократное добавление дополнительнхы осей.
                    //switch выбирает сторону
          switch (r_L)
          {
            case "left":
                      //Cжимаем все арены, кроме нашей с осью, последней, на axisOffset. И передвигаем их вперед (расстояние между осями создаем).
              for (int i = 0; i < myChart.ChartAreas.Count; i++)
              {
              myChart.ChartAreas[i].Position.Width -= axisOffset;
              myChart.ChartAreas[i].Position.X += axisOffset;
              }           
                            //Последнюю (с осью) сдвинем влево.  
              areaAxis.Position.X -= axisOffset ;
                            //Если уже еще добавлли арены, сдвинуть надо больше
              int countArenasLeftHere=CountArenas(myChart,chartareaName).Item1;
              areaAxis.Position.X -= (axisOffset)*countArenasLeftHere;//отодвинуть на число построенных дополнительных осей
                          //Имена зададим: 10 символов 1 часть. + 7 + 2 + 1 + 1. Итого, 17-ый означает номер главной арены. (Рядом с которой рисуем.)
              areaSeria.Name = "invisible_" + chartareaName + "_#" + (countArenasLeftHere + 1) + "L";
              areaAxis.Name = "axisYin___" + chartareaName + "_#" + (countArenasLeftHere + 1) + "L";
              
            break;

                      //Сдвигаем в другую сторону
            case "right":
                      
                       //Cжимаем все арены, кроме нашей с осью, последней, на axisOffset. А позицию -- без изменений.
            for (int i = 0; i < myChart.ChartAreas.Count; i++)
            {
              myChart.ChartAreas[i].Position.Width -= axisOffset;
            }
                        //Последнюю (с осью) сдвинем вправо.
            areaAxis.Position.X +=axisOffset;
            areaAxis.Position.X += (axisOffset) * CountArenas(myChart,chartareaName).Item2;//отодвинуть, учитывая число уже построенных дополнительных осей
                       //Имена зададим: 10 символов 1 часть. + 7 + 2 + 1 + 1. Итого, 17-ый означает номер главной арены. (Рядом с которой рисуем.)
            areaSeria.Name = "invisible_" + chartareaName + "_#" + (CountArenas(myChart, chartareaName).Item2 + 1) + "R";
            areaAxis.Name = "axisYin___" + chartareaName + "_#" + (CountArenas(myChart, chartareaName).Item2 + 1) + "R";
            break;
          }//Конец switch
          ListAreas.Add(areaSeria);
         }//Закрыли обход по списку выделенных KKS.
          return ListAreas;
       }//Закрыли тело метода

                /// <summary>
                /// Создает арену для отображения графика параметра. Виден только график и ничего больше на этой арене.
                /// </summary>
                /// <param name="myChart">График, с которым работаем</param>
                /// <param name="chartareaName">Выбранное пользователем имя главной арены, на которой рисуем. Здесь для привязки к этой арене</param>
                /// <returns></returns>
        private ChartArea CreateArenaSeria(Chart myChart, string chartareaName)
      {
        ChartArea areaSeria = myChart.ChartAreas.Add("invisible_");
        areaSeria.BackColor = Color.Transparent;
        areaSeria.BorderColor = Color.Transparent;
        //X
        areaSeria.AxisX.LineWidth = 0;
        areaSeria.AxisX.MajorGrid.Enabled = false;
        areaSeria.AxisX.MajorTickMark.Enabled = false;
        areaSeria.AxisX.LabelStyle.Enabled = false;
        //Y
        areaSeria.AxisY.LineWidth = 0;
        areaSeria.AxisY.MajorGrid.Enabled = false;
        areaSeria.AxisY.MajorTickMark.Enabled = false;
        areaSeria.AxisY.LabelStyle.Enabled = false;
        areaSeria.AxisY.IsStartedFromZero = myChart.ChartAreas[chartareaName].AxisY.IsStartedFromZero;
        //Y2
        areaSeria.AxisY2.LineWidth = 0;
        areaSeria.AxisY2.MajorGrid.Enabled = false;
        areaSeria.AxisY2.MajorTickMark.Enabled = false;
        areaSeria.AxisY2.LabelStyle.Enabled = false;
        areaSeria.AxisY2.IsStartedFromZero = myChart.ChartAreas[chartareaName].AxisY.IsStartedFromZero;
        //Масштабирование
        //Добавление полосы прокрутки.
        areaSeria.AxisX.ScrollBar.Enabled = true;
        areaSeria.AxisY.ScrollBar.Enabled = true;
        areaSeria.AxisY2.ScrollBar.Enabled = true;
        //Разрешение увеличивать промежуток выделения
        areaSeria.AxisX.ScaleView.Zoomable = true;
        areaSeria.AxisY.ScaleView.Zoomable = true;
        areaSeria.AxisY2.ScaleView.Zoomable = true;
        //Разрешение пользоваться курсором
        areaSeria.CursorX.IsUserEnabled = true;
        areaSeria.CursorX.IsUserSelectionEnabled = true;
        areaSeria.CursorY.IsUserEnabled = true;
        areaSeria.CursorY.IsUserSelectionEnabled = true;
        //Выбор интервала для курсора. Ноль даст возможность выделять прямоугольные области нормально. Иначе коряво работает
        areaSeria.CursorX.Interval = 0;
        areaSeria.CursorY.Interval = 0;
        return areaSeria;
      }

                /// <summary>
        /// Создает арену для отображения оси параметра. Видна только ось и ничего больше на этой арене.
      /// </summary>
      /// <param name="myChart"></param>
      /// <param name="chartareaName"></param>
      /// <returns></returns>
        private ChartArea CreateArenaAxis(Chart myChart, string chartareaName)
      {
        ChartArea areaAxis = myChart.ChartAreas.Add("axisArena_");
        areaAxis.BackColor = Color.Transparent;
        areaAxis.BorderColor = Color.Transparent;
        // X
        areaAxis.AxisX.LineWidth = 0;
        areaAxis.AxisX.MajorGrid.Enabled = false;
        areaAxis.AxisX.MajorTickMark.Enabled = false;
        areaAxis.AxisX.LabelStyle.Enabled = false;
        //Y
        areaAxis.AxisY.MajorGrid.Enabled = false;
        areaAxis.AxisY.IsStartedFromZero = myChart.ChartAreas[chartareaName].AxisY.IsStartedFromZero;
        //Y2
        areaAxis.AxisY2.MajorGrid.Enabled = false;
        areaAxis.AxisY2.IsStartedFromZero = myChart.ChartAreas[chartareaName].AxisY.IsStartedFromZero;
        //Масштабирование
        //Добавление полосы прокрутки.
        areaAxis.AxisX.ScrollBar.Enabled = true;
        areaAxis.AxisY.ScrollBar.Enabled = true;
        areaAxis.AxisY2.ScrollBar.Enabled = true;
        //Разрешение увеличивать промежуток выделения
        areaAxis.AxisX.ScaleView.Zoomable = true;
        areaAxis.AxisY.ScaleView.Zoomable = true;
        areaAxis.AxisY2.ScaleView.Zoomable = true;
        //Разрешение пользоваться курсором
        areaAxis.CursorX.IsUserEnabled = true;
        areaAxis.CursorX.IsUserSelectionEnabled = true;
        areaAxis.CursorY.IsUserEnabled = true;
        areaAxis.CursorY.IsUserSelectionEnabled = true;
        //Выбор интервала для курсора. Ноль даст возможность выделять прямоугольные области нормально. Иначе коряво работает
        areaAxis.CursorX.Interval = 0;
        areaAxis.CursorY.Interval = 0;
        return areaAxis;
      }
              /// <summary>
        /// Добавить арену снизу.
        /// </summary>
        /// <param name="myChart"></param>
        public void Add_Area_Bottom(Chart myChart)
        {
                      //Добавляем арену.
          LoadFirstArena(myChart);
                      //Выравнить по ширине добавляемую с основной перой. Выравнивание работает, но только внешне. ПОзиции неправильные
                      //myChart.ChartAreas[myChart.ChartAreas.Count()-1].AlignWithChartArea = "Area" + "# " + 1;//1 площадка рисования
                      //myChart.ChartAreas[myChart.ChartAreas.Count() - 1].AlignmentOrientation = AreaAlignmentOrientations.Vertical;
          myChart.ChartAreas[myChart.ChartAreas.Count() - 1].Position.FromRectangleF(myChart.ChartAreas["Area" + "# " + 1].Position.ToRectangleF());
          myChart.ChartAreas[myChart.ChartAreas.Count() - 1].InnerPlotPosition.FromRectangleF(myChart.ChartAreas["Area" + "# " + 1].InnerPlotPosition.ToRectangleF());
                        //Новый размер окон по высоте.
          float New_size_H = (mainArenaCounter - 1) * myChart.ChartAreas[0].Position.Height / mainArenaCounter;
                        // УДалить отметка оси "Х" у предыдущей главной арены 
          myChart.ChartAreas["Area" + "# " + (mainArenaCounter-1)].AxisX.LabelStyle.Enabled = false;
          myChart.ChartAreas["Area" + "# " + (mainArenaCounter - 1)].AxisX.MajorTickMark.Enabled = false;
                        //Задать положение по высоте, высоту 
          for (int i = 0; i < myChart.ChartAreas.Count; i++)
          {
            myChart.ChartAreas[i].Position.Height = New_size_H-1;
                                      //Положение по высоте
                          //1) Для дополнительных осей/ У них 16 символ--номер арены, к которой они прилегают.
            if (myChart.ChartAreas[i].Name.Count() > 10)
            { myChart.ChartAreas[i].Position.Y = (Convert.ToInt32(myChart.ChartAreas[i].Name[16].ToString()) - 1) * (New_size_H) + 3; }
                          //2) Для основных/ У них 6 символ--номер арены.
            if (myChart.ChartAreas[i].Name.Count() < 10)
            {myChart.ChartAreas[i].Position.Y = (Convert.ToInt32(myChart.ChartAreas[i].Name[6].ToString())-1) * (New_size_H) + 3;}
                                   
          }
                                  
                        
                      //Аннотации перенести и заголовки
          for (int i = 0; i < myChart.Annotations.Count; i++)
          {
            myChart.Annotations[i].Y = (mainArenaCounter - 1) * (myChart.Annotations[i].Y) / mainArenaCounter;
            myChart.Annotations[i].Width = myChart.Annotations[i].Width * (mainArenaCounter - 1) / mainArenaCounter;
            myChart.Annotations[i].Height = myChart.Annotations[i].Height * (mainArenaCounter - 1) / mainArenaCounter;

          }
          for (int i = 0; i < myChart.Titles.Count; i++)
          {
            myChart.Titles[i].Position.Y = (mainArenaCounter - 1) * (myChart.Titles[i].Position.Y) / mainArenaCounter;
           // myChart.Titles[i].Font = new System.Drawing.Font("Times New Roman", 9, System.Drawing.FontStyle.Regular);
          }
        }//Тело метода

                /// <summary>
        /// Подсчет арен дополнительных справа(item2) и слева(item1) привязанных к данной главной арене на данном chart
        /// </summary>
        /// <param name="myChart">Передаваемый график</param>
        /// <param name="mainArenaName">Передаваемая главная арена, связи к которой ищем</param>
        /// <returns></returns>
        private Tuple<int,int> CountArenas(Chart myChart, string mainArenaName)
        {
        //Количество справ и слева, ибо кортеж изменять нельзя, приходится сразу его задавать.
        int countL=0,countR=0;
        //Подсчет арен дополнительных справа и слева привязанных к данной главной арене
        foreach (ChartArea area_i in myChart.ChartAreas)
	        {
         //Определим, что в названии арены -- имя главной арены, которое передали в метод
		        if (area_i.Name.Contains(mainArenaName) )
	            {
             //Определим, слева или справа дополнительная ось. Сохраним количество справа и слева осей.
              if (area_i.Name.Contains("L") && area_i.Name.Contains("axisYin___"))
	                {
		                countL++;
	                }
		             if (area_i.Name.Contains("R") && area_i.Name.Contains("axisYin___"))
	                {
		                countR++;
	                }
	            }
	        }//Обход по элементам myChart.ChartAreas
         return Tuple.Create(countL, countR);
        }
        /// <summary>
        /// Добавление графика на уже имеющуюся ось.
        /// </summary>
        /// <param name="myChart">Передаваймый график</param>
        /// <param name="selectedKKSs">Выделеннные параметры для построения </param>
        /// <param name="myFileActs">Передаваемые данные по файлу</param>
        /// <param name="arenaName">Имя , на какую арену добавить график</param>
        public void AddLine_OnAdditionalAxis(Chart myChart, System.Collections.IList selectedKKSs, File_Acts myFileActs, string arenaName)
        {
                  //Сейчас обрабатываемый KKS из списка переданного.
          string serName = "";
                  //Цикл по выдленным параметрам.
          foreach (Parameter selectedParametr_i in selectedKKSs)
          {
            //Была ли построена серия с таким именем. Тогда, в имя номер повторно добавленной серии.
            bool isNameIn = false; // ПРисутствует ли добавляемое имя линии
            foreach (var Seria_i in myChart.Series)
            {
              if (Seria_i.Name == selectedParametr_i.KKS)
              {
                isNameIn = true;
              }
            }
            if (isNameIn)
            {
              slisesCounter++;
              serName = selectedParametr_i.KKS + "_" + slisesCounter;
            }
            else
              serName = selectedParametr_i.KKS;

                           //Добавляем серию на график
            myChart.Series.Add(new Series(serName));
            //Выбор стороны
            if (arenaName.Contains("L") )
            {
              myChart.Series[serName].YAxisType = AxisType.Primary;
            }
            else
            {
              myChart.Series[serName].YAxisType = AxisType.Secondary;
            }
                          //Легенда видимая будет.
            myChart.Series[serName].IsVisibleInLegend = true;
                            //Тип линии
            myChart.Series[serName].ChartType = SeriesChartType.Line;
            myChart.Series[serName].BorderDashStyle = ChartDashStyle.Solid;
                  //Арена линии соответствующая
            myChart.Series[serName].ChartArea = arenaName;
                    //Пополнение серии.   
            foreach (Time_and_Value item1 in myFileActs.Find_Parametr(serName).Time_and_Value_List)
            {
              myChart.Series[serName].Points.AddXY(item1.Time, item1.Value);
            }
          }
        }



     }//Класс
}//Пространство имен
