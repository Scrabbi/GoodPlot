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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Forms.DataVisualization.Charting;



namespace GoodPlot
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Счетчик вертикальных подписей. Для размещения друг за другом на графике удобно подписей.
        /// </summary>
        int Title_Count = 0;

        //Перемещеаемые элемент ыграфика
        
        Title MovingTitle = null;
        Legend MovingLegend=null;
        /// <summary>
        /// Акт. экз. SaveLoadClass
        /// </summary>
        SaveLoadClass SLC;// = new SaveLoadClass(chart1);
        /// <summary>
        /// Активный экземпляр класса File_Acts.
        /// </summary>
        File_Acts File_Acts_One = new File_Acts();
        /// <summary>
        /// Активный экземпляр класса Chart_Acts.
        /// </summary>
        Chart_Acts Chart_Acts_One;

        //Задаем контекстное меню на график
        System.Windows.Forms.ContextMenu ContMenuChart = new System.Windows.Forms.ContextMenu();
        //Задаем элементы контекстного меню
        /// <summary>
        /// Выделять по оси Х/общее выделение
        /// </summary>
        System.Windows.Forms.MenuItem allotment_X_menuItem = new System.Windows.Forms.MenuItem();
        /// <summary>
        /// Выделять по оси Y/общее выделение
        /// </summary>
        System.Windows.Forms.MenuItem allotment_Y_menuItem = new System.Windows.Forms.MenuItem();
        /// <summary>
        /// Добавить снизу форму графика
        /// </summary>
        public System.Windows.Forms.MenuItem Add_Area_menuItem = new System.Windows.Forms.MenuItem();
        /// <summary>
        /// Список имеющихся осей отмечать, откуда брать координаты.
        /// </summary>
        private readonly List<string> ItemsInComboBox;

        public MainWindow()
        {
          Chart_Acts_One = new Chart_Acts();
          SLC = new SaveLoadClass(Chart1);
          
            InitializeComponent();
            

            //Заполняю список значений имен осей , чтобы координаты курсора соотв. отображать
            ItemsInComboBox = new List<string> ();
            ItemsInComboBox.Add("L1");
            ItemsInComboBox.Add("R1");
            ItemsInComboBox.Add("L2");
            ItemsInComboBox.Add("R2");
            AreasList.ItemsSource = ItemsInComboBox;

            //Создадим арену "по умолчанию"
            bsLeft.IsEnabled = false;
            bsRight.IsEnabled = false;
            Chart_Acts_One.LoadNEWArena(Chart1, Chart1.ChartAreas.Count()+1);
            
              //Перемещать элемент графика.
            Chart1.MouseDown += Chart1_MouseDown;
            Chart1.MouseLeave += Chart1_MouseLeave;
            
            
            //Редактировать элементы графика
            Chart1.MouseDoubleClick+= Chart1_M_DoubleClick;

            //Задаем собитие для обнаружения нахождения курсора над элементом 
            Chart1.MouseClick += new System.Windows.Forms.MouseEventHandler(Chart1_Click);
            
            //Меню
            allotment_X_menuItem.Text = "Выделять по X";
            allotment_Y_menuItem.Text = "Выделять по Y";
            Add_Area_menuItem.Text = "Еще график снизу";
            //Пропишем обработчик собитий по нажатию на элементы меню
            allotment_X_menuItem.Click += allotment_X_menuItem_Click;
            allotment_Y_menuItem.Click += allotment_Y_menuItem_Click;
            Add_Area_menuItem.Click += Add_Area_menuItem_Click;
            //Добавление элементов меню в меню
            ContMenuChart.MenuItems.AddRange(new[] { allotment_X_menuItem, allotment_Y_menuItem, Add_Area_menuItem });
            //Привязка этого меню к меню графика
            Chart1.ContextMenu = ContMenuChart;

            //Назначим "горячие" КЛАВИШИ.
            Chart1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Chart1_KeyDown);

            //Координаты курсора
            Chart1.CursorPositionChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CursorEventArgs>(chart1_CursorPositionChanged);

          
        }
        /// <summary>
        /// Запрещает перемещать элемент графика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Chart1_MouseLeave(object sender, EventArgs e)
        {
          MovingTitle = null;
          MovingLegend = null; 
        }
                                    
                                    //--------------------РЕДАКТИРОВАТЬ ОБЪЕКТЫ ГРАФИКА
        void Chart1_M_DoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
          Chart1.Focus();
          //Перемещение отменяем
          MovingTitle = null;
          MovingLegend = null; 
          // Call HitTest. 
          HitTestResult result = Chart1.HitTest(e.X, e.Y, true);

          //Определяем, что за элемент и поехали
          if (result.ChartElementType == ChartElementType.Title)
          {
            // Title result
            Title TitleItem = (Title)result.Object;
            TitleFormatWindow TFWindow = new TitleFormatWindow(TitleItem);
            TFWindow.Show();
          }
          //Тест на легенду. Вызов окна с чейками для правки серии.
          if (result.Object is LegendItem)
          {
            // Legend item result
            LegendItem legendItem = (LegendItem)result.Object;
            Legend_Format_Window LFWindow = new Legend_Format_Window(Chart1, Chart1.Series[legendItem.SeriesName]);
            LFWindow.Show();
          }
          //НА ось
          if (result.Object is Axis)
          {
            // Axis item result
            Axis AxisItem = (Axis)result.Object;

            // НА случай если собираемся добавочную ось редактировать добавлено имя арены в вызов конструктора.
            Axis_format_Window AFWindow = new Axis_format_Window(AxisItem, Chart1, TextBox_Arena_N.Text);
            //Вызов окна
            AFWindow.Show();
            //По закрытии будем все оси времени объединять. Но только в случае друг под другом построения.
            AFWindow.Closing += AFWindow_Closed;
          }
        }
          //-------------------------ПЕРЕМЕЩАТЬ ОБЪЕКТЫ ГРАФИКА
        void Chart1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HitTestResult result = Chart1.HitTest(e.X, e.Y, false);
          
          //Определяем, что за элемент и поехали. Сейчас Title
          if (result.ChartElementType == ChartElementType.Title && e.Button == System.Windows.Forms.MouseButtons.Left)
          {
            MovingTitle = (Title)result.Object;
          }

          if (MovingTitle!=null)
          {
            MovingTitle.Position.X = e.X * 100F / (float)(Chart1.Size.Width - 1);
            MovingTitle.Position.Y = e.Y * 100F / (float)(Chart1.Size.Height - 1);
          }

          //Определяем, что за элемент и поехали. Сейчас Legend
          if (result.ChartElementType == ChartElementType.LegendArea && e.Button == System.Windows.Forms.MouseButtons.Left)
          {
            MovingLegend = (Legend)result.Object;
          }

          if (MovingLegend != null && e.X * 100F / (float)(Chart1.Size.Width) < 90 && e.Y * 100F / (float)(Chart1.Size.Height)<90)
          {
            MovingLegend.Position.X = e.X * 100F / (float)(Chart1.Size.Width - 1);
            MovingLegend.Position.Y = e.Y * 100F / (float)(Chart1.Size.Height - 1);
          }

          ////Сейчас -- легенду
          //if (result.ChartElementType == ChartElementType.LegendArea)
          //{
          //  Legend MovingLegend = (Legend)result.Object;
          //  if (e.Button == System.Windows.Forms.MouseButtons.Left)      //if (isDown)
          //  {
          //    //MovingLegend.Position.Auto=false;
          //    MovingLegend.Position.X = e.X * 100F / ((float)(Chart1.Size.Width)) - 2;
          //    MovingLegend.Position.Y = e.Y * 100F / ((float)(Chart1.Size.Height)) - 2;
          //    Chart1.Invalidate();
          //  }
          //}
        }
        /// <summary>
        /// Обнаружение, где курсор. Для изменения названия арены в заголовке текущей арены.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chart1_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
            // Call HitTest. 
            HitTestResult result = Chart1.HitTest(e.X, e.Y,true);
            if (result.Object is ChartArea)
            {
              TextBox_Arena_N.Text = ((ChartArea)result.ChartArea).Name;
            }
            
            // Покрасим в цвет текстбокс в соответствии с выбранной ареной. Красим по порядку классическому цветов.
            switch (TextBox_Arena_N.Text)
            {
                case "Area# 1":
                    TextBox_Arena_N.Background =  System.Windows.Media.Brushes.Red;
                    break;
                case "Area# 2":
                    TextBox_Arena_N.Background = System.Windows.Media.Brushes.Orange;
                    break;
                case "Area# 3":
                    TextBox_Arena_N.Background = System.Windows.Media.Brushes.Yellow;
                    break;
                case "Area# 4":
                    TextBox_Arena_N.Background = System.Windows.Media.Brushes.Green;
                    break;
                case "Area# 5":
                    TextBox_Arena_N.Background = System.Windows.Media.Brushes.Blue;
                    break;
                case "Area# 6":
                    TextBox_Arena_N.Background = System.Windows.Media.Brushes.Indigo;
                    break;
                case "Area# 7":
                    TextBox_Arena_N.Background = System.Windows.Media.Brushes.Violet;
                    break;
                default:
                    //System.Windows.MessageBox.Show("Не выбрана площадка для графика");
                    break;
            }
        }
        /// <summary>
        /// Объединение по времени всех осей "Х" (ось времени)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AFWindow_Closed(object sender, EventArgs e)
        {
          if (Chart1.ChartAreas[Chart1.ChartAreas.Count - 1].AxisX.Maximum!=1)
          {
            for (int i = 0; i < Chart1.ChartAreas.Count - 1; i++)
            {
              Chart1.ChartAreas[i].AxisX.Minimum = Chart1.ChartAreas[Chart1.ChartAreas.Count - 1].AxisX.Minimum;
              Chart1.ChartAreas[i].AxisX.Maximum = Chart1.ChartAreas[Chart1.ChartAreas.Count - 1].AxisX.Maximum;
            }
          }
          
          
        }
        
        /// <summary>
        /// Выделение элемента в списке. Последует вывод значений параметра.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_Parameters_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Магия
            dynamic selectedItem = List_Parameters.SelectedItem;
            //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента.
            if (selectedItem != null)
            {
                string tempKKS = selectedItem.KKS;
                //Добавление в таб информации           
                Points_List.ItemsSource = File_Acts_One.Find_Parametr(tempKKS).Time_and_Value_List;
                //Points_List.Items.Add(File_Acts_One.Find_Parametr(tempKKS).Time_and_Value_List);
            }
        }
        /// <summary>
        /// Обработчик добавления на правую ось.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuildRight_Click(object sender, RoutedEventArgs e)
        {
            //Магия
            dynamic selectedItem = List_Parameters.SelectedItem;

            string tempKKS;
            //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента,
            //т.к. нет выбора элемента.
            if (selectedItem != null)
            {
                tempKKS = selectedItem.KKS;
                // Добавим линию, и назначим ее в созданную область по имени текста в TextBox_Arena_N
                Chart_Acts_One.AddLine(Chart1, tempKKS, TextBox_Arena_N.Text, File_Acts_One, "right","No");
            }
            if (selectedItem == null)
            {
                return;
            }
            
            //На вкладку с графиком переход.
            TabCont1.SelectedIndex = 1;
            Chart1.Focus();
        }
        /// <summary>
        /// Построить по левой оси.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuildLeft_Click(object sender, RoutedEventArgs e)
        {
            //Магия
            dynamic selectedItem = List_Parameters.SelectedItem;
            string tempKKS;
            //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента,
            //т.к. нет выбора элемента.
            if (selectedItem != null)
            {
                tempKKS = selectedItem.KKS;
                // Добавим линию, и назначим ее в созданную область по имени текста в TextBox_Arena_N

                Chart_Acts_One.AddLine(Chart1, tempKKS, TextBox_Arena_N.Text, File_Acts_One, "left", "No");

            }
            if (selectedItem == null)
            {
                return;
            }
            
            //На вкладку с графиком переход.
            TabCont1.SelectedIndex = 1;
            Chart1.Focus();
        }
        

        /// <summary>
        /// Нажатие на кнопку меню "File". Открываем файл, парсим, заполняем список параметров.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Удалить все параметры. Можно заново загружать файл.
            File_Acts_One.Parameters.Clear();
            File_Acts_One.ListFiles.Clear();
            //Очищаем наш комбокс
            ComBoxWahtFile.Items.Clear();
            List_Parameters.Items.Refresh();
            Chart1.Series.Clear();

            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Multiselect=true;
            if (openFileDialog1.ShowDialog() == true)
            {
            //Лист открывающихся файлов
              foreach (var item in openFileDialog1.FileNames)
	              {
		                //Считываем файлы
                  File_Acts_One.Read_File(item);
                }
            }
            //Заполнение комбокса списком файлов
            
            foreach (var item in File_Acts_One.ListFiles)
            {
              ComBoxWahtFile.Items.Add(item.filename); 
            }
             
            //Заполнение списка параметров. (При загрузке приложения.)
            List_Parameters.ItemsSource = File_Acts_One.Parameters;
            //Переносит на вкладочку с видом на данные по выбранному KKS. Фокусимся на графике.
            List_Parameters.Items.Refresh();
            TabCont1.SelectedIndex = 0;
            Chart1.Focus();
        }

        //________________________________________Функции для графика. Горячие клавиши.
        /// <summary>
        /// Удалить линии с конца. По одной.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ButtonClearLines_Click (object sender, RoutedEventArgs e)
        {
            for (int i = Chart1.Series.Count-1; i >= 0; i--)
            {
                Chart1.Series.RemoveAt(i);
                return;
            }
        }
        /// <summary>
        /// Горячие клавиши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chart1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //Вернуть масштаб
            if (e.KeyCode == System.Windows.Forms.Keys.Back)
            {
                foreach (var item in Chart1.ChartAreas)
                {
                    item.AxisX.ScaleView.ZoomReset();
                    item.AxisY.ScaleView.ZoomReset();
                    item.AxisY2.ScaleView.ZoomReset();
                }

                //Chart1.ChartAreas[TextBox_Arena_N.Text].AxisX.ScaleView.ZoomReset();
                //Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY.ScaleView.ZoomReset();
                //Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY2.ScaleView.ZoomReset();
            }
            //Передвигать клавой курсор шагами 1/100
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad4)
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position -= (Chart1.ChartAreas[TextBox_Arena_N.Text].AxisX.Maximum
                                                                  - Chart1.ChartAreas[TextBox_Arena_N.Text].AxisX.Minimum) / 100;
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad6)
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position += (Chart1.ChartAreas[TextBox_Arena_N.Text].AxisX.Maximum
                                                                  - Chart1.ChartAreas[TextBox_Arena_N.Text].AxisX.Minimum) / 100;
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad8)
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Position += (Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY.Maximum
                                                                  - Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY.Minimum) / 100;
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad2)
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Position -= (Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY.Maximum
                                                                  - Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY.Minimum) / 100;
            
        }
        /// <summary>
        /// Изменение на выделение по оси Х/обратно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void allotment_X_menuItem_Click(object sender, EventArgs e)
        {
            if (allotment_X_menuItem.Text == "Выделять по X")
            {
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Interval = (Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY.Maximum
                                                                      - Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY.Minimum) / 3;
                allotment_X_menuItem.Text = "Обычное выделение";
                return;
            }
            if (allotment_X_menuItem.Text == "Обычное выделение")
            {
                allotment_X_menuItem.Text = "Выделять по X";
                allotment_Y_menuItem.Text = "Выделять по Y";
                //Задаем стандартное выделение везде
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Interval = 0;
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Interval = 0;
                return;
            }
        }
        /// <summary>
        /// Изменение на выделение по оси Y/обратно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void allotment_Y_menuItem_Click(object sender, EventArgs e)
        {
            if (allotment_Y_menuItem.Text == "Выделять по Y")
            {
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Interval = (Chart1.ChartAreas[TextBox_Arena_N.Text].AxisX.Maximum
                                                                      - Chart1.ChartAreas[TextBox_Arena_N.Text].AxisX.Minimum) / 3;
                allotment_Y_menuItem.Text = "Обычное выделение";
                return;
            }
            if (allotment_Y_menuItem.Text == "Обычное выделение")
            {
                allotment_X_menuItem.Text = "Выделять по X";
                allotment_Y_menuItem.Text = "Выделять по Y";
                //Задаем стандартное выделение везде
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Interval = 0;
                Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Interval = 0;
                return;
            }
        }
        /// <summary>
        /// Добавить снизу график.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Add_Area_menuItem_Click(object sender, EventArgs e)
        {
            //Маркер типа графика.
            if (Many_GraphButton.Background != System.Windows.Media.Brushes.BurlyWood)
            {
              Many_GraphButton.Background = System.Windows.Media.Brushes.BurlyWood;
            }
            //Добавили арену
            Chart_Acts_One.Add_Area_Bottom(Chart1);
        }
        /// <summary>
        /// Построить на дополнительной оси слева.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bsLeft_Click(object sender, RoutedEventArgs e)
        {
          //Не давать строить , если еще по главным осям нет графиков.
          bool Is_there_Left_Axis=false;
          foreach (Series item in Chart1.Series)
          {
            if (item.YAxisType==AxisType.Primary)
            {
              Is_there_Left_Axis = true;
            }
          }
           if (Chart1.Series.Count == 0 || !Is_there_Left_Axis)
            {
                MessageBox.Show("Пожалуйста, сначала постройте график данной линии по основной левой оси.");
                return;
            }

            //Магия
            dynamic selectedItem = List_Parameters.SelectedItem;
            string tempKKS="";
            //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента,
            //т.к. нет выбора элемента.

            if (selectedItem != null)
            {
                tempKKS = selectedItem.KKS;
                // Добавим линию, и назначим ее в созданную область по имени текста в TextBox_Arena_N
            }

            //Для повторного добавления. Вначале, при добавлении одного параметры на график по доп. оси, нет никакго "b".
            foreach (var item in Chart1.ChartAreas)
            {
                if (item.Name.Contains("1b"))
                {

                  Chart_Acts_One.AddLine(Chart1, tempKKS, "Area# 11a", File_Acts_One, "left", "No");
                    TabCont1.SelectedIndex = 1;
                    Chart1.Focus();
                    return;
                } 
            }

            //Добавляем линию 
            Chart_Acts_One.AddLine(Chart1, tempKKS, TextBox_Arena_N.Text, File_Acts_One, "left", "L");
            TabCont1.SelectedIndex = 1;
            Chart1.Focus();
        }
        /// <summary>
        /// Строить по дополнительной правой оси.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bsRight_Click(object sender, RoutedEventArgs e)
        {
          //Не давать строить , если еще по главным осям нет граиков.
          bool Is_there_Right_Axis = false;
          foreach (Series item in Chart1.Series)
          {
            if (item.YAxisType == AxisType.Secondary)
            {
              Is_there_Right_Axis = true;
            }
          }
          if (Chart1.Series.Count == 0 || !Is_there_Right_Axis)
          {
            MessageBox.Show("Пожалуйста, сначала постройте график данной линии по основной правой оси.");
            return;
          }

            //Магия
            dynamic selectedItem = List_Parameters.SelectedItem;
            string tempKKS = "";
            //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента,
            //т.к. нет выбора элемента.

            if (selectedItem != null)
            {
                tempKKS = selectedItem.KKS;
                // Добавим линию, и назначим ее в созданную область по имени текста в TextBox_Arena_N
            }
            //Дополнительные линии к дополнительной оси.
            foreach (var item in Chart1.ChartAreas)
            {
                if (item.Name.Contains("2b"))
                {
                  Chart_Acts_One.AddLine(Chart1, tempKKS, "Area# 12a", File_Acts_One, "right", "No");
                    TabCont1.SelectedIndex = 1;
                    Chart1.Focus();
                    return;
                }
            }
            
            //Для начала строим просто справа.
            Chart_Acts_One.AddLine(Chart1, tempKKS, TextBox_Arena_N.Text, File_Acts_One, "right", "R");
            TabCont1.SelectedIndex = 1;
            Chart1.Focus();
        }
        /// <summary>
        /// Строить 1 график, но с 4 осями. Разблокировать такую возможность. Заблокировать добавление снизу.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void One_Graph_Click(object sender, RoutedEventArgs e)
        {
            if (Chart1.ChartAreas.Count!=1)
            {
                System.Windows.MessageBox.Show("Либо 1 график и 4 оси, либо много графиков с 2 осями!");
                return;
            }
            else
            {
                //Задействуем элементы меню для добавления на доп. оси гравиков.
                bsLeft.IsEnabled = true;
                bsRight.IsEnabled = true;
                //Не дает построить снизу график теперь
                Add_Area_menuItem.Enabled = false;
                //Кнопку подсвечиват типа графика
                One_GraphButton.Background = System.Windows.Media.Brushes.BurlyWood;
                //Кнопку графика подграфиком обычным цеветом, что бы не выделялась
                Many_GraphButton.Background = default(System.Windows.Media.Brush);
            }
        }
        /// <summary>
        /// Много графиков.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Many_Graph_Click(object sender, RoutedEventArgs e)
        {
            if (Chart1.ChartAreas.Count != 1)
            {
                System.Windows.MessageBox.Show("Либо 1 график и 4 оси, либо много графиков с 2 осями!");
                return;
            }
            else
            {
                bsLeft.IsEnabled = false;
                bsRight.IsEnabled = false;
                Add_Area_menuItem.Enabled = true;
                Many_GraphButton.Background = System.Windows.Media.Brushes.BurlyWood;
                One_GraphButton.Background = default(System.Windows.Media.Brush);
            }
        }
        
        private void ButtonDrowLine_Click(object sender, RoutedEventArgs e)
        {
                //Отказаться сейчас от выделения областей.
                //Разрешение увеличивать промежуток выделения
                Chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                Chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
                //Разрешение пользоваться курсором
                Chart1.ChartAreas[0].CursorX.IsUserEnabled = false;
                Chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
                Chart1.ChartAreas[0].CursorY.IsUserEnabled = false;
                Chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;

                DrawLine();
        }
        /// <summary>
        /// Нарисовать линию
        /// </summary>
        private void DrawLine()
        {
            PolylineAnnotation polyline = new PolylineAnnotation();
            polyline.LineColor = System.Drawing.Color.Black;
            //По ширине линии аннотации далее вычислетя тип ее. Поэтому, внимательно здесь, есть свзь.
            polyline.LineWidth = 2;
            
            polyline.AllowPathEditing = true;
            polyline.AllowSelecting = true;
            polyline.AllowMoving = true;
            //polyline.IsFreeDrawPlacement = true;

            Chart1.Annotations.Add(polyline);
            polyline.BeginPlacement();
            
        }
        /// <summary>
        /// Удалить предыдущую линию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelLastLine_Click(object sender, RoutedEventArgs e)
        {
            if (Chart1.Annotations.Count > 0)
            {
                // remove the last annotation object that was added
                Chart1.Annotations.RemoveAt(Chart1.Annotations.Count-1);
                            
            }
        }
        /// <summary>
        /// Все дулаить линии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelAllLines_Click(object sender, RoutedEventArgs e)
        {
            if (Chart1.Annotations.Count > 0)
            {
                // if in drawing mode, end the drawing mode...
                if (ButtonDrowLine.Content.ToString() == "Рисуем линию")
                {
                    MessageBox.Show("Не закочено рисование линии. Щелкните по кнопке 'Рисуем линию'");
                    return;
                    //// uncheck the drawing mode button, which will cause
                    //// the end placement method to be called for the check changed event
                    //ButtonDrowLine.Content = "Нарисовать линию";

                    //// Call self to remove the annotation... as simply removing the 
                    //// annotation does not work 
                    //DelLastLine_Click(sender, e);
                }
                if (ButtonDrowLine.Content.ToString() == "Нарисовать линию")
                {
                    
                    for (int i = 0; i < Chart1.Annotations.Count; i++)
                    {
                        //Tсть свзь между шириной линии аннотации и ее типом.
                        if (Chart1.Annotations[i].LineWidth == 2)
                        {
                            Chart1.Annotations.RemoveAt(i);
                            i = i-1;
                            //Chart1.Annotations.Remove(Chart1.Annotations[Chart1.Annotations.Count - 1]);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Удалить последний заголовок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelTitle_Click(object sender, RoutedEventArgs e)
        {
            if (Chart1.Titles.Count>0)
            {
                Chart1.Titles.RemoveAt(Chart1.Titles.Count-1);  
            }
           
        }
        /// <summary>
        /// Удалить все заголовки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelAllTitles_Click(object sender, RoutedEventArgs e)
        {
            if (Chart1.Titles.Count > 0)
            {
                Chart1.Titles.Clear();
            } 
        }
        /// <summary>
        /// Сохранить изображение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            // Create a new save file dialog
            Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();

            // Sets the current file name filter string, which determines 
            // the choices that appear in the "Save as file type" or 
            // "Files of type" box in the dialog box.
            saveFileDialog1.Filter =
   "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|EMF (*.emf)|*.emf|PNG (*.png)|*.png|SVG (*.svg)|*.svg|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            // Set image file format
            if (saveFileDialog1.ShowDialog() == true)
            {
                ChartImageFormat format = ChartImageFormat.Bmp;

                if (saveFileDialog1.FileName.EndsWith("bmp"))
                {
                    format = ChartImageFormat.Bmp;
                }
                else if (saveFileDialog1.FileName.EndsWith("jpg"))
                {
                    format = ChartImageFormat.Jpeg;
                }
                else if (saveFileDialog1.FileName.EndsWith("emf"))
                {
                    format = ChartImageFormat.Emf;
                }
                else if (saveFileDialog1.FileName.EndsWith("gif"))
                {
                    format = ChartImageFormat.Gif;
                }
                else if (saveFileDialog1.FileName.EndsWith("png"))
                {
                    format = ChartImageFormat.Png;
                }
                else if (saveFileDialog1.FileName.EndsWith("tif"))
                {
                    format = ChartImageFormat.Tiff;
                }
                
                // Save image
                Chart1.SaveImage(saveFileDialog1.FileName, format);
            }

        }
        /// <summary>
        /// УДалить курсор
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelKursorButton_Click(object sender, RoutedEventArgs e)
        {
            if (DelKursorButton.Header.ToString() == "Убрать")
            {
                foreach (var item in Chart1.ChartAreas)
                {
                    item.CursorX.IsUserSelectionEnabled = false;
                    item.CursorX.IsUserEnabled = false;
                    item.CursorX.LineWidth = 0;
                    item.CursorY.IsUserSelectionEnabled = false;
                    item.CursorY.IsUserEnabled = false;
                    item.CursorY.LineWidth = 0;
                    item.CursorX.Interval = 100;
                    item.CursorY.Interval = 100;
                }
                DelKursorButton.Header = "Вернуть";
                return;
            }
            if (DelKursorButton.Header.ToString() == "Вернуть")
            {
                foreach (var item in Chart1.ChartAreas)
                {
                    item.CursorX.IsUserSelectionEnabled = true;
                    item.CursorX.IsUserEnabled = true;
                    item.CursorX.LineWidth = 2;
                    item.CursorY.IsUserSelectionEnabled = true;
                    item.CursorY.IsUserEnabled = true;
                    item.CursorY.LineWidth = 2;
                    item.CursorX.Interval = 0;
                    item.CursorY.Interval = 0;
                }
                DelKursorButton.Header = "Убрать";
                return;
            }
            
 
        }
        /// <summary>
        /// Сохранение состояния
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {

            if (Chart1.ChartAreas.Count==1)
            {
               //Запускаем метод, который запишет, что надо.
                SaveLoadClass.Save_Condition("Simple", Chart1, File_Acts_One); 
            }
            if (One_GraphButton.Background==System.Windows.Media.Brushes.BurlyWood)
            {
                SaveLoadClass.Save_Condition("1_4_Graph", Chart1, File_Acts_One); 
            }
            if (Many_GraphButton.Background == System.Windows.Media.Brushes.BurlyWood)
            {
                SaveLoadClass.Save_Condition("Many_Each_Down", Chart1, File_Acts_One);
            }
        }
        /// <summary>
        /// Загрузить состояние
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Проверим, что открывался файл данных
            if (File_Acts_One.Parameters.Count==0)
            {
                MessageBox.Show("Нет. Вначале необходимо открыть файл данных, затем загружать файл сохранения.");
                return;
            }
            SaveLoadClass.Load_Condition(Chart1, File_Acts_One,List_Parameters,this);
            //НА вкладку с графиком. Фокусимся на графике.
            TabCont1.SelectedIndex = 1;
            Chart1.Focus();
        }
        /// <summary>
        /// Событие изменения курсара. Отображать его
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart1_CursorPositionChanged(object sender, System.Windows.Forms.DataVisualization.Charting.CursorEventArgs e)
        {
            string comboItem = (string)AreasList.SelectedItem;
            string ChosenAxis = comboItem;
            if (ChosenAxis == "L1")
            {
                try
                {
                    LabelXcoord.Content = DateTime.FromOADate(Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position).ToString();
                    Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.AxisType = AxisType.Primary;
                    LabelYcoord.Content = Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Position.ToString();
                }
                catch (Exception) { }
                return;
            }
            if (ChosenAxis == "R1")
            {
                try
                {
                    LabelXcoord.Content = DateTime.FromOADate(Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position).ToString();
                    Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.AxisType = AxisType.Secondary;
                    LabelYcoord.Content = Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Position.ToString();
                }
                catch (Exception) { }
                return;
            }
            if (ChosenAxis == "L2")
            {
                try
                {
                    LabelXcoord.Content = DateTime.FromOADate(Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position).ToString();
                    LabelYcoord.Content = Chart1.ChartAreas["Area# 11a"].CursorY.AxisType = AxisType.Primary;
                    LabelYcoord.Content = Chart1.ChartAreas["Area# 11a"].CursorY.Position.ToString();

                }
                catch (Exception) { }
                return;
            }
            if (ChosenAxis == "R2")
            {
                try
                {
                    LabelXcoord.Content = DateTime.FromOADate(Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position).ToString();
                    LabelYcoord.Content = Chart1.ChartAreas["Area# 12a"].CursorY.AxisType = AxisType.Secondary;
                    LabelYcoord.Content = Chart1.ChartAreas["Area# 12a"].CursorY.Position.ToString();

                }
                catch (Exception) { }
                return;
            }
            Chart1.Focus();
        }



        /// <summary>
        /// А это дает изменение сразу цифр. Вверху достаточно етода, а это уже айс.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AreasList_Selected(object sender, SelectionChangedEventArgs e)
               {
                   string comboItem = (string)AreasList.SelectedItem;
                   string ChosenAxis = comboItem;
                   if (ChosenAxis == "L1")
                   {
                       try
                       {
                           LabelXcoord.Content = DateTime.FromOADate(Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position).ToString();
                           LabelYcoord.Content = Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.AxisType = AxisType.Primary;
                           LabelYcoord.Content = Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Position.ToString();
                       }
                       catch (Exception) { }
                       return;
                   }
                   if (ChosenAxis == "R1")
                   {
                       try
                       {
                           LabelXcoord.Content = DateTime.FromOADate(Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position).ToString();
                           LabelYcoord.Content = Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.AxisType = AxisType.Secondary;
                           LabelYcoord.Content = Chart1.ChartAreas[TextBox_Arena_N.Text].CursorY.Position.ToString();
                       }
                       catch (Exception) { }
                       return;
                   }
                   if (ChosenAxis == "L2")
                   {
                       try
                       {
                           LabelXcoord.Content = DateTime.FromOADate(Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position).ToString();
                           LabelYcoord.Content = Chart1.ChartAreas["Area# 11a"].CursorY.AxisType = AxisType.Primary;
                           LabelYcoord.Content = Chart1.ChartAreas["Area# 11b"].CursorY.Position.ToString();

                       }
                       catch (Exception) { }
                       return;
                   }
                   if (ChosenAxis == "R2")
                   {
                       try
                       {
                           LabelXcoord.Content = DateTime.FromOADate(Chart1.ChartAreas[TextBox_Arena_N.Text].CursorX.Position).ToString();
                           LabelYcoord.Content = Chart1.ChartAreas["Area# 12a"].CursorY.AxisType = AxisType.Secondary;
                           LabelYcoord.Content = Chart1.ChartAreas["Area# 12b"].CursorY.Position.ToString();

                       }
                       catch (Exception) { }
                       return;
                   }
                   
               }
       
        //Вертикальная подпись
        private void ButtonDrowTitle_Click(object sender, RoutedEventArgs e)
        {

          System.Windows.Forms.DataVisualization.Charting.Title new_tile = new System.Windows.Forms.DataVisualization.Charting.Title();
          //new_tile.TextOrientation = TextOrientation.Rotated90;
          new_tile.Text = "Назови меня и перемести";
          new_tile.Position.Auto = false;
          //Для удобного графического отображения добавления, а то улетает за пределы.
          Title_Count++;
          new_tile.Position.X = 40+(float) 1.3 * Title_Count;
          new_tile.Position.Y = 40 + (float)2.3 * Title_Count;
          new_tile.Font = new System.Drawing.Font("Times New Roman", 12, System.Drawing.FontStyle.Regular);

          Chart1.Titles.Add(new_tile);

          
          
        }
            // ==================================ПЕЧАТЬ=============================================================
            //Настройка страницы.
        private void PageOptions_Click(object sender, RoutedEventArgs e)
        {
          Chart1.Printing.PageSetup(); 

        }
            //Выбор принтера
        private void PrintOptions_Click(object sender, RoutedEventArgs e)
        {
          Chart1.Printing.Print(true);
        }
            //Предварительный просмотр
        private void PreviewPrint_Click(object sender, RoutedEventArgs e)
        {
          Chart1.Printing.PrintPreview();
          
        }
            
            //Начать с новым графиком
        private void NewBeginning_Click(object sender, RoutedEventArgs e)
        {
        Chart1.Series.Clear();
        Chart1.Titles.Clear();
        Chart1.Annotations.Clear();
        foreach (var item in Chart1.ChartAreas)
        {
          item.AxisX.ScaleView.ZoomReset(100);
          item.AxisY.ScaleView.ZoomReset(100);
          item.AxisY2.ScaleView.ZoomReset(100);
        }
        //Chart1
        }

        private void LoadSvrkNames_Click(object sender, RoutedEventArgs e)
        {
          Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
          
          if (openFileDialog1.ShowDialog() == true)
          {

              //Считываем файл
              File_Acts_One.Read_File(openFileDialog1.FileName);
            
            
          }
          //Заполнение списка параметров. 
          //List_Parameters.ItemsSource = File_Acts_One.Parameters;
          //Переносит на вкладочку с видом на данные по выбранному KKS. Фокусимся на графике.
          List_Parameters.Items.Refresh();
          Chart1.Focus();
        }
        /// <summary>
        /// Добавление файлов. Все аналогично, но не затирается график и т.п.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
        //Очищаем наш комбокс
        ComBoxWahtFile.Items.Clear();

          Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
          openFileDialog1.Multiselect=true;
          if (openFileDialog1.ShowDialog() == true)
          {
            //В случае нескольких файлов тут надо будет несколько файлов открывать.
            foreach (var item in openFileDialog1.FileNames)
            {
              //Считываем файл
              File_Acts_One.Read_File(item);
            }
          }

          //Заполнение комбокса списком файлов
          foreach (var item in File_Acts_One.ListFiles)
          {
            ComBoxWahtFile.Items.Add(item.filename);
          }

          //Заполнение списка параметров. (При загрузке приложения.)
          List_Parameters.ItemsSource = File_Acts_One.Parameters;
          //Переносит на вкладочку с видом на данные по выбранному KKS. Фокусимся на графике.
          List_Parameters.Items.Refresh();
          TabCont1.SelectedIndex = 0;
          Chart1.Focus();
        }
        /// <summary>
        /// Для обработки дифф. эфф.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffEffModule_Click(object sender, RoutedEventArgs e)
        {
          //У нас тут только обыный график. Ведем обработку! Исключаем все фарианты с несколькими графиками!
          One_GraphButton.IsEnabled=false;
          Many_GraphButton.IsEnabled=false;

          ModuleDiffEff DiffEffWindow = new ModuleDiffEff(Chart1, File_Acts_One, TableCurrent);
          DiffEffWindow.Show(); 
        }

        private void List_Parameters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          ListBox lb = (ListBox)(sender);
          Parameter selectedParametr = (Parameter)lb.SelectedItem;
          if (selectedParametr != null) Clipboard.SetText(selectedParametr.KKS);
        }

        private void CalculOptions_Click(object sender, RoutedEventArgs e)
        {
          Calculations CalculationsWindow = new Calculations(File_Acts_One,Chart1);
          CalculationsWindow.Show(); 
        }

        private void Values_Module_Click(object sender, RoutedEventArgs e)
        {
          ModuleValues ValuesWindow = new ModuleValues(File_Acts_One, Chart1);
          ValuesWindow.Show(); 
        }
        /// <summary>
        /// Сдвинуть времена в выбранном файле.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonShift_Click(object sender, RoutedEventArgs e)
        {
                    
                 File_Acts_One.ShiftIt(ComBoxWahtFile.Text, Convert.ToInt32(TextBoxTime.Text));
                 List_Parameters.Items.Refresh();
                 //List_Parameters.ItemsSource = File_Acts_One.Parameters; ;
               Chart1.Series.Clear();
                  
        }
        /// <summary>
        /// Сохраняем набор параметров выделенных в csv-файл.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToCSV_Click(object sender, RoutedEventArgs e)
        {
          SaveFileDialog saveFileDialog = new SaveFileDialog();
          saveFileDialog.Filter = "Текстовый документ (*.txt)|*.txt|Все файлы (*.*)|*.*";

          if (saveFileDialog.ShowDialog() == true)
                {
                  //Создаем поток записи
                  System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(saveFileDialog.FileName);
                  //Создаем массив, куда запишем линии файла
                  List<string> Myfile = new List<string>();
                
                  foreach (Parameter item in List_Parameters.SelectedItems)
                  {
                    Myfile.Add(item.KKS + " " + item.Description + " " + item.Dimention + ";" + "Time" + ";" + "Value" );
                    foreach (Time_and_Value item2 in item.Time_and_Value_List)
                    {
                     Myfile.Add(" " + ";" + item2.Time + ";" + item2.Value);
                    }
                    

	                 }
                  for (int i = 0; i < length; i++)
                  {
                    
                  }
                  streamWriter.Close();  
	               }
         }


      

       


        
        
  }
}
