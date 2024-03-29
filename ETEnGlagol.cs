﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;

namespace ETEnTranslator
{
    class ETEnGlagol : IModule
    {
        string logOsnovaEl, strok;
        Predlozhenie predl = null;
        int pozition_predl = 0;
        Slovo slovo;

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
            AnalyzeCharacteristicsFromNoun(pr, place, ref slovo);
            AnalyzeGlagol(pr, place, ref slovo);
            SetExtraData(ref slovo);
        }

        private void GetTranslate(Predlozhenie pr, int place, ref Slovo slovo)
        {
            this.slovo = slovo;
            SQLiteConnection connection = new SQLiteConnection(@"Data Source=dict.sqlitedb;Version=3;");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "SELECT eng FROM dict WHERE el=@el";
            command.Parameters.Add(new SQLiteParameter("el", slovo.eSlovo));
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows && reader.Read() && !reader.IsDBNull(0))
            {
                slovo.enSlovo.slovo = Processing(reader.GetString(0), pr, place);
            }
            else
            {
                slovo.enSlovo.slovo = " [Нет перевода для " + slovo.eSlovo + "] ";
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
            glagol.chislo = slovo.chislo;
            glagol.lico = slovo.lico;
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
            switch (temp[0][0])
            {
                case 'T':
                    slovo.vremya = Vremya.Nastoyaschee;
                    break;
                case 'Y':
                    slovo.vremya = Vremya.Proshedshee;
                    break;
                case 'U':
                    slovo.vremya = Vremya.Buduschee;
                    break;
                default: break;
            }
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
            /*else
                slovo.eSlovo = OsnovaElPrich(slovo.eSlovo);*/

            logOsnovaEl += "\n***Характеристики*** " + "\nЧисло: " + slovo.chislo.ToString() + "\nНаклонение: " + slovo.naklonenie.ToString() + "\nРод: " + slovo.rod.ToString() + "\nСостояние: " + slovo.sostoynie.ToString() + "\nВид: " + slovo.vid.ToString() + "\nВремя: " + slovo.vremya.ToString() + "\nЗалог: " + slovo.zalog.ToString();
        }

        /*private string OsnovaElPrich(string p)
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
               /* if (osnova[osnova.Length - 1] == 'A')
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
           /*     osnova = osnova.Remove(0, 1);
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
            GetTranslate(pr, place, ref analyzed);
            //Soglas_Gram(pr, place);
            return analyzed;
        }

        string Processing(string infin, Predlozhenie pr, int place)
        {
            string stroka = infin;
            string stroka_copy = stroka;
            Soglas_Gram(pr, place);
            try
            {
                if (slovo.zalog == Zalog.Dejstvitekniy)
                {
                    if (slovo.vid == Vid.Indefinite)
                    {
                        // преобразования для Indefenite
                        if (slovo.vremya == Vremya.Present)
                        {
                            if (stroka == "be")
                            {
                                if (slovo.chislo == Chislo.Edinstvennoe)
                                {
                                    if (slovo.lico == Lico.Первое)
                                        stroka = "am";
                                    else
                                        stroka = "is";
                                }
                                else
                                    stroka = "are";
                            }
                            else if (slovo.lico == Lico.Третье && slovo.chislo == Chislo.Edinstvennoe)
                            {
                                    char[] slovo1 = stroka.ToCharArray();
                                    if (slovo1[slovo1.Length - 1] == 's' || slovo1[slovo1.Length - 1] == 'x' || slovo1[slovo1.Length - 1] == 'z' || slovo1[slovo1.Length - 1] == 'o' || slovo1[slovo1.Length - 1] == 'h' && slovo1[slovo1.Length - 2] == 's' || slovo1[slovo1.Length - 1] == 'h' && slovo1[slovo1.Length - 2] == 'c')
                                        stroka = stroka + "es";
                                    else if (slovo1[slovo1.Length - 1] == 'f')
                                    {
                                        stroka = slovo1[0].ToString();
                                        for (int i = 1; i < slovo1.Length - 1; i++)
                                            stroka = stroka + slovo1[i].ToString();
                                        stroka = stroka + "ves";
                                    }
                                    else if (slovo1[slovo1.Length - 1] == 'e' && slovo1[slovo1.Length - 2] == 'f')
                                    {
                                        stroka = slovo1[0].ToString();
                                        for (int i = 1; i < slovo1.Length - 2; i++)
                                            stroka = stroka + slovo1[i].ToString();
                                        stroka = stroka + "ves";
                                    }
                                    else if (slovo1[slovo1.Length - 1] == 'y' && slovo1[slovo1.Length - 2] > 97 && slovo1[slovo1.Length - 2] < 101
                                        || slovo1[slovo1.Length - 1] == 'y' && slovo1[slovo1.Length - 2] > 101 && slovo1[slovo1.Length - 2] < 105
                                        || slovo1[slovo1.Length - 1] == 'y' && slovo1[slovo1.Length - 2] > 105 && slovo1[slovo1.Length - 2] < 111
                                        || slovo1[slovo1.Length - 1] == 'y' && slovo1[slovo1.Length - 2] > 111 && slovo1[slovo1.Length - 2] < 117
                                        || slovo1[slovo1.Length - 1] == 'y' && slovo1[slovo1.Length - 2] > 117 && slovo1[slovo1.Length - 2] < 121
                                        || slovo1[slovo1.Length - 1] == 'y' && slovo1[slovo1.Length - 2] == 'z')
                                    {
                                        stroka = slovo1[0].ToString();
                                        for (int i = 1; i < slovo1.Length - 1; i++)
                                            stroka = stroka + slovo1[i].ToString();
                                        stroka = stroka + "ies";
                                    }
                                    else
                                        stroka = stroka + 's';
                            }
                        }
                        else if (slovo.vremya == Vremya.Future)
                        {
                                if (slovo.lico == Lico.Первое)
                                    stroka = "shall " + stroka;
                                else
                                    stroka = "will " + stroka;
                        }
                        else if (slovo.vremya == Vremya.Past)
                        {
                                // образование второй формы глагола (V2)
                                SQLiteConnection con = new SQLiteConnection(@"Data Source=non_verbs.sqlitedb;Version=3;");
                                con.Open();
                                SQLiteCommand com = new SQLiteCommand(con);
                                com.CommandText = "SELECT v2 FROM verbs WHERE v1=@str";
                                com.Parameters.Add(new SQLiteParameter("str", stroka));
                                SQLiteDataReader reader = com.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    if (!reader.IsDBNull(0))
                                    {
                                        stroka = reader.GetString(0);
                                        if (stroka == "was,were")
                                        {
                                            if (slovo.chislo == Chislo.Mnozhestvennoe)
                                                stroka = "were";
                                            else
                                                stroka = "was";
                                        }
                                    }
                                }
                                else
                                    stroka = V_ed(stroka);
                                reader.Close();
                                con.Close();
                        }
                        /* ??????????????????????????????????????????????????????????????????????????*/
                         else if (slovo.vremya == Vremya.Future_Past)
                         {
                                if (slovo.lico == Lico.Первое)
                                     stroka = "should " + stroka;
                                 else
                                     stroka = "would " + stroka;
                         }
                        // преобразования для Indefenite
                    }
                    else if (slovo.vid == Vid.Continuous)
                    {
                        // преобразования для Continuous
                        if (slovo.vremya == Vremya.Present)
                        {
                            if (slovo.chislo == Chislo.Edinstvennoe)
                            {
                                if (slovo.lico == Lico.Первое)
                                    stroka = "am ";
                                else
                                    stroka = "is ";
                            }
                            else
                                stroka = "are ";
                        }
                        else if (slovo.vremya == Vremya.Future)
                        {
                                if (slovo.lico == Lico.Первое)
                                    stroka = "shall be ";
                                else
                                    stroka = "will be ";
                        }
                        else if (slovo.vremya == Vremya.Past)
                        {
                            if (slovo.chislo == Chislo.Mnozhestvennoe)
                                stroka = "were ";
                            else
                                stroka = "was ";
                        }
                        else if (slovo.vremya == Vremya.Future_Past)
                         {
                              {
                                 if (slovo.lico == Lico.Первое)
                                     stroka = "should be ";
                                 else
                                     stroka = "would be ";
                             }
                         }
                        stroka = stroka + V_ing(stroka_copy);
                    }
                    else if (slovo.vid == Vid.Perfect)
                    {
                        // преобразования для Perfect
                        if (slovo.vremya == Vremya.Present)
                        {
                            if (slovo.chislo == Chislo.Edinstvennoe && slovo.lico == Lico.Третье)
                                stroka = "has ";
                            else
                                stroka = "have ";
                        }
                        else if (slovo.vremya == Vremya.Future)
                        {
                                if (slovo.lico == Lico.Первое)
                                    stroka = "shall have ";
                                else
                                    stroka = "will have ";
                        }
                        else if (slovo.vremya == Vremya.Past)
                        {
                            stroka = "had ";
                        }
                         else if (slovo.vremya == Vremya.Future_Past)
                         {
                             {
                                 if (slovo.lico == Lico.Первое)
                                     stroka = "should have ";
                                 else
                                     stroka = "would have ";
                             }
                         }
                        stroka = stroka + Viii(stroka_copy);
                    }
                    else if (slovo.vid == Vid.Perfect_Continuous)
                    {
                        // преобразование для Perfect-Continuous
                        if (slovo.vremya == Vremya.Present)
                        {
                            if (slovo.chislo == Chislo.Edinstvennoe && slovo.lico == Lico.Третье)
                                stroka = "has ";
                            else
                                stroka = "have ";
                        }
                        else if (slovo.vremya == Vremya.Future)
                        {
                                if (slovo.lico == Lico.Первое)
                                    stroka = "shall have ";
                                else
                                    stroka = "will have ";
                        }
                        else if (slovo.vremya == Vremya.Past)
                        {
                            stroka = "had ";
                        }
                        else if (slovo.vremya == Vremya.Future_Past)
                         {
                             {
                                 if (slovo.lico == Lico.Первое)
                                     stroka = "should have ";
                                 else
                                     stroka = "would have ";
                             }
                         }
                        stroka = stroka + "been " + V_ing(stroka_copy);
                    }
                }
                //---------------------------------------------------------------------------------------------
                else if (slovo.zalog == Zalog.Stradatelniy)
                {
                    // преобразование слова для страдательного залога
                    if (slovo.vid == Vid.Indefinite)
                    {
                        // преобразования для Indefenite
                        if (slovo.vremya == Vremya.Present)
                        {
                            if (slovo.chislo == Chislo.Edinstvennoe)
                            {
                                if (slovo.lico == Lico.Первое)
                                    stroka = "am ";
                                else
                                    stroka = "is ";
                            }
                            else
                                stroka = "are ";
                        }
                        else if (slovo.vremya == Vremya.Future)
                        {
                                if (slovo.lico == Lico.Первое)
                                    stroka = "shall be ";
                                else
                                    stroka = "will be ";
                        }
                        else if (slovo.vremya == Vremya.Past)
                        {
                            if (slovo.chislo == Chislo.Mnozhestvennoe)
                                stroka = "were ";
                            else
                                stroka = "was ";
                        }
                        else if (slovo.vremya == Vremya.Future_Past)
                          {
                              {
                                  if (slovo.lico == Lico.Первое)
                                      stroka = "should be ";
                                  else
                                      stroka = "would be ";
                              }
                              // преобразования для Indefenite
                          }
                    }
                    else if (slovo.vid == Vid.Continuous)
                    {
                        // преобразования для Continuous
                        if (slovo.vremya == Vremya.Present)
                        {
                            if (slovo.chislo == Chislo.Edinstvennoe)
                            {
                                if (slovo.lico == Lico.Первое)
                                    stroka = "am being ";
                                else
                                    stroka = "is being ";
                            }
                            else
                                stroka = "are being ";
                        }
                        else if (slovo.vremya == Vremya.Future)
                        {
                            stroka = "!!! EROR !!!";
                        }
                        else if (slovo.vremya == Vremya.Past)
                        {
                            if (slovo.chislo == Chislo.Mnozhestvennoe)
                                stroka = "were being ";
                            else
                                stroka = "was being ";
                        }
                        else if (slovo.vremya == Vremya.Future_Past)
                         {
                             stroka = "!!! EROR !!!";
                         }
                    }
                    else if (slovo.vid == Vid.Perfect)
                    {
                        // преобразования для Perfect
                        if (slovo.vremya == Vremya.Present)
                        {
                                if (slovo.chislo == Chislo.Edinstvennoe && slovo.lico == Lico.Третье)
                                    stroka = "has been ";
                                else
                                    stroka = "have been ";
                         }
                        else if (slovo.vremya == Vremya.Future)
                        {
                               if (slovo.lico == Lico.Первое)
                                    stroka = "shall have been ";
                                else
                                    stroka = "will have been ";
                        }
                        else if (slovo.vremya == Vremya.Past)
                        {
                            stroka = "had been ";
                        }
                        else if (slovo.vremya == Vremya.Future_Past)
                         {
                             {
                                 if (slovo.lico == Lico.Первое)
                                     stroka = "should have been ";
                                 else
                                     stroka = "would have been ";
                             }

                         }
                    }
                    else if (slovo.vid == Vid.Perfect_Continuous)
                    {
                        stroka = "!!! EROR !!!";
                    }
                    if (stroka != "!!! EROR !!!")
                        stroka = stroka + Viii(stroka_copy);
                }
            }
            catch
            { }
            return stroka;
        }
        
        public string V_ed(string stroka)
        // метод преобразования глагола при добавлении окончания "-ed"
        {
            char[] slovo = stroka.ToCharArray();
            if (slovo[slovo.Length - 1] == 'e')
                stroka = stroka + "d";
            else if (slovo[slovo.Length - 1] == 'y')
            {
                stroka = slovo[0].ToString();
                for (int i = 1; i < slovo.Length - 1; i++)
                    stroka = stroka + slovo[i].ToString();
                stroka = stroka + "ied";
            }
            else if (slovo[slovo.Length - 1] == 'd' && slovo[slovo.Length - 2] != 'd' || slovo[slovo.Length - 1] == 'p' && slovo[slovo.Length - 2] != 'p' || slovo[slovo.Length - 1] == 't' && slovo[slovo.Length - 2] != 't' || slovo[slovo.Length - 1] == 'l' && slovo[slovo.Length - 2] != 'l')
            {
                if (slovo[slovo.Length - 2] == 'o' || slovo[slovo.Length - 1] == 'a' || slovo[slovo.Length - 1] == 'e' || slovo[slovo.Length - 1] == 'u' || slovo[slovo.Length - 1] == 'i' || slovo[slovo.Length - 1] == 'y')
                {
                    if (slovo[slovo.Length - 3] != 'o' || slovo[slovo.Length - 3] != 'a' || slovo[slovo.Length - 3] != 'e' || slovo[slovo.Length - 3] != 'u' || slovo[slovo.Length - 3] != 'i' || slovo[slovo.Length - 3] != 'y')
                        stroka = stroka + slovo[slovo.Length - 1] + "ed";
                }
                else
                    stroka = stroka + "ed";
            }
            else
                stroka = stroka + "ed";
            return stroka;
        }

        public string Viii(string stroka)
        {
            // метод преобразования глагола в 3-ью форму глагола
            SQLiteConnection con = new SQLiteConnection(@"Data Source=non_verbs.sqlitedb;Version=3;");
            con.Open();
            SQLiteCommand com = new SQLiteCommand(con);
            com.CommandText = "SELECT v3 FROM verbs WHERE v1=@str";
            com.Parameters.Add(new SQLiteParameter("str", stroka));
            SQLiteDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                if (!reader.IsDBNull(0))
                {
                    stroka = reader.GetString(0);
                }
            }
            else
                stroka = V_ed(stroka);
            reader.Close();
            con.Close();
            return stroka;
        }

        public string V_ing(string stroka)
        // метод преобразования глагола при добавлении окончания "-ing"
        {
            if (stroka != "be")
            {
                SQLiteConnection con = new SQLiteConnection(@"Data Source=non_verbs.sqlitedb;Version=3;");
                con.Open();
                SQLiteCommand com = new SQLiteCommand(con);
                com.CommandText = "SELECT v4 FROM verbs WHERE v1=@str";
                com.Parameters.Add(new SQLiteParameter("str", stroka));
                SQLiteDataReader reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    if (!reader.IsDBNull(0))
                    {
                        stroka = reader.GetString(0);
                    }
                }
                else
                {
                    char[] slovo = stroka.ToCharArray();
                    if (slovo[slovo.Length - 1] == 'e')
                    {
                        for (int i = 0; i < slovo[slovo.Length - 1]; i++)
                            stroka = stroka + slovo[i].ToString();
                        stroka = stroka + "ing";
                    }
                    else if (slovo[slovo.Length - 1] == 'e' && slovo[slovo.Length - 2] == 'i')
                    {
                        stroka = slovo[0].ToString();
                        for (int i = 1; i < slovo.Length - 2; i++)
                            stroka = stroka + slovo[i].ToString();
                        stroka = stroka + "ying";
                    }
                    else if (slovo[slovo.Length - 1] == 'n' && slovo[slovo.Length - 2] != 'n' || slovo[slovo.Length - 1] == 'p' && slovo[slovo.Length - 2] != 'p' || slovo[slovo.Length - 1] == 't' && slovo[slovo.Length - 2] != 't' || slovo[slovo.Length - 1] == 'l' && slovo[slovo.Length - 2] != 'l')
                    {
                        if (slovo[slovo.Length - 2] == 'o' || slovo[slovo.Length - 1] == 'a' || slovo[slovo.Length - 1] == 'e' || slovo[slovo.Length - 1] == 'u' || slovo[slovo.Length - 1] == 'i' || slovo[slovo.Length - 1] == 'y')
                        {
                            if (slovo[slovo.Length - 3] != 'o' || slovo[slovo.Length - 3] != 'a' || slovo[slovo.Length - 3] != 'e' || slovo[slovo.Length - 3] != 'u' || slovo[slovo.Length - 3] != 'i' || slovo[slovo.Length - 3] != 'y')
                                stroka = stroka + slovo[slovo.Length - 1] + "ing";
                        }
                        else
                            stroka = stroka + "ing";
                    }
                    else
                        stroka = stroka + "ing";
                }
                reader.Close();
                con.Close();
            }
            return stroka;
        }

       /* public void Vremia_eng()
        {
            switch (slovo.vremya)
            {
                case Vremya.BuduscheeDalekoe:
                case Vremya.BuduscheeDlitelnoe:
                case Vremya.BuduscheeVNastoyaschem:
                case Vremya.BuduscheeSProshedshim:
                    slovo.vremya = Vremya.Buduschee;
                    break;
                case Vremya.NastoyascheeDlitelnoe:
                case Vremya.NastoyascheeSBuduschim:
                case Vremya.NastoyascheeSProshedshim:
                case Vremya.NastoyascheeVNastoyaschem:
                case Vremya.Postoyannoe:
                    slovo.vremya = Vremya.Nastoyaschee;
                    break;
                case Vremya.ProshedsheeBuduscheeBezNastoyaschego:
                case Vremya.ProshedsheeDlitelnoe:
                case Vremya.ProshedsheeSNastoyaschim:
                case Vremya.Davnoproshedshee:
                    slovo.vremya = Vremya.Proshedshee;
                    break;
                default:
                    break;
            }
        }*/

        protected void AnalyzeCharacteristicsFromNoun(Predlozhenie pr, int place, ref Slovo analyzed)
        {
            // поиск в предложении существительного
            //и копировaние его хaрaктеристик (числa)
            int max = place + 4 > pr.Count - 1 ? pr.Count - 1 : place + 4;
            int min = place - 4 > 0 ? place - 4 : 0;
            int such = 0;

            if (analyzed.chastRechi == ChastRechi.Glagol && pr.Count > 1)
            {
                Hashtable sush = new Hashtable();
                for (int i = min; i < max; i++)
                {
                    Slovo slovpoisk = pr[i];
                    if (slovpoisk.chastRechi == ChastRechi.Suschestvitelnoe)
                    {
                        sush[i] = slovpoisk; such = 1;
                    }
                    else if (slovpoisk.chastRechi == ChastRechi.Mestoimenie)
                        such = 2;
                }
                Slovo res = null;
                int minres = 999;

                foreach (int j in sush.Keys)
                {
                    if (Math.Abs(place - j) < minres || (Math.Abs(place - j) == minres && j > place))
                    {
                        res = (Slovo)sush[j];
                        minres = Math.Abs(place - j);
                    }
                }
                if (res != null)
                {
                    if (such == 1)
                    {
                        analyzed.chislo = res.chislo;
                        analyzed.lico = Lico.Третье;
                    }
                    else if (such == 2)
                    {
                        if (res.eSlovo == "Q-" || res.eSlovo == "Q-Q") //местоимение "Я"
                        {
                            analyzed.chislo = Chislo.Edinstvennoe;
                            analyzed.lico = Lico.Первое;
                        }
                        else if (res.eSlovo == "Y") //местоимение "Мы"
                        {
                            analyzed.chislo = Chislo.Mnozhestvennoe;
                            analyzed.lico = Lico.Первое;
                        }
                        else if (res.eSlovo == "W" || res.eSlovo == "U") //местоимение "Ты,Вы"
                        {
                            analyzed.chislo = Chislo.Mnozhestvennoe;
                            analyzed.lico = Lico.Второе;
                        }
                        else if (res.eSlovo == "E-" || res.eSlovo == "T-" || res.eSlovo == "R-") //местоимение "Он,Она,Оно"
                        {
                            analyzed.chislo = Chislo.Edinstvennoe;
                            analyzed.lico = Lico.Третье;
                        }
                        else if (res.eSlovo == "I-") //местоимение "Они"
                        {
                            analyzed.chislo = Chislo.Mnozhestvennoe;
                            analyzed.lico = Lico.Третье;
                        }
                    }
                }
                else
                {
                    analyzed.chislo = Chislo.Edinstvennoe;
                    analyzed.lico = Lico.Третье;
                }
                if (analyzed.chislo == Chislo.Odinochnoe || analyzed.chislo == Chislo.Odinochnoe)
                    analyzed.chislo = Chislo.Edinstvennoe;
                else if (analyzed.chislo == Chislo.Neopredelennoe || analyzed.chislo == Chislo.Mnogoobraznoe)
                    analyzed.chislo = Chislo.Mnozhestvennoe;
            }

        }

        protected void Soglas_Gram(Predlozhenie pr, int place)
        {
            bool fl = false; bool fl1 = false;
            if (slovo.zalog == Zalog.Dejstvitekniy || slovo.zalog == Zalog.Vzaimniy || slovo.zalog == Zalog.Vozvratniy)
                slovo.zalog = Zalog.Dejstvitekniy;
            else
                slovo.zalog = Zalog.Stradatelniy;
            int max = place + 4 > pr.Count - 1 ? pr.Count - 1 : place + 4;
            int min = place - 4 > 0 ? place - 4 : 0;
            //int such = 0;
            if (slovo.chastRechi == ChastRechi.Glagol && pr.Count > 1)
            {
                // * определение времени и формы глагола по соответствующим им словам в предложении *
                int i = min;
                while (i < max + 1)
                {
                    Slovo slovpoisk = pr[i];
                    if (slovpoisk.enSlovo != slovo.enSlovo)
                    {
                        if (slovpoisk.enSlovo.ToString().ToString() == "A-RB" || slovpoisk.enSlovo.ToString().ToString() == "P-RBI" || slovpoisk.enSlovo.ToString().ToString() == "E-TV")
                        {
                            fl = true; slovo.vid = Vid.Indefinite; slovo.vremya = Vremya.Present; i = max;
                        }
                        else if (slovpoisk.enSlovo.ToString() == "E-S" || slovpoisk.enSlovo.ToString() == "A-S" || slovpoisk.enSlovo.ToString() == "E-1-Y" || slovpoisk.enSlovo.ToString() == "P-YBU")
                        {
                            fl1 = false;
                            for (int y = 0; y < i; y++)
                            {
                                Slovo sl = pr[y];
                                if (sl.enSlovo.ToString() == "Q-RBY")
                                    slovo.vid = Vid.Continuous;
                                else if (sl.enSlovo.ToString() == "FQ")
                                    slovo.vid = Vid.Perfect;
                                else
                                    slovo.vid = Vid.Indefinite;
                            }
                            slovo.vremya = Vremya.Past; fl = true; i = max;
                        }
                        else if (slovpoisk.enSlovo.ToString() == "Q-RBY" && pr[i - 1].enSlovo.ToString() == "A-W" || slovpoisk.enSlovo.ToString() == "Q-RBY" && pr[i - 1].enSlovo.ToString() == "A-T" || slovpoisk.enSlovo.ToString() == "Q-RBY" && pr[i - 1].enSlovo.ToString() == "A-Q" || slovpoisk.enSlovo.ToString() == "P-EXY" || slovpoisk.eSlovo.ToString() == "P-TB")
                        {
                            fl = true; slovo.vremya = Vremya.Present; slovo.vid = Vid.Continuous; i = max;
                        }
                        else if (slovpoisk.enSlovo.ToString() == "P-UBU" || slovpoisk.enSlovo.ToString() == "E-UB" || slovpoisk.enSlovo.ToString() == "E-F" || slovpoisk.enSlovo.ToString() == "I-F")
                        {
                            fl1 = false;
                            for (int y = 0; y < i; y++)
                            {
                                Slovo sl = pr[y];
                                if (sl.enSlovo.ToString() == "E-UCY" || sl.enSlovo.ToString() == "E-SCI" || sl.enSlovo.ToString() == "Q-UCY" || sl.enSlovo.ToString() == "till")
                                    slovo.vid = Vid.Continuous;
                                else if (pr[i].enSlovo.ToString() == "A-W" || pr[i].enSlovo.ToString() == "A-T" || pr[i].enSlovo.ToString() == "A-Q")
                                    slovo.vid = Vid.Perfect;
                                else
                                    slovo.vid = Vid.Indefinite;
                            }
                            slovo.vremya = Vremya.Future; fl = true; i = max;
                        }
                        /*else if (slovpoisk.enSlovo.ToString() == "already" || slovpoisk.enSlovo.ToString() == "ever" || slovpoisk.enSlovo.ToString() == "never")
                        {
                            fl = true; slovo.vid = Vid.Perfect; slovo.vremya = Vremya.Present; i = max;
                        }*/
                    } i++;
                }

            }
            //* определение времени и формы глагола по характеристикам, угаследоаным с эльюнди *
            if (fl == false)
            {
                switch (slovo.vremya)
                {
                    case Vremya.Buduschee:
                        slovo.vremya = Vremya.Future;
                        slovo.vid = Vid.Indefinite;
                        break;
                    case Vremya.NastoyascheeDlitelnoe:
                        slovo.vremya = Vremya.Present;
                        slovo.vid = Vid.Continuous;
                        break;
                    case Vremya.Proshedshee:
                    case Vremya.ProshedsheeDlitelnoe:
                    case Vremya.ProshedsheeBuduscheeBezNastoyaschego:
                        slovo.vremya = Vremya.Past;
                        break;
                    case Vremya.ProshedsheeSNastoyaschim:
                    case Vremya.Davnoproshedshee:
                        slovo.vid = Vid.Perfect;
                        slovo.vremya = Vremya.Present;
                        break;
                    case Vremya.BuduscheeDlitelnoe:
                    case Vremya.BuduscheeVNastoyaschem:
                    case Vremya.BuduscheeSProshedshim:
                    case Vremya.BuduscheeDalekoe:
                        slovo.vremya = Vremya.Future;
                        break;
                    default:
                        slovo.vremya = Vremya.Present;
                        break;
                }
                if (slovo.vremya == Vremya.Present && slovo.vid == Vid.NeopredelennayaDlitelnost)
                    slovo.vid = Vid.Continuous;
                else
                switch (slovo.vid)
                {
                    case Vid.NevozvratnayaZavershennost:
                        slovo.vid = Vid.Perfect;
                        break;
                    default:
                        slovo.vid = Vid.Indefinite;
                        break;
                }
            }
           //slovo.enSlovo.slovo = Processing(slovo.enSlovo.ToString());
        }


    }
}
