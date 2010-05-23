using System;
using System.Text;
using System.Collections;
using System.Data.SQLite;

namespace ETEnTranslator
{
   public class ETEnPredlog : IModule
    {
       public ETEnPredlog()
        {
        }
        public Slovo Analyze(Predlozhenie pr, int place)
        {
            Slovo analyzed = pr[place];

            PreAnalyze(pr, place, ref analyzed);

            return analyzed;
        }
        protected void PreAnalyze(Predlozhenie pr, int place, ref Slovo slovo)
        {
            //GetTranslate(ref slovo);
            SetExtraData(ref slovo);
        }
        private void SetExtraData(ref Slovo slovo)
        {
            Predlog predlog = new Predlog();

            predlog.english = slovo.enSlovo.slovo;
            slovo.ExtraData = predlog;
        }
        private void GetTranslate(ref Slovo slovo)
        {
            SQLiteConnection connection = new SQLiteConnection(@"Data Source=dict.sqlitedb;Version=3;");
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(connection);
            //command.CommandText = "SELECT n, rus FROM dict";
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

        /**
          * ЗАГЛУШКА
          * На выходе должно быть слово на ангийском языке
          */
        public Slovo Translate(Predlozhenie pr, int place)
        {
            Slovo analyzed = pr[place];

            bool translated = false;

            if (analyzed.eSlovo == "FQV" || analyzed.eSlovo == "FZJ" || analyzed.eSlovo == "FZP")
            {
                analyzed.enSlovo.slovo = "";
                translated = true;
            }
            else if (analyzed.eSlovo == "FT")
            {
                if (place - 1 > 0 && pr[place - 1].eSlovo[0] != 'Q')
                {
                    analyzed.enSlovo.slovo = "";
                    translated = true;
                }
            }

            if (!translated)
            {
                GetTranslate(ref analyzed);
            }

            return analyzed;
        }
    }
}
