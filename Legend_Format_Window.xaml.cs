﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms.DataVisualization.Charting;

namespace GoodPlot
{
    /// <summary>
    /// Логика взаимодействия для Axis_Format_Window.xaml
    /// </summary>
    public partial class Legend_Format_Window : Window
    {
        /// <summary>
        /// Переедавайемая серия
        /// </summary>
        Series Seria_ref;
        /// <summary>
        /// Передавайемый график
        /// </summary>
        Chart chart_ref;
        /// <summary>
        /// Передать легенду
        /// </summary>
        Legend legendHere;
        public Legend_Format_Window(Chart chart1, Series Seria, Legend legend)
        {
            InitializeComponent();
            //Ссылаемся на серию
            Seria_ref = Seria;
            chart_ref = chart1;
            
            //На легенду
            legendHere=legend;

            //Название серии текущее
            KKS_Text.Text = Seria_ref.Name;
            //Обработчик изменения названия
            KKS_Text.TextChanged+=KKS_Text_TextChanged;

            //Название арены
            TextBoxArenaName.Text = legend.Name;

                    //СТИЛЬ серии текущий
            ComBoxLineType.Text = Seria_ref.BorderDashStyle.ToString();
            //Обработчик изменения стиля
            ComBoxLineType.DropDownClosed += ComBoxLineType_DropDownClosed;

                  //ЦВЕТ текущий. Он сформировна системой и каким-то образом автоматический. Но это не принципиально и не так важно .
            ColorBox.SelectedColor = System.Windows.Media.Color.FromArgb(Seria_ref.Color.A, Seria_ref.Color.R, Seria_ref.Color.G, Seria_ref.Color.B);
            //Обработчик изменения цвета
            ColorBox.SelectedColorChanged += ColorBox_SelectedColorChanged;

            //Текущий цвет ГРАНИЦЫ легенды
            ColorBox_legend.SelectedColor = System.Windows.Media.Color.FromArgb(legendHere.BorderColor.A, legendHere.BorderColor.R, legendHere.BorderColor.G, legendHere.BorderColor.B);
            

                  //Текущая ШИРИНА
            LineWidhtTextBox.Text = Seria_ref.BorderWidth.ToString();
            //Обработчик изменения ширины линии
            LineWidhtTextBox.TextChanged += LineWidhtTextBox_TextChanged;

                  //Изменение ТИПА линии
                  //текущий
            switch (Seria_ref.ChartType)
              {
                case SeriesChartType.Column:
                  ComBoxType.Text =  "Колонки";
                  break;
                case SeriesChartType.Line:
                  ComBoxType.Text  = "Линия (соединяет точки по очереди)";
                  break;
                case SeriesChartType.Spline:
                  ComBoxType.Text = "Сглаженная линия (сплайн)";
                  break;
                case SeriesChartType.Point:
                  ComBoxType.Text = "Точки данных";
                  break;
                case SeriesChartType.StepLine:
                  ComBoxType.Text = "Кусочная линия (идет от точки к точке, между ними оставаясь горизогтальной)";
                  break;
                case SeriesChartType.Area:
                  ComBoxType.Text = "Площадь под кривой";
                  break;
                case SeriesChartType.SplineArea:
                  ComBoxType.Text = "Сглаженная площадь";
                  break;

                default:
                  MessageBox.Show("Непредвиденная ошибка в выборе типа линии");
                  break;
              }
	           
                  //обработчик
            ComBoxType.DropDownClosed += ComBoxType_DropDownClosed;

            //Обработчик изменения положения легенды. Размеров
            chart1.CustomizeLegend += chart1_CustomizeLegend;
        }
        /// <summary>
        /// Отображение изменения положения и размеров легенды
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chart1_CustomizeLegend(object sender, CustomizeLegendEventArgs e)
        {
        CoordX.Text=legendHere.Position.X.ToString();
        CoordY.Text = legendHere.Position.Y.ToString();
        WideTB.Text = legendHere.Position.Width.ToString();
        HeightT.Text = legendHere.Position.Height.ToString();
        }

        //Изменение типа линии
        void ComBoxType_DropDownClosed(object sender, EventArgs e)
        {
          switch (ComBoxType.Text)
          {
            case "Колонки":
              Seria_ref.ChartType= SeriesChartType.Column;
              break;
            case "Линия (соединяет точки по очереди)":
              Seria_ref.ChartType = SeriesChartType.Line;
              break;
            case "Сглаженная линия (сплайн)":
              Seria_ref.ChartType = SeriesChartType.Spline;
              break;
            case "Точки данных":
              Seria_ref.ChartType = SeriesChartType.Point;
              break;
            case "Кусочная линия (идет от точки к точке, между ними оставаясь горизогтальной)":
              Seria_ref.ChartType = SeriesChartType.StepLine;
              break;
            case "Площадь под кривой":
              Seria_ref.ChartType = SeriesChartType.Area;
              break;
            case "Сглаженная площадь":
              Seria_ref.ChartType = SeriesChartType.SplineArea;
              break;
            
            default:
              MessageBox.Show("Непредвиденная ошибка в выборе типа линии");
              break;
          }
        }
        /// <summary>
        /// Обработчик изменения ширины линии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LineWidhtTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(LineWidhtTextBox.Text) < 20)
                {
                    Seria_ref.BorderWidth = Convert.ToInt32(LineWidhtTextBox.Text);
                    Seria_ref.MarkerSize = Convert.ToInt32(LineWidhtTextBox.Text);
                }
            }
            catch (FormatException) { }
        }
        /// <summary>
        /// Обрабочик изменения цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ColorBox_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Seria_ref.Color = System.Drawing.Color.FromArgb(ColorBox.SelectedColor.Value.A, ColorBox.SelectedColor.Value.R, ColorBox.SelectedColor.Value.G, ColorBox.SelectedColor.Value.B);
        }
        /// <summary>
        /// Изменение типа линии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComBoxLineType_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                ChartDashStyle Mystyle = (ChartDashStyle) ChartDashStyle.Parse(typeof(ChartDashStyle), ComBoxLineType.Text);
                Seria_ref.BorderDashStyle = Mystyle;
            }
            catch (FormatException)
            {
                MessageBox.Show("Ввод непредвиденного значения");
            }
        }
        /// <summary>
        /// Изменение названия легенды серии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void KKS_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Seria_ref.LegendText=KKS_Text.Text;
        }
        /// <summary>
        /// Удалить эту линию. С легенды, с графика.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_Del_Click(object sender, RoutedEventArgs e)
        {
          //ПРоверяем, дополнительная ли ось. ЛЕВАЯ ось удаляется
          if (Seria_ref.ChartArea.Contains("1a"))
	          {

            //Вычисляем число линий по этой доп. оси
            int Counter = 0;
            foreach (Series item in chart_ref.Series)
	            {
		              if (item.ChartArea.Contains("1a"))
	                {
		                Counter++;
	                }
	            }

            //Действуем: если линий несколько, просто удаляем ее. Если она одна, скроем координатную ось ее.
              if (Counter>1)
              {
                chart_ref.Series.Remove(Seria_ref);
              }
              if (Counter==1)
              {
                chart_ref.Series.Remove(Seria_ref);
                chart_ref.ChartAreas["Area# 11b"].AxisY.Enabled = AxisEnabled.False;
              }
	          }

          //ПРоверяем, дополнительная ли ось. ПРАВАЯ ось удаляется
          if (Seria_ref.ChartArea.Contains("2a"))
          {

            //Вычисляем число линий по этой доп. оси
            int Counter = 0;
            foreach (Series item in chart_ref.Series)
            {
              if (item.ChartArea.Contains("2a"))
              {
                Counter++;
              }
            }

            //Действуем: если линий несколько, просто удаляем ее. Если она одна, скроем координатную ось ее.
            if (Counter > 1)
            {
              chart_ref.Series.Remove(Seria_ref);
            }
            if (Counter == 1)
            {
              chart_ref.Series.Remove(Seria_ref);
              chart_ref.ChartAreas["Area# 12b"].AxisY2.Enabled = AxisEnabled.False;
            }
          }


           //Удаление, когда построено по основной оси
          if (!Seria_ref.ChartArea.Contains("1a") && !Seria_ref.ChartArea.Contains("2a"))
          {
             chart_ref.Series.Remove(Seria_ref);
          } 

          //Закроем окно
            this.Close();
	      }

        private void AllDifferentMarkers_Click(object sender, RoutedEventArgs e)
        {
          Random rnd = new Random();
        for (int i = 0; i < chart_ref.Series.Count; i++)
			      {
			        chart_ref.Series[i].MarkerStyle = (MarkerStyle) (rnd.Next(0,9));   
			      }
         
        }
        /// <summary>
        /// Скрыть легенду
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BUtHideLeg_Click(object sender, RoutedEventArgs e)
        {
          legendHere.Enabled = !legendHere.Enabled;
        }
        /// <summary>
        /// Изменение цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorBox_legend_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
          legendHere.BackColor = System.Drawing.Color.FromArgb(ColorBox_legend.SelectedColor.Value.A, ColorBox_legend.SelectedColor.Value.R, ColorBox_legend.SelectedColor.Value.G, ColorBox_legend.SelectedColor.Value.B);
        }
        /// <summary>
        /// Изменение цвета границы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorBox_legend_board_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
          legendHere.BorderColor = System.Drawing.Color.FromArgb(ColorBox_legend_board.SelectedColor.Value.A, ColorBox_legend_board.SelectedColor.Value.R, ColorBox_legend_board.SelectedColor.Value.G, ColorBox_legend_board.SelectedColor.Value.B);
        }
        /// <summary>
        /// Изменение стиля границы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComBoxType_board_DropDownClosed(object sender, EventArgs e)
        {
          switch (ComBoxType_board.Text)
          {
            case "Линия":
              legendHere.BorderDashStyle = ChartDashStyle.Solid;
              break;
            case "Точка":
              legendHere.BorderDashStyle = ChartDashStyle.Dot;
              break;
            case "Черта":
              legendHere.BorderDashStyle = ChartDashStyle.Dash;
              break;

            default: break;
          }
        }
        //Изменение рзмера границы
        private void ComBType_DropDownClosed(object sender, EventArgs e)
        {
          legendHere.BorderWidth = int.Parse(ComBType.Text);
        }
        /// <summary>
        /// Изменение координаты "Х"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordX_TextChanged(object sender, TextChangedEventArgs e)
        {
        double Rezult=0;
        double.TryParse(CoordX.Text, out Rezult);

          if (0<Rezult && Rezult<100)
          {
            legendHere.Position.X = (float)double.Parse(CoordX.Text);  
          }
          
        }
        /// <summary>
        /// Изменение координаты "Y"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordY_TextChanged(object sender, TextChangedEventArgs e)
        {
          double Rezult=0;
        double.TryParse(CoordX.Text, out Rezult);

        if (0 < Rezult && Rezult < 100)
          {
            legendHere.Position.Y = (float)double.Parse(CoordY.Text); 
          }
        }
        /// <summary>
        /// Изменение ширины легенды
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WideTB_TextChanged(object sender, TextChangedEventArgs e)
        {
          double Rezult = 0;
          double.TryParse(WideTB.Text, out Rezult);

          if (0 < Rezult && Rezult < 100)
          {
            legendHere.Position.Width = (float)double.Parse(WideTB.Text);
          }
        }

        private void HeightT_TextChanged(object sender, TextChangedEventArgs e)
        {
          double Rezult = 0;
          double.TryParse(HeightT.Text, out Rezult);

          if (0 < Rezult && Rezult < 100)
          {
            legendHere.Position.Height = (float)double.Parse(HeightT.Text);
          }
        }

  }
}
