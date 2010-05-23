﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Text.RegularExpressions;

namespace ETEnTranslator
{
    class ETEnGlagol : IModule
    {
        string logOsnovaEl;
        Predlozhenie predl = null;
        int pozition_predl = 0;
        //Slovo slovo;

        public ETEnGlagol()
        {}

        public Slovo Analyze(Predlozhenie pr, int place)
        {
            Slovo analyzed = pr[place];

            PreAnalyze(pr, place, ref analyzed);

            return analyzed;

        }

        protected void PreAnalyze(Predlozhenie pr, int place, ref Slovo slovo)
        {
            AnalyzeGlagol(pr, place, ref slovo);
            //GetTranslate(ref slovo);
            SetExtraData(ref slovo);
        }

        private void GetTranslate(ref Slovo slovo)
        {
            SQLiteConnection connection = new SQLiteConnection(@"Data Source=dict.sqlitedb;Version=3;");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "SELECT eng FROM dict WHERE el=@el";
            command.Parameters.Add(new SQLiteParameter("el", slovo.eSlovo));
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows && reader.Read() && !reader.IsDBNull(0))
            {
                slovo.enSlovo.slovo = reader.GetString(0);
            }
            else
            {
                slovo.enSlovo.slovo = "[Нет перевода для " + slovo.eSlovo + "]";
            }
            reader.Close();
            connection.Close();
        }

        private void SetExtraData(ref Slovo slovo)
        {

            Glagol glagol = new Glagol();
            glagol.vremya = slovo.vremya;
            glagol.zalog = slovo.zalog;
            glagol.naklonenie = slovo.naklonenie;
            glagol.vid = slovo.vid;
            glagol.sostoyanie = slovo.sostoynie; 
            glagol.english = slovo.enSlovo.slovo;
            slovo.ExtraData = glagol;
        }

        static string TireSame(string str)
        {
            string temp = str;
            bool tire = true;
            Match match = null;
            while (tire)
            {
                tire = false;
                if ((match = OsEl(temp, @"\-{2,}")).Success)
                {
                    tire = true;
                    temp = match.Groups["часть1"].Value + "-" + match.Groups["часть3"].Value;
                }
            }
            if (temp[temp.Length - 1] == '-') temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }

        static Match OsEl(string el, string suffix)
        {
            Regex reg = new Regex(@"(?<часть1>.*)(?<часть2>" + suffix + ")(?<часть3>.*)");
            return reg.Match(el);
        }

        /// <summary>
        /// ставит глагол на Эльюнди в начальную форму(необходимо для поиска слова в словаре-там всё в инфинитиве)
        /// </summary>
        /// <param name="el">глагол</param>
        /// <returns>возвращает инфинитив</returns>
        public static string OsnovaEl(string el)
        {
            if (el.Length < 3) return el;
            string logOsnovaEl = "\nсоздание основы для " + el;
            string temp = el;
            Match match = null;
            bool find = true;
            while (find)
            {
                find = false;
                if ((match = OsEl(temp, @"\-(A|S|F|J)\-")).Success)
                {
                    switch (match.Groups["часть2"].Value)
                    {
                        case "-A-": logOsnovaEl += "\nудалён суффикс возвратного залога '"; break;
                        case "-S-": logOsnovaEl += "\nудалён суффикс совершенного вида '"; break;
                        case "-F-": logOsnovaEl += "\nудалён суффикс страдательного залога '"; break;
                        case "-J-": logOsnovaEl += "\nудалён суффикс повелительного наклонения '"; break;
                    }
                    logOsnovaEl += match.Groups["часть2"].Value + "'";
                    temp = match.Groups["часть1"].Value + "-" + match.Groups["часть3"].Value;
                    find = true;
                }
                if ((match = OsEl(temp, @"\-((RBY)|(SCY)|(TVY)|(PVI)|(PVS)|(UCS)|(SBA)|(RBA)|(TBA)|(YBA)|(UBA))")).Success)
                {
                    switch (match.Groups["часть2"].Value)
                    {
                        case "-RBY": logOsnovaEl += "\nудалён корень однократности '"; break;
                        case "-SCY": logOsnovaEl += "\nудалён корень начала действия '"; break;
                        case "-TVY": logOsnovaEl += "\nудалён безличный суффикс '"; break;
                        case "-PVI": logOsnovaEl += "\nудалён корень ограничения длительности '"; break;
                        case "-PVS": logOsnovaEl += "\nудалён корень неопределённой длительности '"; break;
                        case "-UCS": logOsnovaEl += "\nудалён корень незавершённости '"; break;
                        case "-SBA": logOsnovaEl += "\nудалён корень (суффикс/префикс) взаимного залога '"; break;
                        default: logOsnovaEl += "\nудалён временной суффикс '"; break;
                    }
                    logOsnovaEl += match.Groups["часть2"].Value + "'";
                    temp = match.Groups["часть1"].Value + "-" + match.Groups["часть3"].Value;
                    find = true;
                }
                temp = TireSame(temp);
            }
            if (temp[2] == '-')
                switch (temp[1])
                {
                    case 'A':
                        logOsnovaEl += "\nудалён префикс возвратного залога 'A'";
                        temp = "R" + temp.Substring(2);
                        break;
                    case 'S':
                        logOsnovaEl += "\nудалён префикс совершенного вида 'S'";
                        temp = "R" + temp.Substring(2);
                        break;
                }
            if (temp[temp.Length - 2] == '-')
                switch (temp[temp.Length - 1])
                {
                    case 'A':
                        logOsnovaEl += "\nудалён суффикс возвратного залога 'A'";
                        temp = temp.Substring(0, temp.Length - 2);
                        break;
                    case 'F':
                        logOsnovaEl += "\nудалён суффикс страдательного залога 'F'";
                        temp = temp.Substring(0, temp.Length - 2);
                        break;
                    case 'J':
                        logOsnovaEl += "\nудалён суффикс повелительного наклонения 'J'";
                        temp = temp.Substring(0, temp.Length - 2);
                        break;
                    case 'S':
                        logOsnovaEl += "\nудалён суффикс совершенного вида 'S'";
                        temp = temp.Substring(0, temp.Length - 2);
                        break;
                }
            temp = "R" + TireSame(temp).Substring(1);
            // if (temp[1] == '-' && temp.Length < 5&&temp[2]=='Q') temp = "R" + temp.Substring(2);
            logOsnovaEl += "\nНачальная форма: < " + temp + " >";
            return temp;
        }

        protected void AnalyzeGlagol(Predlozhenie pr, int place, ref Slovo slovo)
        {
           // if (c.eSlovo.Length < 2) return c;
            bool Zaver = false;
           /* slovo.zalog = Zalog.Dejstvitekniy;//настройки по умолчанию
            slovo.chastRechi = ChastRechi.Glagol;//настройки по умолчанию
            slovo.sostoynie = Sostoynie.Обычное;//настройки по умолчанию
            slovo.chislo = Chislo.Edinstvennoe;//настройки по умолчанию
            slovo.rod = Rod.Obshij;//настройки по умолчанию            
            if (slovo.eSlovo[1] == 'A') slovo.zalog = Zalog.Vozvratniy;*/
            if (slovo.eSlovo.Length > 3) if (slovo.eSlovo[2] == '-' && slovo.eSlovo[1] == 'S')
                {
                    slovo.vid = Vid.Zavershennost;
                    Zaver = true;
                }
           // NaklSoslag();
            string[] temp = slovo.eSlovo.Split('-');
            if (temp[0].Length == 1 && temp.Length > 1)
            {
                switch (temp[1])
                {
                    //совершенный вид
                    case "RBY": //[туко]
                        slovo.vid = Vid.Mgnovennost;
                        break;
                    case "SCY"://[роко]
                        slovo.vid = Vid.NachaloDejstviya;
                        break;
                    case "PVI"://[пэсо]:
                        slovo.vid = Vid.OgranichenieDlitelnosti;
                        break;
                    //несовершенный вид
                    case "PVS"://[пэро]
                        slovo.vid = Vid.NeopredelennayaDlitelnost;
                        break;
                    case "RBA"://[тудо]
                        slovo.vid = Vid.PostoyannayaDlitelnost;
                        break;
                    case "UCS"://[шоро]
                        slovo.vid = Vid.NezavershennostDejstviya;
                        break;
                    default: break;
                }
            }
            for (int i = 1; i < temp.Length; i++)
            {
                switch (temp[i])
                {
                    //определение дополнительных времён
                    case "RBA"://постоянное(длительное)
                        switch (temp[0][0])
                        {
                            case 'T':
                                slovo.vremya = Vremya.NastoyascheeDlitelnoe;
                                break;
                            case 'Y':
                                slovo.vremya = Vremya.ProshedsheeDlitelnoe;
                                break;
                            case 'U':
                                slovo.vremya = Vremya.BuduscheeDlitelnoe;
                                break;
                            default: break;
                        }
                        break;
                    case "TBA"://наст
                        switch (temp[0][0])
                        {
                            case 'T':
                                slovo.vremya = Vremya.NastoyascheeVNastoyaschem;
                                break;
                            case 'Y':
                                slovo.vremya = Vremya.ProshedsheeSNastoyaschim;
                                break;
                            case 'U':
                                slovo.vremya = Vremya.BuduscheeVNastoyaschem;
                                break;
                            default: break;
                        }
                        break;
                    case "YBA"://прош
                        switch (temp[0][0])
                        {
                            case 'T':
                                slovo.vremya = Vremya.NastoyascheeSProshedshim;
                                break;
                            case 'Y':
                                slovo.vremya = Vremya.Davnoproshedshee;
                                break;
                            case 'U':
                                slovo.vremya = Vremya.BuduscheeSProshedshim;
                                break;
                            default: break;
                        }
                        break;
                    case "UBA"://буд
                        switch (temp[0][0])
                        {
                            case 'T':
                                slovo.vremya = Vremya.NastoyascheeSBuduschim;
                                break;
                            case 'Y':
                                slovo.vremya = Vremya.ProshedsheeBuduscheeBezNastoyaschego;
                                break;
                            case 'U':
                                slovo.vremya = Vremya.BuduscheeDalekoe;
                                break;
                            default: break;
                        }
                        break;
                    //определение залога
                    case "A":
                        slovo.zalog = Zalog.Vozvratniy;
                        break;
                    case "F":
                        slovo.zalog = Zalog.Stradatelniy;
                        break;
                    case "SBA":
                        slovo.zalog = Zalog.Vzaimniy;
                        break;
                    //определение наклонения
                    case "J":
                        slovo.naklonenie = Naklonenie.Povelitelnoe;
                        break;
                    //определение вида
                    //совершенный вид
                    case "S"://[ро]
                        if (!Zaver) slovo.vid = Vid.Zavershennost;
                        else slovo.vid = Vid.NevozvratnayaZavershennost;
                        break;
                    //состояние
                    case "TVY"://[чэко]                                        
                        slovo.sostoynie = Sostoynie.Безличное;
                        break;
                    default: break;
                }
            }

            if (slovo.chastRechi == ChastRechi.Glagol)
                slovo.eSlovo = OsnovaEl(slovo.eSlovo);
            else
                slovo.eSlovo = OsnovaElPrich(slovo.eSlovo);

            logOsnovaEl += "\n***Характеристики*** " + "\nЧисло: " + slovo.chislo.ToString() + "\nНаклонение: " + slovo.naklonenie.ToString() + "\nРод: " + slovo.rod.ToString() + "\nСостояние: " + slovo.sostoynie.ToString() + "\nВид: " + slovo.vid.ToString() + "\nВремя: " + slovo.vremya.ToString() + "\nЗалог: " + slovo.zalog.ToString();
        }

        private string OsnovaElPrich(string p)
        {
                string osnova = p;

                osnova = osnova.Replace("-SVA", "");   // взaимный зaлог
                osnova = osnova.Replace("-TYU", "");  // Нaстоящее время
                osnova = osnova.Replace("-YYU", "");  // Прошедшее время
                osnova = osnova.Replace("-UYU", "");  // Будущее время
                osnova = osnova.Replace("-RBA", "");  // для постоянного или длительного времени
                osnova = osnova.Replace("-TBA", "");  // для нaстоящего времени
                osnova = osnova.Replace("-YBA", "");  // для прошедшего времени
                osnova = osnova.Replace("-UBA", "");  // для будущего времени
                osnova = osnova.Replace("-RBY", "");  // Однокрaтность
                osnova = osnova.Replace("-SCY", "");  // Нaчaло действия
                osnova = osnova.Replace("-PVI", "");  // Огрaничение длительности
                osnova = osnova.Replace("-PVS", "");  // Неопределеннaя длительность
                osnova = osnova.Replace("-RBA", "");  // Постояннaя длительность
                osnova = osnova.Replace("-UCS", "");  // Незaвершенность действия

                if (osnova[osnova.Length - 1] == 'J' && osnova[osnova.Length - 2] == '-')
                    osnova = osnova.Remove(osnova.Length - 2, 2);       // Повелительное нaклонение
                if (osnova[1] == 'S' && osnova[2] == '-')              //зaвершенность действия
                    osnova = osnova.Remove(1, 1);
                if (osnova[osnova.Length - 1] == 'S' && osnova[osnova.Length - 2] == '-')
                    osnova = osnova.Remove(osnova.Length - 2, 2);       //зaвершенность

                /*
                if (osnova[1] == 'A' && osnova[2] == '-')                   //Возврaтный зaлог
                    osnova = osnova.Remove(1, 1);*/
                if (osnova[osnova.Length - 1] == 'A')
                    osnova = osnova.Remove(osnova.Length - 2, 2);       //Возврaтный зaлог


                if (osnova[osnova.Length - 1] == 'F' && osnova[osnova.Length - 2] == '-')
                    osnova = osnova.Remove(osnova.Length - 1, 1);       //Стрaдaтельный зaлог


                /*      for (int i = 0; i < osnova.Length; i++)
                      {
                          if (osnova[i] == '-')
                          {
                              osnova = osnova.Remove(i, 1);
                              i--;
                          }
                      }
       * */
                osnova = osnova.Remove(0, 1);
                osnova = "R" + osnova;
                //MessageBox.Show(osnova);

                return osnova;
        }
       /* 
        private void NaklSoslag()//
        {
            int i = pozition_predl - 1;
            //пока смотрим только предыдущие слова
            while (i > 0)
            {
                if (predl[i].chastRechi != ChastRechi.Znak)
                {
                    if (predl[i].eSlovo == "J-OBP" || predl[i].eSlovo == "J-TVQ" || predl[i].eSlovo == "G-OBP" || predl[i].eSlovo == "S-OBP")
                    {
                        // "J-OBP": //бы                          
                        // "J-TVQ": //бы...ни                           
                        // "G-OBP": //бы (вот бы)                           
                        // "S-OBP": //чтобы                           
                        slovo.naklonenie = Naklonenie.Soslagatelnoe;
                        break;
                    }
                }
                else break;
                i--;
            }
        }*/

        /**
         * ЗАГЛУШКА
         * На выходе должно быть слово на ангийском языке
         */
        public Slovo Translate(Predlozhenie pr, int place)
        {
            Slovo analyzed = pr[place];

            GetTranslate(ref analyzed);

            return analyzed;
        }
    }
}
