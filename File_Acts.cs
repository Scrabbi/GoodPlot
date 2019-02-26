using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace GoodPlot
{
    /// <summary>
    /// Пара время-значение. Или одна точка на графике для параметра. Логика: создаем структуру для хранения точки, затем создадим список точек.
    /// Список точек, информацию о параметре в классе храним.
    /// </summary>
    public struct Time_and_Value
    {
        /// <summary>
        /// Время точки.
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// Значение точки.
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// Признак достоверности
        /// </summary>
        public string IsOk { get; set; }
    }
    /// <summary>
    /// Список точек, информацию о параметре в классе храним. 
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Список всех для параметра струтур Time_and_Value – времени и значения (список всех точек данных параметра).
        /// </summary>
        public List<Time_and_Value> Time_and_Value_List = new List<Time_and_Value>();
        /// <summary>
        /// KKS параметра. 
        /// </summary>
        public string KKS { get; set; }
        /// <summary>
        /// Описание параметра. 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Размерность. 
        /// </summary>
        public string Dimention { get; set; }
        /// <summary>
        /// Дополнительная информация (к примеру, обозначить где какие группы и реактивности). 
        /// </summary>
        public string Add_Info { get; set; }
    }
    /// <summary>
    /// File_Acts – это действия с файлом. Файл у нас – файл данных, снятый, например, с АПИКа после испытаний.
    /// Содержит список параметров, public методы работы с данными из файла. 
    /// </summary>
    public class File_Acts
    {
        /// <summary>
        /// Список классов Parameter. (Список параметров с данными их.)
        /// </summary>
        public  List<Parameter> Parameters = new List<Parameter>();
        /// <summary>
        /// Запускает методы : определить какой файл, "разобраться" с файлом (создать список Parameters).
        /// </summary>
        /// <param name="File_Full_Name">имя файла, включая полный путь</param>
        public void Read_File(string File_Full_Name, bool IsFirst)
        {
            //WhatFile – узнает тип файла (Бушер, другие станции, модификации на станции файла данных)
            switch (WhatFile(File_Full_Name))
            {
                case "APIK_Bush":
                    //Load_ApikBnpp – считывает файл данных с Бушера-1 с АПИК.
                    this.Load_ApikBnpp(File_Full_Name, IsFirst);
                    break;
                case "APIK_Nvaes":
                    //Load_ApikNVAES6 – считывает файл данных НВАЭС-6 с АПИК.
                    this.Load_ApikNVAES6(File_Full_Name, IsFirst);
                    break;
                case "SVBU_NVAES":
                    //Load_ApikNVAES6 – считывает файл данных НВАЭС-6 с АПИК.
                    this.Load_SvbuNVAES(File_Full_Name, IsFirst);
                    break;
                case "SvrkNvaes":
                    //Load_ApikNVAES6 – считывает файл данных НВАЭС-6 с АПИК.
                    this.Load_SvrkNvaes(File_Full_Name, IsFirst);
                    break;
                    //Дозагрузка обозначений параметров
                case "SvrkNamesNvaesFile":
                    //Load_ApikNVAES6 – считывает файл данных НВАЭС-6 с АПИК.
                    this.Load_SvrkNvaesNames(File_Full_Name);
                    break;
                    
                default:
                    MessageBox.Show("На данный момент поддерживаются файлы: \n1) АПИК Бушер-1; \n2)АПИК НВАЭС-6; \n3)СВРК НВАЭС");
                    break;
            }
        }
        /// <summary>
        /// Распознавание, что за файл.
        /// </summary>
        /// <param name="File_Full_Name">Имя фала с расширением.</param>
        /// <returns></returns>
        private string WhatFile(string File_Full_Name)
        {
            //Почитаем файл...
            //Считать файл. Саздаем поток чтения. Применяем правильную кодировку.
            StreamReader FileStream = new StreamReader(File_Full_Name, Encoding.GetEncoding("Windows-1251"));
            //Считать линию из файла.
            string File_line = FileStream.ReadLine();
            //Теперь можем понять, откуда он:
            //Бушер
            if (File_line.Contains("002. Астрономическое время со СКУД") && File_line.Contains("003. 10YQR00FX901XQ01 - N CORE, MW"))
            {
                return "APIK_Bush";
            }
            //НВАЭС-6
            if (File_line.Contains("2 Время СКУД") && File_line.Contains("3 11CLA75FX013AXQ1 М Н ККНП"))
            {
              return "APIK_Nvaes";
            }
            //СВБУ НВАЭС
            if (File_line.Contains("РќРІРђР­РЎ2  "))
            {
              return "SVBU_NVAES";
            }
            if (File_line.Contains("TypeID") )
            {
              return "SvrkNvaes";
            }
            if (File_line.Contains("HВАЭС            Кампания"))
            {
            //В 4 строке отличие (признак) , что файл с обозначениями.
              for (int i = 0; i < 4; i++)
              {
                File_line = FileStream.ReadLine();
              }
              if (File_line.Contains("Идентификатор"))
              {
                return "SvrkNamesNvaesFile";
              }
              else return "NULL";
            }

            return "NULL";
        }
        /// <summary>
        /// Метод, производящий обработку файла данных, снятого с АПИК Бушер-1.
        /// </summary>
        private void Load_ApikBnpp(string File_Full_Name, bool IsFirst)
        {
                  //ДОБАВЛЕНИЕ
            List<Parameter> Parameters0 = new List<Parameter>();
          
            //Линия из файла.
            string File_line = "";
            //Считать файл. Саздаем поток чтения. Применяем правильную кодировку.
            StreamReader FileRead = new StreamReader(File_Full_Name, Encoding.GetEncoding("Windows-1251"));
            //Список. Хранит разбитую линию файла. А первая строка в АПИКе Бушерском – заголовки к параметрам.
            List<string> File_line_Slices = new List<string>();
            //Считать линию из файла.
            File_line = FileRead.ReadLine();
            //Записываем разбитую линию в список по кускам.
            File_line_Slices = File_line.Split('\t').ToList();
            //Записываем все параметры в список. Время в этот список не записываем. А элементы 0 , 1 есть время в АПИКе Бушерском.
            for (int i = 2; i < File_line_Slices.Count(); i++)
            {
                //Записываемый параметр
                Parameter param = new Parameter();
                //Название так выглядит: Номер.+ +KKS +'-'+ +описание,+ + размерность
                //Всегда есть запятая, если нет, то параметр без KKS. (А это токи и реактивности такие.)
                if (File_line_Slices[i].Contains(','))
                {
                    param.KKS = File_line_Slices[i].Split('-').ToArray()[0];
                    param.Dimention = File_line_Slices[i].Split(',').ToArray()[1];
                    //Все-таки хочется вытащить что написано посередине. Для этого с обеих сторон "обрубаем".
                    param.Description = File_line_Slices[i].Replace(param.KKS, "");
                    param.Description = param.Description.Replace(param.Dimention, "");
                    param.Description = param.Description.Replace(',', ' ');
                }
                else
                {
                    param.KKS = File_line_Slices[i];
                    param.Dimention = "нет";
                    param.Description = "нет";
                }
                        //ДОБАВЛЕНИЕ
                if (IsFirst)
                {
                  Parameters.Add(param);
                }
                else Parameters0.Add(param);
            }
            //Первую строку с названиями считали!

            //Массив временный, хранящий значения параметров в строке.
            double[] tempdouble = new double[File_line_Slices.Count];
            //Считываем файл. Записываем в List параметры (создаем Parametrs). Признак конца файла – пустая строка.
            //Линия из файла очередная.
            File_line = FileRead.ReadLine();
            while (File_line != null)
            {

                  tempdouble = CustomConvert(File_line).Split('\t').Select(n => double.Parse(n)).ToArray();
                //tempdouble = File_line.Replace('.', ',').Split('\t').Select(n => double.Parse(n)).ToArray();

                    for (int i = 2; i < tempdouble.Length; i++)
                    {
                        Time_and_Value TAD = new Time_and_Value();
                        TAD.Time = DateTime.FromOADate(tempdouble[0]);
                        TAD.Value = tempdouble[i];
                        //ДОБАВЛЕНИЕ
                        if (IsFirst)
                        {
                          Parameters[i - 2].Time_and_Value_List.Add(TAD);
                        }
                        else Parameters0[i - 2].Time_and_Value_List.Add(TAD);
                    }
                    //Линия из файла очередная.
                    File_line = FileRead.ReadLine();
            }
            //Закрытие потока
            FileRead.Close();
            //ДОБАВЛЕНИЕ
            if (!IsFirst)
            {
              foreach (Parameter item in Parameters)
              {
                foreach (Parameter item0 in Parameters0)
                {
                  if (item.KKS==item0.KKS)
                  {
                    item0.KKS+="_ApikBnpp_Copy";
                  }
                }
              }
              Parameters.AddRange(Parameters0);
            }
        }

        /// <summary>
        /// Метод, производящий обработку файла данных, снятого с АПИК НВАЭС-6
        /// </summary>
        private void Load_ApikNVAES6(string File_Full_Name, bool IsFirst)
        {
            List<Parameter> Parameters0 = new List<Parameter>();

            //Считать файл. Саздаем поток чтения. Применяем правильную кодировку.
            StreamReader FileRead = new StreamReader(File_Full_Name, Encoding.GetEncoding("Windows-1251"));
            //Список. Хранит разбитую линию файла. А первая строка в АПИКе Бушерском – заголовки к параметрам.
            List<string> File_line_Slices = new List<string>();
            //Считать линию из файла.
            string File_line = FileRead.ReadLine();
            //Записываем разбитую линию в список по кускам.
            File_line_Slices = File_line.Split('\t').ToList();
            //Записываем все параметры в список. Время в этот список не записываем. А элементы 0 , 1 есть время в АПИКе Бушерском.
            for (int i = 2; i < File_line_Slices.Count(); i++)
            {
                //Записываемый параметр
                Parameter param = new Parameter();
                //Название так выглядит: Номер+ +KKS+' '+описание'
                //Бывает параметр без KKS. (А это токи и реактивности такие и еще резерв.). В общем, парсим и очередной заголовок параметра
                string[] slice_i = File_line_Slices[i].Split(' ').ToArray();
                param.KKS = slice_i[0] + " " + slice_i[1];
                param.Description = slice_i[2];
                if (slice_i.Length > 3)
                {
                    for (int m = 3; m < slice_i.Length; m++)
                    {
                        param.Description += " " + slice_i[m];
                    }
                }
                param.Dimention = "-";
                //Добавляем.
                if (IsFirst)
                {
                  Parameters.Add(param);
                }
                else Parameters0.Add(param);
            }
            //Первую строку с названиями считали!

            //Массив временный, хранящий значения параметров в строке.
            double[] tempdouble = new double[File_line_Slices.Count];
            //Считываем файл. Записываем в List параметры (создаем Parametrs). Признак конца файла – пустая строка.
            while (File_line != null)
            {
                //Линия из файла очередная.
                File_line = FileRead.ReadLine();
                //Необходимо, т.к. последняя строка с Null будет считана "заранее" с помощью FileRead.
                if (File_line != null)
                {
                    tempdouble = CustomConvert(File_line).Split('\t').Select(n => double.Parse(n)).ToArray();

                    for (int i = 2; i < tempdouble.Length; i++)
                    {
                        Time_and_Value TAD = new Time_and_Value();
                        TAD.Time = DateTime.FromOADate(tempdouble[0]);
                        TAD.Value = tempdouble[i];
                        if (IsFirst)
                        {
                          Parameters[i - 2].Time_and_Value_List.Add(TAD);
                        }
                        else Parameters0[i - 2].Time_and_Value_List.Add(TAD);
                    }
                }
            }
            //Закрытие потока
            FileRead.Close();
            //ДОБАВЛЕНИЕ
            if (!IsFirst)
            {
              foreach (Parameter item in Parameters)
              {
                foreach (Parameter item0 in Parameters0)
                {
                  if (item.KKS == item0.KKS)
                  {
                    item0.KKS += "_ApikNvaes_Copy";
                  }
                }
              }
              Parameters.AddRange(Parameters0);
            }
        }

        /// <summary>
        /// Метод, производящий обработку файла данных, снятого с СВБУ НВАЭС-6
        /// </summary>
        private void Load_SvbuNVAES(string File_Full_Name, bool IsFirst)
        {
          List<Parameter> Parameters0 = new List<Parameter>();
          //Считать файл. Саздаем поток чтения. Применяем правильную кодировку.
          StreamReader FileRead = new StreamReader(File_Full_Name);
          //Список. Хранит разбитую линию файла. 
          List<string> File_line_Slices = new List<string>();
          //Линия файла
          string File_line="";
                
                //Переходим к данным в файле
                    int j = 0;
                    while (j < 5)
                    {
                      File_line = FileRead.ReadLine();
                      j++;
                    }
	               //Линиия разбивается на кусочки.
                File_line_Slices = File_line.Split('\t').ToList();
                //Если встретили время первый раз, составим список параметров
                DateTime MyTime;
                if (DateTime.TryParse(File_line_Slices[0], out MyTime))
	              {
		                Parameter param = new Parameter();
                  param.KKS = File_line_Slices[1];
                  param.Description = File_line_Slices[5];
                  param.Dimention = File_line_Slices[3];
                  //Добавляем.
                  if (IsFirst)
                  {
                    Parameters.Add(param);
                  }
                  else Parameters0.Add(param);
                  //Переходим на след. строку, дозаполняем список параметров
                  File_line = FileRead.ReadLine();
                  while (!DateTime.TryParse(File_line.Split('\t').ToList()[0], out MyTime))
	                  {
	                   Parameter paramAdd = new Parameter();
                    paramAdd.KKS = File_line.Split('\t').ToList()[1];
                    paramAdd.Description = File_line.Split('\t').ToList()[5];
                    paramAdd.Dimention = File_line.Split('\t').ToList()[3];
                    //Добавляем.
                    if (IsFirst)
                    {
                      Parameters.Add(paramAdd);
                    }
                    else Parameters0.Add(paramAdd);
                    
                    File_line = FileRead.ReadLine();
	                  }
                 }
                //Закрытие потока
                FileRead.Close();

            //Заполняем значения
                //Считать файл. Саздаем поток чтения. Применяем правильную кодировку.
                StreamReader FileRead2 = new StreamReader(File_Full_Name);
                //Линия файла
                string File_line2= FileRead2.ReadLine();
                //Читаем все строки файла
                while (File_line2!=null)
                {
                  DateTime CurrTime;
                  if (DateTime.TryParse(File_line2.Split('\t').ToList()[0], out CurrTime))
                  {
                    //К параметрам подбираем значение
                    int paramcount=0;
                    if (IsFirst)
                    {
                     paramcount= Parameters.Count;
                    }
                    else paramcount = Parameters0.Count;
                    for (int i = 0; i < paramcount; i++)
                    {
                      Time_and_Value TAD = new Time_and_Value();
                      TAD.Time = CurrTime;
                      TAD.Value = double.Parse(CustomConvert(File_line2.Split('\t').ToList()[2]));
                      TAD.IsOk = File_line2.Split('\t').ToList()[4];
                      if (IsFirst)
                      {
                        Parameters[i].Time_and_Value_List.Add(TAD);
                      }
                      else Parameters0[i].Time_and_Value_List.Add(TAD);
                      //Переход на строку ниже, столько раз сколько параметров.
                      File_line2 = FileRead2.ReadLine();
                    }
                  }
                  File_line2 = FileRead2.ReadLine();
                }
            FileRead2.Close();
            if (!IsFirst)
            {
              foreach (Parameter item in Parameters)
              {
                foreach (Parameter item0 in Parameters0)
                {
                  if (item.KKS == item0.KKS)
                  {
                    item0.KKS += "SvbuNVAES_Copy";
                  }
                }
              }
              Parameters.AddRange(Parameters0);
            }
        }
        
                //Чтение СВРК с НВАЭС-6
        /// <summary>
        /// Метод, производящий обработку файла данных, снятого с СВРК НВАЭС
        /// </summary>
        private void Load_SvrkNvaes(string File_Full_Name, bool IsFirst)
        {
          List<Parameter> Parameters0 = new List<Parameter>();
          //Считать файл. Саздаем поток чтения. Применяем правильную кодировку.
          StreamReader FileRead = new StreamReader(File_Full_Name, Encoding.GetEncoding("Windows-1251"));
          //Список. Хранит разбитую линию файла. Заголовки к параметрам в 19 строке.
          List<string> File_line_Slices = new List<string>();
          //Считать линию 19 из файла. Заводим счетчик и строковую переменную.
          int j=0;
          string File_line="";
          while ( j < 19)
          {
            File_line = FileRead.ReadLine();
            j++;
          }
          
          //Записываем разбитую линию в список по кускам. Напоминание: здесь названия параметров.
          File_line_Slices = File_line.Split(';').ToList();
          //Первый элемент не как другие, содержит лишнюю часть до KKS.
          File_line_Slices[0]=File_line_Slices[0].Split('=').ToList()[1];

          //Записываем все параметры в список. "-1" потому что в конце линии пробел стоит, который записался как элемент таблицы.
          for (int i = 0; i < File_line_Slices.Count()-1; i++)
          {
            //Записываемый параметр
            Parameter param = new Parameter();
            param.KKS = File_line_Slices[i];
            param.Description = "-";
            param.Dimention = "-";
            //Добавляем.
            if (IsFirst)
            {
              Parameters.Add(param);
            }
            else Parameters0.Add(param);
            
          }

          //Первую строку файла с названиями считали! Дочитаем до 28 строки, где мы встанем перед началом данных
          int k=0;
          while (k < 9)
          {
            File_line = FileRead.ReadLine();
            k++;
          }
          
          //Считываем файл. Записываем в List параметры (создаем Parametrs). Признак конца файла – пустая строка.
          while (File_line != null)
          {
            //Линия из файла очередная.
            File_line = FileRead.ReadLine();
            //Необходимо, т.к. последняя строка с Null будет считана "заранее" с помощью FileRead.
            if (File_line != null)
            {
              //Как видно по файлу rsa, линию с параметров вначале стоит отделит до знака равно и после:
              string[] tempdouble0 = File_line.Split('=').ToArray();
              string[] tempdouble1 = tempdouble0[1].Split(';').ToArray();//.Select(n => double.Parse(n)).ToArray(); 

              //Length-1 потому как последняя величина в файле -- пробел.
              for (int i = 1; i < tempdouble1.Length-1; i++)
              {
                Time_and_Value TAD = new Time_and_Value();
                TAD.Time = DateTime.Parse(tempdouble1[0]);   
                //Теперь элемент массивва , к которому мы пришли, содержит вначале порядковый номер и при 1 встрече ошибочную цифру, так что начиная с конца считаем нужное.
                string[] tempdouble2=tempdouble1[i].Split(' ').ToArray();
                if (Convert.ToInt32(tempdouble2[tempdouble2.Count()-1]) == 0)
                {
                  TAD.IsOk="Yes";
                }
                if (Convert.ToInt32(CustomConvert(tempdouble2[tempdouble2.Count() - 1])) > 0)
                {
                  TAD.IsOk = "Нет, код: "+ tempdouble2[tempdouble2.Count() - 1];  
                }

                TAD.Value = Double.Parse(CustomConvert(tempdouble2[tempdouble2.Count() - 2]));
                if (IsFirst)
                {
                  Parameters[i - 1].Time_and_Value_List.Add(TAD);
                }
                else Parameters0[i - 1].Time_and_Value_List.Add(TAD);
                
              }
            }
          }
          //Закрытие потока
          FileRead.Close();
          if (!IsFirst)
          {
            foreach (Parameter item in Parameters)
            {
              foreach (Parameter item0 in Parameters0)
              {
                if (item.KKS == item0.KKS)
                {
                  item0.KKS += "_SvrkNvaes_Copy";
                }
              }
            }
            Parameters.AddRange(Parameters0);
          }
        }

        private void Load_SvrkNvaesNames(string File_Full_Name)
        {
          //Считать файл. Саздаем поток чтения. Применяем правильную кодировку.
          StreamReader FileRead = new StreamReader(File_Full_Name, Encoding.GetEncoding("Windows-1251"));
          //Список. Хранит разбитую линию файла. Интересное нам-- в 6 сроке начинается.
          List<string> File_line_Slices = new List<string>();
          //Перейти к линии 5. Заводим счетчик и строковую переменную.
          int j = 0;
          string File_line = "";
          while (j < 5)
          {
            File_line = FileRead.ReadLine();
            j++;
          }

            //Parameters уже заполнены, тут лишь 2 поля там дописываем: Discription и Demention.
          //Считываем файл. Записываем в List параметры (создаем Parametrs). Признак конца файла – пустая строка.
          while (File_line != null)
          {
            //Линия из файла очередная.
            File_line = FileRead.ReadLine();
            //Необходимо, т.к. последняя строка с Null будет считана "заранее" с помощью FileRead.
            if (File_line != null)
            {
              //Как видно по файлу rsa, линию с параметров вначале стоит отделит до знака равно и после:
              string[] tempdouble = File_line.Split('|').ToArray();
              //.Select(n => double.Parse(n)).ToArray(); 
              foreach (Parameter item in Parameters)
              {
                if (tempdouble[0].Contains(item.KKS))
                {
                  item.Description=tempdouble[1];
                  item.Dimention = tempdouble[3];
                  continue;
                }
              }
            }
          }
          
          //Закрытие потока
          FileRead.Close();
        }
        /// <summary>
        /// Даст Parameter по его описанию, KKS (по Discription, по KKS). Доработан на случай копии линии (+"_Copy")
        /// </summary>
        /// <param name="Discription_"></param>
        /// <returns></returns>
        public Parameter Find_Parametr(string Text)
        {
            if (Text.Contains("Copy"))
            {
                Text = Text.Replace("_Copy", "");
            }
            foreach (Parameter one in Parameters)
            {
                if (one.Description.Contains(Text) || one.KKS.Contains(Text))
                    return one;
            }
            MessageBox.Show("Была запущена процедура поиска параметра Find_Parametr, но параметр не был обнаружен!");
            return null;
        }
        /// <summary>
        /// универсальный конвектор точек и запятых.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CustomConvert (string s)
        {
          s = s.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
          s = s.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
          return s;
        }

       
    }
}
