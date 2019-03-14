using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;

namespace GoodPlot
{
    
    /// <summary>
    /// Клас для работы с сохранением и загрузкой состояния
    /// </summary>
   class SaveLoadClass
  {
        static Chart chart_ref;
        
        int k;
        public SaveLoadClass(Chart chart)
        { 
        chart_ref=chart;
        }
        /// <summary>
        /// Активный экземпляр класса Chart_Acts.
        /// </summary>
        static Chart_Acts Chart_Acts_On = new Chart_Acts(chart_ref);
        

        /// <summary>
        /// Сохранить состояние
        /// </summary>
        public static void Save_Condition(string GraphType, Chart chart1_given, File_Acts File_Acts_given)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовый документ (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
                {
                System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(saveFileDialog.FileName);
                
                streamWriter.WriteLine(GraphType);
                streamWriter.WriteLine(chart1_given.ChartAreas.Count);
                streamWriter.WriteLine("KKS|Наименование|Название арены|Лев./прав. ось|Max|Min");
                foreach (Series item in chart1_given.Series)
                {
                    //Обходим ее, будем далее рисовать ее и видимую ее, но не из файла беря информацию. НЕудобно иначе.
                    if (item.Name.Contains("Copy"))
                    {
                        continue;
                    }
                    //Выбор оси. А то макс и мин  не удаетс задать.
                    if (item.YAxisType.ToString()=="Primary")
	                {
		            streamWriter.WriteLine(item.Name + "|" + File_Acts_given.Find_Parametr(item.Name).Description + "|" + item.ChartArea+ "|" +
                        item.YAxisType.ToString() + "|" + chart1_given.ChartAreas[item.ChartArea].AxisY.Maximum.ToString()
                        + "|" + chart1_given.ChartAreas[item.ChartArea].AxisY.Minimum.ToString());
	                }
                    if (item.YAxisType.ToString() == "Secondary")
	                {
                        streamWriter.WriteLine(item.Name + "|" + File_Acts_given.Find_Parametr(item.Name).Description + "|" + item.ChartArea + "|" +
                            item.YAxisType.ToString() + "|" + chart1_given.ChartAreas[item.ChartArea].AxisY2.Maximum.ToString()
                            + "|" + chart1_given.ChartAreas[item.ChartArea].AxisY2.Minimum.ToString());
	                }

                }
                streamWriter.Close();
        }

      }
        /// <summary>
        /// Загрузить состояние
        /// </summary>
        /// <param name="File_Full_Name"></param>
        public static void Load_Condition(Chart chart1_given, File_Acts File_Acts_given, ListView ListVies_given, MainWindow MainWind)
    {
            
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовый документ (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            //Линия из файла.
            string File_line = "";

            if (openFileDialog.ShowDialog() == true)
            {
                //Считать файл. Саздаем поток чтения. Применяем правильную кодировку.
                StreamReader FileRead = new StreamReader(openFileDialog.FileName);

                //Считать линию из файла.
                File_line = FileRead.ReadLine();
                //Узнать тип графика
                if (File_line == "Simple")
                {
                  //Пропустить заголовок и количество арен
                    File_line = FileRead.ReadLine();
                    File_line = FileRead.ReadLine();
                    //Считать первую строку после заголовка. (Описание первой построенной линии.)
                    File_line = FileRead.ReadLine();
                    //Начать чтение файла.
                  while (File_line != null)
                  {
                        //Разделить линию на составляющие
                        string[] File_Line_slises = File_line.Split('|');
                        //НАйдем и выделим элемент LIstView с KKS из строки файла
                        foreach (Parameter item in ListVies_given.Items)
                            {
                                if (item.KKS == File_Line_slises[0])
                                {
                                    ListVies_given.Focus();
                                    ListVies_given.SelectedItem = item;
                                    string tempKKS = item.KKS;
                                    // Добавим линию, и назначим ее в созданную область по имени текста в TextBox_Arena_N. Строим, в общем.
                                    if (File_Line_slises[3] == "Primary")
                                    {
                                      Chart_Acts_On.AddLine(chart1_given, tempKKS, File_Line_slises[2], File_Acts_given, "left", "No");
                                        chart1_given.ChartAreas[File_Line_slises[2]].AxisY.Minimum = Convert.ToDouble(File_Line_slises[5]);
                                        chart1_given.ChartAreas[File_Line_slises[2]].AxisY.Maximum = Convert.ToDouble(File_Line_slises[4]);
                                    }
                                    if (File_Line_slises[3] == "Secondary")
                                    {
                                      Chart_Acts_On.AddLine(chart1_given, tempKKS, File_Line_slises[2], File_Acts_given, "right", "No");
                                        chart1_given.ChartAreas[File_Line_slises[2]].AxisY2.Minimum = Convert.ToDouble(File_Line_slises[5]);
                                        chart1_given.ChartAreas[File_Line_slises[2]].AxisY2.Maximum = Convert.ToDouble(File_Line_slises[4]);
                                    }
                                }
                            }
                        //К следующей строке.
                        File_line = FileRead.ReadLine();
                  }
                }
                //Это 1 график с 4 осями
                if (File_line == "1_4_Graph")
                {
                //Задействуем элементы меню для добавления на доп. оси гравиков.
                  MainWind.bsRight.IsEnabled = true;
                  MainWind.bsLeft.IsEnabled = true;
                  //Не дает построить снизу график теперь
                  MainWind.Add_Area_menuItem.Enabled = false;
                  //Кнопку подсвечиват типа графика
                  MainWind.One_GraphButton.Background = System.Windows.Media.Brushes.BurlyWood;
                  //Кнопку графика подграфиком обычным цеветом, что бы не выделялась
                  MainWind.Many_GraphButton.Background = default(System.Windows.Media.Brush);

                    //Пропустить заголовок и к-во арен
                    File_line = FileRead.ReadLine();
                    File_line = FileRead.ReadLine();
                    //Считать первую строку после заголовка. (Описание первой построенной линии.)
                    File_line = FileRead.ReadLine();
                    //----------------------------Сразу все арены доп. построить
                   // Chart_Acts_On.CreateYAxis_and_Areas(chart1_given, chart1_given.ChartAreas[0], 4);

                    //Счетчик добавлений арен для дополнительной оси.
                    int Add_Arena_Count_L = 0;
                    int Add_Arena_Count_R = 0;
                     
                    //Начать чтение файла.
                    while (File_line != null)
                    {
                        //Разделить линию на составляющие
                        string[] File_Line_slises = File_line.Split('|');
                        //НАйдем и выделим элемент LIstView с KKS из строки файла
                        foreach (Parameter item in ListVies_given.Items)
                        {
                            if (File_Line_slises[0] == item.KKS)
                            {
                                ListVies_given.Focus();
                                ListVies_given.SelectedItem = item;
                                string tempKKS = item.KKS;
                                // Строим 
                                if (File_Line_slises[3] == "Primary")
                                {
                                    if (File_Line_slises[2].Contains("1a")&&Add_Arena_Count_L==0)
                                    {
                                        Add_Arena_Count_L++;
                                        Chart_Acts_On.AddLine(chart1_given, tempKKS, "Area# 1", File_Acts_given, "left", "L");
                                        //Chart_Acts_On.CreateSecondYAxis("L", chart1_given, chart1_given.ChartAreas[0], chart1_given.Series[tempKKS], 4, 0);
                                        //Max Min
                                        chart1_given.ChartAreas["Area# 11b"].AxisY.Minimum = Convert.ToDouble(File_Line_slises[5]);
                                        chart1_given.ChartAreas["Area# 11b"].AxisY.Maximum = Convert.ToDouble(File_Line_slises[4]);
                                        //Выход выше
                                        continue;
                                    }

                                    //И если уже есть по доп. осям линии.
                                    if (File_Line_slises[2].Contains("1a") && Add_Arena_Count_L != 0)
                                    {
                                      Chart_Acts_On.AddLine(chart1_given, tempKKS, "Area# 11a", File_Acts_given, "left", "No");
                                      continue;
                                    }

                                    //Если не по доп. осям строим
                                    Chart_Acts_On.AddLine(chart1_given, File_Line_slises[0], File_Line_slises[2], File_Acts_given, "left", "No");
                                    //Max Min
                                    chart1_given.ChartAreas["Area# 1"].AxisY.Minimum = Convert.ToDouble(File_Line_slises[5]);
                                    chart1_given.ChartAreas["Area# 1"].AxisY.Maximum = Convert.ToDouble(File_Line_slises[4]);
                                }
                                if (File_Line_slises[3] == "Secondary")
                                {
                                  if (File_Line_slises[2].Contains("2a") && Add_Arena_Count_R == 0)
                                    {
                                    Add_Arena_Count_R++;
                                      Chart_Acts_On.AddLine(chart1_given, tempKKS, "Area# 1", File_Acts_given, "right", "R");
                                        //Chart_Acts_On.CreateSecondYAxis("R", chart1_given, chart1_given.ChartAreas[0], chart1_given.Series[tempKKS], 4, 0);
                                        //Max Min
                                        chart1_given.ChartAreas["Area# 12b"].AxisY2.Minimum = Convert.ToDouble(File_Line_slises[5]);
                                        chart1_given.ChartAreas["Area# 12b"].AxisY2.Maximum = Convert.ToDouble(File_Line_slises[4]);
                                        continue;
                                    }

                                  //И если уже есть по доп. осям линии.
                                  if (File_Line_slises[2].Contains("2a") && Add_Arena_Count_R != 0)
                                  {
                                    Chart_Acts_On.AddLine(chart1_given, tempKKS, "Area# 12a", File_Acts_given, "right", "No");
                                    continue;
                                  }

                                  //Если не по доп. осям строить
                                    Chart_Acts_On.AddLine(chart1_given, File_Line_slises[0], File_Line_slises[2], File_Acts_given, "right", "No");
                                    chart1_given.ChartAreas["Area# 1"].AxisY2.Minimum = Convert.ToDouble(File_Line_slises[5]);
                                    chart1_given.ChartAreas["Area# 1"].AxisY2.Maximum = Convert.ToDouble(File_Line_slises[4]);
                                }
                            }
                    }

                        //К следующей строке.
                    File_line = FileRead.ReadLine();
                    } 
                }
                if (File_line == "Many_Each_Down")
                {
                  //Задействуем элементы меню для добавления на доп. оси гравиков.
                  MainWind.bsRight.IsEnabled = false;
                  MainWind.bsLeft.IsEnabled = false;
                  //Снизу график теперь
                  MainWind.Add_Area_menuItem.Enabled = true;
                  //Кнопку подсвечиват типа графика
                  MainWind.One_GraphButton.Background =default(System.Windows.Media.Brush);
                  //Кнопку графика подграфиком обычным цеветом, что бы не выделялась
                  MainWind.Many_GraphButton.Background = System.Windows.Media.Brushes.BurlyWood;

                  //Считать к-во арен. Построить все.
                  File_line = FileRead.ReadLine();
                  int Arena_Count = Convert.ToInt32(File_line)-1;
                  for (int i = 0; i < Arena_Count; i++)
                  {
                      Chart_Acts_On.Add_Area_Bottom(chart1_given);
                  }

                  //Считать заголовок
                  File_line = FileRead.ReadLine();
                  //Считать первую строку после заголовка. (Описание первой построенной линии.)
                  File_line = FileRead.ReadLine();



                  while (File_line != null)
                  {
                      //Разделить линию на составляющие
                      string[] File_Line_slises = File_line.Split('|');
                      //НАйдем и выделим элемент LIstView с KKS из строки файла
                      foreach (Parameter item in ListVies_given.Items)
                      {
                          if (item.KKS == File_Line_slises[0])
                          {
                              ListVies_given.Focus();
                              ListVies_given.SelectedItem = item;
                              string tempKKS = item.KKS;
                              // Добавим линию, и назначим ее в созданную область по имени текста в TextBox_Arena_N. Строим, в общем.
                              if (File_Line_slises[3] == "Primary")
                              {
                                Chart_Acts_On.AddLine(chart1_given, tempKKS, File_Line_slises[2], File_Acts_given, "left", "No");
                                  chart1_given.ChartAreas[File_Line_slises[2]].AxisY.Minimum = Convert.ToDouble(File_Line_slises[5]);
                                  chart1_given.ChartAreas[File_Line_slises[2]].AxisY.Maximum = Convert.ToDouble(File_Line_slises[4]);
                              }
                              if (File_Line_slises[3] == "Secondary")
                              {
                                Chart_Acts_On.AddLine(chart1_given, tempKKS, File_Line_slises[2], File_Acts_given, "right", "No");
                                  chart1_given.ChartAreas[File_Line_slises[2]].AxisY2.Minimum = Convert.ToDouble(File_Line_slises[5]);
                                  chart1_given.ChartAreas[File_Line_slises[2]].AxisY2.Maximum = Convert.ToDouble(File_Line_slises[4]);
                              }
                          }
                      }
                      //К следующей строке.
                      File_line = FileRead.ReadLine();
                  }

                }
                FileRead.Close();
            }
            
    }
  }
}
