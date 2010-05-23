/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 04.05.2009
 * Time: 8:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SQLite;
using System.Data;

namespace ETRTranslator
{
	public class GlagDict : Dictionary
    {
        public  GlagDict() { }
        /// <summary>
        /// ищет в словаре перевод слова с эльюнди на русский 
        /// </summary>
        /// <param name="el">искомое слово</param>
        /// <returns>русский перевод</returns>
        public string TranslateEl(string el)
        {
            string a="";
			string connectionString = "Data Source=dict.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT rus, el FROM dict WHERE el='"+el+"'";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
                a = reader["rus"].ToString();             
			}
			conn.Close();
			return a;		
        }        
        public string TranslateRus(string rus)
        {
            string a = "";
            string connectionString = "Data Source=dict.sqlitedb";
            System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();
            SQLiteCommand command = new SQLiteCommand(conn);
            command.CommandText = "SELECT rus, el FROM dict WHERE rus='" + rus + "'";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                a = reader["el"].ToString();
            }
            conn.Close();
            return a;
        }
    }
    public class GlagModule : IModule
    {
        enum Haract { глагол, деепричастие }
        Haract haract;
        Predlozhenie predl = null;
        int pozition_predl = 0;
        Slovo c;
        /// <summary>
        /// выделение основы эльюнди
        /// </summary>
        string logOsnovaEl;
        string logFormRus;
        int lico;
        public string LogOsnovaEl { get { return logOsnovaEl; } }
        public string LogFormRus { get { return logFormRus; } }
        public Slovo SLOVO { get { return c; } }
        public GlagModule() { }
        /// <summary>
        /// меняет лицо
        /// </summary>
        /// <param name="a">лицо</param>
        void LicoConvert()
        {
            /*switch (c.lico)
            {
                case Lico.Первое: lico = 1; break;
                case Lico.Второе: lico = 2; break;
                case Lico.Третье: lico = 3; break;
            }*/
        }
        public GlagModule(Slovo slovo, bool haract)//для тестирования
        {
            if (slovo != null)
            {
                if (haract) this.haract = Haract.глагол;
                else this.haract = Haract.деепричастие;
                this.c = slovo;
                LicoConvert();
                c.rSlovo = RusForm(slovo.rSlovo);
            }
        }
        public class GlagDict : Dictionary
        {
            public GlagDict() { }
            /// <summary>
            /// ищет в словаре перевод слова с эльюнди на русский 
            /// </summary>
            /// <param name="el">искомое слово</param>
            /// <returns>русский перевод</returns>
            public string TranslateEl(string el)
            {
                string a = "";
                string connectionString = "Data Source=dict.sqlitedb";
                System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
                conn.Open();
                SQLiteCommand command = new SQLiteCommand(conn);
                command.CommandText = "SELECT rus, el FROM dict WHERE el='" + el + "'";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    a = reader["rus"].ToString();
                }
                conn.Close();
                return a;
            }
            public string TranslateRus(string rus)
            {
                string a = "";
                string connectionString = "Data Source=dict.sqlitedb";
                System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
                conn.Open();
                SQLiteCommand command = new SQLiteCommand(conn);
                command.CommandText = "SELECT rus, el FROM dict WHERE rus='" + rus + "'";
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    a = reader["el"].ToString();
                }
                conn.Close();
                return a;
            }
        }
        public class ZaliznyakMy : Zaliznyak
        {
            public ZaliznyakMy() { info = 1; }
            int info;
            /// <summary>
            /// 0 - слова нет в словаре, 1 - вид не удалось определить и по умолчанию соверш., 2 - вид определён
            /// </summary>
            public int Info { get { return info; } }
            public Vid VidGlag(string slovo)
            {
                ArrayList temp;
                temp = GetStrict(slovo.ToLower());
                if (temp.Count > 0)
                {
                    string SlovoRus = (string)temp[0];
                    //определяем основу
                    Regex reg = new Regex(@"\d{1,2}\s(?<вид>(св)|(нсв))\s.*");
                    Match a = reg.Match(SlovoRus.Split(';')[1]);
                    if (a.Success) { info = 2; if (a.Groups["вид"].Value == "св") return Vid.Zavershennost; else return Vid.PostoyannayaDlitelnost; }
                    else info = 1;
                }
                else info = 0;
                return Vid.Zavershennost;
            }
        }
        /// <summary>
        /// определяет, явл. ли посл.буква гласной
        /// </summary>
        /// <param name="slovo">слово на русском языке</param>
        /// <returns>true - буква гласная</returns>
        private bool Glasnay(string slovo)
        {
            if (slovo != "")
            {
                char t = slovo[slovo.Length - 1];
                if (t == 'а' || t == 'о' || t == 'у' || t == 'и' || t == 'ы' || t == 'э' || t == 'е' || t == 'ё' || t == 'ю' || t == 'я') return true;
            }
            return false;
        }
        private bool Glasnay(char t)
        {
            if (t == 'а' || t == 'о' || t == 'у' || t == 'и' || t == 'ы' || t == 'э' || t == 'е' || t == 'ё' || t == 'ю' || t == 'я') return true;
            return false;
        }
        /// <summary>
        /// замена буквы в позициии на новую букву
        /// </summary>
        /// <param name="original">оригиналное слово</param>
        /// <param name="litera">новая буква</param>
        /// <param name="pozition">позиция замены</param>
        /// <returns></returns>
        public static string ZamLit(string original, char litera, int pozition)
        {
            if (pozition < original.Length)
            {
                char[] newstr = original.ToCharArray();
                newstr[pozition] = litera;
                original = "";
                foreach (char a in newstr) original += a.ToString();
            }
            return original;
        }
        public static string ZamLit(string original, char litera)
        {
            char[] newstr = original.ToCharArray();
            newstr[original.Length - 1] = litera;
            original = "";
            foreach (char a in newstr) original += a.ToString();
            return original;
        }
        /// <summary>
        /// замена послед. буквы на новую букву, заданную параметром чередования (! если имеется. !Удвоенность исключается)
        /// </summary>
        /// <param name="original">оригиналное слово</param>
        /// <param name="черёд">параметр чередования</param>
        /// <returns></returns>
        public static string ZamLit(string original, string черёд)
        {
            Regex a = new Regex(@"\(\-(?<буква>[а-я]*)\-\)");
            if (a.Match(черёд).Success)
            {
                string temp = original;
                original = "";
                for (int b = 0; b < temp.Length - 1; b++) original += temp[b].ToString();
                if (a.Match(черёд).Groups["буква"].Value != original[original.Length - 1].ToString()) original += a.Match(черёд).Groups["буква"].Value;
            }
            return original;
        }
        /// <summary>
        /// выделяет нужную букву (набор букв)
        /// </summary>
        /// <param name="original">набор символов</param>
        /// <param name="priznak">признак: a - выделяет чередование</param>
        /// <returns>возвращает нужную букву (набор букв)</returns>
        private string Lit(string litera, char priznak)
        {
            switch (priznak)
            {
                case 'a':
                    Regex a = new Regex(@"\(\-(?<буква>[а-я]*)\-\)");
                    if (a.Match(litera).Success)
                    {
                        return a.Match(litera).Groups["буква"].Value;
                    }
                    break;
            }
            return "";
        }
        static Match OsEl(string el, string suffix)
        {
            Regex reg = new Regex(@"(?<часть1>.*)(?<часть2>"+suffix+")(?<часть3>.*)");
            return reg.Match(el);
        }
        /// <summary>
        /// ставит глагол на Эльюнди в начальную форму(необходимо для поиска слова в словаре-там всё в инфинитиве)
        /// </summary>
        /// <param name="el">глагол</param>
        /// <returns>возвращает инфинитив</returns>
        public static string OsnovaEl(string el)
        {
            if(el.Length<3)return el;
            string logOsnovaEl = "\nсоздание основы для " + el;
            string temp = el;
            Match match=null;
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
                temp=TireSame(temp);
            }
            if(temp[2]=='-')
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
            if(temp[temp.Length-2]=='-')
                switch (temp[temp.Length-1])
                {
                    case 'A':
                        logOsnovaEl += "\nудалён суффикс возвратного залога 'A'";
                        temp = temp.Substring(0,temp.Length-2);
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
        static string TireSame(string str)
        {            
            string temp=str;
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
        bool Pom(ArrayList arPometa, string pometa)
        {
            if (arPometa == null) return false;
            if (arPometa.Count > 0)
            {
                for (int i = 1; i < arPometa.Count; i++) if (arPometa[i].ToString() == pometa) return true;
            }
            return false;
        }
        /// <summary>
        /// постановка русского слова в правильную форму (с учётом спряжения и т.д.)
        /// </summary>
        /// <param name="SlovoRus">слово на русском</param>
        /// <returns>возвращает слово в требуемой форме</returns>
        string RusForm(string SlovoRus)
        {
        	if (c.naklonenie == Naklonenie.Infinitiv)
        	{
        		if(c.zalog == Zalog.Vozvratniy)
        			return SlovoRus+"ся";
        		else return SlovoRus;
        	}
            Regex check = new Regex(@"\s");
            if (check.Match(SlovoRus).Success) return SlovoRus;
            string newSlovoRus = SlovoRus;
            ArrayList temp;
            ArrayList arPometa = new ArrayList();//полная помета в первом элементе массива  
            Zaliznyak zal = new Zaliznyak();
            temp = zal.GetStrict(newSlovoRus.ToLower());
            if (temp.Count > 0)
            {
                newSlovoRus = (string)temp[0];
                //определяем основу
                string osnova = newSlovoRus.Split(';')[0];
                ZaliznyakMy zmy = new ZaliznyakMy();
                logFormRus += "\nвид скоректирован для перевода как: " + (c.vid = zmy.VidGlag(osnova)).ToString();
                if (osnova.Length > 4) if (osnova.Substring(osnova.Length - 2) == "ся" || osnova.Substring(osnova.Length - 2) == "сь") { logFormRus += "\nзалог скоректирован для перевода как: " + (c.zalog = Zalog.Vozvratniy).ToString(); osnova = osnova.Substring(0, osnova.Length - 2); }
                if (c.naklonenie == Naklonenie.Soslagatelnoe) { logFormRus += "\nвремя для сослаг.накл. скоректировано для перевода как: " + (c.vremya = Vremya.Proshedshee).ToString(); }
                //выделяем индекс
                string temp1 = newSlovoRus.Split(';')[1];
                logFormRus += "\n***Постановка в нужную форму***:\nПолный индекс: " + temp1 + "\n";
                string temp2 = "";
                Regex regpom = new Regex(@".*\[?(?<помета>.[0-9].)\]?.?");
                string pometa = "";// = regpom.Match(temp1).Groups["помета"].Value;
                if (regpom.Match(temp1).Success)
                {
                    //краткое выделение
                    if (regpom.Match(temp1).Groups["помета"].Value[0] == regpom.Match(temp1).Groups["помета"].Value[2])
                    {
                        if (regpom.Match(temp1).Groups["помета"].Value[0] == '"') pometa = "[" + regpom.Match(temp1).Groups["помета"].Value[1] + "]";
                        else pometa = regpom.Match(temp1).Groups["помета"].Value[1].ToString();
                    }
                    //выделяем полностью помету
                    Regex r = new Regex(@".*(?<помета>(\[[^*а-я/s$][0-9][^*а-я/s$]{2}[0-9][^*а-я/s$]\])|(\[[^*а-я/s$][0-9][^*а-я/s$]\][^*а-я/s$][0-9][^*а-я/s$]))");
                    Match m = r.Match(temp1);
                    if (!m.Success) { r = new Regex(@".*(?<помета>(([^*а-я/s$][0-9][^*а-я/s$])|(\[[^*а-я/s$][0-9][^*а-я/s$]\])){2})"); m = r.Match(temp1); }
                    if (!m.Success) { r = new Regex(@".*(?<помета>(\[[^*\sа-я$][0-9][^*\sа-я$]\]))"); m = r.Match(temp1); }
                    if (!m.Success) { r = new Regex(@".*(?<помета>([^*а-я\s\$][0-9][^*а-я\s$]))"); m = r.Match(temp1); }
                    if (m.Success)
                    {
                        arPometa.Add(m.Groups["помета"].Value);
                        string[] s = m.Groups["помета"].Value.Split('"');
                        foreach (string e in s) { if (e != "")if (Char.IsNumber(e[0]))arPometa.Add(e); }
                    }
                }
                //избавляемся от лишних примечаний
                int j = 0;
                for (; j < temp1.Length; j++)
                {
                    if (temp1[j] == '[') break;
                    temp2 += temp1[j].ToString();
                }
                for (j++; j < temp1.Length; j++) if (temp1[j] == ']') break;
                for (j += 2; j < temp1.Length; j++) temp2 += temp1[j].ToString();
                string[] mas = temp2.Split(',');
                temp2 = mas[0];
                if (mas.Length > 1)
                {
                    Regex a = new Regex(@"[1-9]*\s(св)|(нсв){1}");
                    if (a.Match(temp2).Success)
                    {
                        temp2 += mas[1];
                        a = new Regex(@"(?<часть1>[1-9\w\s]*)\(_[\w\s]*_\)\s(?<часть2>.*)");
                        Match b = a.Match(temp2);
                        if (b.Success) temp2 = b.Groups["часть1"].Value + b.Groups["часть2"].Value;
                    }
                }
                //разбираем выражение
                //Regex reg = new Regex(@"\d{1,2}\s([а-я\-]{1,}\s)*(?<type>\d{1,2})(?<index>[а-я]?)(?<подтип>((\*\*)[а-я]){0,1})(?<черед>(\s\(\-[а-я]{1,}\-\)){0,1})");//0
                //Regex reg = new Regex(@"\d{1,2}\s([а-я\-]{1,}\s)*(?<type>\d{1,2})(?<index>[а-я]?)(/[а-я])?(?<подтип>((\*\*)[а-я]){0,1})(?<черед>(\s\(\-[а-я]{1,}\-\)){0,1})");//1
                //Regex reg = new Regex(@"\d{1,2}\s([а-я\-]{1,}\s)*(?<type>\d{1,2})(?<index>[а-я]?)(/[а-я])?\s?(?<черед>(\(\-[а-я]*\-\))?).{0,}(?<подтип>((\*\*)[а-я])?)\s?\k'черед'?");//2     
                //Regex reg = new Regex(@"\d{1,2}\s([а-я\-]{1,}\s)*(?<type>\d{1,2})(?<index>[а-я]?)((/[а-я]\'{0,2})|(\*[а-я]))?\s?(?<черед>(\(\-[а-я]*\-\))?)(?<буд>(\([а-я]{1,}\-(//[а-я]{1,}\-)?\))?).{0,}(?<подтип>((\*\*)[а-я])?)\s?\k'черед'?");//3               
                Regex reg = new Regex(@"\d{1,2}\s([а-я\-]{1,}\s)*(?<type>\d{1,2})(?<index>[а-я]?)((/[а-я]\'{0,2})|(\*[а-я]))?\s?(?<подтип>((\*\*)[а-я])?)(?<черед>(\(\-[а-я]*\-\))?)(?<буд>(\([а-я]{1,}\-(//[а-я]{1,}\-)?\))?).{0,}\s?\k'черед'?");//4              
                Match match = reg.Match(temp2);
                logFormRus += "\nВходной индекс: " + temp2 + "\nРабочий индекс: " + match.Value + "\n***Разбор индекса***\nиндекс: " + match.Groups["index"].Value + "\nподтип: " + match.Groups["подтип"].Value + "\nтип: " + match.Groups["type"].Value.ToString() + "\nчередование: " + match.Groups["черед"].Value + "\nоснова буд.: " + match.Groups["буд"].Value + "\nпомета: ";
                if (arPometa.Count > 0) logFormRus += arPometa[0].ToString();
                if (haract == Haract.деепричастие)
                {
                    switch (VremyElRus())
                    {
                        case 0://настоящ
                            int i = 0;
                            if (match.Success) i = Convert.ToInt32(match.Groups["type"].Value);
                            if (!OsnovaProizv(osnova, "тянуть(ся)?"))
                                if (VidElRus() < 2 || (i == 3 || i == 8 || i == 9 || i == 11 || i == 14 || i == 15)) { logFormRus += "\nФормы в настоящем не существует"; return osnova; }
                            switch (i)
                            {
                                case 13:
                                    Match slovo = Osnova(osnova, "(?<окончание>(ать)|(аться))");
                                    osnova = slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value[0].ToString() + "я";
                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                    return osnova;
                                default:
                                    c.vid = Vid.PostoyannayaDlitelnost;//несоверш
                                    lico = 3;
                                    c.chislo = Chislo.Mnozhestvennoe;
                                    break;
                            }
                            break;
                        default://прош
                            if (Pom(arPometa, "9") || ((match.Groups["type"].Value == "7" && match.Groups["черед"].Value == "(-д-)" || match.Groups["type"].Value == "7" && match.Groups["черед"].Value == "(-т-)")) && OsnovaProizv(osnova, "сти(сь)?"))
                            {
                                c.vid = Vid.Zavershennost;
                                c.vremya = Vremya.Buduschee;
                                lico = 1;
                            }
                            else c.rod = Rod.Muzhskoj;
                            c.chislo = Chislo.Edinstvennoe;
                            break;
                    }
                }
                if (match.Success)
                {
                    switch (Convert.ToInt32(match.Groups["type"].Value))//работаем со спряжением
                    {
                        case 1://тип 1
                            Match slovo = Osnova(osnova, "(?<окончание>(ать)|(ять)|(еть)|(аться)|(яться)|(еться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        osnova += slovo.Groups["окончание"].Value[0].ToString() + "й";
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova += slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            default://будущее
                                                osnova += slovo.Groups["окончание"].Value[0].ToString();
                                                switch (ChisloElRus())
                                                {

                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ют";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                case 2://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        osnova += slovo.Groups["окончание"].Value[0].ToString() + "й";
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova += slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "буду " + osnova + slovo.Groups["окончание"].Value; ;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + osnova + slovo.Groups["окончание"].Value; ;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + osnova + slovo.Groups["окончание"].Value; ;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + osnova + slovo.Groups["окончание"].Value; ;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + osnova + slovo.Groups["окончание"].Value; ;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + osnova + slovo.Groups["окончание"].Value; ;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                osnova += slovo.Groups["окончание"].Value[0].ToString();
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ют";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 2://тип 2
                            slovo = Osnova(osnova, "(?<окончание>(овать)|(евать)|(еваться)|(оваться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        switch (slovo.Groups["окончание"].Value)
                                        {
                                            case "овать":
                                                if (match.Groups["index"].Value == "а") osnova += "уй";
                                                else osnova += "уй";
                                                break;
                                            default:
                                                if (Ship(slovo.Groups["основа"].Value)) osnova += "уй";
                                                else osnova += "юй";
                                                break;
                                        }
                                        if (ChisloElRus() > 0) osnova += "те";
                                        if (c.zalog == Zalog.Vozvratniy) { if (ChisloElRus() > 0) osnova += "сь"; else osnova += "ся"; }
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova += "овал"; break;
                                                                    default: osnova += "евал"; break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova += "овала"; break;
                                                                    default: osnova += "евала"; break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova += "овало"; break;
                                                                    default: osnova += "евало"; break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (slovo.Groups["окончание"].Value)
                                                        {
                                                            case "овать": osnova += "овали"; break;
                                                            default: osnova += "евали"; break;
                                                        }
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            default://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova += "ую"; break;
                                                                    default:
                                                                        if (Ship(slovo.Groups["основа"].Value)) osnova += "ую";
                                                                        else osnova += "юю";
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova += "уешь";
                                                                        else osnova += "уёшь";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уешь";
                                                                            else osnova += "юешь";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уёшь";
                                                                            else osnova += "юёшь";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova += "ует";
                                                                        else osnova += "уёт";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "ует";
                                                                            else osnova += "юет";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уёт";
                                                                            else osnova += "юёт";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova += "уем";
                                                                        else osnova += "уём";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уем";
                                                                            else osnova += "юем";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уём";
                                                                            else osnova += "юём";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova += "уете";
                                                                        else osnova += "уёте";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уете";
                                                                            else osnova += "юете";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уёте";
                                                                            else osnova += "юёте";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova += "уют";
                                                                        else osnova += "уют";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уют";
                                                                            else osnova += "юют";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova += "уют";
                                                                            else osnova += "юют";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                case 2://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        switch (slovo.Groups["окончание"].Value)
                                        {
                                            case "овать": osnova += "уй"; break;
                                            default:
                                                if (Ship(slovo.Groups["основа"].Value)) osnova += "уй";
                                                else osnova += "юй";
                                                break;
                                        }
                                        if (ChisloElRus() > 0) osnova += "те";
                                        if (c.zalog == Zalog.Vozvratniy) { if (ChisloElRus() > 0) osnova += "сь"; else osnova += "ся"; }
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova = osnova + "овал"; break;
                                                                    default: osnova = osnova + "евал"; break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova = osnova + "овала"; break;
                                                                    default: osnova = osnova + "евала"; break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova = osnova + "овало"; break;
                                                                    default: osnova = osnova + "евало"; break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (slovo.Groups["окончание"].Value)
                                                        {
                                                            case "овать": osnova = osnova + "овали"; break;
                                                            default: osnova = osnova + "евали"; break;
                                                        }
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "буду " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova = osnova + "ую"; break;
                                                                    default:
                                                                        if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "ую";
                                                                        else osnova = osnova + "юю";
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova = osnova + "уешь";
                                                                        else osnova = osnova + "уёшь";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "уешь";
                                                                            else osnova = osnova + "юешь";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "уёшь";
                                                                            else osnova = osnova + "юёшь";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova = osnova + "ует";
                                                                        else osnova = osnova + "уёт";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "ует";
                                                                            else osnova = osnova + "юет";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "уёт";
                                                                            else osnova = osnova + "юёт";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova = osnova + "уем";
                                                                        else osnova = osnova + "уём";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "уем";
                                                                            else osnova = osnova + "юем";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "уём";
                                                                            else osnova = osnova + "юём";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать":
                                                                        if (match.Groups["index"].Value == "а") osnova = osnova + "уете";
                                                                        else osnova = osnova + "уёте";
                                                                        break;
                                                                    default:
                                                                        if (match.Groups["index"].Value == "а")
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "уете";
                                                                            else osnova = osnova + "юете";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "уёте";
                                                                            else osnova = osnova + "юёте";
                                                                        }
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "овать": osnova += "уют"; break;
                                                                    default:
                                                                        if (Ship(slovo.Groups["основа"].Value)) osnova = osnova + "уют";
                                                                        else osnova = osnova + "юют";
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 3://тип 3
                            slovo = Osnova(osnova, "(?<окончание>(нуть)|(нуться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 2://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        if ((match.Groups["index"].Value == "а" && Glasnay(osnova)) || (match.Groups["подтип"].Value == "**а" && Glasnay(osnova)) || slovo.Groups["основа"].Value == "сты") osnova += "нь";
                                        else osnova += "ни";
                                        osnova = ZamСловоВнутри_or(osnova, "ыни", "ынь", 'a');
                                        if (match.Groups["index"].Value != "") osnova = ZamСловоВнутри_or(osnova, "янь", "яни", 'a');
                                        if (ChisloElRus() > 0) osnova += "те";
                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                        osnova = ZamСловоВнутри_or(osnova, "ьсь", "ься", 'a');
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (match.Groups["подтип"].Value == "**а")
                                                                {
                                                                    if (Pom(arPometa, "5")) osnova += "нул";
                                                                    else if (Glasnay(slovo.Groups["основа"].Value)) osnova += "л";
                                                                }
                                                                else osnova = osnova + "нул";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                if (match.Groups["подтип"].Value == "**а") osnova += "ла";
                                                                else osnova = osnova + "нула";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                if (match.Groups["подтип"].Value == "**а") osnova += "ло";
                                                                else osnova = osnova + "нуло";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        if (match.Groups["подтип"].Value == "**а") osnova += "ли";
                                                        else osnova = osnova + "нули";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "буду " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = osnova + "ну";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                if (match.Groups["index"].Value == "в") osnova = osnova + "нёшь";
                                                                else osnova = osnova + "нешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                if (match.Groups["index"].Value == "в") osnova = osnova + "нёт";
                                                                else osnova = osnova + "нет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (match.Groups["index"].Value == "в") osnova = osnova + "нём";
                                                                else osnova = osnova + "нем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                if (match.Groups["index"].Value == "в") osnova = osnova + "нёте";
                                                                else osnova = osnova + "нете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо 
                                                                osnova += "нут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                default://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        if ((match.Groups["index"].Value == "а" && Glasnay(osnova)) || (match.Groups["подтип"].Value == "**а" && Glasnay(osnova)) || slovo.Groups["основа"].Value == "сты") osnova += "нь";
                                        else osnova += "ни";
                                        osnova = ZamСловоВнутри_or(osnova, "ыни", "ынь", 'a');
                                        if (match.Groups["index"].Value != "") osnova = ZamСловоВнутри_or(osnova, "янь", "яни", 'a');
                                        if (ChisloElRus() > 0) osnova += "те";
                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                        osnova = ZamСловоВнутри_or(osnova, "ьсь", "ься", 'a');
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (match.Groups["подтип"].Value == "**а")
                                                                {
                                                                    if (Pom(arPometa, "5")) osnova += "нул";
                                                                    else if (Glasnay(slovo.Groups["основа"].Value)) osnova += "л";
                                                                }
                                                                else osnova = osnova + "нул";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                if (match.Groups["подтип"].Value == "**а") osnova += "ла";
                                                                else osnova = osnova + "нула";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                if (match.Groups["подтип"].Value == "**а") osnova += "ло";
                                                                else osnova = osnova + "нуло";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        if (match.Groups["подтип"].Value == "**а") osnova += "ли";
                                                        else osnova = osnova + "нули";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ну";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "нешь";
                                                                if (match.Groups["index"].Value == "в") osnova = ZamLit(osnova, 'ё', osnova.Length - 3);
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "нет";
                                                                if (match.Groups["index"].Value == "в") osnova = ZamLit(osnova, 'ё', osnova.Length - 2);
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "нем";
                                                                if (match.Groups["index"].Value == "в") osnova = ZamLit(osnova, 'ё', osnova.Length - 2);
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "нете";
                                                                if (match.Groups["index"].Value == "в") osnova = ZamLit(osnova, 'ё', osnova.Length - 3);
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "нут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                            default://настоящее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = osnova + "ну";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                if (match.Groups["index"].Value == "в") osnova = osnova + "нёшь";
                                                                else osnova = osnova + "нешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                if (match.Groups["index"].Value == "в") osnova = osnova + "нёт";
                                                                else osnova = osnova + "нет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (match.Groups["index"].Value == "в") osnova = osnova + "нём";
                                                                else osnova = osnova + "нем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                if (match.Groups["index"].Value == "в") osnova = osnova + "нёте";
                                                                else osnova = osnova + "нете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо 
                                                                osnova += "нут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 4:
                            slovo = Osnova(osnova, "(?<окончание>(ить)|(иться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        if (match.Groups["index"].Value == "а")
                                        {
                                            if (Glasnay(osnova)) osnova += "й";
                                            else
                                            {
                                                if (osnova.Length > 1) { if (Glasnay(osnova[osnova.Length - 2])) osnova += "ь"; else osnova += "и"; }
                                                else osnova += "и";
                                            }
                                        }
                                        else osnova += "и";
                                        if (ChisloElRus() > 0) osnova += "те";
                                        if (c.zalog == Zalog.Vozvratniy) { if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь"; }
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                osnova += "ил";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "ила";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "ило";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "или";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (Ship(osnova)) osnova += "у";
                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                else if (Сзтд(osnova)) { osnova = Чередов4(osnova, match.Groups["черед"].Value) + "у"; }
                                                                else osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ишь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ит";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "им";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ите";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                if (Ship(osnova)) osnova += "ат";
                                                                else osnova += "ят";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                            default://настоящее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (Ship(osnova)) osnova += "у";
                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                else if (Сзтд(osnova)) { osnova = Чередов4(osnova, match.Groups["черед"].Value) + "у"; }
                                                                else osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ишь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ит";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "им";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ите";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                if (Ship(osnova)) osnova += "ат";
                                                                else osnova += "ят";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                default://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        if (match.Groups["index"].Value == "а")
                                        {
                                            if (Glasnay(osnova)) osnova += "й";
                                            else
                                            {
                                                if (osnova.Length > 1) { if (Glasnay(osnova[osnova.Length - 2])) osnova += "ь"; else osnova += "и"; }
                                                else osnova += "и";
                                            }
                                        }
                                        else osnova += "и";
                                        if (ChisloElRus() > 0) osnova += "те";
                                        if (c.zalog == Zalog.Vozvratniy) { if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь"; }
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                osnova += "ил";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "ила";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "ило";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "или";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "буду " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (Ship(osnova)) osnova += "у";
                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                else if (Сзтд(osnova)) { osnova = Чередов4(osnova, match.Groups["черед"].Value) + "у"; }
                                                                else osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ишь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ит";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "им";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ите";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                if (Ship(osnova)) osnova += "ат";
                                                                else osnova += "ят";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 5:
                            slovo = Osnova(osnova, "(?<окончание>(еть)|(еться)|(ать)|(аться)|(ять)|(яться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        switch (slovo.Groups["окончание"].Value)
                                        {
                                            case "ять": osnova += "й"; break;
                                            default:
                                                if (match.Groups["index"].Value == "а" && !ПриставкаВы(osnova)) osnova += "ь";
                                                else if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "ни"; }
                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гони");
                                                else if (OsnovaProizv(osnova, "беж")) { osnova = ZamLit(osnova, 'г'); osnova += "и"; }
                                                else osnova += "и";
                                                break;
                                        }
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (match.Groups["index"].Value == "а" && slovo.Groups["окончание"].Value == "еть" || match.Groups["index"].Value == "в" && slovo.Groups["окончание"].Value == "еть" || match.Groups["index"].Value == "с" && slovo.Groups["окончание"].Value == "еть") osnova = Чередов5(osnova);
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "ню"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гоню");
                                                                else if (slovo.Groups["окончание"].Value == "ать")
                                                                {
                                                                    if (OsnovaProizv(osnova, "беж")) osnova = ZamLit(osnova, 'г', osnova.Length - 1);
                                                                    if (OsnovaProizv(osnova, "сп")) osnova += "лю";
                                                                    else osnova += "у";
                                                                }
                                                                else if (slovo.Groups["окончание"].Value == "ять") { if (match.Groups["index"].Value == "а") osnova += "ю"; else osnova += "лю"; }
                                                                else if (slovo.Groups["окончание"].Value == "еть")
                                                                {
                                                                    if (match.Groups["index"].Value == "в" || match.Groups["index"].Value == "с")
                                                                    {
                                                                        if (Лнр(osnova)) osnova += "ю";
                                                                        else if (Пбмвф(osnova)) osnova += "лю";
                                                                        else osnova += "у";
                                                                    }
                                                                    else
                                                                    {
                                                                        if (osnova[osnova.Length - 1] == 'т') { osnova = ZamLit(osnova, 'ч', osnova.Length - 1); osnova += "у"; }
                                                                        else if (OsnovaProizv(osnova, "смотр")) osnova += "ю";
                                                                        else if (OsnovaProizv(osnova, "терп")) osnova += "лю";
                                                                        else osnova += "у";
                                                                    }
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                if (OsnovaProizv(osnova, "хот")) { osnova = ZamLit(osnova, 'ч'); osnova += "ешь"; }
                                                                else if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "нишь"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гонишь");
                                                                else osnova += "ишь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                if (OsnovaProizv(osnova, "хот")) { osnova = ZamLit(osnova, 'ч'); osnova += "ет"; }
                                                                else if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "нит"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гонит");
                                                                else osnova += "ит";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "ним"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гоним");
                                                                else osnova += "им";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "ните"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гоните");
                                                                else osnova += "ите";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                if (slovo.Groups["окончание"].Value == "ать")
                                                                {
                                                                    if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "нят"; }
                                                                    else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гонят");
                                                                    else if (match.Groups["index"].Value == "а" && !ПриставкаВы(osnova)) osnova += "ат";
                                                                    else if (OsnovaProizv(osnova, "держ")) osnova += "ат";
                                                                    else if (match.Groups["index"].Value == "а" && OsnovaProizv(osnova, "беж")) { osnova = ZamLit(osnova, 'г'); osnova += "ут"; }
                                                                    else if (match.Groups["index"].Value == "в")
                                                                    {
                                                                        if (OsnovaProizv(osnova, "беж")) { osnova = ZamLit(osnova, 'г'); osnova += "ут"; }
                                                                        else if (OsnovaProizv(osnova, "сп")) osnova += "ят";
                                                                        else osnova += "ат";
                                                                    }
                                                                    else if (match.Groups["index"].Value == "с") osnova += "ат";
                                                                    else osnova += "ят";
                                                                }
                                                                else if (OsnovaProizv(osnova, "киш") || OsnovaProizv(osnova, "держ")) osnova += "ат";
                                                                else osnova += "ят";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                            default://настоящее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (match.Groups["index"].Value == "а" && slovo.Groups["окончание"].Value == "еть" || match.Groups["index"].Value == "в" && slovo.Groups["окончание"].Value == "еть" || match.Groups["index"].Value == "с" && slovo.Groups["окончание"].Value == "еть") osnova = Чередов5(osnova);
                                                                switch (match.Groups["index"].Value)
                                                                {
                                                                    case "в":
                                                                        if (slovo.Groups["окончание"].Value == "ять") osnova += "ю";
                                                                        else if (slovo.Groups["окончание"].Value == "ать")
                                                                        {
                                                                            if (OsnovaProizv(osnova, "беж")) osnova = ZamLit(osnova, 'г');
                                                                            if (OsnovaProizv(osnova, "сп")) osnova += "лю";
                                                                            else osnova += "у";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (OsnovaProizv(osnova, "киш")) osnova += "у";
                                                                            else if (Пбмвф(osnova)) osnova += "лю";
                                                                            else if (Лнр(osnova)) osnova += "ю";
                                                                            else osnova += "у";
                                                                        }
                                                                        break;
                                                                    case "а":
                                                                        if (OsnovaProizv(osnova, "смотр")) osnova += "ю";
                                                                        else if (OsnovaProizv(osnova, "терп")) osnova += "лю";
                                                                        else if (OsnovaProizv(osnova, "гн")) { osnova = ZamLit(osnova, 'о'); osnova += "няю"; }
                                                                        else if (OsnovaProizv(osnova, "беж")) { osnova = ZamLit(osnova, 'г'); osnova += "у"; }
                                                                        else osnova += "у";
                                                                        break;
                                                                    default:
                                                                        if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гоню");
                                                                        else if (Лнр(osnova)) osnova += "ю";
                                                                        else if (Пбмвф(osnova)) osnova += "лю";
                                                                        else osnova += "у";
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "няешь"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гонишь");
                                                                else if (OsnovaProizv(osnova, "хот")) { osnova = Чередов5(osnova); osnova += "ешь"; }
                                                                else osnova += "ишь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "няют"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гонит");
                                                                else if (OsnovaProizv(osnova, "хот")) { osnova = Чередов5(osnova); osnova += "ет"; }
                                                                else if (OsnovaProizv(osnova, "киш")) osnova += "ат";
                                                                else osnova += "ит";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гон");
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "им";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ите";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                switch (match.Groups["index"].Value)
                                                                {
                                                                    case "а":
                                                                        if (slovo.Groups["окончание"].Value == "ать" && !ПриставкаВы(osnova)) osnova += "ат";
                                                                        else if (OsnovaProizv(osnova, "держ") || OsnovaProizv(osnova, "беж")) osnova += "ат";
                                                                        else osnova += "ят";
                                                                        break;
                                                                    case "в":
                                                                        if (slovo.Groups["окончание"].Value == "ать")
                                                                        {
                                                                            if (OsnovaProizv(osnova, "беж")) { osnova = ZamLit(osnova, 'г'); osnova += "ут"; }
                                                                            else if (OsnovaProizv(osnova, "сп")) osnova += "ят";
                                                                            else osnova += "ат";
                                                                        }
                                                                        else if (OsnovaProizv(osnova, "киш")) osnova += "ат";
                                                                        else osnova += "ят";
                                                                        break;
                                                                    case "с":
                                                                        if (slovo.Groups["окончание"].Value == "ать")
                                                                        {
                                                                            if (OsnovaProizv(osnova, "гон")) { osnova = БезО(osnova, "гон", "гонят"); }
                                                                            else osnova += "ат";
                                                                        }
                                                                        else osnova += "ят";
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                default://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        if (slovo.Groups["окончание"].Value == "ять") osnova += "й";
                                        else if (match.Groups["index"].Value == "а" && !ПриставкаВы(osnova)) osnova += "ь";
                                        else if (OsnovaProizv(osnova, "беж")) { osnova = ZamLit(osnova, 'г'); osnova += "и"; }
                                        else if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "ни"; }
                                        else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гони");
                                        else osnova += "и";
                                        if (ChisloElRus() > 0) osnova += "те";
                                        if (c.zalog == Zalog.Vozvratniy) { if (ChisloElRus() > 0)osnova += "сь"; else osnova += "ся"; }
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                
                                                                osnova = "буду " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + slovo.Groups["основа"].Value + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (match.Groups["index"].Value == "а" && slovo.Groups["окончание"].Value == "еть" || match.Groups["index"].Value == "в" && slovo.Groups["окончание"].Value == "еть" || match.Groups["index"].Value == "с" && slovo.Groups["окончание"].Value == "еть") osnova = Чередов5(osnova);
                                                                switch (match.Groups["index"].Value)
                                                                {
                                                                    case "в":
                                                                        if (slovo.Groups["окончание"].Value == "ять") osnova += "ю";
                                                                        else if (slovo.Groups["окончание"].Value == "ать")
                                                                        {
                                                                            if (OsnovaProizv(osnova, "беж")) osnova = ZamLit(osnova, 'г');
                                                                            if (OsnovaProizv(osnova, "сп")) osnova += "лю";
                                                                            else osnova += "у";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (OsnovaProizv(osnova, "киш")) osnova += "у";
                                                                            else if (Пбмвф(osnova)) osnova += "лю";
                                                                            else if (Лнр(osnova)) osnova += "ю";
                                                                            else osnova += "у";
                                                                        }
                                                                        break;
                                                                    case "а":
                                                                        if (OsnovaProizv(osnova, "смотр")) osnova += "ю";
                                                                        else if (OsnovaProizv(osnova, "гн")) { osnova = ZamLit(osnova, 'о'); osnova += "няю"; }
                                                                        else if (OsnovaProizv(osnova, "терп")) osnova += "лю";
                                                                        else if (OsnovaProizv(osnova, "беж")) { osnova = ZamLit(osnova, 'г'); osnova += "у"; }
                                                                        else osnova += "у";
                                                                        break;
                                                                    default:
                                                                        if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гон");
                                                                        if (Лнр(osnova)) osnova += "ю";
                                                                        else if (Пбмвф(osnova)) osnova += "лю";
                                                                        else osnova += "у";
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "няешь"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гонишь");
                                                                else if (OsnovaProizv(osnova, "хот")) { osnova = Чередов5(osnova); osnova += "ешь"; }
                                                                else osnova += "ишь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "няет"; }
                                                                else if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гонит");
                                                                else if (OsnovaProizv(osnova, "хот")) { osnova = Чередов5(osnova); osnova += "ет"; }
                                                                else osnova += "ит";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        if (OsnovaProizv(osnova, "гн")) osnova = БезО(osnova, "гн", "гон");
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "няем"; }
                                                                else osnova += "им";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                if (OsnovaProizv(osnova, "гн") && match.Groups["index"].Value == "а") { osnova = ZamLit(osnova, 'о'); osnova += "няете"; }
                                                                else osnova += "ите";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                switch (match.Groups["index"].Value)
                                                                {
                                                                    case "а":
                                                                        if (slovo.Groups["окончание"].Value == "ать" && !ПриставкаВы(osnova)) osnova += "ат";
                                                                        else if (OsnovaProizv(osnova, "держ") || OsnovaProizv(osnova, "беж")) osnova += "ат";
                                                                        else if (OsnovaProizv(osnova, "гн")) { osnova = ZamLit(osnova, 'о'); osnova += "няют"; }
                                                                        else osnova += "ят";
                                                                        break;
                                                                    case "в":
                                                                        if (slovo.Groups["окончание"].Value == "ать")
                                                                        {
                                                                            if (OsnovaProizv(osnova, "беж")) { osnova = ZamLit(osnova, 'г'); osnova += "ут"; }
                                                                            else if (OsnovaProizv(osnova, "сп")) osnova += "ят";
                                                                            else osnova += "ат";
                                                                        }
                                                                        else if (OsnovaProizv(osnova, "киш")) osnova += "ат";
                                                                        else osnova += "ят";
                                                                        break;
                                                                    case "с":
                                                                        if (slovo.Groups["окончание"].Value == "ать")
                                                                        {
                                                                            if (OsnovaProizv(osnova, "гон")) osnova = БезО(osnova, "гон", "гонят");
                                                                            else osnova += "ат";
                                                                        }
                                                                        else osnova += "ят";
                                                                        break;
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 6:
                            slovo = Osnova(osnova, "(?<окончание>(ать)|(аться)|(ять)|(яться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        switch (slovo.Groups["окончание"].Value)
                                        {
                                            case "ять": osnova += "й"; break;
                                            default:
                                                switch (match.Groups["index"].Value)
                                                {
                                                    case "а":
                                                        if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "и"; }
                                                        else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "ли";
                                                        else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); if (Glasnay(osnova[osnova.Length - 2])) osnova += "ь"; else osnova += "и"; }
                                                        else osnova += "ь";
                                                        break;
                                                    case "в":
                                                        if (OsnovaProizv(osnova, "рж")) osnova += "и";
                                                        else osnova = ZamOsnova(osnova, "сл", "шли");
                                                        break;
                                                    case "с":
                                                        if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "и"; }
                                                        else if (Пбмвф(osnova)) osnova += "ли";
                                                        else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "и"; }
                                                        break;
                                                }
                                                break;
                                        }
                                        if (match.Groups["index"].Value == "")
                                        {
                                            osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                            if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                            else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                            else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                            else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                            osnova += "и";
                                        }
                                        osnova = ZamСловоВнутри_or(osnova, "щь", "щи", 'a');
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ю"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "у"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "у"; }
                                                                                else osnova += "ю";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "у";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлю");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ю"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "у"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    osnova += "у";
                                                                    if (OsnovaProizv(osnova, "стлу")) osnova = ZamOsnova(osnova, "стлу", "стелю");
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ешь"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ешь"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лешь";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ешь"; }
                                                                                else osnova += "ешь";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёшь";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёшь");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ешь"; }
                                                                                else if (Пбмвф(osnova)) osnova += "ешь";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ешь"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ешь";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ет"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ет"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лет";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ет"; }
                                                                                else osnova += "ет";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёт";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёт");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ет"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лет";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ет"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ет";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ем"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "им"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "им";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ем"; }
                                                                                else osnova += "ем";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ём";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлём");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "им"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лем";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ем"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ем";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ете"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ите"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лите";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ите"; }
                                                                                else osnova += "ите";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёте";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёте");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ите"; }
                                                                                else if (Пбмвф(osnova)) osnova += "ите";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ите"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ете";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ют"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ут"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лют";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ут"; }
                                                                                else osnova += "ют";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ут";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлют");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ют"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лют";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ут"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    osnova += "ут";
                                                                    if (OsnovaProizv(osnova, "стлут")) osnova = ZamOsnova(osnova, "стлут", "стелют");
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                            default://настоящее
                                                if (match.Groups["index"].Value == "") osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ю"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "у"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "у"; }
                                                                                else osnova += "ю";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "у";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлю");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ю"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "у"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    osnova += "у";
                                                                    if (OsnovaProizv(osnova, "стлу")) osnova = ZamOsnova(osnova, "стлу", "стелю");
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ешь"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ешь"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лешь";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ешь"; }
                                                                                else osnova += "ешь";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёшь";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёшь");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ешь"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лешь";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ешь"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ешь";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ет"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ет"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лет";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ет"; }
                                                                                else osnova += "ет";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёт";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёт");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ет"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лет";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ет"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ет";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ем"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "им"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лем";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ем"; }
                                                                                else osnova += "ем";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ём";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлём");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ем"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лем";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ем"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ем";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ете"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ите"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лете";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ете"; }
                                                                                else osnova += "ете";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёте";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёте");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ете"; }
                                                                                else if (Пбмвф(osnova)) osnova += "ете";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ете"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ете";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ют"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ут"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лют";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ут"; }
                                                                                else osnova += "ют";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ут";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлют");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ют"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лют";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ут"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    osnova += "ут";
                                                                    if (OsnovaProizv(osnova, "стлут")) osnova = ZamOsnova(osnova, "стлут", "стелют");
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                default://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        switch (slovo.Groups["окончание"].Value)
                                        {
                                            case "ять": osnova += "й"; break;
                                            default:
                                                switch (match.Groups["index"].Value)
                                                {
                                                    case "а":
                                                        if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "и"; }
                                                        else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "ли";
                                                        else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); if (Glasnay(osnova[osnova.Length - 2])) osnova += "ь"; else osnova += "и"; }
                                                        else osnova += "ь";
                                                        break;
                                                    case "в":
                                                        if (OsnovaProizv(osnova, "рж")) osnova += "и";
                                                        else osnova = ZamOsnova(osnova, "сл", "шли");
                                                        break;
                                                    case "с":
                                                        if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "и"; }
                                                        else if (Пбмвф(osnova)) osnova += "ли";
                                                        else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "и"; }
                                                        break;
                                                }
                                                break;
                                        }
                                        if (match.Groups["index"].Value == "")
                                        {
                                            osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                            if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                            else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                            else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                            else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                            osnova += "и";
                                        }
                                        osnova = ZamСловоВнутри_or(osnova, "щь", "щи", 'a');
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                if (match.Groups["index"].Value == "")
                                                {
                                                    if (Приставка(osnova))
                                                    {
                                                        osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                        if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зыв"); }
                                                        else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бир"); }
                                                        else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дир");
                                                        else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                    }
                                                }
                                                //osnova = Чередов6(osnova, match.Groups["черед"].Value);
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                
                                                                osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                if (match.Groups["index"].Value == "") osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ю"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "у"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "у"; }
                                                                                else osnova += "ю";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "у";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлю");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ю"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лю";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "у"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    osnova = БезО_ret_orig(osnova, "(зв)|(бр)|(др)|(стл)");
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    osnova += "у";
                                                                    if (OsnovaProizv(osnova, "стлу")) osnova = ZamOsnova(osnova, "стлу", "стелю");
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ешь"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ешь"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лешь";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ешь"; }
                                                                                else osnova += "ешь";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёшь";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёшь");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ешь"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лешь";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ешь"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ешь";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ет"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ет"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лет";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ет"; }
                                                                                else osnova += "ет";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёт";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёт");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ет"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лет";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ет"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ет";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ем"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "им"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лем";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ем"; }
                                                                                else osnova += "ем";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ём";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлём");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ем"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лем";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ем"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ем";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ете"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ите"; }
                                                                                else if (Пбмвф(osnova) && !OsnovaProizv(osnova, "сып")) osnova += "лете";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ете"; }
                                                                                else osnova += "ете";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ёте";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлёте");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ете"; }
                                                                                else if (Пбмвф(osnova)) osnova += "ете";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ете"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    else if (OsnovaProizv(osnova, "стл")) osnova = ZamOsnova(osnova, "стл", "стел");
                                                                    else if (OsnovaProizv(osnova, "лг")) osnova = ZamOsnova(osnova, "лг", "лж");
                                                                    osnova += "ете";
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                switch (slovo.Groups["окончание"].Value)
                                                                {
                                                                    case "ять": osnova += "ют"; break;
                                                                    default:
                                                                        switch (match.Groups["index"].Value)
                                                                        {
                                                                            case "а":
                                                                                if (ПриставкаВы(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova = Сл_Стл(osnova); osnova += "ут"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лют";
                                                                                else if (Сзтдкгх(osnova)) { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ут"; }
                                                                                else osnova += "ют";
                                                                                break;
                                                                            case "в":
                                                                                if (OsnovaProizv(osnova, "рж")) osnova += "ут";
                                                                                else osnova = ZamOsnova(osnova, "сл", "шлют");
                                                                                break;
                                                                            case "с":
                                                                                if (Лнр(osnova)) { if (OsnovaProizv(osnova, "стл"))osnova = ZamOsnova(osnova, "стл", "стел"); osnova += "ют"; }
                                                                                else if (Пбмвф(osnova)) osnova += "лют";
                                                                                else { osnova = Чередов6(osnova, match.Groups["черед"].Value); osnova += "ут"; }
                                                                                break;
                                                                        }
                                                                        break;
                                                                }
                                                                if (match.Groups["index"].Value == "")
                                                                {
                                                                    if (OsnovaProizv(osnova, "зв")) { osnova = ZamOsnova(osnova, "зв", "зов"); }
                                                                    else if (OsnovaProizv(osnova, "бр")) { osnova = ZamOsnova(osnova, "бр", "бер"); }
                                                                    else if (OsnovaProizv(osnova, "др")) osnova = ZamOsnova(osnova, "др", "дер");
                                                                    osnova += "ут";
                                                                    if (OsnovaProizv(osnova, "стлут")) osnova = ZamOsnova(osnova, "стлут", "стелют");
                                                                }
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 7:
                            slovo = Osnova(osnova, "(?<окончание>(ть)|(ться)|(ти)|(тись)|(тися))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        if (OsnovaProizv(osnova, "чес")) switch (Приставка_(osnova))
                                            {
                                                case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                            }
                                        osnova = БезО_(osnova, "обо");
                                        switch (pometa)
                                        {
                                            case "[3]": osnova += "ь"; break;
                                            default:
                                                switch (match.Groups["index"].Value)
                                                {
                                                    case "а":
                                                        if (ПриставкаВы(osnova))
                                                        {
                                                            if (OsnovaProizv(osnova, "крас") || OsnovaProizv(osnova, "вес")) osnova = ZamLit(osnova, 'д');
                                                            else if (OsnovaProizv(osnova, "мес")) osnova = ZamLit(osnova, 'т');
                                                            else if (OsnovaProizv(osnova, "грес")) osnova = ZamLit(osnova, 'б');
                                                            else if (OsnovaProizv(osnova, "чес")) osnova = ZamOsnova(osnova, "чес", "чт");
                                                            else if (OsnovaProizv(osnova, "рас")) osnova = ZamOsnova(osnova, "рас", "раст");
                                                            osnova += "и";
                                                        }
                                                        else if (OsnovaProizv(osnova, "сес")) osnova = ZamOsnova(osnova, "сес", "сядь");
                                                        else if (OsnovaProizv(osnova, "лез")) osnova += "ь";
                                                        else osnova += "и";
                                                        break;
                                                    default:
                                                        if (OsnovaProizv(osnova, "чес")) osnova = ZamOsnova(osnova, "чес", "чт");
                                                        else osnova = ZamLit(osnova, match.Groups["черед"].Value);
                                                        osnova += "и";
                                                        break;
                                                }
                                                break;
                                        }
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее 
                                                if (OsnovaProizv(osnova, "мес") || OsnovaProizv(osnova, "вес") || OsnovaProizv(osnova, "крас") || OsnovaProizv(osnova, "чес") || OsnovaProizv(osnova, "сес") || match.Groups["черед"].Value == "(-д-)" || match.Groups["черед"].Value == "(-т-)") osnova = ZamLit(osnova, 'л');
                                                else if (match.Groups["черед"].Value == "(-б-)" || OsnovaProizv(osnova, "грес")) osnova = ZamLit(osnova, 'б');
                                                else if (match.Groups["черед"].Value == "(-ст-)" || OsnovaProizv(osnova, "рас")) osnova = ZamLit(osnova, 'о', osnova.Length - 2);
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                if (OsnovaProizv(osnova, "чел"))
                                                                {
                                                                    switch (Приставка_(osnova))
                                                                    {
                                                                        case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                                        case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                                                    }
                                                                    osnova = ZamOsnova(osnova, "чел", "чла");
                                                                }
                                                                else if (osnova[osnova.Length - 1] == 'л') osnova += "а";
                                                                else osnova += "ла";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                if (OsnovaProizv(osnova, "чел"))
                                                                {
                                                                    switch (Приставка_(osnova))
                                                                    {
                                                                        case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                                        case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                                                    }
                                                                    osnova = ZamOsnova(osnova, "чел", "чло");
                                                                }
                                                                else if (osnova[osnova.Length - 1] == 'л') osnova += "о";
                                                                else osnova += "ло"; ;
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        if (OsnovaProizv(osnova, "чел"))
                                                        {
                                                            switch (Приставка_(osnova))
                                                            {
                                                                case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                                case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                                            }
                                                            osnova = ZamOsnova(osnova, "чел", "чли");
                                                        }
                                                        else if (osnova[osnova.Length - 1] == 'л') osnova += "и";
                                                        else osnova += "ли";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                if (OsnovaProizv(osnova, "чес")) switch (Приставка_(osnova))
                                                    {
                                                        case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                        case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                                    }
                                                osnova = БезО_(osnova, "обо");
                                                if (OsnovaProizv(osnova, "чес")) osnova = ZamOsnova(osnova, "чес", "чт");
                                                else if (OsnovaProizv(osnova, "сес")) osnova = ZamOsnova(osnova, "сес", "сяд");
                                                else if (OsnovaProizv(osnova, "вес") || OsnovaProizv(osnova, "крас") || match.Groups["черед"].Value == "(-д-)") osnova = ZamLit(osnova, 'д');
                                                else if (match.Groups["черед"].Value == "(-т-)" || OsnovaProizv(osnova, "мес")) osnova = ZamLit(osnova, 'т');
                                                else if (match.Groups["черед"].Value == "(-б-)") osnova = ZamLit(osnova, 'б');
                                                else if (match.Groups["черед"].Value == "(-ст-)") { osnova += "т"; }
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                                                     
                                                                osnova += "у";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                            default://настоящее
                                                osnova = БезО_(osnova, "обо");
                                                if (OsnovaProizv(osnova, "чес")) osnova = ZamOsnova(osnova, "чес", "чт");
                                                else if (OsnovaProizv(osnova, "сес")) osnova = ZamOsnova(osnova, "сес", "сяд");
                                                else if (OsnovaProizv(osnova, "вес") || OsnovaProizv(osnova, "крас") || match.Groups["черед"].Value == "(-д-)") osnova = ZamLit(osnova, 'д');
                                                else if (match.Groups["черед"].Value == "(-т-)" || OsnovaProizv(osnova, "мес")) osnova = ZamLit(osnova, 'т');
                                                else if (match.Groups["черед"].Value == "(-б-)") osnova = ZamLit(osnova, 'б');
                                                else if (match.Groups["черед"].Value == "(-ст-)") { osnova += "т"; }
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "у";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                default://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        if (OsnovaProizv(osnova, "чес")) switch (Приставка_(osnova))
                                            {
                                                case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                            }
                                        osnova = БезО_(osnova, "обо");
                                        switch (pometa)
                                        {
                                            case "[3]": osnova += "ь"; break;
                                            default:
                                                switch (match.Groups["index"].Value)
                                                {
                                                    case "а":
                                                        if (ПриставкаВы(osnova))
                                                        {
                                                            if (OsnovaProizv(osnova, "крас") || OsnovaProizv(osnova, "вес")) osnova = ZamLit(osnova, 'д');
                                                            else if (OsnovaProizv(osnova, "мес")) osnova = ZamLit(osnova, 'т');
                                                            else if (OsnovaProizv(osnova, "грес")) osnova = ZamLit(osnova, 'б');
                                                            else if (OsnovaProizv(osnova, "чес")) osnova = ZamOsnova(osnova, "чес", "чт");
                                                            else if (OsnovaProizv(osnova, "рас")) osnova = ZamOsnova(osnova, "рас", "раст");
                                                            osnova += "и";
                                                        }
                                                        else if (OsnovaProizv(osnova, "сес")) osnova = ZamOsnova(osnova, "сес", "сядь");
                                                        else if (OsnovaProizv(osnova, "лез")) osnova += "ь";
                                                        else osnova += "и";
                                                        break;
                                                    default:
                                                        if (OsnovaProizv(osnova, "чес")) osnova = ZamOsnova(osnova, "чес", "чт");
                                                        else osnova = ZamLit(osnova, match.Groups["черед"].Value);
                                                        osnova += "и";
                                                        break;
                                                }
                                                break;
                                        }
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                if (OsnovaProizv(osnova, "мес") || OsnovaProizv(osnova, "вес") || OsnovaProizv(osnova, "крас") || OsnovaProizv(osnova, "чес") || OsnovaProizv(osnova, "сес") || match.Groups["черед"].Value == "(-д-)" || match.Groups["черед"].Value == "(-т-)") osnova = ZamLit(osnova, 'л');
                                                else if (match.Groups["черед"].Value == "(-б-)" || OsnovaProizv(osnova, "грес")) osnova = ZamLit(osnova, 'б');
                                                else if (match.Groups["черед"].Value == "(-ст-)" || OsnovaProizv(osnova, "рас")) osnova = ZamLit(osnova, 'о', osnova.Length - 2);
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                if (OsnovaProizv(osnova, "чел"))
                                                                {
                                                                    switch (Приставка_(osnova))
                                                                    {
                                                                        case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                                        case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                                                    }
                                                                    osnova = ZamOsnova(osnova, "чел", "чла");
                                                                }
                                                                else if (osnova[osnova.Length - 1] == 'л') osnova += "а";
                                                                else osnova += "ла";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                if (OsnovaProizv(osnova, "чел"))
                                                                {
                                                                    switch (Приставка_(osnova))
                                                                    {
                                                                        case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                                        case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                                                    }
                                                                    osnova = ZamOsnova(osnova, "чел", "чло");
                                                                }
                                                                else if (osnova[osnova.Length - 1] == 'л') osnova += "о";
                                                                else osnova += "ло"; ;
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        if (OsnovaProizv(osnova, "чел"))
                                                        {
                                                            switch (Приставка_(osnova))
                                                            {
                                                                case "с": osnova = ZamПриставка(osnova, "со"); break;
                                                                case "рас": osnova = ZamПриставка(osnova, "разо"); break;
                                                            }
                                                            osnova = ZamOsnova(osnova, "чел", "чли");
                                                        }
                                                        else if (osnova[osnova.Length - 1] == 'л') osnova += "и";
                                                        else osnova += "ли";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                
                                                                osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                osnova = БезО_(osnova, "обо");
                                                if (OsnovaProizv(osnova, "чес")) osnova = ZamOsnova(osnova, "чес", "чт");
                                                else if (OsnovaProizv(osnova, "сес")) osnova = ZamOsnova(osnova, "сес", "сяд");
                                                else if (OsnovaProizv(osnova, "вес") || OsnovaProizv(osnova, "крас") || match.Groups["черед"].Value == "(-д-)") osnova = ZamLit(osnova, 'д');
                                                else if (match.Groups["черед"].Value == "(-т-)" || OsnovaProizv(osnova, "мес")) osnova = ZamLit(osnova, 'т');
                                                else if (match.Groups["черед"].Value == "(-б-)") { osnova = ZamLit(osnova, 'б'); }
                                                else if (match.Groups["черед"].Value == "(-ст-)") { osnova += "т"; }
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "у";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            osnova = ZamСловоВнутри_or(osnova, "ьсь", "ься", 'a');
                            break;
                        case 8:
                            slovo = Osnova(osnova, "(?<окончание>(чь)|(чься))");
                            osnova = slovo.Groups["основа"].Value;
                            if (osnova.Length > 0)
                                switch (VidElRus())
                                {
                                    case 1://совершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            if (OsnovaProizv(osnova, "ле")) osnova = ZamOsnova(osnova, "ле", "ляг");
                                            else
                                            {
                                                if (OsnovaProizv(osnova, "мо")) osnova = ZamOsnova(osnova, "мо", "мог");
                                                else if (OsnovaProizv(osnova, "толо")) osnova = ZamOsnova(osnova, "толо", "толк");
                                                else if (OsnovaProizv(osnova, "же")) { osnova = С_О_ret_orig(osnova); osnova = ZamOsnova(osnova, "же", "жг"); }
                                                else osnova += Lit(match.Groups["черед"].Value, 'a');
                                                if (Glasnay(osnova[osnova.Length - 1])) osnova += "к";
                                                osnova += "и";
                                            }
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    if (OsnovaProizv(osnova, "ле")) osnova = ZamOsnova(osnova, "ле", "лег");
                                                    else
                                                    {
                                                        if (OsnovaProizv(osnova, "мо")) osnova = ZamOsnova(osnova, "мо", "мог");
                                                        else if (OsnovaProizv(osnova, "толо")) osnova = ZamOsnova(osnova, "толо", "толк");
                                                        else if (OsnovaProizv(osnova, "же")) { osnova = С_О_ret_orig(osnova); osnova = ZamOsnova(osnova, "же", "жг"); }
                                                        else osnova += Lit(match.Groups["черед"].Value, 'a');
                                                        if (Glasnay(osnova[osnova.Length - 1])) osnova += "к";
                                                    }
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (OsnovaProizv(osnova, "жг"))
                                                                    {
                                                                        osnova = БезО_ret_orig(osnova);
                                                                        osnova = ZamOsnova_ro(osnova, "жг", "жёг");
                                                                    }
                                                                    osnova = ZamOsnova_ro(osnova, "толк", "толок");
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "ла";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "ло";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "ли";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее
                                                    if (OsnovaProizv(osnova, "ле")) osnova = ZamOsnova(osnova, "ле", "ляг");
                                                    else
                                                    {
                                                        if (OsnovaProizv(osnova, "мо")) osnova = ZamOsnova(osnova, "мо", "мог");
                                                        else if (OsnovaProizv(osnova, "толо")) osnova = ZamOsnova(osnova, "толо", "толк");
                                                        else if (OsnovaProizv(osnova, "же")) { osnova = С_О_ret_orig(osnova); osnova = ZamOsnova(osnova, "же", "жг"); }
                                                        else osnova += Lit(match.Groups["черед"].Value, 'a');
                                                        if (Glasnay(osnova[osnova.Length - 1])) osnova += "к";
                                                    }
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                                default://настоящее
                                                    if (OsnovaProizv(osnova, "ле")) osnova = ZamOsnova(osnova, "ле", "ляг");
                                                    else
                                                    {
                                                        if (OsnovaProizv(osnova, "мо")) osnova = ZamOsnova(osnova, "мо", "мог");
                                                        else if (OsnovaProizv(osnova, "толо")) osnova = ZamOsnova(osnova, "толо", "толк");
                                                        else if (OsnovaProizv(osnova, "же")) { osnova = С_О_ret_orig(osnova); osnova = ZamOsnova(osnova, "же", "жг"); }
                                                        else osnova += Lit(match.Groups["черед"].Value, 'a');
                                                        if (Glasnay(osnova[osnova.Length - 1])) osnova += "к";
                                                    }
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                    default://несовершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            if (OsnovaProizv(osnova, "ле")) osnova = ZamOsnova(osnova, "ле", "ляг");
                                            else
                                            {
                                                if (OsnovaProizv(osnova, "мо")) osnova = ZamOsnova(osnova, "мо", "мог");
                                                else if (OsnovaProizv(osnova, "толо")) osnova = ZamOsnova(osnova, "толо", "толк");
                                                else if (OsnovaProizv(osnova, "же")) { osnova = С_О_ret_orig(osnova); osnova = ZamOsnova(osnova, "же", "жг"); }
                                                else osnova += Lit(match.Groups["черед"].Value, 'a');
                                                if (Glasnay(osnova[osnova.Length - 1])) osnova += "к";
                                                osnova += "и";
                                            }
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    if (OsnovaProizv(osnova, "ле")) osnova = ZamOsnova(osnova, "ле", "лег");
                                                    else
                                                    {
                                                        if (OsnovaProizv(osnova, "мо")) osnova = ZamOsnova(osnova, "мо", "мог");
                                                        else if (OsnovaProizv(osnova, "толо")) osnova = ZamOsnova(osnova, "толо", "толк");
                                                        else if (OsnovaProizv(osnova, "же")) { osnova = С_О_ret_orig(osnova); osnova = ZamOsnova(osnova, "же", "жг"); }
                                                        else osnova += Lit(match.Groups["черед"].Value, 'a');
                                                        if (Glasnay(osnova[osnova.Length - 1])) osnova += "к";
                                                    }
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (OsnovaProizv(osnova, "жг"))
                                                                    {
                                                                        osnova = БезО_ret_orig(osnova);
                                                                        osnova = ZamOsnova_ro(osnova, "жг", "жёг");
                                                                    }
                                                                    osnova = ZamOsnova_ro(osnova, "толк", "толок");
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "ла";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "ло";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "ли";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо                                                                
                                                                    osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                    break;
                                                default://настоящее
                                                    if (OsnovaProizv(osnova, "ле")) osnova = ZamOsnova(osnova, "ле", "ляг");
                                                    else
                                                    {
                                                        if (OsnovaProizv(osnova, "мо")) osnova = ZamOsnova(osnova, "мо", "мог");
                                                        else if (OsnovaProizv(osnova, "толо")) osnova = ZamOsnova(osnova, "толо", "толк");
                                                        else if (OsnovaProizv(osnova, "же")) { osnova = С_О_ret_orig(osnova); osnova = ZamOsnova(osnova, "же", "жг"); }
                                                        else osnova += Lit(match.Groups["черед"].Value, 'a');
                                                        if (Glasnay(osnova[osnova.Length - 1])) osnova += "к";
                                                    }
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    switch (osnova[osnova.Length - 1])
                                                                    {
                                                                        case 'г': osnova = ZamLit(osnova, 'ж'); break;
                                                                        case 'к': osnova = ZamLit(osnova, 'ч'); break;
                                                                    }
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                }
                            break;
                        case 9:
                            slovo = Osnova(osnova, "(?<окончание>(еть)|(еться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        osnova = ZamOsnova_ro(osnova, "ер", "ри");
                                        if (OsnovaProizv(osnova, "растри") || OsnovaProizv(osnova, "распри") || OsnovaProizv(osnova, "истри"))
                                        {
                                            osnova = ZamСловоВнутри_or(osnova, "рас", "разо", 'a');
                                            osnova = ZamСловоВнутри_or(osnova, "ис", "изо", 'a');
                                        }
                                        else
                                        {
                                            osnova = С_О_ret_orig(osnova);
                                            if (!OsnovaProizv(osnova, "отопри") && !Приставка(osnova)) { osnova = ZamOsnova_ro(osnova, "опри", "обопри"); }
                                            osnova = ZamСловоВнутри_or(osnova, "отори", "отри", 'b');
                                        }
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "ла";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "ло";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "ли";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                osnova = ZamOsnova_ro(osnova, "ер", "р");
                                                if (OsnovaProizv(osnova, "растр") || OsnovaProizv(osnova, "распр") || OsnovaProizv(osnova, "истр"))
                                                {
                                                    osnova = ZamСловоВнутри_or(osnova, "рас", "разо", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "ис", "изо", 'a');
                                                }
                                                else
                                                {
                                                    osnova = С_О_ret_orig(osnova);
                                                    if (!OsnovaProizv(osnova, "отопр") && !Приставка(osnova)) osnova = ZamOsnova_ro(osnova, "опр", "обопр");
                                                    osnova = ZamСловоВнутри_or(osnova, "отор", "отр", 'b');
                                                }
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "у";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                            default://настоящее
                                                osnova = ZamOsnova_ro(osnova, "ер", "р");
                                                if (OsnovaProizv(osnova, "растр") || OsnovaProizv(osnova, "распр") || OsnovaProizv(osnova, "истр"))
                                                {
                                                    osnova = ZamСловоВнутри_or(osnova, "рас", "разо", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "ис", "изо", 'a');
                                                }
                                                else
                                                {
                                                    osnova = С_О_ret_orig(osnova);
                                                    if (!OsnovaProizv(osnova, "отопр") && !Приставка(osnova)) osnova = ZamOsnova_ro(osnova, "опр", "обопр");
                                                    osnova = ZamСловоВнутри_or(osnova, "отор", "отр", 'b');
                                                }
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "у";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                default://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        osnova = ZamOsnova_ro(osnova, "ер", "ри");
                                        if (OsnovaProizv(osnova, "растри") || OsnovaProizv(osnova, "распри") || OsnovaProizv(osnova, "истри"))
                                        {
                                            osnova = ZamСловоВнутри_or(osnova, "рас", "разо", 'a');
                                            osnova = ZamСловоВнутри_or(osnova, "ис", "изо", 'a');
                                        }
                                        else
                                        {
                                            osnova = С_О_ret_orig(osnova);
                                            if (!OsnovaProizv(osnova, "отопри") && !Приставка(osnova)) osnova = ZamOsnova_ro(osnova, "опри", "обопри");
                                            osnova = ZamСловоВнутри_or(osnova, "отори", "отри", 'b');
                                        }
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "ла";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "ло";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "ли";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                
                                                                osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                osnova = ZamOsnova_ro(osnova, "ер", "р");
                                                if (OsnovaProizv(osnova, "растр") || OsnovaProizv(osnova, "распр") || OsnovaProizv(osnova, "истр"))
                                                {
                                                    osnova = ZamСловоВнутри_or(osnova, "рас", "разо", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "ис", "изо", 'a');
                                                }
                                                else
                                                {
                                                    osnova = С_О_ret_orig(osnova);
                                                    if (!OsnovaProizv(osnova, "отопр") && !Приставка(osnova)) osnova = ZamOsnova_ro(osnova, "опр", "обопр");
                                                    osnova = ZamСловоВнутри_or(osnova, "отор", "отр", 'b');
                                                }
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "у";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ёшь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ёт";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ём";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ёте";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 10:
                            slovo = Osnova(osnova, "(?<окончание>(оть)|(оться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        osnova = ZamСловоВнутри_or(osnova, "мол", "мел", 'a') + "и";
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                osnova = ZamСловоВнутри_or(osnova, "мол", "мел", 'a');
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ют";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                            default://настоящее
                                                osnova = ZamСловоВнутри_or(osnova, "мол", "мел", 'a');
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ют";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                default://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        osnova = ZamСловоВнутри_or(osnova, "мол", "мел", 'a') + "и";
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                
                                                                osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                osnova = ZamСловоВнутри_or(osnova, "мол", "мел", 'a');
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ют";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 11:
                            slovo = Osnova(osnova, "(?<окончание>(ить)|(иться))");
                            osnova = slovo.Groups["основа"].Value;
                            switch (VidElRus())
                            {
                                case 1://совершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        osnova += "ей";
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                if (Приставка(osnova))
                                                {
                                                    osnova = ZamСловоВнутри_or(osnova, "рас", "раз", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "оп", "об", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "и(з|с)", "из", 'a');
                                                    if (osnova.Length > 1) { osnova = С_О_ret_orig(osnova); if (osnova[0] == 'в' && !Glasnay(osnova[1])) osnova = "во" + osnova.Substring(1); }
                                                }
                                                osnova += "ь";
                                                osnova = ZamСловоВнутри_or(osnova, "оь", "о" + slovo.Groups["основа"].Value[slovo.Groups["основа"].Value.Length - 1] + "ь", 'a');
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ют";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                            default://настоящее
                                                if (Приставка(osnova))
                                                {
                                                    osnova = ZamСловоВнутри_or(osnova, "рас", "раз", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "оп", "об", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "и(з|с)", "из", 'a');
                                                    if (osnova.Length > 1) { osnova = С_О_ret_orig(osnova); if (osnova[0] == 'в' && !Glasnay(osnova[1])) osnova = "во" + osnova.Substring(1); }
                                                }
                                                osnova += "ь";
                                                osnova = ZamСловоВнутри_or(osnova, "оь", "о" + slovo.Groups["основа"].Value[slovo.Groups["основа"].Value.Length - 1] + "ь", 'a');
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ют";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                                default://несовершенный
                                    if (c.naklonenie == Naklonenie.Povelitelnoe)
                                    {
                                        osnova += "ей";
                                        if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                        else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                    }
                                    else
                                        switch (VremyElRus())
                                        {
                                            case 1://прошедшее
                                                osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (c.rod)
                                                        {
                                                            case Rod.Muzhskoj:
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case Rod.Zhenskij:
                                                                osnova += "а";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default:
                                                                osnova += "о";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        osnova += "и";
                                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                        break;
                                                }
                                                break;
                                            case 2://будущее
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                
                                                                osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            case 2://2 лицо
                                                                osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                            default://3 лицо
                                                                osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                break;
                                            default://настоящее
                                                if (Приставка(osnova))
                                                {
                                                    osnova = ZamСловоВнутри_or(osnova, "рас", "раз", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "оп", "об", 'a');
                                                    osnova = ZamСловоВнутри_or(osnova, "и(з|с)", "из", 'a');
                                                    if (osnova.Length > 1) { osnova = С_О_ret_orig(osnova); if (osnova[0] == 'в' && !Glasnay(osnova[1])) osnova = "во" + osnova.Substring(1); }
                                                }
                                                osnova += "ь";
                                                osnova = ZamСловоВнутри_or(osnova, "оь", "о" + slovo.Groups["основа"].Value[slovo.Groups["основа"].Value.Length - 1] + "ь", 'a');
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ю";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                osnova += "ют";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                    break;
                            }
                            break;
                        case 12:
                            slovo = Osnova(osnova, "(?<окончание>(ыть)|(ыться)|(уть)|(уться)|(еть)|(еться)|(ить)|(иться))");
                            osnova = slovo.Groups["основа"].Value;
                            if (osnova.Length > 0)
                                switch (VidElRus())
                                {
                                    case 1://совершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            if (OsnovaProizv(osnova, "бр")) osnova += "ей";
                                            else if (OsnovaProizv(slovo.Value, "(петь)|(петься)")) osnova += "ой";
                                            else
                                                switch (slovo.Groups["окончание"].Value[0])
                                                {
                                                    case 'ы': osnova += "ой"; break;
                                                    default: osnova = osnova + slovo.Groups["окончание"].Value[0] + "й"; break;
                                                }
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее
                                                    if (OsnovaProizv(osnova, "бр")) osnova += "е";
                                                    else if (OsnovaProizv(slovo.Value, "(петь)|(петься)")) osnova += "о";
                                                    else
                                                        switch (slovo.Groups["окончание"].Value[0])
                                                        {
                                                            case 'ы': osnova += "о"; break;
                                                            default: osnova = osnova + slovo.Groups["окончание"].Value[0]; break;
                                                        }
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ю";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ют";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                                default://настоящее
                                                    if (OsnovaProizv(osnova, "бр")) osnova += "е";
                                                    else if (OsnovaProizv(slovo.Value, "(петь)|(петься)")) osnova += "о";
                                                    else
                                                        switch (slovo.Groups["окончание"].Value[0])
                                                        {
                                                            case 'ы': osnova += "о"; break;
                                                            default: osnova = osnova + slovo.Groups["окончание"].Value[0]; break;
                                                        }
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ю";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ют";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                    default://несовершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            if (OsnovaProizv(osnova, "бр")) osnova += "ей";
                                            else if (OsnovaProizv(slovo.Value, "(петь)|(петься)")) osnova += "ой";
                                            else
                                                switch (slovo.Groups["окончание"].Value[0])
                                                {
                                                    case 'ы': osnova += "ой"; break;
                                                    default: osnova = osnova + slovo.Groups["окончание"].Value[0] + "й"; break;
                                                }
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо                                                                
                                                                    osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                    break;
                                                default://настоящее
                                                    if (OsnovaProizv(osnova, "бр")) osnova += "е";
                                                    else if (OsnovaProizv(slovo.Value, "(петь)|(петься)")) osnova += "о";
                                                    else
                                                        switch (slovo.Groups["окончание"].Value[0])
                                                        {
                                                            case 'ы': osnova += "о"; break;
                                                            default: osnova = osnova + slovo.Groups["окончание"].Value[0]; break;
                                                        }
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ю";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ют";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                }
                            break;
                        case 13:
                            slovo = Osnova(osnova, "(?<окончание>(вать)|(ваться))");
                            osnova = slovo.Groups["основа"].Value;
                            if (osnova.Length > 0)
                                switch (VidElRus())
                                {
                                    case 1://совершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            osnova += "вай";
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova += "вал";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее                                                    
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ю";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёшь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ёт";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ём";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёте";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ют";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                                default://настоящее
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ю";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёшь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ёт";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ём";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёте";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ют";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                    default://несовершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            osnova += "вай";
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova += "вал";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо                                                                
                                                                    osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                    break;
                                                default://настоящее                                                    
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ю";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёшь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ёт";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ём";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёте";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ют";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                }
                            break;
                        case 14:
                            slovo = Osnova(osnova, "(?<окончание>(ать)|(аться)|(ять)|(яться))");
                            osnova = slovo.Groups["основа"].Value;
                            if (osnova.Length > 0)
                                switch (VidElRus())
                                {
                                    case 1://совершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            if (match.Groups["буд"].Value != "") osnova = Чередов(match.Groups["буд"].Value) + "и";
                                            else osnova = osnova + Чередов(match.Groups["черед"].Value) + "и";
                                            if (match.Groups["index"].Value == "") osnova = ZamПриставка(osnova, "(из)|(под)|(над)|(в)|(раз)|(с)|(об)|(от)", 'о');
                                            else if (match.Groups["index"].Value == "с" && match.Groups["буд"].Value == "" && match.Groups["черед"].Value != "(-им-)") osnova = ZamПриставка(osnova, "(раз)|(из)|(от)", 'ы');
                                            osnova = ZamСловоВнутри_or(osnova, "ъи", "ми", 'c');
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее   
                                                    if (match.Groups["буд"].Value != "") osnova = Чередов(match.Groups["буд"].Value);
                                                    else osnova = osnova + Чередов(match.Groups["черед"].Value);
                                                    if (match.Groups["index"].Value == "") osnova = ZamПриставка(osnova, "(из)|(под)|(над)|(в)|(раз)|(с)|(об)|(от)", 'о');
                                                    else if (match.Groups["index"].Value == "с" && match.Groups["буд"].Value == "" && match.Groups["черед"].Value != "(-им-)") osnova = ZamПриставка(osnova, "(раз)|(из)|(от)", 'ы');
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъу", "му", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъешь", "мешь", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъет", "мет", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъем", "мем", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъете", "мете", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъут", "мут", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                                default://настоящее
                                                    if (match.Groups["буд"].Value != "") osnova = Чередов(match.Groups["буд"].Value);
                                                    else osnova = osnova + Чередов(match.Groups["черед"].Value);
                                                    if (match.Groups["index"].Value == "") osnova = ZamПриставка(osnova, "(из)|(под)|(над)|(в)|(раз)|(с)|(об)|(от)", 'о');
                                                    else if (match.Groups["index"].Value == "с" && match.Groups["буд"].Value == "" && match.Groups["черед"].Value != "(-им-)") osnova = ZamПриставка(osnova, "(раз)|(из)|(от)", 'ы');
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъу", "му", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъешь", "мешь", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъет", "мет", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъем", "мем", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъете", "мете", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъут", "мут", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                    default://несовершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            if (match.Groups["буд"].Value != "") osnova = Чередов(match.Groups["буд"].Value) + "и";
                                            else osnova = osnova + Чередов(match.Groups["черед"].Value) + "и";
                                            if (match.Groups["index"].Value == "") osnova = ZamПриставка(osnova, "(из)|(под)|(над)|(в)|(раз)|(с)|(об)|(от)", 'о');
                                            else if (match.Groups["index"].Value == "с" && match.Groups["буд"].Value == "" && match.Groups["черед"].Value != "(-им-)") osnova = ZamПриставка(osnova, "(раз)|(из)|(от)", 'ы');
                                            osnova = ZamСловоВнутри_or(osnova, "ъи", "ми", 'c');
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо                                                                
                                                                    osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                    break;
                                                default://настоящее  
                                                    if (match.Groups["буд"].Value != "") osnova = Чередов(match.Groups["буд"].Value);
                                                    else osnova = osnova + Чередов(match.Groups["черед"].Value);
                                                    if (match.Groups["index"].Value == "") osnova = ZamПриставка(osnova, "(из)|(под)|(над)|(в)|(раз)|(с)|(об)|(от)", 'о');
                                                    else if (match.Groups["index"].Value == "с" && match.Groups["буд"].Value == "" && match.Groups["черед"].Value != "(-им-)") osnova = ZamПриставка(osnova, "(раз)|(из)|(от)", 'ы');
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъу", "му", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъешь", "мешь", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъет", "мет", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъем", "мем", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъёте", "мете", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    osnova = ZamСловоВнутри_or(osnova, "ъут", "мут", 'c');
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                }
                            break;
                        case 15:
                            slovo = Osnova(osnova, "(?<окончание>(ть)|(ться))");
                            osnova = slovo.Groups["основа"].Value;
                            if (osnova.Length > 0)
                                switch (VidElRus())
                                {
                                    case 1://совершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            osnova += "нь";
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova += "л";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее   
                                                    osnova += "н";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                                default://настоящее
                                                    osnova += "н";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                    default://несовершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            osnova += "нь";
                                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy) osnova += "сь"; }
                                            else if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova += "л";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо                                                                
                                                                    osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                    break;
                                                default://настоящее  
                                                    osnova += "н";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ешь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ет";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ем";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ете";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                }
                            break;
                        case 16:
                            slovo = Osnova(osnova, "(?<окончание>(ть)|(ться))");
                            osnova = slovo.Groups["основа"].Value;
                            if (osnova.Length > 0)
                                switch (VidElRus())
                                {
                                    case 1://совершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            osnova += "ви";
                                            if (ChisloElRus() > 0) osnova += "те";
                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova += "л";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее   
                                                    osnova += "в";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёшь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ёт";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ём";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёте";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                                default://настоящее
                                                    osnova += "в";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёшь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ёт";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ём";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёте";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                    default://несовершенный
                                        if (c.naklonenie == Naklonenie.Povelitelnoe)
                                        {
                                            osnova += "ви";
                                            if (ChisloElRus() > 0) osnova += "те";
                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                        }
                                        else
                                            switch (VremyElRus())
                                            {
                                                case 1://прошедшее
                                                    osnova += "л";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (c.rod)
                                                            {
                                                                case Rod.Muzhskoj:
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case Rod.Zhenskij:
                                                                    osnova += "а";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default:
                                                                    osnova += "о";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            osnova += "и";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 2://будущее
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо                                                                
                                                                    osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                                default://3 лицо
                                                                    osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                    break;
                                                default://настоящее  
                                                    osnova += "в";
                                                    switch (ChisloElRus())
                                                    {
                                                        case 0://ед.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "у";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёшь";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ёт";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                        case 1://множ.число
                                                            switch (lico)
                                                            {
                                                                case 1://1 лицо
                                                                    osnova += "ём";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                                case 2://2 лицо
                                                                    osnova += "ёте";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                    break;
                                                                default://3 лицо
                                                                    osnova += "ут";
                                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                    break;
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        break;
                                }
                            break;
                    }
                }
                if (match.Groups["type"].Value == "" || !match.Success)//изолированные глаголы
                {
                    Match slovo = Osnova(osnova, "(?<окончание>(хать)|(хаться))");
                    if (!slovo.Success)
                    {
                        slovo = Osnova(osnova, "(?<окончание>(шибить)|(шибиться))");
                        if (!slovo.Success) slovo = Osnova(osnova, "(?<окончание>(ыть)|(ыться)|(сться)|(сть)|(ти)|(тись)|(еть)|(еться)|(ить)|(иться)|(ать)|(аться))");
                    }
                    osnova = slovo.Groups["основа"].Value;
                    if (osnova.Length > 0)
                        switch (VidElRus())
                        {
                            case 1://совершенный
                                if (c.naklonenie == Naklonenie.Povelitelnoe)
                                {
                                    switch (slovo.Groups["окончание"].Value[0])
                                    {
                                        case 'ш': osnova += "шиби"; break;
                                        case 'а': osnova = osnova + slovo.Groups["окончание"].Value[0] + "й"; break;
                                        case 'ы': osnova += "удь"; break;
                                        case 'с':
                                            if (osnova[osnova.Length - 1] == 'е') osnova += "шь";
                                            else osnova += "ни";
                                            break;
                                        case 'т':
                                            if (osnova[osnova.Length - 1] == 'й') { osnova += "ди"; osnova = ZamСловоВнутри_or(osnova, "прийди", "приди", 'a'); }
                                            else osnova += "и";
                                            break;
                                        case 'х': osnova = ZamСловоВнутри_or(slovo.Value, "ехать", "езжай", 'a'); break;
                                        case 'и':
                                            if (osnova[osnova.Length - 1] == 'б') osnova += "ли";
                                            else osnova += "и";
                                            break;
                                        default: osnova += "и"; break;
                                    }
                                    if (ChisloElRus() > 0) { osnova += "те"; }
                                    if (c.zalog == Zalog.Vozvratniy) { if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь"; }
                                    osnova = ZamСловоВнутри_or(osnova, "ьсь", "ься", 'a');
                                }
                                else
                                    switch (VremyElRus())
                                    {
                                        case 1://прошедшее
                                            switch (slovo.Groups["окончание"].Value[0])
                                            {
                                                case 'ш': osnova += "шиб"; break;
                                                case 'с': osnova += "л"; break;
                                                case 'т': { osnova = ZamСловоВнутри_or(slovo.Value, "((ид)|(й))ти", "шёл", 'a'); break; }
                                                case 'х': osnova += "хал"; break;
                                                default: osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л"; break;
                                            }
                                            switch (ChisloElRus())
                                            {
                                                case 0://ед.число
                                                    switch (c.rod)
                                                    {
                                                        case Rod.Muzhskoj:
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                            break;
                                                        case Rod.Zhenskij:
                                                            if (slovo.Groups["окончание"].Value[0] == 'ш') osnova += "л";
                                                            osnova += "а";
                                                            osnova = ZamСловоВнутри_or(osnova, "шёл", "шл", 'a');
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                        default:
                                                            if (slovo.Groups["окончание"].Value[0] == 'ш') osnova += "л";
                                                            osnova += "о";
                                                            osnova = ZamСловоВнутри_or(osnova, "шёл", "шл", 'a');
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 1://множ.число
                                                    if (slovo.Groups["окончание"].Value[0] == 'ш') osnova += "л";
                                                    osnova += "и";
                                                    osnova = ZamСловоВнутри_or(osnova, "шёл", "шл", 'a');
                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                    break;
                                            }
                                            break;
                                        case 2://будущее   
                                            switch (slovo.Groups["окончание"].Value[0])
                                            {
                                                case 'ш': osnova += "шиб"; break;
                                                case 'ы': osnova += "уд"; break;
                                                case 'т': if (osnova[osnova.Length - 1] == 'й') { osnova += "д"; } break;
                                                case 'с': if (osnova[osnova.Length - 1] == 'я') osnova += "н"; break;
                                                case 'х': osnova += "д"; break;
                                                case 'и': if (osnova[osnova.Length - 1] == 'б') osnova += "л"; break;
                                            }
                                            Match n = Osnova_(slovo.Value, "(есть)|(дать)", 'b');
                                            if (n.Success && slovo.Groups["окончание"].Value[0] == 'а') osnova += "а";
                                            switch (ChisloElRus())
                                            {
                                                case 0://ед.число
                                                    switch (lico)
                                                    {
                                                        case 1://1 лицо                                                                
                                                            if (n.Success) osnova += "м";
                                                            else if (OsnovaProizv(osnova, "зыбл")) osnova += "ю";
                                                            else osnova += "у";
                                                            if (c.zalog == Zalog.Vozvratniy) { if (osnova[osnova.Length - 1] != 'м') osnova += "сь"; else osnova += "ся"; }
                                                            break;
                                                        case 2://2 лицо
                                                            if (n.Success) osnova += "шь";
                                                            else osnova += "ешь";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                            break;
                                                        default://3 лицо
                                                            if (n.Success) osnova += "ст";
                                                            else osnova += "ет";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                            break;
                                                    }
                                                    break;
                                                case 1://множ.число
                                                    switch (lico)
                                                    {
                                                        case 1://1 лицо
                                                            if (n.Success) osnova += "дим";
                                                            else osnova += "ем";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                            break;
                                                        case 2://2 лицо
                                                            if (n.Success) osnova += "дите";
                                                            else osnova += "ете";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                        default://3 лицо
                                                            if (n.Success) { osnova += "дят"; if (slovo.Groups["окончание"].Value[0] == 'а') osnova = ZamСловоВнутри_or(osnova, "дадят", "дадут", 'a'); }
                                                            else if (OsnovaProizv(osnova, "зыбл")) osnova += "ют";
                                                            else osnova += "ут";
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                            break;
                                                    }
                                                    break;
                                            }
                                            break;
                                        default://настоящее
                                            if (osnova == "б") { osnova = ZamСловоВнутри_or(slovo.Value, "быть", "есть", 'a'); }
                                            else
                                            {
                                                switch (slovo.Groups["окончание"].Value[0])
                                                {
                                                    case 'т': if (osnova[osnova.Length - 1] == 'й') { osnova += "д"; } break;
                                                    case 'ш': osnova += "шиб"; break;
                                                    case 'ы': osnova += "уд"; break;
                                                    case 'с': if (osnova[osnova.Length - 1] == 'я') osnova += "н"; break;
                                                    case 'х': osnova += "д"; break;
                                                    case 'и': if (osnova[osnova.Length - 1] == 'б') osnova += "л"; break;
                                                }
                                                Match n1 = Osnova_(slovo.Value, "(есть)|(дать)", 'b');
                                                if (n1.Success && slovo.Groups["окончание"].Value[0] == 'а') osnova += "а";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                
                                                                if (n1.Success) osnova += "м";
                                                                else if (OsnovaProizv(osnova, "зыбл")) osnova += "ю";
                                                                else osnova += "у";
                                                                if (c.zalog == Zalog.Vozvratniy) { if (osnova[osnova.Length - 1] != 'м') osnova += "сь"; else osnova += "ся"; }
                                                                break;
                                                            case 2://2 лицо
                                                                if (n1.Success) osnova += "шь";
                                                                else osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                if (n1.Success) osnova += "ст";
                                                                else osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (n1.Success) osnova += "дим";
                                                                else osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                if (n1.Success) osnova += "дите";
                                                                else osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                if (n1.Success) { osnova += "дят"; if (slovo.Groups["окончание"].Value[0] == 'а') osnova = ZamСловоВнутри_or(osnova, "дадят", "дадут", 'a'); }
                                                                else if (OsnovaProizv(osnova, "зыбл")) osnova += "ют";
                                                                else osnova += "ут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                break;
                            default://несовершенный                                
                                if (c.naklonenie == Naklonenie.Povelitelnoe)
                                {
                                    switch (slovo.Groups["окончание"].Value[0])
                                    {
                                        case 'ш': osnova += "шиби"; break;
                                        case 'а': osnova = osnova + slovo.Groups["окончание"].Value[0] + "й"; break;
                                        case 'ы': osnova += "удь"; break;
                                        case 'с':
                                            if (osnova[osnova.Length - 1] == 'е') osnova += "шь";
                                            else osnova += "ни";
                                            break;
                                        case 'т':
                                            if (osnova[osnova.Length - 1] == 'й') { osnova += "ди"; osnova = ZamСловоВнутри_or(osnova, "прийди", "приди", 'a'); }
                                            else osnova += "и";
                                            break;
                                        case 'х': osnova = ZamСловоВнутри_or(slovo.Value, "ехать", "езжай", 'a'); break;
                                        case 'и':
                                            if (osnova[osnova.Length - 1] == 'б') osnova += "ли";
                                            else osnova += "и";
                                            break;
                                        default: osnova += "и"; break;
                                    }
                                    if (ChisloElRus() > 0) { osnova += "те"; }
                                    if (c.zalog == Zalog.Vozvratniy) { if (osnova[osnova.Length - 1] == 'й') osnova += "ся"; else osnova += "сь"; }
                                    osnova = ZamСловоВнутри_or(osnova, "ьсь", "ься", 'a');
                                }
                                else
                                    switch (VremyElRus())
                                    {
                                        case 1://прошедшее
                                            switch (slovo.Groups["окончание"].Value[0])
                                            {
                                                case 'с': osnova += "л"; break;
                                                case 'т': osnova = ZamСловоВнутри_or(slovo.Value, "((ид)|(й))ти", "шёл", 'a'); break;
                                                case 'х': osnova += "хал"; break;
                                                case 'ш': osnova += "шиб"; break;
                                                default: osnova = osnova + slovo.Groups["окончание"].Value[0].ToString() + "л"; break;
                                            }
                                            switch (ChisloElRus())
                                            {
                                                case 0://ед.число
                                                    switch (c.rod)
                                                    {
                                                        case Rod.Muzhskoj:
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                            break;
                                                        case Rod.Zhenskij:
                                                            if (slovo.Groups["окончание"].Value[0] == 'ш') osnova += "л";
                                                            osnova += "а";
                                                            osnova = ZamСловоВнутри_or(osnova, "шёл", "шл", 'a');
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                        default:
                                                            if (slovo.Groups["окончание"].Value[0] == 'ш') osnova += "л";
                                                            osnova += "о";
                                                            osnova = ZamСловоВнутри_or(osnova, "шёл", "шл", 'a');
                                                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                            break;
                                                    }
                                                    break;
                                                case 1://множ.число
                                                    if (slovo.Groups["окончание"].Value[0] == 'ш') osnova += "л";
                                                    osnova += "и";
                                                    osnova = ZamСловоВнутри_or(osnova, "шёл", "шл", 'a');
                                                    if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                    break;
                                            }
                                            break;
                                        case 2://будущее
                                            switch (ChisloElRus())
                                            {
                                                case 0://ед.число
                                                    switch (lico)
                                                    {
                                                        case 1://1 лицо   
                                                            if (slovo.Groups["окончание"].Value[0] == 'ы') osnova = ZamСловоВнутри_or(slovo.Value, "быть", "буду", 'a');
                                                            else osnova = "буду " + osnova + slovo.Groups["окончание"].Value;
                                                            break;
                                                        case 2://2 лицо
                                                            if (slovo.Groups["окончание"].Value[0] == 'ы') osnova = ZamСловоВнутри_or(slovo.Value, "быть", "будешь", 'a');
                                                            else osnova = "будешь " + osnova + slovo.Groups["окончание"].Value;
                                                            break;
                                                        default://3 лицо
                                                            if (slovo.Groups["окончание"].Value[0] == 'ы') osnova = ZamСловоВнутри_or(slovo.Value, "быть", "будет", 'a');
                                                            else osnova = "будет " + osnova + slovo.Groups["окончание"].Value;
                                                            break;
                                                    }
                                                    break;
                                                case 1://множ.число
                                                    switch (lico)
                                                    {
                                                        case 1://1 лицо
                                                            if (slovo.Groups["окончание"].Value[0] == 'ы') osnova = ZamСловоВнутри_or(slovo.Value, "быть", "будем", 'a');
                                                            else osnova = "будем " + osnova + slovo.Groups["окончание"].Value;
                                                            break;
                                                        case 2://2 лицо
                                                            if (slovo.Groups["окончание"].Value[0] == 'ы') osnova = ZamСловоВнутри_or(slovo.Value, "быть", "будете", 'a');
                                                            else osnova = "будете " + osnova + slovo.Groups["окончание"].Value;
                                                            break;
                                                        default://3 лицо
                                                            if (slovo.Groups["окончание"].Value[0] == 'ы') osnova = ZamСловоВнутри_or(slovo.Value, "быть", "будут", 'a');
                                                            else osnova = "будут " + osnova + slovo.Groups["окончание"].Value;
                                                            break;
                                                    }
                                                    break;
                                            }
                                            if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                            break;
                                        default://настоящее  
                                            if (slovo.Groups["окончание"].Value[0] == 'ы') { osnova = ZamСловоВнутри_or(slovo.Value, "быть", "есть", 'a'); }
                                            else
                                            {
                                                switch (slovo.Groups["окончание"].Value[0])
                                                {
                                                    case 'ш': osnova += "шиб"; break;
                                                    case 'т': if (osnova[osnova.Length - 1] == 'й') { osnova += "д"; } break;
                                                    case 'с': if (osnova[osnova.Length - 1] == 'я') osnova += "н"; break;
                                                    case 'х': osnova += "д"; break;
                                                    case 'и': if (osnova[osnova.Length - 1] == 'б') osnova += "л"; break;
                                                }
                                                Match n = Osnova_(slovo.Value, "(есть)|(дать)", 'b');
                                                if (n.Success && slovo.Groups["окончание"].Value[0] == 'а') osnova += "а";
                                                switch (ChisloElRus())
                                                {
                                                    case 0://ед.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо                                                                
                                                                if (n.Success) osnova += "м";
                                                                else if (OsnovaProizv(osnova, "зыбл")) osnova += "ю";
                                                                else osnova += "у";
                                                                if (c.zalog == Zalog.Vozvratniy) { if (osnova[osnova.Length - 1] != 'м') osnova += "сь"; else osnova += "ся"; }
                                                                break;
                                                            case 2://2 лицо
                                                                if (n.Success) osnova += "шь";
                                                                else osnova += "ешь";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            default://3 лицо
                                                                if (n.Success) osnova += "ст";
                                                                else osnova += "ет";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                    case 1://множ.число
                                                        switch (lico)
                                                        {
                                                            case 1://1 лицо
                                                                if (n.Success) osnova += "дим";
                                                                else osnova += "ем";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                            case 2://2 лицо
                                                                if (n.Success) osnova += "дите";
                                                                else osnova += "ете";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                                                break;
                                                            default://3 лицо
                                                                if (n.Success) { osnova += "дят"; if (slovo.Groups["окончание"].Value[0] == 'а') osnova = ZamСловоВнутри_or(osnova, "дадят", "дадут", 'a'); }
                                                                else if (OsnovaProizv(osnova, "зыбл")) osnova += "ют";
                                                                else osnova += "ут";
                                                                if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                                                                break;
                                                        }
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                break;
                        }
                }
                /*osnova=ZamСловоВнутри_or(osnova, "ьсь", "ься", 'a');
                newSlovoRus =osnova= ZamСловоВнутри_or(osnova, "йсь", "йся", 'a');*/
                // MessageBox.Show(osnova);
                if (c.naklonenie == Naklonenie.Povelitelnoe)
                {
                    if (Pom(arPometa, "2"))
                    {
                        Match a = Osnova_(osnova, "и|й", 'c');
                        if (a.Groups["часть1"].Value + a.Groups["часть2"].Value != osnova) a = Osnova_(osnova, "(и|й)((те)|(сь))", 'c');
                        if (a.Groups["часть1"].Value + a.Groups["часть2"].Value != osnova) a = Osnova_(osnova, "(и|й)тесь", 'c');
                        if (a.Success && (a.Groups["часть1"].Value + a.Groups["часть2"].Value == osnova))
                        {
                            if (Glasnay(a.Groups["часть1"].Value)) osnova = a.Groups["часть1"].Value + "й";
                            else osnova = a.Groups["часть1"].Value + "ь";
                            if (ChisloElRus() > 0) { osnova += "те"; if (c.zalog == Zalog.Vozvratniy)osnova += "сь"; }
                            else if (c.zalog == Zalog.Vozvratniy) osnova += "ся";
                        }
                    }
                    else if (Pom(arPometa, "3"))
                    {
                        Match a = Osnova_(osnova, "(и|ь)тесь", 'c');
                        if (a.Groups["часть1"].Value + a.Groups["часть2"].Value != osnova) a = Osnova_(osnova, "(и|ь)((те)|(сь))", 'c');
                        if (a.Groups["часть1"].Value + a.Groups["часть2"].Value != osnova) a = Osnova_(osnova, "и|ь", 'c');
                        if (a.Success && (a.Groups["часть1"].Value + a.Groups["часть2"].Value == osnova))
                        {
                            if (ChisloElRus() < 1) osnova = a.Groups["часть1"].Value + "и";
                            else { osnova = a.Groups["часть1"].Value + "ьте"; }
                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                        }
                    }
                }
                newSlovoRus = osnova;
                if (haract == Haract.деепричастие)
                {
                    string i = match.Groups["type"].Value;
                    switch (VremyElRus())
                    {
                        case 0://настоящ                            
                            if (SlovoRus == "быть") osnova = "будучи";                           
                            else
                            {
                                Match slovo = null;
                                if (i == "13") slovo = Osnova_(osnova, "ть", 'c');
                                else slovo = Osnova(osnova, "((ю|(у|я|а)т)(ся)?)");
                                if (slovo.Success)
                                {
                                    osnova = slovo.Groups["основа"].Value;
                                    if (Ship(osnova)) osnova += "а";
                                    else { osnova += "я"; }
                                }
                            }
                            if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                            break;
                        default://прош                            
                            Match b;
                            if (SlovoRus == "идти") osnova = "шедши";
                            else if (Pom(arPometa, "9"))
                            {
                                //MessageBox.Show(osnova,"вход");
                                b = Osnova(osnova, "(?<окончание>усь)");
                                //if (b.Success) MessageBox.Show(b.Groups["окончание"].Value);
                                if (b.Success && (b.Groups["основа"].Value + b.Groups["окончание"].Value == osnova)) { osnova = b.Groups["основа"].Value + "я"; }
                                else if (osnova[osnova.Length - 1] == 'у') osnova = ZamLit(osnova, 'я');
                                if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                //MessageBox.Show(osnova,"выход");
                            }
                            else
                            {
                                b = Osnova_(osnova, "л(ся)?", 'c');
                                if (b.Success && (b.Groups["часть1"].Value + b.Groups["часть2"].Value) == osnova)
                                {
                                    //"вший";
                                    /*if (i == "9" && c.zalog != Zalog.Vozvratniy && OsnovaProizv("ереть(ся)?"))
                                    {
                                        b = Osnova_(osnova, "ереть(ся)?", 'c');
                                        if (b.Success && (b.Groups["часть1"].Value + b.Groups["часть2"].Value) == osnova) osnova = b.Groups["часть1"].Value + "ерев";
                                    }                                  
                                    else*/
                                    {
                                        osnova = b.Groups["часть1"].Value + "в";
                                        if (c.zalog == Zalog.Vozvratniy) osnova += "шись";
                                    }
                                }
                                else//ший
                                {
                                    if (Pom(arPometa, "6"))
                                    {
                                        b = Osnova_(osnova, "ся", 'c');
                                        if (b.Success && (b.Groups["часть1"].Value + b.Groups["часть2"].Value) == osnova) osnova = b.Groups["часть1"].Value + "нув";
                                        else osnova += "нув";
                                        if (c.zalog == Zalog.Vozvratniy) osnova += "шись";
                                    }
                                    else
                                    {
                                        if (((match.Groups["type"].Value == "7" && match.Groups["черед"].Value == "(-д-)" || match.Groups["type"].Value == "7" && match.Groups["черед"].Value == "(-т-)")) && OsnovaProizv(SlovoRus, "сти(сь)?"))
                                        {
                                            b = Osnova(osnova, "усь");
                                            if (!b.Success) { if (osnova[osnova.Length - 1] == 'у') osnova = osnova.Substring(0, osnova.Length - 1); }
                                            else osnova = b.Groups["основа"].Value;
                                        }
                                        else
                                        {
                                            b = Osnova_(osnova, "ся", 'c');
                                            if (b.Success && (b.Groups["часть1"].Value + b.Groups["часть2"].Value) == osnova) osnova = b.Groups["часть1"].Value;

                                        }
                                        osnova += "ши";
                                        if (c.zalog == Zalog.Vozvratniy) osnova += "сь";
                                    }
                                }
                            }
                            break;
                    }
                    newSlovoRus = osnova;
                }
                if (c.naklonenie == Naklonenie.Soslagatelnoe) newSlovoRus += " бы";
            }
            return newSlovoRus;
        }
        /// <summary>
        /// проверяет послед.букву шипящая или нет
        /// </summary>
        /// <param name="slovo">слово на русском</param>
        /// <returns>true - если шипящая</returns>
        private bool Ship(string slovo)
        {
            if (slovo != "")
                switch (slovo[slovo.Length - 1])
                {
                    case 'ж': return true;
                    case 'ш': return true;
                    case 'ч': return true;
                    case 'щ': return true;
                    case 'ц': return true;
                }
            return false;
        }
        /// <summary>
        /// определяет наличие приставки "вы"
        /// </summary>
        /// <param name="slovo">слово на русском</param>
        /// <returns>если приставка найдена - возвращает true</returns>
        private bool ПриставкаВы(string slovo)
        {
            if (slovo != "")
            {
                Regex reg = new Regex("вы([а-я]){1,}");
                Match match = reg.Match(slovo);
                if (match.Success) return true;
            }
            return false;
        }
        /// <summary>
        /// определяет приставку (кроме "в")
        /// </summary>
        /// <param name="slovo">основа</param>
        private bool Приставка(string slovo)
        {
            if (slovo != "")
            {
                Regex reg = new Regex(@"(?<приставка>(у)|(пре)|(пере)|(при)|(раз)|(разо)|(об)|(обо)|(вы)|(за)|(от)|(ото)|(по)|(с)|(со)|(на)|(про)|(рас)|(о)|(из)|(изо)|(под)|(подо)|(над)|(надо)|в)(?<основа>[а-я]{1,})");
                Match match = reg.Match(slovo);
                if (match.Success && (match.Groups["приставка"].Value + match.Groups["основа"].Value == slovo)) { return true; }
            }
            return false;
        }
        /// <summary>
        /// определяет приставку(начало слова) (кроме "в")
        /// </summary>
        /// <param name="slovo">основа</param>
        /// <returns>возвращает приставку, если есть, или пустую строку</returns>
        private string Приставка_(string slovo)
        {
            if (slovo != "")
            {
                Regex reg = new Regex(@"(?<приставка>(у)|(пре)|(пере)|(при)|(раз)|(разо)|(об)|(обо)|(вы)|(за)|(от)|(ото)|(по)|(с)|(со)|(на)|(про)|(рас)|(о)|(из)|(изо)|(под)|(подо)|(над)|(надо))(?<основа>[а-я]{1,})");
                Match match = reg.Match(slovo);
                if (match.Success && (match.Groups["приставка"].Value + match.Groups["основа"].Value == slovo)) return match.Groups["приставка"].Value;
            }
            return "";
        }
        /// <summary>
        /// заменяет приставку (!кроме приставки 'о')
        /// </summary>
        /// <param name="slovo">анализируемое слово</param>
        /// <returns>возвращает изменённую основу, если есть приставка, или первон. slovo</returns>
        private string ZamПриставка(string slovo, string newприставка)
        {
            if (slovo != "")
            {
                Regex reg = new Regex(@"(?<приставка>(у)|(пре)|(пере)|(при)|(раз)|(разо)|(об)|(обо)|(вы)|(за)|(от)|(ото)|(по)|(с)|(со)|(на)|(про)|(рас))(?<основа>([а-я]){1,})");
                Match match = reg.Match(slovo);
                if (match.Success) return newприставка + match.Groups["основа"].Value;
            }
            return slovo;
        }
        /// <summary>
        /// заменяет старое начало слова на новое
        /// </summary>
        /// <param name="slovo">слово целиком</param>
        /// <param name="oldприставка">старое начало</param>
        /// <param name="newприставка">новое начало</param>
        /// <returns>возвращает изменённое слово, если была oldприставка, иначе - slovo</returns>
        private string ZamПриставка(string slovo, string oldприставка, string newприставка)
        {
            if (slovo != "")
            {
                Regex reg = new Regex(@"(?<приставка>" + oldприставка + ")(?<основа>[а-яёй]*)");
                Match match = reg.Match(slovo);
                if (match.Success) return newприставка + match.Groups["основа"].Value;
            }
            return slovo;
        }
        /// <summary>
        /// дописывает к приставке дополнительную букву
        /// </summary>
        /// <param name="slovo">слово целиком</param>
        /// <param name="oldприставка">старое начало</param>
        ///<param name="doplit">прибавляемая буква к oldприставка</param>
        /// <returns>возвращает изменённое слово, если была oldприставка, иначе - slovo</returns>
        private string ZamПриставка(string slovo, string приставка, char doplit)
        {
            if (slovo != "")
            {
                Regex reg = new Regex(@"(?<приставка>" + приставка + ")(?<основа>[а-яёй]*)");
                Match match = reg.Match(slovo);
                if (match.Success) return match.Groups["приставка"].Value + doplit + match.Groups["основа"].Value;
            }
            return slovo;
        }
        private bool Пбмвф(string slovo)
        {
            if (slovo != null)
                switch (slovo[slovo.Length - 1])
                {
                    case 'п': return true;
                    case 'б': return true;
                    case 'м': return true;
                    case 'в': return true;
                    case 'ф': return true;
                }
            return false;
        }
        private bool Сзтд(string slovo)
        {
            if (slovo != null)
                switch (slovo[slovo.Length - 1])
                {
                    case 'с': return true;
                    case 'з': return true;
                    case 'т': return true;
                    case 'д': return true;
                }
            return false;
        }
        private bool Сзтдкгх(string slovo)
        {
            if (slovo != null)
                switch (slovo[slovo.Length - 1])
                {
                    case 'с': return true;
                    case 'з': return true;
                    case 'т': return true;
                    case 'д': return true;
                    case 'к': return true;
                    case 'г': return true;
                    case 'х': return true;
                }
            return false;
        }
        /// <summary>
        /// ищет в конце основы сочетание "сл" и заменяеи его на "шл" или "стл" на "стел"
        /// </summary>
        /// <param name="osnova"></param>
        /// <returns></returns>
        private string Сл_Стл(string osnova)
        {
            Regex a = new Regex(@"(?<приставка>\w{0,})(?<окончание>(сл)|(стл))");
            Match b = a.Match(osnova);
            if (b.Success)
            {
                osnova = b.Groups["приставка"].Value;
                switch (b.Groups["окончание"].Value)
                {
                    case "сл": osnova += "шл"; break;
                    case "стл": osnova += "стел"; break;
                }
            }
            return osnova;
        }
        private bool ЛнрГлас(string slovo)
        {
            if (slovo != null)
                switch (slovo[slovo.Length - 1])
                {
                    case 'л': return true;
                    case 'н': return true;
                    case 'р': return true;
                    default:
                        if (Glasnay(slovo)) return true;
                        break;
                }
            return false;
        }
        private bool Лнр(string slovo)
        {
            if (slovo != null)
                switch (slovo[slovo.Length - 1])
                {
                    case 'л': return true;
                    case 'н': return true;
                    case 'р': return true;
                }
            return false;
        }
        /// <summary>
        /// меняет чередуемую букву в глаголах типа 4
        /// </summary>
        /// <param name="slovo">слово на русском без окончания ить</param>
        /// <param name="черед">признак чередования</param>
        /// <returns>изменённую основу</returns>
        private string Чередов4(string slovo, string черед)
        {
            string rezult = slovo;
            if (slovo != "")
            {
                if (slovo.Length > 1)
                    if (slovo.Substring(slovo.Length - 2) == "ст") { rezult = slovo.Substring(0, slovo.Length - 2) + "щ"; return rezult; }
                if (Сзтд(slovo))
                    switch (slovo[slovo.Length - 1])
                    {
                        case 'с': rezult = ZamLit(slovo, 'ш', slovo.Length - 1); break;
                        case 'т':
                            if (черед == "(-щ-)") rezult = ZamLit(slovo, 'щ', slovo.Length - 1);
                            else rezult = ZamLit(slovo, 'ч', slovo.Length - 1);
                            break;
                        default: rezult = ZamLit(slovo, 'ж', slovo.Length - 1); break;

                    }
            }
            return rezult;
        }
        /// <summary>
        /// меняет чередуемую букву в глаголах типа 5
        /// </summary>
        /// <param name="slovo">слово на русском без окончания еть</param>
        /// <returns>изменённую основу</returns>
        private string Чередов5(string slovo)
        {
            string rezult = slovo;
            if (slovo != "")
            {
                if (slovo.Length > 1)
                    if (slovo.Substring(slovo.Length - 2) == "ст") { rezult = slovo.Substring(0, slovo.Length - 2) + "щ"; return rezult; }
                switch (slovo[slovo.Length - 1])
                {
                    case 'с': rezult = ZamLit(slovo, 'ш'); break;
                    case 'т': rezult = ZamLit(slovo, 'ч'); break;
                    case 'д': rezult = ZamLit(slovo, 'ж'); break;
                }
            }
            return rezult;
        }
        /// <summary>
        /// меняет чередуемую букву в глаголах типа 6
        /// </summary>
        /// <param name="slovo">слово на русском без окончания ать</param>
        /// <param name="черед">признак чередования</param>
        /// <returns>изменённую основу</returns>
        private string Чередов6(string slovo, string черед)
        {
            string rezult = slovo;
            if (slovo != "")
            {
                if (slovo.Length > 1)
                    if (slovo.Substring(slovo.Length - 2) == "ст" || slovo.Substring(slovo.Length - 2) == "ск") { rezult = slovo.Substring(0, slovo.Length - 2) + "щ"; return rezult; }
                switch (slovo[slovo.Length - 1])
                {
                    case 'с': rezult = ZamLit(slovo, 'ш'); break;
                    case 'з': rezult = ZamLit(slovo, 'ж'); break;
                    case 'д': rezult = ZamLit(slovo, 'ж'); break;
                    case 'т':
                        if (черед == "(-щ-)") rezult = ZamLit(slovo, 'щ'); else rezult = ZamLit(slovo, 'ч');
                        break;
                    case 'к':
                        rezult = ZamLit(slovo, 'ч');
                        break;
                    case 'г': rezult = ZamLit(slovo, 'ж'); break;
                    case 'х': rezult = ZamLit(slovo, 'ш'); break;
                }
            }
            return rezult;
        }
        /// <summary>
        /// выделяет чередование или буд.основу
        /// </summary>
        /// <param name="черед">признак чередования (-черед-)</param>
        /// <returns>возвращает выделен.черед или первон.черед</returns>
        private string Чередов(string черед)
        {
            if (черед != "")
            {
                Regex a = new Regex(@"\(\-?(?<черед>[а-я]{1,})\-.*\)");
                Match temp = a.Match(черед);
                if (temp.Success) return temp.Groups["черед"].Value;
            }
            return черед;
        }
        /// <summary>
        /// Выделяет основу слова без окончания по образцу (\w{1,}okonchanie)
        /// </summary>
        /// <param name="slovo">слово полностью</param>
        /// <param name="okonchanie">возможное окончание</param>
        /// <returns>возвращает основу</returns>
        private Match Osnova(string slovo, string okonchanie)
        {
            Regex a = new Regex(@"(?<основа>\w{1,})" + okonchanie);
            return a.Match(slovo);
        }
        /// <summary>
        /// выделяет основу и окончание по образцу. На выходе характеристики Groups["основа"]-часть слова до окончания, Groups["окончание"]-окончание,Groups["заключение"]- часть слова после окончания
        /// </summary>
        /// <param name="slovo">слово целиком</param>
        /// <param name="okonchanie">образец поиска</param>
        /// <param name="parametr">'a' - образец (\w*okonchanie), 'b' - (\w*okonchanie\w*)</param>
        private Match Osnova_(string slovo, string okonchanie, char parametr)
        {
            Regex a = null;
            switch (parametr)
            {
                case 'a': a = new Regex(@"(?<основа>\w*)(?<окончание>" + okonchanie + ")"); break;
                case 'b': a = new Regex(@"(?<основа>\w*)(?<окончание>" + okonchanie + @")(?<заключение>\w*)"); break;
                case 'c': a = new Regex(@"(?<часть1>[а-яёй]{1,})(?<часть2>" + okonchanie + ")"); break;
            }
            return a.Match(slovo);
        }
        /// <summary>
        /// определяет производные слова (чем длинее основа, тем лучше)
        /// </summary>
        /// <param name="slovo">предполагаемое производное слово</param>
        /// <param name="osnova">начальная основа</param>
        /// <returns>возвращает true, если слово slovo производное от osnova</returns>
        private bool OsnovaProizv(string slovo, string osnova)
        {
            Regex a = new Regex(osnova);
            Match rezult = a.Match(slovo);
            if (rezult.Success) return true;
            else return false;
        }
        /// <summary>
        /// создаёт соответствие между числами на Эльюнди и Русским языком
        /// </summary>
        /// <returns>Возвращает 0-единств.число, 1-множеств.число</returns>
        private int ChisloElRus()
        {
            lico = PoiskLico();
            switch (c.chislo)
            {
                case Chislo.Edinstvennoe: return 0;
                case Chislo.Mnozhestvennoe: return 1;
                case Chislo.Malochislennoe: return 0;
                case Chislo.Mnogoobraznoe: return 1;
                case Chislo.Neopredelennoe: return 1;
                case Chislo.Odinochnoe: return 0;
                default: return 0;
            }
        }
        /// <summary>
        /// Ищет подлежащее в предложении и определяет недостающие характеристики (лицо, число, род)
        /// </summary>
        /// <returns>1-1 лицо, 2-2 лицо, 3-3 лицо</returns>
        private int PoiskLico()
        {
            if (predl != null)
            {
                //заглядываем на 3 слова вперёд/назад
                bool finish = true;
                int i = pozition_predl - 1;
                //сначала смотрим что впереди, потом (если не нашли) позади
                while (i > pozition_predl - 4)
                {
                    if (i >= 0)
                    {//определяем характеристики подлежащего
                        if (predl[i].padezh == Padezh.Imenitelnij)
                        {
                            if (predl[i].chastRechi == ChastRechi.Suschestvitelnoe || predl[i].chastRechi == ChastRechi.Chislitelnoe)
                            {
                                c.rod = predl[i].rod;
                                c.chislo = predl[i].chislo;
                                finish = false;
                                lico = 3;
                                break;
                            }
                            else if (predl[i].chastRechi == ChastRechi.Mestoimenie)
                            {
                                c.rod = predl[i].rod;
                                c.chislo = predl[i].chislo;
                                finish = false;
                                switch (predl[i].eSlovo)
                                {
                                    case "Q": lico = 1; break;//я
                                    case "W": lico = 1; break;//ты
                                    case "Y": lico = 2; break;//мы
                                    case "U": lico = 2; break;//вы
                                    case "E": lico = 3; break;//он
                                    case "R": lico = 3; break;//она
                                    case "I": lico = 3; break;//они
                                    case "T": lico = 3; break;//онo
                                    default: lico = 3; break;
                                }
                                break;
                            }
                        }
                    }
                    else break;
                    i--;
                }
                if (finish)
                {
                    i = pozition_predl + 1;
                    while (i < pozition_predl + 4)
                    {
                        if (i < predl.Count)
                        {//определяем характеристики подлежащего
                            if (predl[i].chastRechi == ChastRechi.Suschestvitelnoe && predl[i].padezh == Padezh.Imenitelnij || predl[i].chastRechi == ChastRechi.Chislitelnoe && predl[i].padezh == Padezh.Imenitelnij)
                            {
                                c.rod = predl[i].rod;
                                c.chislo = predl[i].chislo;
                                finish = false;
                                if (predl[i].chastRechi == ChastRechi.Suschestvitelnoe) lico = 3;
                                break;
                            }
                            else if (predl[i].chastRechi == ChastRechi.Mestoimenie && predl[i].padezh == Padezh.Imenitelnij)
                            {
                                c.rod = predl[i].rod;
                                c.chislo = predl[i].chislo;
                                finish = false;
                                switch (predl[i].eSlovo)
                                {
                                    case "Q": lico = 1; break;//я
                                    case "W": lico = 1; break;//ты
                                    case "Y": lico = 2; break;//мы
                                    case "U": lico = 2; break;//вы
                                    case "E": lico = 3; break;//он
                                    case "R": lico = 3; break;//она
                                    case "I": lico = 3; break;//они
                                    case "T": lico = 3; break;//онo
                                }
                                break;
                            }
                        }
                        else break;
                        i++;
                    }
                }
            }

            //определяем лицо
            return lico;
        }
        /// <summary>
        /// создаёт соответствие между видами глагола на Эльюнди и Русским языком (1-совершенный, 2-несовершенный)
        /// </summary>
        /// <returns>возвращает 1-совершенный, 2-несовершенный</returns>
        private int VidElRus()
        {
            switch (c.vid)
            {
                case Vid.Zavershennost:
                    return 1;
                case Vid.NevozvratnayaZavershennost:
                    return 1;
                case Vid.Mgnovennost:
                    return 1;
                case Vid.NachaloDejstviya:
                    return 1;
                case Vid.OgranichenieDlitelnosti:
                    return 1;
                default: return 2;
            }
        }
        /// <summary>
        /// создаёт соответствие между временами глагола на Эльюнди и Русским языком
        /// </summary>
        /// <returns>возвращает 0-если настоящее, 1-прошедшее, 2-будущее</returns>
        private int VremyElRus()
        {
            switch (c.vremya)
            {
                case Vremya.Buduschee: return 2;
                case Vremya.BuduscheeDalekoe: return 2;
                case Vremya.BuduscheeDlitelnoe: return 2;
                case Vremya.BuduscheeSProshedshim: return 2;
                case Vremya.BuduscheeVNastoyaschem: return 2;
                case Vremya.Davnoproshedshee: return 1;
                case Vremya.Nastoyaschee: return 0;
                case Vremya.NastoyascheeDlitelnoe: return 0;
                case Vremya.NastoyascheeSBuduschim: return 0;
                case Vremya.NastoyascheeSProshedshim: return 0;
                case Vremya.NastoyascheeVNastoyaschem: return 0;
                case Vremya.Postoyannoe: return 0;
                case Vremya.Proshedshee: return 1;
                case Vremya.ProshedsheeBuduscheeBezNastoyaschego: return 1;
                case Vremya.ProshedsheeDlitelnoe: return 1;
                case Vremya.ProshedsheeSNastoyaschim: return 1;
                default: return 0;
            }
        }
        public Slovo Analyze(Predlozhenie a, int b)
        {
            haract = Haract.глагол;
            c = new Slovo();
            if (a == null || a[b] == null) return c;
            c = a[b];
            if (c.eSlovo == "") return c;
            predl = new Predlozhenie();
            predl = a;
            pozition_predl = b;
            c.eSlovo = c.eSlovo.ToUpper();
            switch (c.eSlovo[0])
            {
                case 'R'://[та] неопред.форма (инфинитив)
                    c.naklonenie = Naklonenie.Infinitiv;
                    Analize();
                    break;
                case 'T'://[ча] настоящее время изъяв. накл.
                    c.naklonenie = Naklonenie.Izjavitelnoe;
                    c.vremya = Vremya.Nastoyaschee;
                    Analize();
                    break;
                case 'Y'://[ка] прошедшее время изъяв. накл.
                    c.naklonenie = Naklonenie.Izjavitelnoe;
                    c.vremya = Vremya.Proshedshee;
                    Analize();
                    break;
                case 'U'://[ша] будущее время изъяв. накл.
                    c.naklonenie = Naklonenie.Izjavitelnoe;
                    c.vremya = Vremya.Buduschee;
                    Analize();
                    break;
                case 'O'://[ма] деепричастие
                    c.naklonenie = Naklonenie.Izjavitelnoe;
                    haract = Haract.деепричастие;
                    c.vremya = Vremya.Nastoyaschee;
                    c.eSlovo = ZamLit(c.eSlovo, 'T', 0);
                    Analize();
                    c.eSlovo = ZamLit(c.eSlovo, 'O', 0);
                    break;
            }
            return c;
        }
        /// <summary>
        /// вставляет вместо чередуемой буквы 'o' перед основой osnova новую основу newosnova
        /// </summary>
        /// <param name="original">заданное слово</param>
        /// <param name="osnova">образец поиска</param>
        /// <param name="newosnova">новая основа</param>
        /// <returns>возвращает новое слово (в случае неудачи - newosnova)</returns>
        private string БезО(string original, string osnova, string newosnova)
        {
            Regex a = new Regex(@"(?<приставка>\w{1,})о" + osnova);
            Match b = a.Match(original);
            if (b.Success) return b.Groups["приставка"].Value + newosnova;
            else return newosnova;
        }
        /// <summary>
        /// удаляет чередуемые буквы 'o' перед основой osnova
        /// </summary>
        /// <param name="original">заданное слово</param>
        /// <param name="osnova">образец поиска</param>
        /// <param name="newosnova">новая основа</param>
        /// <returns>возвращает новое слово (в случае неудачи - original)</returns>
        private string БезО_ret_orig(string original, string osnova, string newosnova)
        {
            Regex a = new Regex(@"(?<приставка>\w{1,})о" + osnova);
            Match b = a.Match(original);
            if (b.Success) return b.Groups["приставка"].Value + newosnova;
            else return original;
        }
        /// <summary>
        /// удаляет чередуемые буквы 'o' перед основой osnova
        /// </summary>
        /// <param name="original">заданное слово</param>
        /// <param name="osnova">образец поиска</param>
        /// <returns>возвращает новое слово (в случае неудачи - original)</returns>
        private string БезО_ret_orig(string original, string osnova)
        {
            Regex a = new Regex(@"(?<приставка>\w{1,})о(?<окончание>" + osnova + ")");
            Match b = a.Match(original);
            if (b.Success) return b.Groups["приставка"].Value + b.Groups["окончание"].Value;
            else return original;
        }
        /// <summary>
        /// удаляет чередуемые буквы 'o' перед основой osnova, просчитывая всевозможные приставки на 'о'
        /// </summary>
        /// <param name="osnova">заданное слово - основа</param>
        /// <returns>возвращает новое слово</returns>
        private string БезО_ret_orig(string osnova)
        {
            if (osnova != "")
            {
                Regex reg = new Regex(@"(?<приставка>(разо)|(обо)|(ото)|(со)|(про))(?<основа>[а-я]*)");
                Match match = reg.Match(osnova);
                if (match.Success) return match.Groups["приставка"].Value.Substring(0, match.Groups["приставка"].Value.Length - 1) + match.Groups["основа"].Value;
            }
            return osnova;
        }
        /// <summary>
        /// вставляет чередуемые буквы 'o' перед основой osnova, просчитывая всевозможные приставки на 'о'
        /// </summary>
        /// <param name="osnova">заданное слово - основа</param>
        /// <returns>возвращает новое слово</returns>
        private string С_О_ret_orig(string osnova)
        {
            if (osnova != "")
            {
                Regex reg = new Regex(@"(?<приставка>(раз)|(над)|(пред)|(низ)|(из)|(воз)|(вз)|(под)|(об)|(от)|(с))(?<основа>[а-я]*)");
                Match match = reg.Match(osnova);
                if (match.Success && (match.Groups["приставка"].Value + match.Groups["основа"].Value == osnova)) return match.Groups["приставка"].Value + "о" + match.Groups["основа"].Value;
            }
            return osnova;
        }
        /// <summary>
        /// удаляет чередуемые буквы 'o' перед основой osnova в заданной приставке
        /// </summary>
        /// <param name="osnova">заданное слово - основа</param>
        /// <param name="приставка">искомая приставка</param>
        /// <returns>возвращает объединнёный результат или первонач. основу, если приставка не удовл.условиям</returns>
        private string БезО_(string osnova, string приставка)
        {
            if (приставка.Length > 1)
            {
                if (приставка[приставка.Length - 1] == 'о')
                {
                    Regex a = new Regex(приставка + "(?<основа>[а-я]*)");
                    Match b = a.Match(osnova);
                    if (b.Success) return приставка.Substring(0, приставка.Length - 1) + b.Groups["основа"].Value;
                }
            }
            return osnova;
        }
        private string БезО(string original, string osnova)
        {
            Regex a = new Regex(@"(?<приставка>\w{1,})о(?<окончание>" + osnova + ")");
            Match b = a.Match(original);
            if (b.Success) return b.Groups["приставка"].Value + b.Groups["окончание"].Value;
            else return original;
        }
        /// <summary>
        /// заменяет в слове одну основу на другую
        /// </summary>
        /// <param name="original">заданное слово</param>
        /// <param name="osnova">образец поиска</param>
        /// <param name="newosnova">новая основа</param>
        /// <returns>возвращает новое слово</returns>
        private string ZamOsnova(string original, string osnova, string newosnova)
        {
            Regex a = new Regex(@"(?<приставка>\w{0,})" + osnova);
            Match b = a.Match(original);
            if (b.Success) return b.Groups["приставка"].Value + newosnova;
            else return newosnova;
        }
        /// <summary>
        /// заменяет в слове одну основу на другую
        /// </summary>
        /// <param name="original">заданное слово</param>
        /// <param name="osnova">образец поиска</param>
        /// <param name="newosnova">новая основа</param>
        /// <returns>возвращает новое слово, если образец найден, иначе - original</returns>
        private string ZamOsnova_ro(string original, string osnova, string newosnova)
        {
            Regex a = new Regex(@"(?<приставка>\w{0,})" + osnova);
            Match b = a.Match(original);
            if (b.Success) return b.Groups["приставка"].Value + newosnova;
            else return original;
        }
        /// <summary>
        /// заменяет часть слова на новую часть (!лев.часть и прав.часть не обязательны)
        /// </summary>
        /// <param name="original">слово целиком</param>
        /// <param name="osnova">искомая часть</param>
        /// <param name="newosnova">новая часть</param>
        /// <param name="parametr">a - суммируется всё, b - суммируется newosnova и правая часть, c - суммируется newosnova и левая часть</param>
        /// <returns>возвращает изменнёное слово, если найден образец , иначе - original</returns>
        private string ZamСловоВнутри_or(string original, string osnova, string newosnova, char parametr)
        {
            Regex a = new Regex(@"(?<часть1>\w{0,})" + osnova + @"(?<часть2>\w{0,})");
            Match b = a.Match(original);
            if (b.Success)
                switch (parametr)
                {
                    case 'a': return b.Groups["часть1"].Value + newosnova + b.Groups["часть2"].Value;
                    case 'b': return newosnova + b.Groups["часть2"].Value;
                    case 'c': return b.Groups["часть1"].Value + newosnova;
                }
            return original;
        }
        public Slovo Translate(Predlozhenie a, int b)
        {
            c = new Slovo();
            pozition_predl = b;
            predl = a;
            if (a == null || a[b] == null) return c;
            c = a[b];
            if (c.eSlovo == "") return c;
            c.eSlovo = c.eSlovo.ToUpper();
            char t = c.eSlovo[0];
            if (t == 'R' || t == 'T' || t == 'Y' || t == 'U' || t == 'O')
            {
                lico = 3;//по умолчанию    
                bool y = true;
                //переводим слово на русский и возвращаем
                GlagDict dict = new GlagDict();
                logFormRus += "\nПеревод начальной формы: ";
                if (t == 'O') { haract = Haract.деепричастие; }
                if ((c.rSlovo = dict.TranslateEl(a[b].eSlovo)) != "") y = false;//поиск 1                
                if (c.rSlovo == "") c.rSlovo = dict.TranslateEl(ZamLit(a[b].eSlovo, 'R', 0));//поиск 2                
                if (c.rSlovo == "") c.rSlovo = dict.TranslateEl(OsnovaEl(a[b].eSlovo));//поиск 3 
                if (c.rSlovo == "") { logFormRus += "НЕТ В БАЗЕ ДАННЫХ"; c.rSlovo = c.eSlovo; return c; }
                logFormRus += "< " + c.rSlovo.ToUpper() + " >";
                if (y)
                {
                    c.rSlovo = RusForm(c.rSlovo);
                    logFormRus += "\nПостановка формы: < " + c.rSlovo.ToUpper() + " >";
                }
            }
            return c;
        }
        /// <summary>
        /// детальный анализ 
        /// 1.для выявления допол. времён
        /// 2.для определения залога
        /// 3.для определения наклонения
        /// 4.для определения вида
        /// 5.определение категории состояния
        /// </summary>
        private void Analize()
        {
            if (c.eSlovo.Length < 2) return;
            bool Zaver = false;
            c.zalog = Zalog.Dejstvitekniy;//настройки по умолчанию
            c.chastRechi = ChastRechi.Glagol;//настройки по умолчанию
            c.sostoynie = Sostoynie.Обычное;//настройки по умолчанию
            c.chislo = Chislo.Edinstvennoe;//настройки по умолчанию
            c.rod = Rod.Obshij;//настройки по умолчанию            
            if (c.eSlovo[1] == 'A') c.zalog = Zalog.Vozvratniy;
            if (c.eSlovo.Length > 3) if (c.eSlovo[2] == '-' && c.eSlovo[1] == 'S')
                {
                    c.vid = Vid.Zavershennost;
                    Zaver = true;
                }
            NaklSoslag();
            string[] temp = c.eSlovo.Split('-');
            if (temp[0].Length == 1 && temp.Length > 1)
            {
                switch (temp[1])
                {
                    //совершенный вид
                    case "RBY": //[туко]
                        c.vid = Vid.Mgnovennost;
                        break;
                    case "SCY"://[роко]
                        c.vid = Vid.NachaloDejstviya;
                        break;
                    case "PVI"://[пэсо]:
                        c.vid = Vid.OgranichenieDlitelnosti;
                        break;
                    //несовершенный вид
                    case "PVS"://[пэро]
                        c.vid = Vid.NeopredelennayaDlitelnost;
                        break;
                    case "RBA"://[тудо]
                        c.vid = Vid.PostoyannayaDlitelnost;
                        break;
                    case "UCS"://[шоро]
                        c.vid = Vid.NezavershennostDejstviya;
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
                                c.vremya = Vremya.NastoyascheeDlitelnoe;
                                break;
                            case 'Y':
                                c.vremya = Vremya.ProshedsheeDlitelnoe;
                                break;
                            case 'U':
                                c.vremya = Vremya.BuduscheeDlitelnoe;
                                break;
                            default: break;
                        }
                        break;
                    case "TBA"://наст
                        switch (temp[0][0])
                        {
                            case 'T':
                                c.vremya = Vremya.NastoyascheeVNastoyaschem;
                                break;
                            case 'Y':
                                c.vremya = Vremya.ProshedsheeSNastoyaschim;
                                break;
                            case 'U':
                                c.vremya = Vremya.BuduscheeVNastoyaschem;
                                break;
                            default: break;
                        }
                        break;
                    case "YBA"://прош
                        switch (temp[0][0])
                        {
                            case 'T':
                                c.vremya = Vremya.NastoyascheeSProshedshim;
                                break;
                            case 'Y':
                                c.vremya = Vremya.Davnoproshedshee;
                                break;
                            case 'U':
                                c.vremya = Vremya.BuduscheeSProshedshim;
                                break;
                            default: break;
                        }
                        break;
                    case "UBA"://буд
                        switch (temp[0][0])
                        {
                            case 'T':
                                c.vremya = Vremya.NastoyascheeSBuduschim;
                                break;
                            case 'Y':
                                c.vremya = Vremya.ProshedsheeBuduscheeBezNastoyaschego;
                                break;
                            case 'U':
                                c.vremya = Vremya.BuduscheeDalekoe;
                                break;
                            default: break;
                        }
                        break;
                    //определение залога
                    case "A":
                        c.zalog = Zalog.Vozvratniy;
                        break;
                    case "F":
                        c.zalog = Zalog.Stradatelniy;
                        break;
                    case "SBA":
                        c.zalog = Zalog.Vzaimniy;
                        break;
                    //определение наклонения
                    case "J":
                        c.naklonenie = Naklonenie.Povelitelnoe;
                        break;
                    //определение вида
                    //совершенный вид
                    case "S"://[ро]
                        if (!Zaver) c.vid = Vid.Zavershennost;
                        else c.vid = Vid.NevozvratnayaZavershennost;
                        break;
                    //состояние
                    case "TVY"://[чэко]                                        
                        c.sostoynie = Sostoynie.Безличное;
                        break;
                    default: break;
                }
            }
            logOsnovaEl += "\n***Характеристики*** " + "\nЧисло: " + c.chislo.ToString() + "\nНаклонение: " + c.naklonenie.ToString() + "\nРод: " + c.rod.ToString() + "\nСостояние: " + c.sostoynie.ToString() + "\nВид: " + c.vid.ToString() + "\nВремя: " + c.vremya.ToString() + "\nЗалог: " + c.zalog.ToString();
        }
        //ДОРАБОТАТЬ !!!! вопрос по запятым
        //поиск частицы сослаг.наклонения
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
                        c.naklonenie = Naklonenie.Soslagatelnoe;
                        break;
                    }
                }
                else break;
                i--;
            }
        }  
    }
}
