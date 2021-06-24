using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Collections . ObjectModel;
using System.IO;
using System.Windows;

namespace GoodPlot
{
    public class RezultTable
    {
        public ObservableCollection<Row> Rows;
        public RezultTable()
        {
            Rows = new ObservableCollection<Row>();
        }

        public RezultTable(RezultTable rezTable)
        {

        }
        /// <summary>
        /// Вернуть массив значений перемещений группы
        /// </summary>
        /// <param name="rezTable">Таблица значений</param>
        /// <param name="thisName">Группа (H12,H11,H10 или H9)</param>
        /// <returns></returns>
        public static List<double> ThisToList(RezultTable rezTable, string thisName)
        {
            List<double> rezult = new List<double>();
            foreach (Row row_i in rezTable.Rows)
            {
                if (thisName.Contains("H12"))
                    rezult.Add(row_i.H12);
                if (thisName.Contains("H11"))
                    rezult.Add(row_i.H11);
                if (thisName.Contains("H10"))
                    rezult.Add(row_i.H10);
                if (thisName.Contains("H9"))
                    rezult.Add(row_i.H9);
                if (thisName.Contains("Po"))
                    rezult.Add(Math.Abs( row_i.Δρ) );
                if (thisName.Contains("dPdH"))
                    rezult.Add(row_i.Dρ_Dh);
            }
            return rezult;
        }

        /// <summary>
        /// Вернуть индикацию группы (от 1000 )
        /// </summary>
        /// <param name="rezTable"></param>
        /// <returns></returns>
        public static List<double> ReturnGroupIndication(RezultTable rezTable)
        {
            List<double> rezult = new List<double>();

            foreach (Row row_i in rezTable.Rows)
            {
                rezult.Add(row_i.Indication);
            }
            return rezult;
        }

        /// <summary>
        /// Рассчитать перемещения группы, начиная от 1000% вниз.
        /// </summary>
        /// <param name="listGroupsList">Список из показаний перемещения групп</param>
        /// <returns></returns>
        public List<double> CalculateGroupSteps(List<List<double>> listGroupsList)
        {
            List<double> rezult = new List<double>();

            int groupnamber = 0;
            double curHeight = 1000;
            rezult.Add(curHeight);

            for (int step_i = 0; step_i < listGroupsList[0].Count; step_i++)
            {
                if (listGroupsList[groupnamber][step_i] < 100 && listGroupsList[groupnamber][step_i + 1] > 5 && groupnamber < listGroupsList.Count - 1)
                {
                    curHeight -= listGroupsList[groupnamber][step_i + 1] - listGroupsList[groupnamber][step_i];
                    rezult.Add(curHeight);
                }
                else
                    groupnamber++;
            }
            return rezult;
        }

        /// <summary>
        /// Двигалась ли группа
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="rezTable"></param>
        /// <returns></returns>
        static bool IsGroupMoove(string groupName, RezultTable rezTable)
        {
            bool down = false;

            if (groupName == "H12")
            {
                for (int row_i = 0; row_i < rezTable.Rows.Count-1; row_i++)
                {
                    if (Math.Abs (rezTable.Rows[row_i + 1].H12 - rezTable.Rows[row_i].H12) > 0.5)
                    {
                        down = true;
                        break;
                    }
                }
            }
            if (groupName == "H11")
            {
                for (int row_i = 0; row_i < rezTable.Rows.Count - 1; row_i++)
                {
                    if (Math.Abs(rezTable.Rows[row_i + 1].H11 - rezTable.Rows[row_i].H11) > 0.5)
                    {
                        down = true;
                        break;
                    }
                }
            }
            if (groupName == "H10")
            {
                for (int row_i = 0; row_i < rezTable.Rows.Count - 1; row_i++)
                {
                    if (Math.Abs(rezTable.Rows[row_i + 1].H10 - rezTable.Rows[row_i].H10) > 0.5)
                    {
                        down = true;
                        break;
                    }
                }
            }
            if (groupName == "H9")
            {
                for (int row_i = 0; row_i < rezTable.Rows.Count - 1; row_i++)
                {
                    if (Math.Abs(rezTable.Rows[row_i + 1].H9 - rezTable.Rows[row_i].H9) > 0.5)
                    {
                        down = true;
                        break;
                    }
                }
            }
            return down;

        }

        /// <summary>
        /// Тип эксперимента -- все вниз, какая-то одна подъем
        /// </summary>
        /// <param name="rezTable">Таблица результатов</param>
        /// <returns></returns>
        public static string TypeExpeariment(RezultTable rezTable)
        {
            bool down12 = IsGroupMoove("H12", rezTable);
            bool down11 = IsGroupMoove("H11", rezTable);
            bool down10 = IsGroupMoove("H10", rezTable);
            bool down9 = IsGroupMoove("H9", rezTable);

            if (down12 && down11)
                return "AllDown";

            
                if (down12)
                {
                    return "H12";
                }
                if (down11)
                {
                    return "H11";
                }
                if (down10)
                {
                    return "H10";
                }
                if (down10)
                {
                    return "H9";
                }

                bool Issome = false;

                Issome = down12 || down11 || down10 || down9;

                if (!Issome)
                {
                    MessageBox.Show("Ошибка в поиске двигающейся группы");
                }

            return "Fuck";
        }

        /// <summary>
        /// Считать таблицу расчетов для эксперимента.
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns></returns>
        public static RezultTable GetTable(string filePath)
        {
            RezultTable rezult = new RezultTable();

            //Считываем файл
            StreamReader paramsReader = new StreamReader(filePath, Encoding.GetEncoding("Windows-1251"));
            //Задаем сепаратор файла
            char separator = '\t';
            //Из файла записываем список параметров
            paramsReader.ReadLine();//1 пропускаем строку
            string File_line = paramsReader.ReadLine();

            int number = 0;
            while (File_line != null)
            {
                string[] File_line_Slices = File_Acts.CustomConvert(File_line).Split(separator);

                rezult.Rows.Add(new Row
                {
                    Number = number,
                    Indication = Convert.ToDouble(File_line_Slices[0]),
                    H12 = Convert.ToDouble(File_line_Slices[1]),
                    H11 = Convert.ToDouble(File_line_Slices[2]),
                    H10 = Convert.ToDouble(File_line_Slices[3]),
                    H9 = Convert.ToDouble(File_line_Slices[4]),
                    Dρ_Dh = Convert.ToDouble(File_line_Slices[5]),
                    Δρ = Math.Abs( Convert.ToDouble(File_line_Slices[6]) ),
                    DρDC = Convert.ToDouble(File_line_Slices[7])
                });
                number++;
                File_line = paramsReader.ReadLine();
            }
            paramsReader.Close();

            return rezult;
        }

        /// <summary>
        /// Сориентировать таблицу на погружение
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static RezultTable DirectToDownTable(RezultTable tablerezult)
        {
                    //Скопировали табличку
            RezultTable rezult= new RezultTable();
            RezultTable finaRezult = new RezultTable();
            foreach (var item in tablerezult.Rows)
	        {
		        rezult.Rows.Add(item);
	        }

                    //Какая группа двигалась
            List<string> groups = new List<string>();
            groups.AddRange(new List<string>{ "H12","H11","H10","H9"});
            string grupMooving="";

            foreach (var item in groups)
	        {
                if (IsGroupMoove(item, tablerezult))
                {
                    grupMooving = item;
                    break;
                }
	        }

                    //Уменьшались ли значения группы
            if ((ThisToList(tablerezult, grupMooving)[0] - ThisToList(tablerezult, grupMooving)[ThisToList(tablerezult, grupMooving).Count - 1]) < 0)
            {
                foreach (var item in rezult.Rows.Reverse())
                {
                    finaRezult.Rows.Add(item);
                }
            }
            else
                finaRezult = rezult;
            return finaRezult;
        }
    }

        public struct Row
        {
            public int Number { get; set; }
            public double Indication { get; set; }
            public double H12 { get; set; }
            public double H11 { get; set; }
            public double H10 { get; set; }
            public double H9 { get; set; }
            public double H8 { get; set; }
            public double Dh { get; set; }
            public double Dρ { get; set; }
            public double Dρ_Dh { get; set; }
            public double Δρ { get; set; }
            public double DρDC { get; set; }

            public double Dρ_Dh_Calculate { get; set; }
            public double Dρ_Dh_Deviation { get; set; }

            public double Δρ_Calculate { get; set; }
            public double Δρ_Deviation { get; set; }
        }
    
}
