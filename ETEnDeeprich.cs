using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace ETEnTranslator
{
    class ETEnDeeprich : IModule
    {
        public ETEnDeeprich()
        {}

        public Slovo Analyze(Predlozhenie pr, int place)
        {
            Slovo analyzed = pr[place];
            PreAnalyze(pr, place, ref analyzed);
            return analyzed;
        }

        protected void PreAnalyze(Predlozhenie pr, int place, ref Slovo slovo)
        {
            AnalyzeDeeprich(pr, place, ref slovo);
            // FindOsnova(ref slovo);
            //GetTranslate(ref slovo);
            Processing(ref slovo);
            SetExtraData(ref slovo);
        }

        private void SetExtraData(ref Slovo slovo)
        {
            Deeprichastie deepr = new Deeprichastie();
            deepr.english = slovo.enSlovo.slovo;
            deepr.vremya = slovo.vremya;
            deepr.zalog = slovo.zalog;
            deepr.osnova = slovo.eSlovo;
           // deepr.new_engl = slovo.new_slovo;
            slovo.ExtraData = deepr;
        }

        private void GetTranslate(ref Slovo slovo)
        {
            string stroka = perevod(slovo.eSlovo);
            if (stroka == "")
            {
                string[] temp = slovo.eSlovo.Split('-');
                string str = slovo.eSlovo;
                string str1 = "";
                if (temp.Length == 1) str1 = "R-"; else str1 = "R";
                char[] st = str.ToCharArray();
                for (int i = 1; i < st.Length; i++)
                    str1 = str1 + st[i].ToString();
                stroka = perevod(str1);
            }
            slovo.enSlovo.slovo = stroka;
        }

        protected void AnalyzeDeeprich(Predlozhenie pr, int place, ref Slovo slovo)
        {
            string[] temp = slovo.eSlovo.Split('-');
            if (temp[0].Length == 1 && temp.Length > 1)
            {
                switch (temp[1])
                {
                    //совершенный вид
                    case "RBA":
                        slovo.vremya = Vremya.Postoyannoe;
                        break;
                    case "TBA":
                        slovo.vremya = Vremya.Nastoyaschee;
                        break;
                    case "YBA":
                        slovo.vremya = Vremya.Proshedshee;
                        break;
                    case "UBA":
                        slovo.vremya = Vremya.Buduschee;
                        break;
                    default: break;
                }
            }
        }

        public string perevod(string str)
        {
            string perevod = "";
            SQLiteConnection connection = new SQLiteConnection(@"Data Source=dict.sqlitedb;Version=3;");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);
            //command.CommandText = "SELECT n, rus FROM dict";
            command.CommandText = "SELECT eng FROM dict WHERE el=@el";
            command.Parameters.Add(new SQLiteParameter("el", str));
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                if (!reader.IsDBNull(0))
                {
                    perevod = reader.GetString(0);
                }
                else
                {
                    perevod = "[Нет перевода]";
                }
            }
            reader.Close();
            connection.Close();
            return perevod;
        }

        private void Processing(ref Slovo slovo)
        {
            string stroka = "";
            char[] znak = slovo.enSlovo.slovo.ToCharArray();
            if (znak[znak.Length - 1] != 'g' & znak[znak.Length - 2] != 'n' & znak[znak.Length - 3] != 'i')
            {
                stroka = slovo.enSlovo.slovo.ToString();
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
                        char[] slovo1 = stroka.ToCharArray();
                        if (slovo1[slovo1.Length - 1] == 'e')
                        {
                            for (int i = 0; i < slovo1[slovo1.Length - 1]; i++)
                                stroka = stroka + slovo1[i].ToString();
                            stroka = stroka + "ing";
                        }
                        else if (slovo1[slovo1.Length - 1] == 'e' && slovo1[slovo1.Length - 2] == 'i')
                        {
                            stroka = slovo1[0].ToString();
                            for (int i = 1; i < slovo1.Length - 2; i++)
                                stroka = stroka + slovo1[i].ToString();
                            stroka = stroka + "ying";
                        }
                        else if (slovo1[slovo1.Length - 1] == 'n' && slovo1[slovo1.Length - 2] != 'n' || slovo1[slovo1.Length - 1] == 'p' && slovo1[slovo1.Length - 2] != 'p' || slovo1[slovo1.Length - 1] == 't' && slovo1[slovo1.Length - 2] != 't' || slovo1[slovo1.Length - 1] == 'l' && slovo1[slovo1.Length - 2] != 'l')
                        {
                            if (slovo1[slovo1.Length - 2] == 'o' || slovo1[slovo1.Length - 1] == 'a' || slovo1[slovo1.Length - 1] == 'e' || slovo1[slovo1.Length - 1] == 'u' || slovo1[slovo1.Length - 1] == 'i' || slovo1[slovo1.Length - 1] == 'y')
                            {
                                if (slovo1[slovo1.Length - 3] != 'o' || slovo1[slovo1.Length - 3] != 'a' || slovo1[slovo1.Length - 3] != 'e' || slovo1[slovo1.Length - 3] != 'u' || slovo1[slovo1.Length - 3] != 'i' || slovo1[slovo1.Length - 3] != 'y')
                                    stroka = stroka + slovo1[slovo1.Length - 1] + "ing";
                            }
                            else
                                stroka = stroka + "ing";
                        }
                        /* else if (slovo1[slovo1.Length-1] == 'r')
                             stroka = stroka + slovo1[slovo1.Length - 1] + "ed";*/
                        else
                            stroka = stroka + "ing";
                    }
                    reader.Close();
                    con.Close();
                }
            }
            //slovo.new_slovo = stroka;
        }

        public Slovo Translate(Predlozhenie pr, int place)
        {
            Slovo analyzed = pr[place];

            GetTranslate(ref analyzed);

            return analyzed;
        }
    }
}
