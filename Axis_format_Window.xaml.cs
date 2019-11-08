using System;
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
    /// Логика взаимодействия для Axis_format_Window.xaml
    /// </summary>
    public partial class Axis_format_Window : Window
    {
        /// <summary>
        /// Переедавайемая ось
        /// </summary>
        Axis Axis_ref;
        /// <summary>
        /// Передаваемое имя арены
        /// </summary>
        string chartAreaName_ref;
        /// <summary>
        /// Передаваемый чарт
        /// </summary>
        Chart chart1_ref;
        public Axis_format_Window(Axis axis, Chart chart1, string arenaName)
        {
            InitializeComponent();
            //Ссылаемся на ось
            Axis_ref = axis;
            //ссылаемся на название арены
            chartAreaName_ref = arenaName;
            //ссылаемся на график
            chart1_ref = chart1;

            //Название серии текущее. А изменения нет и не надо.
            Axis_name.Text = Axis_ref.Name;

            //Заполнение стилей для делений ДЛЯ ОСНОВНОЙ
            foreach (string lineName in Enum.GetNames(typeof(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle)))
            {
              this.ComboBoxStyleMain.Items.Add(lineName);
              //this.MajorLineDashStyle.Items.Add(lineName);
            }
            //Заполнение стилей для делений ДЛЯ ВТОРОСТЕПЕННОЙ
            foreach (string lineName in Enum.GetNames(typeof(System.Windows.Forms.DataVisualization.Charting.ChartDashStyle)))
            {
              this.ComboBoxStyle_Minor.Items.Add(lineName);
              //this.MajorLineDashStyle.Items.Add(lineName);
            }
            //Выбор, что менять: линии или деления
            TickorGridMarks.Items.Add("GridMarks");
            TickorGridMarks.Items.Add("TickMarks");
            TickorGridMarks_Minor.Items.Add("GridMarks");
            TickorGridMarks_Minor.Items.Add("TickMarks");
            
          //Заполнение вариатов вида рисок
          foreach (string lineName in Enum.GetNames(typeof(System.Windows.Forms.DataVisualization.Charting.TickMarkStyle)))
            {
              this.ComboBoxView.Items.Add(lineName);
              //this.MajorLineDashStyle.Items.Add(lineName);
            }
          //Для второстепенной оси
          foreach (string lineName in Enum.GetNames(typeof(System.Windows.Forms.DataVisualization.Charting.TickMarkStyle)))
          {
            this.ComboBoxView_Minor.Items.Add(lineName);
            //this.MajorLineDashStyle.Items.Add(lineName);
          }

          ////Отобразить или стили линий сетки или стили рисок
          //  if (TickorGridMarks.SelectedItem == "GridMarks")
          //  {
          //    //Интервал текущий отобразить
          //    TextBoxInterval.Text = Axis_ref.MajorGrid.Interval.ToString();
          //    //Ширина текущая
          //    TextBoxWidth.Text = Axis_ref.MajorGrid.LineWidth.ToString();
          //    //Цвет текущий
          //    ColorBoxMainTick.SelectedColor = System.Windows.Media.Color.FromArgb(Axis_ref.MajorGrid.LineColor.A, Axis_ref.MajorGrid.LineColor.R, Axis_ref.MajorGrid.LineColor.G, Axis_ref.MajorGrid.LineColor.B);
          //    //Стиль текущий
          //    ComboBoxStyleMain.SelectedItem = Axis_ref.MajorGrid.LineDashStyle;
          //  }
          //  else
          //  {
          //    //Интервал текущий отобразить
          //    TextBoxInterval.Text = Axis_ref.MajorTickMark.Interval.ToString();
          //    //Ширина текущая
          //    TextBoxWidth.Text = Axis_ref.MajorTickMark.LineWidth.ToString();
          //    //Цвет текущий
          //    ColorBoxMainTick.SelectedColor = System.Windows.Media.Color.FromArgb(Axis_ref.MajorTickMark.LineColor.A, Axis_ref.MajorTickMark.LineColor.R, Axis_ref.MajorTickMark.LineColor.G, Axis_ref.MajorTickMark.LineColor.B);
          //    //Стиль текущий
          //    ComboBoxStyleMain.SelectedItem = Axis_ref.MajorTickMark.LineDashStyle;
            
          //  }


            
            //-----------------------------------------------------------МИНИМУМ
            //Изменение
            TextBoxMin.TextChanged += TextBoxMin_TextChanged;
            
            //-----------------------------------------------------------МАКСИМУМ
            if (Axis_ref.Name.Contains("X"))
            {
              ////Избежать неправльного начала оси времени
              //DateTime time1 = DateTime.FromOADate(Axis_ref.Maximum);
              //DateTime rounded1 = time1.AddMilliseconds(-time1.Millisecond).AddSeconds(-time1.Second).AddMinutes(-time1.Minute);
              //TextBoxMax.Text = rounded1.ToString();
              ////chart1_ref.Invalidate();
              TextBoxMax.Text = DateTime.FromOADate(Axis_ref.Maximum).ToString();
              TextBoxMin.Text = DateTime.FromOADate(Axis_ref.Minimum).ToString();
            }
            if (Axis_ref.Name.Contains("Y"))
            {
              TextBoxMax.Text = Axis_ref.Maximum.ToString();
              TextBoxMin.Text = Axis_ref.Minimum.ToString();
            }
            //Изменение
            TextBoxMax.TextChanged += TextBoxMax_TextChanged;

            //-----------------------------------------------------------ЦВЕТ
            ColorBox.SelectedColor = System.Windows.Media.Color.FromArgb(Axis_ref.LineColor.A, Axis_ref.LineColor.R, Axis_ref.LineColor.G, Axis_ref.LineColor.B);
            //Обработчик изменения цвета
            ColorBox.SelectedColorChanged += ColorBox_SelectedColorChanged;

            //-----------------------------------------------------------ШРИФТ
            FontSizeTextBox.Text = Axis_ref.LabelStyle.Font.Size.ToString();
            //ОБработчик
            FontSizeTextBox.TextChanged += FontSizeTextBox_TextChanged;

            //-----------------------------------------------------------ФОРМАТ
            //Текущий
            ValueFormatTextBox.Text = Axis_ref.LabelStyle.Format;
            //Задаваемый
            ValueFormatTextBox.TextChanged += ValueFormatTextBox_TextChanged;
            
            //-----------------------------------------------------------ИНТЕРВАЛ
            //Для оси "У"
            if (Axis_ref.Name.Contains("Y"))// && MarksCountTextBox.Text == "5")
            {
                //Как обычно записываем интервал
              if (Axis_ref.Interval!=0)
              {
                MarksCountTextBox.Text = ((Convert.ToDouble(TextBoxMax.Text) - Convert.ToDouble(TextBoxMin.Text))
                    / Axis_ref.Interval).ToString();
              }
                  //Не как обычно , для выравнивания осей сходу.
              if (MarksCountTextBox.Text == "5")//В остальных случаях в обработчике изменится.
              {
                Axis_ref.Interval = (Convert.ToDouble(TextBoxMax.Text) - Convert.ToDouble(TextBoxMin.Text))
                    / Convert.ToDouble(MarksCountTextBox.Text);
              }
            }

            //Для оси "Х"
            if (Axis_ref.Name.Contains("X")  )
            {
              //Основной
              if (Axis_ref.Interval != 0)
              {
                MarksCountTextBox.Text = ((DateTime.Parse(TextBoxMax.Text).Subtract(DateTime.Parse(TextBoxMin.Text)).TotalMilliseconds)
                     / Axis_ref.Interval).ToString();
              }
            }
            //ОБработчик
            MarksCountTextBox.TextChanged += MarksCountTextBox_TextChanged;

            //-----------------------------------------------------------ОФСЕТ
            OfsetTextBox.TextChanged += OfsetTextBox_TextChanged;

            //-----------------------------------------------------------ШИРИНА
            //Текущий
            WideTextBox.Text = chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] ].BorderWidth.ToString();
            //Задаваемый
            WideTextBox.TextChanged += WideTextBox_Textchanged;
        }

        /// <summary>
        /// Измение офсета оси
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OfsetTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Основная ось
                if (Axis_ref.Name == "Y (Value) axis" && Convert.ToDouble(OfsetTextBox.Text) < 20 && 0 <= Convert.ToDouble(OfsetTextBox.Text))
                    chart1_ref.ChartAreas["Area" + "# " + 1].InnerPlotPosition.X = (float) Convert.ToDouble(OfsetTextBox.Text);
                if (Axis_ref.Name == "Secondary Y (Value) axis" && Convert.ToDouble(OfsetTextBox.Text) < 20 && 0 <= Convert.ToDouble(OfsetTextBox.Text))
                {
                    chart1_ref.ChartAreas["Area" + "# " + 1].InnerPlotPosition.X = (float)Convert.ToDouble(OfsetTextBox.Text);
                }
                if (Axis_ref.Name == "Y1b" && Convert.ToDouble(OfsetTextBox.Text) < 20 && 0 <= Convert.ToDouble(OfsetTextBox.Text))
                {
                     chart1_ref.ChartAreas["Area" + "# " + "11b"].InnerPlotPosition.X = (float)Convert.ToDouble(OfsetTextBox.Text);
                }
                if (Axis_ref.Name == "Y2b" && Convert.ToDouble(OfsetTextBox.Text) < 20 && 0 <= Convert.ToDouble(OfsetTextBox.Text))
                {
                    chart1_ref.ChartAreas["Area" + "# " + "12b"].InnerPlotPosition.X = (float)Convert.ToDouble(OfsetTextBox.Text);
                }
            }
            catch (FormatException) { }
        }
        /// <summary>
        /// Изменение количества интервалов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MarksCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                    //ОСЬ "У"
                if (Convert.ToDouble(MarksCountTextBox.Text)>0 && Axis_ref.Name.Contains("Y"))
                    Axis_ref.Interval = (Convert.ToDouble(TextBoxMax.Text) - Convert.ToDouble(TextBoxMin.Text))
                    / Convert.ToDouble(MarksCountTextBox.Text);
                    //ОСЬ "Х"
                if (Convert.ToDouble(MarksCountTextBox.Text)>0 && Axis_ref.Name.Contains("X"))
                {
                    //Axis_ref.IntervalType = DateTimeIntervalType.Minutes; УЖЕ СДЕЛАНО!
                    Axis_ref.Interval= DateTime.Parse(TextBoxMax.Text).Subtract(DateTime.Parse(TextBoxMin.Text)).TotalMilliseconds
                         / Convert.ToInt32(MarksCountTextBox.Text);
                    Axis_ref.IntervalType = DateTimeIntervalType.Milliseconds; //это можно и в начало построения этой оси перенести
                    Axis_ref.LabelStyle.IntervalType = DateTimeIntervalType.Milliseconds;
                    
                    //Если мы строили друг под другом -- выровняем их.
                    foreach (ChartArea item in chart1_ref.ChartAreas)
                    {
                      item.AxisX.Interval = DateTime.Parse(TextBoxMax.Text).Subtract(DateTime.Parse(TextBoxMin.Text)).TotalMilliseconds
                         / Convert.ToInt32(MarksCountTextBox.Text);
                      item.AxisX.IntervalType = DateTimeIntervalType.Milliseconds; //это можно и в начало построения этой оси перенести
                      item.AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Milliseconds;
                    }
                }
            }
            catch (FormatException) { }
        }

        void ValueFormatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Axis_ref.LabelStyle.Format = ValueFormatTextBox.Text;
                
            }
            catch (FormatException) { }
            //Axis_ref.LabelStyle.Format = "0E+0";

            
            
        }

        void FontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if ловит выход за границы. Try ловит ошибку формата.
            try
            {
                if (Convert.ToInt32(FontSizeTextBox.Text) > 0 && Convert.ToInt32(FontSizeTextBox.Text) < 21)
                {
                    Axis_ref.LabelStyle.Font = new System.Drawing.Font("Arial", Convert.ToInt32(FontSizeTextBox.Text));
                }
            }
            catch (FormatException) { }
        }

        void ColorBox_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Axis_ref.LineColor = System.Drawing.Color.FromArgb(ColorBox.SelectedColor.Value.A, ColorBox.SelectedColor.Value.R, ColorBox.SelectedColor.Value.G, ColorBox.SelectedColor.Value.B);
            //Major -- главные, горизонтальные метки. Minor -- вротостепенные, вертикальные метки.
            Axis_ref.MajorTickMark.LineColor = System.Drawing.Color.FromArgb(ColorBox.SelectedColor.Value.A, ColorBox.SelectedColor.Value.R, ColorBox.SelectedColor.Value.G, ColorBox.SelectedColor.Value.B);
        }
        /// <summary>
        /// Изменение минимума
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBoxMin_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Переход на ось
            if (Axis_ref.Name.Contains("X"))
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (DateTime.Parse(TextBoxMin.Text).ToOADate() < Axis_ref.Maximum )
                    {
                        Axis_ref.Minimum = DateTime.Parse(TextBoxMin.Text).ToOADate();
                    }
                }
                catch (FormatException) { }
            }
            //Переход на ось
            if (Axis_ref.Name.Contains("Y"))
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (Convert.ToDouble(TextBoxMin.Text) < Axis_ref.Maximum)
                    {
                    //Это идиотскую ошибку осправляет с перечеркиванием графика при измененении мак мин, отчего-то
                    Axis_ref.Maximum+=10;
                    Axis_ref.Maximum -= 10;

                      Axis_ref.Minimum = Convert.ToDouble(TextBoxMin.Text);
                    }
                }
                catch (FormatException) { }
        }

            //Для изменения при НАЛОЖЕНИИ арен. Слева ось
            if (Axis_ref.Name.Contains("Y") & Axis_ref.Name.Contains("1b"))
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (Convert.ToDouble(TextBoxMin.Text) < Axis_ref.Maximum)
                    {
                        chart1_ref.ChartAreas["Area#" + " "+ chartAreaName_ref[6] + "1a"].AxisY.Minimum
                            = Convert.ToDouble(TextBoxMin.Text);
                    }
                }
                catch (FormatException) { }
            }

            //Для изменения при наложении арен. ОСЬ СПРАВА
            if (Axis_ref.Name.Contains("Y") & Axis_ref.Name.Contains("2b"))
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (Convert.ToDouble(TextBoxMin.Text) < Axis_ref.Maximum)
                    {
                        chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "2a"].AxisY2.Minimum
                            = Convert.ToDouble(TextBoxMin.Text);
                    }
                }
                catch (FormatException) { }
            }
            //Для изменения при наложении арен. Изменит и масштаб оси ВРЕМЕНИ "а", на которой график рисуетя! Ось ВРЕМЕНИ
            if (Axis_ref.Name.Contains("X"))
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (DateTime.Parse(TextBoxMin.Text).ToOADate() < Axis_ref.Maximum)
                    {
                        //Тут трай выполняет функция проверки, есть ли арены
                        try
                        {
                            chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "1a"].AxisX.Minimum
                                                        = DateTime.Parse(TextBoxMin.Text).ToOADate();
                            chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "2a"].AxisX.Minimum
                                                        = DateTime.Parse(TextBoxMin.Text).ToOADate();
                        }
                        catch (Exception) { }
                    }
                }
                catch (FormatException) { }
            }

            //Выставляет интервалы, принуждая максимум и минимум отрисовываться на оси
            MarksCountTextBox_TextChanged(sender, e);
        }
        /// <summary>
        /// Изменение максимума
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBoxMax_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Переход на ось
            if (Axis_ref.Name.Contains("X"))
            { 
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (DateTime.Parse(TextBoxMax.Text).ToOADate() > Axis_ref.Minimum)
                    {
                        Axis_ref.Maximum = DateTime.Parse(TextBoxMax.Text).ToOADate();
                    }
                }
                catch (FormatException) { }
            }
            //Переход на ось
            if (Axis_ref.Name.Contains("Y"))
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (Convert.ToDouble(TextBoxMax.Text) > Axis_ref.Minimum)
                    {
                      //chart1_ref.Invalidate();
                      Axis_ref.Maximum = Convert.ToDouble(TextBoxMax.Text);
                      //chart1_ref.Invalidate();
                    }
                }
                catch (FormatException) { }

                //Выставляет интервалы, принуждая максимум и минимум отрисовываться на оси
                MarksCountTextBox_TextChanged(sender, e);
            }



            //Для изменения при НАЛОЖЕНИИ арен. Изменит и масштаб оси "а", на которой график рисуетя! Ось слева
            if (Axis_ref.Name.Contains("Y") && Axis_ref.Name.Contains("1b"))
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (Convert.ToDouble(TextBoxMax.Text) > Axis_ref.Minimum)
                    {
                        chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "1a"].AxisY.Maximum
                            = Convert.ToDouble(TextBoxMax.Text);
                    }
                }
                catch (FormatException) { }
            }


            //Для изменения при наложении арен. Изменит и масштаб оси "а", на которой график рисуетя! ОСЬ СПРАВА
            if (Axis_ref.Name.Contains("Y") && Axis_ref.Name.Contains("2b"))
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (Convert.ToDouble(TextBoxMax.Text) > Axis_ref.Minimum)
                    {
                        chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "2a"].AxisY2.Maximum
                            = Convert.ToDouble(TextBoxMax.Text);
                    }
                }
                catch (FormatException) { }
            }

            //Для изменения при наложении арен. Изменит и масштаб оси ВРЕМЕНИ "а", на которой график рисуетя! Ось ВРЕМЕНИ
            if (Axis_ref.Name.Contains("X") )
            {
                //if ловит выход за границы. Try ловит ошибку формата.
                try
                {
                    if (DateTime.Parse(TextBoxMax.Text).ToOADate() > Axis_ref.Minimum)
                    {
                        try
                        {
                            chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "1a"].AxisX.Maximum
                                                        = DateTime.Parse(TextBoxMax.Text).ToOADate();
                            chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "2a"].AxisX.Maximum
                            = DateTime.Parse(TextBoxMax.Text).ToOADate();
                        }
                        catch (Exception) { }
                        
                        
                    }
                }
                catch (FormatException) { }
            }
           
        }
        /// <summary>
        /// Изменение ширины
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void WideTextBox_Textchanged(object sender, TextChangedEventArgs e)
        {
            //if ловит выход за границы. Try ловит ошибку формата.
            try
                    {
                if (Convert.ToInt32(WideTextBox.Text) > 0 )
                {
                    
                        chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6]].Position.Width = Convert.ToInt32(WideTextBox.Text);
                        chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "1a"].Position.Width = Convert.ToInt32(WideTextBox.Text);
                        chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "1b"].Position.Width = Convert.ToInt32(WideTextBox.Text);
                        chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "2a"].Position.Width = Convert.ToInt32(WideTextBox.Text);
                        chart1_ref.ChartAreas["Area#" + " " + chartAreaName_ref[6] + "2b"].Position.Width = Convert.ToInt32(WideTextBox.Text);
                }
                    }
            catch (Exception) { }
        }

      //Основные деления. Формат
      //Изменение интервала
        private void TextBoxInterval_TextChanged(object sender, TextChangedEventArgs e)
        {
          //Интревал времени всегда глючит, если не задать, какой именно выираем.
          if (Axis_ref.Name.Contains("X"))
          {
            Axis_ref.MajorGrid.IntervalType = DateTimeIntervalType.Seconds;
            Axis_ref.MajorTickMark.IntervalType = DateTimeIntervalType.Seconds;
          }

          try
          {
            if (Convert.ToDouble(TextBoxInterval.Text) < 0)
            {
              return;
            }
            

            if (TickorGridMarks.SelectedItem == "GridMarks" )
          {
            Axis_ref.MajorGrid.Interval = 0;
            Axis_ref.MajorGrid.Interval = Convert.ToDouble(TextBoxInterval.Text);
            
          }
            if (TickorGridMarks.SelectedItem == "TickMarks" )
          {
            Axis_ref.MajorTickMark.Interval = 0;
            Axis_ref.MajorTickMark.Interval = Convert.ToDouble(TextBoxInterval.Text);
            
          }
            }
          catch (FormatException)
          {}

          //Если друг под другом арены
          if (chart1_ref.ChartAreas.Count>1)
          {
            foreach (ChartArea item in chart1_ref.ChartAreas)
            {
              try
              {
                if (Convert.ToDouble(TextBoxInterval.Text) > 0)
                {
                  item.AxisX.MajorGrid.Interval = Convert.ToDouble(TextBoxInterval.Text);
                  item.AxisX.MajorTickMark.Interval = Convert.ToDouble(TextBoxInterval.Text); 
                }
                
              }
              catch (FormatException)
              {}
            }
          }
        }

      //Изменение ширины
        private void TextBoxWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
          try
          {
            if (TickorGridMarks.SelectedItem == "GridMarks")
            {
              Axis_ref.MajorGrid.LineWidth = Convert.ToInt32(TextBoxWidth.Text);
            }
            else
            {
              Axis_ref.MajorTickMark.LineWidth = int.Parse(TextBoxWidth.Text);
            }
          }
          catch (Exception)
          { }

          //Если друг под другом арены
          if (chart1_ref.ChartAreas.Count > 1)
          {
            foreach (ChartArea item in chart1_ref.ChartAreas)
            {
              try
              {
                item.AxisX.MajorGrid.LineWidth = Convert.ToInt32(TextBoxWidth.Text);
                item.AxisX.MajorTickMark.LineWidth = int.Parse(TextBoxWidth.Text);
              }
              catch (Exception)
              { }
            }
          }
        }
      //Цвет
        private void ColorBoxMainTick_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
          //Это приходитс так как херня какая-то с пространством имен колорпикера, там ? в конце
          System.Windows.Media.Color clr = (System.Windows.Media.Color)ColorBoxMainTick.SelectedColor;
          try
          {
            if (TickorGridMarks.SelectedItem == "GridMarks")
            {
              //Стандартна процедура
               Axis_ref.MajorGrid.LineColor = System.Drawing.Color.FromArgb(clr.A,clr.R,clr.G, clr.B);
            }
            else
            {
              //Стандартна процедура
              Axis_ref.MajorTickMark.LineColor = System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
            }
          }
          catch (Exception)
          { }

          //Если друг под другом арены
          if (chart1_ref.ChartAreas.Count > 1)
          {
            foreach (ChartArea item in chart1_ref.ChartAreas)
            {
              try
              {
                item.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
                item.AxisX.MajorTickMark.LineColor = System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
              }
              catch (Exception)
              { }
            }
          }
        }
      //Стиль
        private void ComboBoxStyleMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          try
          {
            if (TickorGridMarks.SelectedItem == "GridMarks")
            {
              Axis_ref.MajorGrid.LineDashStyle = (ChartDashStyle)ChartDashStyle.Parse(typeof(ChartDashStyle), ComboBoxStyleMain.SelectedItem.ToString());
            }
            else
            {
              Axis_ref.MajorTickMark.LineDashStyle = (ChartDashStyle)ChartDashStyle.Parse(typeof(ChartDashStyle), ComboBoxStyleMain.SelectedItem.ToString());
            }
          }
          catch (Exception)
          { }

          //Если друг под другом арены
          if (chart1_ref.ChartAreas.Count > 1)
          {
            foreach (ChartArea item in chart1_ref.ChartAreas)
            {
              try
              {
                item.AxisX.MajorGrid.LineDashStyle = (ChartDashStyle)ChartDashStyle.Parse(typeof(ChartDashStyle), ComboBoxStyleMain.SelectedItem.ToString());
                item.AxisX.MajorTickMark.LineDashStyle = (ChartDashStyle)ChartDashStyle.Parse(typeof(ChartDashStyle), ComboBoxStyleMain.SelectedItem.ToString());
              }
              catch (Exception)
              { }
            }
          }
        }

      //Второстепенные деления
      //Интервал
        private void TextBoxInterval_Minor_TextChanged(object sender, TextChangedEventArgs e)
        {
          //Интревал времени всегда глючит, если не задать, какой именно выираем.
          if (Axis_ref.Name.Contains("X"))
          {
            Axis_ref.MinorGrid.IntervalType = DateTimeIntervalType.Seconds;
            Axis_ref.MinorTickMark.IntervalType = DateTimeIntervalType.Seconds;
          }

          try
          {
            if (Convert.ToDouble(TextBoxInterval_Minor.Text) < 0)
            {
              return;
            }
            Axis_ref.MinorGrid.Interval = 0;
            if (TickorGridMarks_Minor.SelectedItem == "GridMarks")
            {
              //Отображение разрешаем
              Axis_ref.MinorGrid.Enabled = true;
              Axis_ref.MinorTickMark.Enabled = true;

              
              
              //Задаем интервал 
              
              Axis_ref.MinorGrid.Interval = Convert.ToDouble(TextBoxInterval_Minor.Text);
            }
            else
            {
              Axis_ref.MinorTickMark.Interval = Convert.ToDouble(TextBoxInterval_Minor.Text);
            }
          }
          catch (FormatException)
          { }

          //Если друг под другом арены
          if (chart1_ref.ChartAreas.Count > 1)
          {
            foreach (ChartArea item in chart1_ref.ChartAreas)
            {
              try
              {
                //Отображение разрешаем
                item.AxisX.MinorGrid.Enabled = true;
                item.AxisX.MinorTickMark.Enabled = true;
                //Тип интервала задаем
                item.AxisX.MinorGrid.IntervalType = DateTimeIntervalType.Seconds;
                item.AxisX.MinorTickMark.IntervalType = DateTimeIntervalType.Seconds;


                item.AxisX.MinorGrid.Interval = Convert.ToDouble(TextBoxInterval_Minor.Text);
                item.AxisX.MinorTickMark.Interval = Convert.ToDouble(TextBoxInterval_Minor.Text);
              }
              catch (Exception)
              { }
            }
          }
        }

      //Ширина
        private void TextBoxWidth_Minor_TextChanged(object sender, TextChangedEventArgs e)
        {
          try
          {
            if (TickorGridMarks_Minor.SelectedItem == "GridMarks")
            {
              Axis_ref.MinorGrid.LineWidth = Convert.ToInt32(TextBoxWidth_Minor.Text);
            }
            else
            {
              Axis_ref.MinorTickMark.LineWidth = int.Parse(TextBoxWidth_Minor.Text);
            }
          }
          catch (Exception)
          { }

          //Если друг под другом арены
          if (chart1_ref.ChartAreas.Count > 1)
          {
            foreach (ChartArea item in chart1_ref.ChartAreas)
            {
              try
              {
                item.AxisX.MinorGrid.LineWidth = int.Parse(TextBoxWidth_Minor.Text);
                item.AxisX.MinorTickMark.LineWidth = int.Parse(TextBoxWidth_Minor.Text);
              }
              catch (Exception)
              { }
            }
          }

        }
      //Цвет
        private void ColorBox_Minor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
          //Это приходитс так как херня какая-то с пространством имен колорпикера, там ? в конце
          System.Windows.Media.Color clr = (System.Windows.Media.Color)ColorBox_Minor.SelectedColor;

            if (TickorGridMarks_Minor.SelectedItem == "GridMarks")
            {
             //Стандартна процедура
              Axis_ref.MinorGrid.LineColor = System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
            }
            else
            {
              //Стандартна процедура
              Axis_ref.MinorTickMark.LineColor = System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
            }
          

          //Если друг под другом арены
          if (chart1_ref.ChartAreas.Count > 1)
          {
            foreach (ChartArea item in chart1_ref.ChartAreas)
            {
              item.AxisX.MinorGrid.LineColor = System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
              item.AxisX.MinorTickMark.LineColor = System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
            }
          }
        }

        //Стиль
        private void ComboBoxStyleMain_Minor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TickorGridMarks_Minor.SelectedItem == "GridMarks")
            {
             Axis_ref.MinorGrid.LineDashStyle = (ChartDashStyle)ChartDashStyle.Parse(typeof(ChartDashStyle), ComboBoxStyle_Minor.SelectedItem.ToString());
            }
            else
            {
             Axis_ref.MinorTickMark.LineDashStyle = (ChartDashStyle)ChartDashStyle.Parse(typeof(ChartDashStyle), ComboBoxStyle_Minor.SelectedItem.ToString());
            }
          
          //Если друг под другом арены
          if (chart1_ref.ChartAreas.Count > 1)
          {
            foreach (ChartArea item in chart1_ref.ChartAreas)
            {
              item.AxisX.MinorGrid.LineDashStyle = (ChartDashStyle)ChartDashStyle.Parse(typeof(ChartDashStyle), ComboBoxStyle_Minor.SelectedItem.ToString());
              item.AxisX.MinorTickMark.LineDashStyle = (ChartDashStyle)ChartDashStyle.Parse(typeof(ChartDashStyle), ComboBoxStyle_Minor.SelectedItem.ToString());
            }
          }
        }

      //Длина главных рисочек
        private void TextBoxLength_TextChanged(object sender, TextChangedEventArgs e)
        {
          try
          {
            Axis_ref.MajorTickMark.Size = (float)Convert.ToDouble(TextBoxLength.Text);
          }
          catch (Exception)
          {}
         
        }
      //Вид
        private void ComboBoxView_TextChanged(object sender, SelectionChangedEventArgs e)
        {
          Axis_ref.MajorTickMark.TickMarkStyle = (TickMarkStyle)TickMarkStyle.Parse(typeof(TickMarkStyle), ComboBoxView.SelectedItem.ToString());
        }
      //Длина второстепенных
        private void TextBoxLength_Minor_TextChanged(object sender, TextChangedEventArgs e)
        {
          try
          {
            Axis_ref.MinorTickMark.Size = (float)Convert.ToDouble(TextBoxLength_Minor.Text);
          }
          catch (Exception)
          {}
          
        }
      //Вид второстепенных
        private void ComboBoxView_Minor_TextChanged(object sender, SelectionChangedEventArgs e)
        {
          Axis_ref.MinorTickMark.TickMarkStyle = (TickMarkStyle)TickMarkStyle.Parse(typeof(TickMarkStyle), ComboBoxView_Minor.SelectedItem.ToString());
        }


       
          
          
        
    }
}
