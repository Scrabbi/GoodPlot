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
using System.IO;



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
    Legend MovingLegend = null;
    /// <summary>
            /// Выделенный элемент
    /// </summary>
    HitTestResult result;
          /// <summary>
    /// Активный экземпляр класса File_Acts.
    /// </summary>
    File_Acts myFileActs = new File_Acts();
            /// <summary>
    /// Активный экземпляр класса Chart_Acts.
    /// </summary>
    Chart_Acts myChart_Acts;

          //Задаем контекстное меню на график
    System.Windows.Forms.ContextMenu ContMenuChart = new System.Windows.Forms.ContextMenu();
    //Задаем элементы контекстного меню
          /// <summary>
    /// Добавить снизу форму графика
    /// </summary>
    public System.Windows.Forms.MenuItem Add_Area_menuItem = new System.Windows.Forms.MenuItem();
            /// <summary>
    /// Список имеющихся осей отмечать, откуда брать координаты.
    /// </summary>
    private readonly List<string> ItemsInComboBox;
            /// <summary>
    /// Для счета разностей
    /// </summary>
    private int D = 1;

    public MainWindow()
    {
      myChart_Acts = new Chart_Acts();

      InitializeComponent();

            //Заполняю список значений имен осей , чтобы координаты курсора соотв. отображать
      ItemsInComboBox = new List<string>();
      ItemsInComboBox.Add("L1");
      ItemsInComboBox.Add("R1");
      ItemsInComboBox.Add("L2");
      ItemsInComboBox.Add("R2");
      AreasList.ItemsSource = ItemsInComboBox;

      //Создадим арену "по умолчанию"
      
      myChart_Acts.LoadFirstArena(myChart);

      //Перемещать элемент графика.
      myChart.MouseDown += Chart1_MouseDown;
      myChart.MouseLeave += Chart1_MouseLeave;


      //Редактировать элементы графика
      myChart.MouseDoubleClick += Chart1_M_DoubleClick;

      //Задаем собитие для обнаружения нахождения курсора над элементом 
      myChart.MouseClick += new System.Windows.Forms.MouseEventHandler(Chart1_Click);

      //Меню
      Add_Area_menuItem.Text = "Еще график снизу";
      //Пропишем обработчик собитий по нажатию на элементы меню
      Add_Area_menuItem.Click += Add_Area_menuItem_Click;
      //Добавление элементов меню в меню
      ContMenuChart.MenuItems.AddRange(new[] {  Add_Area_menuItem });
      //Привязка этого меню к меню графика
      myChart.ContextMenu = ContMenuChart;



      //Назначим "горячие" КЛАВИШИ.
      myChart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Chart1_KeyDown);
      //Клавиши для формы
      this.KeyDown += MainWindow_KeyDown;

      //Координаты курсора
      myChart.CursorPositionChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CursorEventArgs>(chart1_CursorPositionChanged);


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
      myChart.Focus();
      //Перемещение отменяем
      MovingTitle = null;
      MovingLegend = null;
      // Call HitTest. 
      HitTestResult result = myChart.HitTest(e.X, e.Y, true);

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

        Legend_Format_Window LFWindow = new Legend_Format_Window(myChart, myChart.Series[legendItem.SeriesName], legendItem.Legend);
        LFWindow.Show();
      }
      //НА ось
      if (result.Object is Axis)
      {
        // Axis item result
        Axis AxisItem = (Axis)result.Object;

        // НА случай если собираемся добавочную ось редактировать добавлено имя арены в вызов конструктора.
        Axis_format_Window AFWindow = new Axis_format_Window(AxisItem, myChart, textBox_Arena_N.Text);
        //Вызов окна
        AFWindow.Show();
        //По закрытии будем все оси времени объединять. Но только в случае друг под другом построения.
        AFWindow.Closing += AFWindow_Closed;
      }
    }
    //-------------------------ПЕРЕМЕЩАТЬ ОБЪЕКТЫ ГРАФИКА
    void Chart1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      //Запомним, что за элемент.
      result = myChart.HitTest(e.X, e.Y, false);



      //Определяем, что за элемент и поехали. Сейчас Title
      if (result.ChartElementType == ChartElementType.Title && e.Button == System.Windows.Forms.MouseButtons.Left)
      {
        MovingTitle = (Title)result.Object;

      }

      if (MovingTitle != null)
      {
        MovingTitle.Position.X = e.X * 100F / (float)(myChart.Size.Width - 1);
        MovingTitle.Position.Y = e.Y * 100F / (float)(myChart.Size.Height - 1);
      }

      //Определяем, что за элемент и поехали. Сейчас Legend
      if (result.ChartElementType == ChartElementType.LegendArea && e.Button == System.Windows.Forms.MouseButtons.Left)
      {
        MovingLegend = (Legend)result.Object;
      }

      if (MovingLegend != null && e.X * 100F / (float)(myChart.Size.Width) < 90 && e.Y * 100F / (float)(myChart.Size.Height) < 90)
      {
        MovingLegend.Position.X = e.X * 100F / (float)(myChart.Size.Width - 1);
        MovingLegend.Position.Y = e.Y * 100F / (float)(myChart.Size.Height - 1);
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
      HitTestResult result = myChart.HitTest(e.X, e.Y, true);
      if (result.Object is ChartArea)
      {
        textBox_Arena_N.Text = ((ChartArea)result.ChartArea).Name;
      }

      // Покрасим в цвет текстбокс в соответствии с выбранной ареной. Красим по порядку классическому цветов.
      switch (textBox_Arena_N.Text)
      {
        case "Area# 1":
          textBox_Arena_N.Background = System.Windows.Media.Brushes.Red;
          break;
        case "Area# 2":
          textBox_Arena_N.Background = System.Windows.Media.Brushes.Orange;
          break;
        case "Area# 3":
          textBox_Arena_N.Background = System.Windows.Media.Brushes.Yellow;
          break;
        case "Area# 4":
          textBox_Arena_N.Background = System.Windows.Media.Brushes.Green;
          break;
        case "Area# 5":
          textBox_Arena_N.Background = System.Windows.Media.Brushes.Blue;
          break;
        case "Area# 6":
          textBox_Arena_N.Background = System.Windows.Media.Brushes.Indigo;
          break;
        case "Area# 7":
          textBox_Arena_N.Background = System.Windows.Media.Brushes.Violet;
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
    //Если имеются дополнительные снизу арены, совместить оси времени
      if (myChart_Acts.mainArenaCounter != 1)
      {
        for (int i = 0; i < myChart.ChartAreas.Count - 1; i++)
        {
          myChart.ChartAreas[i].AxisX.Minimum = myChart.ChartAreas[myChart.ChartAreas.Count - 1].AxisX.Minimum;
          myChart.ChartAreas[i].AxisX.Maximum = myChart.ChartAreas[myChart.ChartAreas.Count - 1].AxisX.Maximum;
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
      dynamic selectedItem = list_Parameters.SelectedItem;
      //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента.
      if (selectedItem != null)
      {
        string tempKKS = selectedItem.KKS;
        //Добавление в таб информации           
        Points_List.ItemsSource = myFileActs.Find_Parametr(tempKKS).Time_and_Value_List;
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
      dynamic selectedItem = list_Parameters.SelectedItem;
      //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента,
      if (selectedItem != null)
      {
              // Добавим линию, и назначим ее в созданную область по имени текста в TextBox_Arena_N
        myChart_Acts.AddMainLine(myChart, list_Parameters.SelectedItems, textBox_Arena_N.Text, myFileActs, "right");
      }
      if (selectedItem == null)
      {
        return;
      }
            //На вкладку с графиком переход.
      TabCont1.SelectedIndex = 1;
      myChart.Focus();
    }
    /// <summary>
    /// Построить по левой оси.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BuildLeft_Click(object sender, RoutedEventArgs e)
    {
            //Магия. Передача выбранного элемента из списка
      dynamic selectedItem = list_Parameters.SelectedItem;
            //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента,
      if (selectedItem != null)
      {
              // Добавим линию, и назначим ее в созданную область по имени текста в TextBox_Arena_N
        myChart_Acts.AddMainLine(myChart, list_Parameters.SelectedItems, textBox_Arena_N.Text, myFileActs, "left");
      }
      if (selectedItem == null)
      {
        return;
      }
            //На вкладку с графиком переход.
      TabCont1.SelectedIndex = 1;
      myChart.Focus();
    }


    /// <summary>
    /// Нажатие на кнопку меню "File". Открываем файл, парсим, заполняем список параметров.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFile_MenuItem_Click(object sender, RoutedEventArgs e)
    {
      //Удалить все параметры. Можно заново загружать файл.
      myFileActs.Parameters.Clear();
      //File_Acts_One.ListFiles.Clear();
      //Очищаем наш комбокс

      list_Parameters.Items.Refresh();
      myChart.Series.Clear();

      Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog1.Multiselect = true;
      if (openFileDialog1.ShowDialog() == true)
      {
        //Лист открывающихся файлов
        foreach (var item in openFileDialog1.FileNames)
        {
          //Считываем файлы
          myFileActs.Read_File(item);
        }
      }


      //Заполнение списка параметров. (При загрузке приложения.)
      list_Parameters.ItemsSource = myFileActs.Parameters;
      //Обновить список параметров
      list_Parameters.Items.Refresh();
      //Заполним начальное и конечное времена у файла
      if (myFileActs.read_satus == "ok")
      {
        TextBoxStartTime.Text = myFileActs.Parameters[0].Time_and_Value_List[0].Time.ToString();
        TextBoxEndTime.Text = myFileActs.Parameters[myFileActs.Parameters.Count - 1].Time_and_Value_List[myFileActs.Parameters[myFileActs.Parameters.Count - 1].Time_and_Value_List.Count - 1].Time.ToString();
      }

      //Переносит на вкладочку с видом на данные по выбранному KKS. Фокусимся на графике.
      TabCont1.SelectedIndex = 0;
      myChart.Focus();
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
        foreach (var item in myChart.ChartAreas)
        {
          item.AxisX.ScaleView.ZoomReset();
          item.AxisY.ScaleView.ZoomReset();
          item.AxisY2.ScaleView.ZoomReset();
          //item.AxisX2.ScaleView.ZoomReset(); Ось неактивна.
        }

        //Chart1.ChartAreas[TextBox_Arena_N.Text].AxisX.ScaleView.ZoomReset();
        //Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY.ScaleView.ZoomReset();
        //Chart1.ChartAreas[TextBox_Arena_N.Text].AxisY2.ScaleView.ZoomReset();
      }
      //Передвигать клавой курсор шагами 1/100
      if (e.KeyCode == System.Windows.Forms.Keys.NumPad4)
        myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position -= (myChart.ChartAreas[textBox_Arena_N.Text].AxisX.Maximum
                                                          - myChart.ChartAreas[textBox_Arena_N.Text].AxisX.Minimum) / 100;
      if (e.KeyCode == System.Windows.Forms.Keys.NumPad6)
        myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position += (myChart.ChartAreas[textBox_Arena_N.Text].AxisX.Maximum
                                                          - myChart.ChartAreas[textBox_Arena_N.Text].AxisX.Minimum) / 100;
      if (e.KeyCode == System.Windows.Forms.Keys.NumPad8)
        myChart.ChartAreas[textBox_Arena_N.Text].CursorY.Position += (myChart.ChartAreas[textBox_Arena_N.Text].AxisY.Maximum
                                                          - myChart.ChartAreas[textBox_Arena_N.Text].AxisY.Minimum) / 100;
      if (e.KeyCode == System.Windows.Forms.Keys.NumPad2)
        myChart.ChartAreas[textBox_Arena_N.Text].CursorY.Position -= (myChart.ChartAreas[textBox_Arena_N.Text].AxisY.Maximum
                                                          - myChart.ChartAreas[textBox_Arena_N.Text].AxisY.Minimum) / 100;

    }
    /// <summary>
    /// Клавиши для формы
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.D0)
      {
        //if (ChartHost.Margin == new System.Windows.Thickness(0, 0, 0, 0))
        //{
        //  ChartHost.Margin = new System.Windows.Thickness(-50, -69, -100, 0);
        //}
        //else ChartHost.Margin = new System.Windows.Thickness(-0, -0, -0, 0);

        // 
        ChartFormatWindow CFWindow = new ChartFormatWindow(ChartHost);
        //Вызов окна
        CFWindow.Show();
      }
      //К исходным размерам
      if (e.Key == System.Windows.Input.Key.D9)
      {
        ChartHost.Width = double.NaN;
        ChartHost.Height = double.NaN;
      }
      if (e.Key == System.Windows.Input.Key.D1)
      {
        myChart.Width = (int)(myChart.Height / 1.41);

      }

    }

            /// <summary>
    /// Добавить снизу график.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Add_Area_menuItem_Click(object sender, EventArgs e)
    {
      myChart_Acts.Add_Area_Bottom(myChart);
    }
    /// <summary>
    /// Построить на дополнительной оси слева.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bsLeft_Click(object sender, RoutedEventArgs e)
    {
          //Магия
      dynamic selectedItem = list_Parameters.SelectedItem;
            //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента,
              //т.к. нет выбора элемента.
          if (selectedItem != null)
            {
                //Добавляем линию 
        List<ChartArea> listAreas = myChart_Acts.AddAdditional_AxisAndlLine(myChart, textBox_Arena_N.Text, list_Parameters.SelectedItems, myFileActs , "left");
                //Добавление в контекстное меню параметров элемента с функциональностью строить графики по добавленной оси.
        foreach (ChartArea Area_i in listAreas)
        {
          //Добавление элементов в меню
          System.Windows.Controls.MenuItem additionalAxis_menuItem = new System.Windows.Controls.MenuItem();
          additionalAxis_menuItem.Header = Area_i.Name;//По какой оси (арене) график.
          list_Parameters.ContextMenu.Items.Add(additionalAxis_menuItem);
          //Пропишем обработчик собитий по нажатию на элементы меню
          additionalAxis_menuItem.Click += Additional_Axis_menuItem_Click;
        }
        
              

                  //Для удобства переход на вкладку с графиком.
        TabCont1.SelectedIndex = 1;
        myChart.Focus();
      }
    }
    //Строить по добавленной дополнительной оси.
    void Additional_Axis_menuItem_Click(object sender, EventArgs e)
    {
      myChart_Acts.AddLine_OnAdditionalAxis(myChart,  list_Parameters.SelectedItems, myFileActs, ((MenuItem)sender).Header.ToString());
      //Для удобства переход на вкладку с графиком.
      TabCont1.SelectedIndex = 1;
      myChart.Focus();
    }
    /// <summary>
    /// Строить по дополнительной правой оси.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bsRight_Click(object sender, RoutedEventArgs e)
    {
            //Магия
      dynamic selectedItem = list_Parameters.SelectedItem;
                  //Проверка потому что не успевает прогрузиться список, и тогда ошибка выбора элемента,
      //т.к. нет выбора элемента.
      if (selectedItem != null)
      {
        //Для начала строим просто справа.
        List<ChartArea> listAreas = myChart_Acts.AddAdditional_AxisAndlLine(myChart, textBox_Arena_N.Text, list_Parameters.SelectedItems, myFileActs, "right");
        //Добавление в контекстное меню параметров элемента с функциональностью строить графики по добавленной оси.
        foreach (ChartArea Area_i in listAreas)
        {
          //Добавление элементов в меню
          System.Windows.Controls.MenuItem additionalAxis_menuItem = new System.Windows.Controls.MenuItem();
          additionalAxis_menuItem.Header = Area_i.Name;//По какой оси (арене) график.
          list_Parameters.ContextMenu.Items.Add(additionalAxis_menuItem);
          //Пропишем обработчик собитий по нажатию на элементы меню
          additionalAxis_menuItem.Click += Additional_Axis_menuItem_Click;
        }

        TabCont1.SelectedIndex = 1;
        myChart.Focus();
      }
    }
    
    /// <summary>
    /// Рисовать линию.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonDrowLine_Click(object sender, RoutedEventArgs e)
    {
      PolylineAnnotation polyline = new PolylineAnnotation();
      polyline.LineColor = System.Drawing.Color.Black;
      //По ширине линии аннотации далее вычислетя тип ее. Поэтому, внимательно здесь, есть свзь.
      polyline.LineWidth = 1;

      polyline.AllowPathEditing = true;
      polyline.AllowSelecting = true;
      polyline.AllowMoving = true;
      //polyline.IsFreeDrawPlacement = true;

      myChart.Annotations.Add(polyline);
      polyline.BeginPlacement();
      //DrawLine(); То что выше в отдельный метод в примере выносилось, а зачем, не понятно.
    }

    /// <summary>
    /// Удалить выделенный элемент.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DelChosen_Click(object sender, RoutedEventArgs e)
    {
      if (myChart.Titles.Count > 0)
      {
        // Удалить  подпись
        if (result.ChartElementType == ChartElementType.Title)
          myChart.Titles.Remove((Title)result.Object);
        //Удалить аннотацию
        if (result.ChartElementType == ChartElementType.Annotation)
          myChart.Annotations.Remove((Annotation)result.Object);
      }
    }
    /// <summary>
    /// Удалить все линии линии
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DelAllLines_Click(object sender, RoutedEventArgs e)
    {
      if (myChart.Annotations.Count > 0)
      {
        // if in drawing mode, end the drawing mode...
        myChart.Annotations.Clear();

      }
    }
    /// <summary>
    /// Удалить все заголовки
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DelAllTitles_Click(object sender, RoutedEventArgs e)
    {
      if (myChart.Titles.Count > 0)
      {
        myChart.Titles.Clear();
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
        myChart.SaveImage(saveFileDialog1.FileName, format);
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
        foreach (var item in myChart.ChartAreas)
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
        foreach (var item in myChart.ChartAreas)
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
      //Диалог сохранения организоввыаем
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "Бинарный файл (*.bin)|*.bin";
      string fileName = "f.bin";
      if (saveFileDialog.ShowDialog() == true)
      {
        fileName = saveFileDialog.FileName;
      }

      // Save chart into the memory stream
      myChart.Serializer.Content = SerializationContents.Default;
      MemoryStream ms = new MemoryStream();
      myChart.Serializer.Save(ms);

      //Запись MemoryStream
      FileStream fileStr = new FileStream(fileName, FileMode.Create);
      ms.CopyTo(fileStr);
      fileStr.Close();
      ms.Close();
    }
    /// <summary>
    /// Загрузить состояние
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
    {
              //Диалог организуем
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Бинарный файл (*.bin)|*.bin";
      string fileName = "f.bin";
      if (openFileDialog.ShowDialog() == true)
      {
        fileName = openFileDialog.FileName;
      }

      MemoryStream ms = new MemoryStream();
      FileStream fileStr = new FileStream(fileName, FileMode.Open);
      fileStr.CopyTo(ms);


      ms.Seek(0, SeekOrigin.Begin);
      myChart.Serializer.Load(ms);
      ms.Close();
      fileStr.Close();
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
          LabelXcoord.Content = DateTime.FromOADate(myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position).ToString();
          myChart.ChartAreas[textBox_Arena_N.Text].CursorY.AxisType = AxisType.Primary;
          LabelYcoord.Content = myChart.ChartAreas[textBox_Arena_N.Text].CursorY.Position.ToString();
        }
        catch (Exception) { }
        return;
      }
      if (ChosenAxis == "R1")
      {
        try
        {
          LabelXcoord.Content = DateTime.FromOADate(myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position).ToString();
          myChart.ChartAreas[textBox_Arena_N.Text].CursorY.AxisType = AxisType.Secondary;
          LabelYcoord.Content = myChart.ChartAreas[textBox_Arena_N.Text].CursorY.Position.ToString();
        }
        catch (Exception) { }
        return;
      }
      if (ChosenAxis == "L2")
      {
        try
        {
          LabelXcoord.Content = DateTime.FromOADate(myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position).ToString();
          LabelYcoord.Content = myChart.ChartAreas["Area# 11a"].CursorY.AxisType = AxisType.Primary;
          LabelYcoord.Content = myChart.ChartAreas["Area# 11a"].CursorY.Position.ToString();

        }
        catch (Exception) { }
        return;
      }
      if (ChosenAxis == "R2")
      {
        try
        {
          LabelXcoord.Content = DateTime.FromOADate(myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position).ToString();
          LabelYcoord.Content = myChart.ChartAreas["Area# 12a"].CursorY.AxisType = AxisType.Secondary;
          LabelYcoord.Content = myChart.ChartAreas["Area# 12a"].CursorY.Position.ToString();

        }
        catch (Exception) { }
        return;
      }
      myChart.Focus();
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
          LabelXcoord.Content = DateTime.FromOADate(myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position).ToString();
          LabelYcoord.Content = myChart.ChartAreas[textBox_Arena_N.Text].CursorY.AxisType = AxisType.Primary;
          LabelYcoord.Content = myChart.ChartAreas[textBox_Arena_N.Text].CursorY.Position.ToString();
        }
        catch (Exception) { }
        return;
      }
      if (ChosenAxis == "R1")
      {
        try
        {
          LabelXcoord.Content = DateTime.FromOADate(myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position).ToString();
          LabelYcoord.Content = myChart.ChartAreas[textBox_Arena_N.Text].CursorY.AxisType = AxisType.Secondary;
          LabelYcoord.Content = myChart.ChartAreas[textBox_Arena_N.Text].CursorY.Position.ToString();
        }
        catch (Exception) { }
        return;
      }
      if (ChosenAxis == "L2")
      {
        try
        {
          LabelXcoord.Content = DateTime.FromOADate(myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position).ToString();
          LabelYcoord.Content = myChart.ChartAreas["Area# 11a"].CursorY.AxisType = AxisType.Primary;
          LabelYcoord.Content = myChart.ChartAreas["Area# 11b"].CursorY.Position.ToString();

        }
        catch (Exception) { }
        return;
      }
      if (ChosenAxis == "R2")
      {
        try
        {
          LabelXcoord.Content = DateTime.FromOADate(myChart.ChartAreas[textBox_Arena_N.Text].CursorX.Position).ToString();
          LabelYcoord.Content = myChart.ChartAreas["Area# 12a"].CursorY.AxisType = AxisType.Secondary;
          LabelYcoord.Content = myChart.ChartAreas["Area# 12b"].CursorY.Position.ToString();

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
      new_tile.Position.X = 40 + (float)1.3 * Title_Count;
      new_tile.Position.Y = 40 + (float)2.3 * Title_Count;
      new_tile.Font = new System.Drawing.Font("Times New Roman", 12, System.Drawing.FontStyle.Regular);



      myChart.Titles.Add(new_tile);



    }
    // ==================================ПЕЧАТЬ=============================================================
    //Настройка страницы.
    private void PageOptions_Click(object sender, RoutedEventArgs e)
    {
      myChart.Printing.PageSetup();

    }
    //Выбор принтера
    private void PrintOptions_Click(object sender, RoutedEventArgs e)
    {
      myChart.Printing.Print(true);
    }
    //Предварительный просмотр
    private void PreviewPrint_Click(object sender, RoutedEventArgs e)
    {
      myChart.Printing.PrintPreview();

    }

    //Начать с новым графиком
    private void NewBeginning_Click(object sender, RoutedEventArgs e)
    {
      //Удалить все элементы графика
      myChart.Series.Clear();
      myChart.Titles.Clear();
      myChart.Annotations.Clear();
      myChart.Legends.Clear();
      myChart.ChartAreas.Clear();
      //Название арены начальное
      textBox_Arena_N.Text = "Area# 1";

      //Начальная арена
      //bsLeft.IsEnabled = false;
      //bsRight.IsEnabled = false;
      myChart_Acts.LoadFirstArena(myChart);

      //Элементы в меню закрасить по умолчанию
    }

    private void LoadSvrkNames_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

      if (openFileDialog1.ShowDialog() == true)
      {

        //Считываем файл
        myFileActs.Read_File(openFileDialog1.FileName);


      }
      //Заполнение списка параметров. 
      //List_Parameters.ItemsSource = File_Acts_One.Parameters;
      //Переносит на вкладочку с видом на данные по выбранному KKS. Фокусимся на графике.
      list_Parameters.Items.Refresh();
      myChart.Focus();
    }
    /// <summary>
    /// Добавление файлов. Все аналогично, но не затирается график и т.п.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddFile_Click(object sender, RoutedEventArgs e)
    {


      Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog1.Multiselect = true;
      if (openFileDialog1.ShowDialog() == true)
      {
        //В случае нескольких файлов тут надо будет несколько файлов открывать.
        foreach (var item in openFileDialog1.FileNames)
        {
          //Считываем файл
          myFileActs.Read_File(item);
        }
      }



      //Заполнение списка параметров. (При загрузке приложения.)
      list_Parameters.ItemsSource = myFileActs.Parameters;
      //Переносит на вкладочку с видом на данные по выбранному KKS. Фокусимся на графике.
      list_Parameters.Items.Refresh();
      TabCont1.SelectedIndex = 0;
      myChart.Focus();
    }
    /// <summary>
    /// Для обработки дифф. эфф.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DiffEffModule_Click(object sender, RoutedEventArgs e)
    {
      ModuleDiffEff DiffEffWindow = new ModuleDiffEff(myChart, myFileActs, TableCurrent);
      DiffEffWindow.Show();
    }
    //Под войному щелчку копировать название KKS.
    private void List_Parameters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      ListBox lb = (ListBox)(sender);
      Parameter selectedParametr = (Parameter)lb.SelectedItem;
      if (selectedParametr != null) Clipboard.SetText(selectedParametr.KKS);
    }

    private void CalculOptions_Click(object sender, RoutedEventArgs e)
    {
      Calculations CalculationsWindow = new Calculations(myFileActs, myChart);
      CalculationsWindow.Show();
    }

    private void Values_Module_Click(object sender, RoutedEventArgs e)
    {
      foreach (var item in myChart.ChartAreas)
      {

        item.CursorY.IsUserSelectionEnabled = false;
        item.CursorY.IsUserEnabled = false;
        item.CursorY.LineWidth = 0;
        item.CursorY.Interval = 100;

      }
      
      ModuleValues ValuesWindow = new ModuleValues(myFileActs, myChart, list_Parameters);
      ValuesWindow.Show();
    }
    /// <summary>
    /// Сдвинуть времена в выбранном файле.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonShift_Click(object sender, RoutedEventArgs e)
    {
      //Сдвиг

      //File_Acts_One.ShiftIt(ComBoxWahtFile.Text, Convert.ToInt32(TextBoxTime.Text));
      //ПРоверка числа вводимого
      double secs = 0;
      double.TryParse(TextBoxTime.Text, out secs);
      if (secs == 0)
      {
        return;
      }


      //Выделенный , по его значениям идем. Проверим, что выделен параметр.
      if (list_Parameters.SelectedItems.Count == 0)
      {
        MessageBox.Show("Выделите параметр");
        return;
      }
      //Запомним, что выделили из параметров
      List<Parameter> selectedParametrs = new List<Parameter>();
      foreach (var item in list_Parameters.SelectedItems)
      {
        selectedParametrs.Add((Parameter)item);
      }


      //Пошли
      foreach (var item in selectedParametrs)
      {
        //Сюда сдвинутый параметр сохраним
        Parameter NewParametrSubstr = new Parameter();

        for (int i = 0; i < item.Time_and_Value_List.Count; i++)
        {
          //Создадим точку данных по разности
          Time_and_Value TaV = new Time_and_Value();
          TaV.Time = item.Time_and_Value_List[i].Time + TimeSpan.FromSeconds(secs);
          TaV.Value = item.Time_and_Value_List[i].Value;
          //Добавим точечку
          NewParametrSubstr.Time_and_Value_List.Add(TaV);
        }
        //Описываем, что получили.
        NewParametrSubstr.KKS = "сдвиг_" + TextBoxTime.Text + "_" + item.KKS;

        NewParametrSubstr.Description = item.KKS;


        myFileActs.Parameters.Add(NewParametrSubstr);
      }




      //Привязка
      //List_Parameters.ItemsSource = File_Acts_One.Parameters;
      list_Parameters.Items.Refresh();
      //Перестройка графика       
      //Chart1.Series.Clear();

    }
    /// <summary>
    /// Сохраняем набор параметров выделенных в csv-файл.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToCSV_Click(object sender, RoutedEventArgs e)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      //Сохранит все выделенные файлы в файлы по отдельности с названиями KKS.
      //Создаем массив, куда запишем линии файла
      List<string> Myfile = new List<string>();


      // Запишем каждый из параметров в свой файл. Счетчик для разделения файлов
      int MyI = 0;
      foreach (Parameter item in list_Parameters.SelectedItems)
      {
        MyI++;
        //Создаем поток записи
        string dsf = saveFileDialog.InitialDirectory;
        System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(saveFileDialog.InitialDirectory + item.KKS + ".csv");
        streamWriter.WriteLine(item.KKS + ";" + "Program_file" + ";");
        foreach (var item1 in item.Time_and_Value_List)
        {
          streamWriter.WriteLine(item1.Time + ";" + item1.Value + ";" + item1.IsOk + ";");
        }
        streamWriter.Close();
      }
    }

    // Для очистки выделения параметров, а это удобство
    private void ContextMenu_Closed(object sender, RoutedEventArgs e)
    {
        list_Parameters.UnselectAll();
    }

    //Удалить точки данных 
    private void Delete_Click(object sender, RoutedEventArgs e)
    {
      //Можно выделить несколько параметров, у которых удалить точки данных
      foreach (Parameter item in list_Parameters.SelectedItems)
      {
        //У каждого искать и удалить соответствующее время
        foreach (Time_and_Value item1 in Points_List.SelectedItems)
        {
          for (int i = 0; i < item.Time_and_Value_List.Count; i++)
          {
            if (item.Time_and_Value_List[i].Time == item1.Time)
            {
              item.Time_and_Value_List.Remove(item.Time_and_Value_List[i]);
            }
          }

        }
      }
    }
    /// <summary>
    /// Вычисление разности 2-х параметров
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Substraction_Men_Item_Click(object sender, RoutedEventArgs e)
    {
      //Если выделено не 2 параметра ровно, ничего не делать, предупредив пользователя.
      if (list_Parameters.SelectedItems.Count != 2)
      {
        MessageBox.Show("Выделите ровно 2 параметра");
        return;
      }


      //Сюда разность сохраним
      Parameter NewParametrSubstr = new Parameter();
      //Первый выделенный , по его значениям идем для вычисления разности
      Parameter selectedParametr0 = (Parameter)list_Parameters.SelectedItems[0];
      Parameter selectedParametr1 = (Parameter)list_Parameters.SelectedItems[1];
      //Пошли
      for (int i = 0; i < selectedParametr0.Time_and_Value_List.Count; i++)
      {
        //Создадим точку данных по разности
        Time_and_Value TaV = new Time_and_Value();
        TaV.Time = selectedParametr0.Time_and_Value_List[i].Time;
        TaV.Value = selectedParametr0.Time_and_Value_List[i].Value - selectedParametr1.Time_and_Value_List[i].Value;
        //Добавим точечку
        NewParametrSubstr.Time_and_Value_List.Add(TaV);
        NewParametrSubstr.KKS = "Разность" + D.ToString();
        D++;
      }
      //Простое диалоговое окно да/нет.
      MessageBoxResult MResult = MessageBox.Show("Вычитается из параметра сверху параметр снизу", "Раззница параметров", MessageBoxButton.YesNo);
      //Если да, то как мы и сделали
      if (MResult == MessageBoxResult.Yes)
      {
        //Описываем, что получили.
        NewParametrSubstr.Description = selectedParametr0.KKS + " - " + selectedParametr1.KKS;
      }
      //Знаки меняем, так как наоборот вычитаем
      if (MResult == MessageBoxResult.No)
      {
        NewParametrSubstr.Time_and_Value_List.ForEach(n => n.Value = -1 * n.Value);
        NewParametrSubstr.Description = selectedParametr1.KKS + " - " + selectedParametr0.KKS;
      }
      myFileActs.Parameters.Add(NewParametrSubstr);


      //Привязка
      //List_Parameters.ItemsSource = File_Acts_One.Parameters;
      list_Parameters.Items.Refresh();




    }


    //Редактировать ось Х
    private void X_Format_Click(object sender, RoutedEventArgs e)
    {
      // Axis item result
      Axis AxisItem = myChart.ChartAreas[textBox_Arena_N.Text].AxisX;

      // НА случай если собираемся добавочную ось редактировать добавлено имя арены в вызов конструктора.
      Axis_format_Window AFWindow = new Axis_format_Window(AxisItem, myChart, textBox_Arena_N.Text);
      //Вызов окна
      AFWindow.Show();
      //По закрытии будем все оси времени объединять. Но только в случае друг под другом построения.
      AFWindow.Closing += AFWindow_Closed;
    }

    private void Yleft_Format_Click(object sender, RoutedEventArgs e)
    {
      // Axis item result
      Axis AxisItem = myChart.ChartAreas[textBox_Arena_N.Text].AxisY;

      // НА случай если собираемся добавочную ось редактировать добавлено имя арены в вызов конструктора.
      Axis_format_Window AFWindow = new Axis_format_Window(AxisItem, myChart, textBox_Arena_N.Text);
      //Вызов окна
      AFWindow.Show();
      //По закрытии будем все оси времени объединять. Но только в случае друг под другом построения.
      AFWindow.Closing += AFWindow_Closed;
    }

    private void Yright_Format_Click(object sender, RoutedEventArgs e)
    {
      // Axis item result
      Axis AxisItem = myChart.ChartAreas[textBox_Arena_N.Text].AxisY2;

      // НА случай если собираемся добавочную ось редактировать добавлено имя арены в вызов конструктора.
      Axis_format_Window AFWindow = new Axis_format_Window(AxisItem, myChart, textBox_Arena_N.Text);
      //Вызов окна
      AFWindow.Show();
      //По закрытии будем все оси времени объединять. Но только в случае друг под другом построения.
      AFWindow.Closing += AFWindow_Closed;
    }

    private void Y2left_Format_Click(object sender, RoutedEventArgs e)
    {
      // Axis item result
      Axis AxisItem = myChart.ChartAreas[textBox_Arena_N.Text + "1b"].AxisY;

      // НА случай если собираемся добавочную ось редактировать добавлено имя арены в вызов конструктора.
      Axis_format_Window AFWindow = new Axis_format_Window(AxisItem, myChart, textBox_Arena_N.Text);
      //Вызов окна
      AFWindow.Show();
      //По закрытии будем все оси времени объединять. Но только в случае друг под другом построения.
      AFWindow.Closing += AFWindow_Closed;
    }

    private void Y2right_Format_Click(object sender, RoutedEventArgs e)
    {
      // Axis item result
      Axis AxisItem = myChart.ChartAreas[textBox_Arena_N.Text + "2b"].AxisY2;

      // НА случай если собираемся добавочную ось редактировать добавлено имя арены в вызов конструктора.
      Axis_format_Window AFWindow = new Axis_format_Window(AxisItem, myChart, textBox_Arena_N.Text);
      //Вызов окна
      AFWindow.Show();
      //По закрытии будем все оси времени объединять. Но только в случае друг под другом построения.
      AFWindow.Closing += AFWindow_Closed;
    }
    /// <summary>
    /// Удалить параметр
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Delete_Men_Item_Click(object sender, RoutedEventArgs e)
    {
      myFileActs.Parameters.RemoveAll(x => list_Parameters.SelectedItems.Contains(x));
      list_Parameters.Items.Refresh();
    }

    /// <summary>
    /// Обрезать время
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonCutTime_Click(object sender, RoutedEventArgs e)
    {
      //Начальное и конечное времена
      DateTime TimeStart;
      DateTime TimeEnd;
      //Проверка на корректность ввода
      if (!(DateTime.TryParse(TextBoxStartTime.Text, out TimeStart) && DateTime.TryParse(TextBoxEndTime.Text, out TimeEnd)))
      {
        return;
      }
      //Обрезание
      foreach (Parameter item in myFileActs.Parameters)
      {
        item.Time_and_Value_List = item.Time_and_Value_List.FindAll(n => n.Time >= DateTime.Parse(TextBoxStartTime.Text) && n.Time <= DateTime.Parse(TextBoxEndTime.Text));
      }

    }
    //Очистить серии на графике
    private void DelAllGraphs_Click(object sender, RoutedEventArgs e)
    {
      myChart.Series.Clear();
    }





  }
}
