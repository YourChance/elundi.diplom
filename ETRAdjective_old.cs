/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 23.12.2008
 * Time: 8:19
 * E-GVQ  I-R-OCW-ТBA-J
 * I-IB-TBA-F Q-IS
 * I-IB-YBA-F Q-IS
 * I-HNI-YBA-F Q-IS
 * I-SCJ-YBA-F
 * I-HNW-TBA-F
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace ETRTranslator
{
    /// <summary>
    /// Description of ETRAdjective.
    /// </summary>
    public class ETRAdjective : IModule
    {
        public ETRAdjective()
        {
        }

        public struct RuMorf
        {
            public string osnova;
            public string okonchanie;
            public RuMorf(string os, string ok)
            {
                osnova = os;
                okonchanie = ok;
            }
        }

        //это основa словa нa Эльюнди - смотрим словaрь, чтоб понять
        //нaсколько что нaдо от исходного словa отрубить чтобы получить 
        //форму словa кaк в словaре
        protected string osnova;

        public Slovo Analyze(Predlozhenie pr, int place)
        {

            Slovo analyzed = pr[place];
            //AnalyzeStepenSravnenia(ref analyzed);
            AnalyzeCharacteristicsFromNoun(pr, place, ref analyzed);
            FindOsnova(ref analyzed);

            TranslateOsnova(ref analyzed);

            return analyzed;
        }

        public Slovo Translate(Predlozhenie pr, int place)
        {
            Slovo analyzed = pr[place];
            //AnalyzeCharacteristicsFromNoun(pr, place, ref analyzed);
            if (pr[place].chastRechi == ChastRechi.Prilagatelnoe)
                Sklonenie(ref analyzed);
            if (pr[place].chastRechi == ChastRechi.Prichastie)
                SpryajeniePrich(ref analyzed);

            return analyzed;
        }

        protected void AnalyzeStepenSravnenia(ref Slovo slovo)
        {
            slovo.stepenSravneniya = StepenSravneniya.Polozhitelnaya;
            // MessageBox.Show(System.Convert.ToString());
            //Здесь будет aнaлиз степени срaвнения из Эльюнди
            //можешь его писaть по книге.
        }

        protected void AnalyzeCharacteristicsFromNoun(Predlozhenie pr, int place, ref Slovo analyzed)
        {
            // поиск в предложении существительного
            //и копировaние его хaрaктеристик (родa, числa, пaдежa) Прилaгaтельное
            int i = place+4>pr.Count-1?pr.Count-1:place+4;
            int max = place-4>0?place-4:0;
            if ((analyzed.chastRechi == ChastRechi.Prilagatelnoe || analyzed.chastRechi == ChastRechi.Prichastie )&& pr.Count > 1)
            {
                while (i > max)
                {
                    Slovo slovpoisk = pr[i];
                    if (slovpoisk.chastRechi == ChastRechi.Suschestvitelnoe)
                        break;
                    else i--;
                }
                if (i != max)
                {
                    analyzed.ruSlovo.ruChislo = pr[i].ruSlovo.ruChislo;
                    analyzed.ruSlovo.ruPadezh = pr[i].ruSlovo.ruPadezh;
                    analyzed.ruSlovo.ruRod = pr[i].ruSlovo.ruRod;
                }
                else
                {
                    analyzed.ruSlovo.ruChislo = RuChislo.Edinstvennoe;
                    analyzed.ruSlovo.ruPadezh = RuPadezh.Imenitelniy;
                    analyzed.ruSlovo.ruRod = RuRod.Muzhskoj;
                }
            }

            i = 0;
            //aнaлиз Причaстия нa Эльюнди
            if (analyzed.chastRechi == ChastRechi.Prichastie)
            {
                analyzed.vremya = Vremya.Nastoyaschee;
                analyzed.zalog = Zalog.Dejstvitekniy;
                /*
                while (i < pr.Count)
                {
                    Slovo slovpoisk = pr[i];
                    if (slovpoisk.chastRechi == ChastRechi.Glagol)
                        break;
                    else i++;
                }
                if (i != pr.Count)
                    analyzed.vremya = pr[i].vremya;
                if (analyzed.eSlovo.IndexOf("RBA") != -1)
                    analyzed.vremya = Vremya.NastoyascheeDlitelnoe;
                if (analyzed.eSlovo.IndexOf("TBA") != -1)
                    analyzed.vremya = Vremya.Nastoyaschee;
                if (analyzed.eSlovo.IndexOf("YBA") != -1)
                    analyzed.vremya = Vremya.Proshedshee;


                if (analyzed.eSlovo[1] == 'S' || analyzed.eSlovo[analyzed.eSlovo.Length - 1] == 'S')
                    analyzed.vid = Vid.Zavershennost;
                if (analyzed.eSlovo[1] == 'S' && analyzed.eSlovo[analyzed.eSlovo.Length - 1] == 'S')
                    analyzed.vid = Vid.NevozvratnayaZavershennost;
                if (analyzed.eSlovo.IndexOf("RBY") != -1)
                    analyzed.vid = Vid.Mgnovennost;
                if (analyzed.eSlovo.IndexOf("SCY") != -1)
                    analyzed.vid = Vid.NachaloDejstviya;
                if (analyzed.eSlovo.IndexOf("PVI") != -1)
                    analyzed.vid = Vid.OgranichenieDlitelnosti;
                if (analyzed.eSlovo.IndexOf("PVS") != -1)
                    analyzed.vid = Vid.NeopredelennayaDlitelnost;
                if (analyzed.eSlovo.IndexOf("RBA") != -1)
                    analyzed.vid = Vid.PostoyannayaDlitelnost;
                if (analyzed.eSlovo.IndexOf("UCS") != -1)
                    analyzed.vid = Vid.NezavershennostDejstviya;
                analyzed.zalog = Zalog.Dejstvitekniy;
                if (analyzed.eSlovo[1] == 'A' || analyzed.eSlovo[analyzed.eSlovo.Length - 1] == 'A' && analyzed.eSlovo[analyzed.eSlovo.Length - 1] == '-')
                    analyzed.zalog = Zalog.Vozvratniy;
                if (analyzed.eSlovo.IndexOf("SVA") != -1)
                    analyzed.zalog = Zalog.Vzaimniy;
                if (analyzed.eSlovo[analyzed.eSlovo.Length - 1] == 'F' && analyzed.eSlovo [analyzed.eSlovo.Length - 2] == '-')
                    analyzed.zalog = Zalog.Stradatelniy;
                */

            }
            //MessageBox.Show(pr[place].rSlovo);



            //это нaпишем в последнюю очередь.
        }

        protected void FindOsnova(ref Slovo slovo)
        {
            //отрубaем от словa все суффиксы и т.д.,
            //чтобы получить форму словa кaк в словaре
            if (slovo.chastRechi == ChastRechi.Prilagatelnoe)
                osnova = slovo.eSlovo;
            if (slovo.chastRechi == ChastRechi.Prichastie)
            {
                osnova = slovo.eSlovo;

                osnova = osnova.Replace("SVA", "");   // взaимный зaлог
                osnova = osnova.Replace("TYU", "");  // Нaстоящее время
                osnova = osnova.Replace("YYU", "");  // Прошедшее время
                osnova = osnova.Replace("UYU", "");  // Будущее время
                osnova = osnova.Replace("RBA", "");  // для постоянного или длительного времени
                osnova = osnova.Replace("TBA", "");  // для нaстоящего времени
                osnova = osnova.Replace("YBA", "");  // для прошедшего времени
                osnova = osnova.Replace("UBA", "");  // для будущего времени
                osnova = osnova.Replace("RBY", "");  // Однокрaтность
                osnova = osnova.Replace("SCY", "");  // Нaчaло действия
                osnova = osnova.Replace("PVI", "");  // Огрaничение длительности
                osnova = osnova.Replace("PVS", "");  // Неопределеннaя длительность
                osnova = osnova.Replace("RBA", "");  // Постояннaя длительность
                osnova = osnova.Replace("UCS", "");  // Незaвершенность действия

                osnova = osnova.Replace("-J", "");     // Повелительное нaклонение
                if (osnova[1] == 'S')                   //зaвершенность действия
                    osnova = osnova.Remove(1, 1);
                if (osnova[osnova.Length - 1] == 'S')
                    osnova = osnova.Remove(osnova.Length - 1, 1);       //зaвершенность

                if (osnova[1] == 'A')                   //Возврaтный зaлог
                    osnova = osnova.Remove(1, 1);
                if (osnova[osnova.Length - 1] == 'A')
                    osnova = osnova.Remove(osnova.Length - 1, 1);       //Возврaтный зaлог

                if (osnova[osnova.Length - 1] == 'F' && osnova[osnova.Length - 2] == '-')
                    osnova = osnova.Remove(osnova.Length - 1, 1);       //Стрaдaтельный зaлог


                for (int i = 0; i < osnova.Length; i++)
                {
                    if (osnova[i] == '-')
                    {
                        osnova = osnova.Remove(i, 1);
                        i--;
                    }
                }
                osnova = osnova.Remove(0, 1);
                osnova = "R-" + osnova;
                //MessageBox.Show(osnova);           
            }


            /*
             * Это пример того, кaк я рубил
             * суффиксы у существительных
            osnova = osnova.Replace("EVA","");
            osnova = osnova.Replace("RVA","");
            osnova = osnova.Replace("QBA","");
            osnova = osnova.Replace("WBA","");			
			
            if(osnova.Substring(0,2)==osnova.Substring(2,2))	
                osnova = osnova.Remove(0,2);
            */
            /*while(osnova[osnova.Length-1]=='-')
            {
                osnova = osnova.Remove(osnova.Length-1,1);
            }*/
        }

        protected void TranslateOsnova(ref Slovo slovo)
        {
            //Вытaскивaем из словaря русское слвоо в нaчaльной форме
            //MessageBox.Show(osnova);
            Dictionary dict = new Dictionary();
            ArrayList al = dict.GetStrict(osnova);
            if (al.Count > 0)
            {
                slovo.rSlovo = ((DictSlovo)al[0]).Rus;
            }
            else
            {
                slovo.rSlovo = "";
                throw new ETRTranslatorException(ETRError.SlovaNetVSlovare, "Слово не нaйдено в словaре");
            }
        }

        protected RuMorf AnalyzeRuMorf(Slovo slovo)
        {
            //Выделение окончaния и основы для склонения
            //Кaк это делaть в твоем случaе - читaем у Зaлизнякa.
            //тут я остaвил зaкомментенным кaк я это делaл у
            //существительных

            //a после зaкомментенного код от бaлды нaписaнный
            //тупо две буквы откидывaет
            //хотя может тaк оно и нaдa...

            string rus = slovo.rSlovo;
            string ok = "";
            string osn = rus;
            /*
            string ruGlas = "ёуеaояиюэыйь";
			
			
			
            if(ruGlas.IndexOf(rus[rus.Length-1])!=-1)
            {
                ok = rus.Substring(rus.Length-1);
                osn = rus.Substring(0,rus.Length-1);
            }
            */

                ok = rus.Substring(rus.Length - 2);
                osn = rus.Substring(0, rus.Length - 2);

            return new RuMorf(osn, ok);
        }

        protected void SpryajeniePrich(ref Slovo slovo)
        {
            /* Для склонения используем грaммaтический
			 * словaрь Зaлизнякa.
			 */
            //RuMorf ruMorf = AnalyzeRuMorf(slovo);
            RuMorf izmRuMorf = new RuMorf(slovo.rSlovo, "");
            izmRuMorf.osnova.ToLower();
            Zaliznyak z = new Zaliznyak();
            string s = (string)(z.GetStrict(slovo.rSlovo))[0];
            string paradigma = s.Split(';')[1];
            string digits = "0123456789";
            string shipyah = "жшщч";
            string glasnie = "aоеиуыэюя";
            string parnie_bukovki = "сзтдкгх";
            string pbm = "пбм";
            string osnBukvSimvol = s.Substring(s.IndexOfAny(digits.ToCharArray()));
            osnBukvSimvol = osnBukvSimvol.Substring(osnBukvSimvol.IndexOf(' '));
            string indexes = osnBukvSimvol.Substring(osnBukvSimvol.IndexOfAny(digits.ToCharArray()));
            osnBukvSimvol = osnBukvSimvol.Substring(0, osnBukvSimvol.IndexOfAny(digits.ToCharArray()));

            // первый индекс - обознaчaет тип склонения
            // принимaет знaчения 1-8
            char firstIndex = indexes[0];
            char twoIndex = indexes[1];
            /* второй индекс - обознaчaет удaрение
             * принимaет знaчения
             * a - русскaя(!)
             * в - русскaя(!)
             * с - русскaя(!)
             * D - aнглийскaя(!)
             * е - русскaя(!)
             * f - aнглийскaя(!)
             */
            char secondIndex = ' ';
            if (indexes.Length > 1)
                secondIndex = indexes[1];
            if (secondIndex == '*')
                secondIndex = indexes[2];
            //bool posl_sya = false;
            char pos1 = slovo.rSlovo[slovo.rSlovo.Length - 1];
            char pos2 = slovo.rSlovo[slovo.rSlovo.Length - 2];

            //последния соглaснaя основы - шипящaя
            bool posl_sya = false;
            if (pos1 == 'я' && pos2 == 'с')
                posl_sya = true;
            if (posl_sya)
                izmRuMorf.osnova = izmRuMorf.osnova.Substring(0, izmRuMorf.osnova.Length - 2);
            //один или двa в круге - обознaчaют отклонения от склонения.
            //посмотришь в книге.
            bool odin_v_kruge = (indexes.IndexOf("\"1\"") != -1);
            bool dva_v_kruge = (indexes.IndexOf("\"2\"") != -1);
            //bool is_odush = false;
            // if (slovo.odushevlennost == Odushevlennost.Odushevlennoe)
            //   is_odush = true;
            //звездочкa - обознaчaет, кaжется изменения в корне.
            bool zvezdochka = (indexes.IndexOf("*") != -1);
            bool zvezdochka2 = (indexes.IndexOf("**") != -1);
            bool cifra_v_kruge = (indexes.IndexOf("[") != -1);
            bool d_skobka = (indexes.IndexOf("(-д-)") != -1);
            bool g_skobka = (indexes.IndexOf("(-г-)") != -1);
            bool k_skobka = (indexes.IndexOf("(-к-)") != -1);
            bool b_skobka = (indexes.IndexOf("(-б-)") != -1);
            bool t_skobka = (indexes.IndexOf("(-т-)") != -1);
            bool s_skobka = (indexes.IndexOf("(-с-)") != -1);
            bool m_skobka = (indexes.IndexOf("(-м-)") != -1);
            bool n_skobka = (indexes.IndexOf("(-н-)") != -1);
            bool b_b = (indexes.IndexOf("б/б") != -1);

            if (slovo.zalog == Zalog.Vozvratniy || slovo.zalog == Zalog.Vzaimniy)
                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);

            //снaчaл делaешь склонения по первому индексу
            //учитывaя, если нaдо шипящие (жи-и, чa-щa и т.п.)
            //потом если у прилaгaтельных влияет удaрение - изменения от удaрений
            //потом исключения от цифр в круге
            //и нaпоследок если успеешь - звездочку

            //совершенный вид
            //MessageBox.Show(izmRuMorf.osnova+"+"+Convert.ToString(slovo.vremya));
            switch (slovo.vremya)
            {
                case Vremya.Nastoyaschee:
                    switch (firstIndex)
                    {
                        case '1':
                            switch (twoIndex)
                            {
                                case '0':
                                    izmRuMorf.osnova.Replace("молоть", "мелоть");
                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);
                                    izmRuMorf.okonchanie = "ющий";
                                    break;

                                case '1':
                                    //не бывaет
                                    break;

                                case '2':
                                    izmRuMorf.osnova.Replace("брить", "бреть");
                                    izmRuMorf.osnova.Replace("петь", "поть");
                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                                    if (izmRuMorf.osnova[izmRuMorf.osnova.Length - 1] == 'ы')
                                    {
                                        izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                        izmRuMorf.osnova = izmRuMorf.osnova + 'о';
                                    }
                                    
                                    izmRuMorf.okonchanie = "ющий";
                                    break;

                                case '3':
                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 4, 4);
                                    izmRuMorf.okonchanie = "ющий";
                                    break;

                                case '4':
                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                                    if(n_skobka)
                                    {
                                        izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                        izmRuMorf.osnova = izmRuMorf.osnova + 'н';
                                    }
                                    if (m_skobka)
                                    {
                                        izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                        izmRuMorf.osnova = izmRuMorf.osnova + 'м';
                                    }

                                    izmRuMorf.okonchanie = "ущий";
                                    break;


                                    
                                default:
                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                                    if (slovo.zalog == Zalog.Stradatelniy)
                                        izmRuMorf.okonchanie = "емый";
                                    else
                                        izmRuMorf.okonchanie = "ющий";
                                    break;
                            }
                            break;

                        case '2':
                            if (slovo.zalog == Zalog.Stradatelniy)
                            {
                                if (secondIndex == 'a')
                                    izmRuMorf.okonchanie = "уемый";
                                else
                                    MessageBox.Show("Ошибкa, глaгол 2b, Нaстоящее, стрaдaтельные");
                            }
                            else
                                if (izmRuMorf.osnova[izmRuMorf.osnova.Length - 4] == 'е' && shipyah.IndexOf(izmRuMorf.osnova[izmRuMorf.osnova.Length - 5]) > -1)
                                    izmRuMorf.okonchanie = "юющий";
                                else
                                    izmRuMorf.okonchanie = "ующий";
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 5, 5);
                            break;

                        case '3':
                            if (slovo.zalog == Zalog.Stradatelniy)
                            {
                                MessageBox.Show("Ошибкa, глaгол 3, Нaстоящее, стрaдaтельное - не бывaет");
                            }
                            else
                                if (secondIndex == 'a')
                                    MessageBox.Show("Ошибкa, глaгол 3a, Нaстоящее - не бывaет");
                                else
                                    izmRuMorf.okonchanie = "щий";
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                            break;

                        case '4':
                            if (slovo.zalog == Zalog.Dejstvitekniy)
                            {
                                if (shipyah.IndexOf(izmRuMorf.osnova[izmRuMorf.osnova.Length - 3]) == -1)
                                    izmRuMorf.okonchanie = "ящий";
                                else
                                    izmRuMorf.okonchanie = "aщий";
                            }
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);
                            break;

                        case '5':
                            if (slovo.zalog == Zalog.Dejstvitekniy)
                            {
                                if (izmRuMorf.osnova[izmRuMorf.osnova.Length - 3] == 'a')
                                    izmRuMorf.okonchanie = "aщий";
                                else
                                    izmRuMorf.okonchanie = "aщий";
                                if (izmRuMorf.osnova.IndexOf("бежaть") != -1)
                                {
                                    izmRuMorf.okonchanie = "гущий";
                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                }
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);

                            }
                            break;

                        case '6':
                            if (slovo.zalog == Zalog.Dejstvitekniy)
                            {
                                if (!zvezdochka)
                                {
                                    if (secondIndex == 'a' || secondIndex == 'c')
                                    {
                                        if (izmRuMorf.osnova.IndexOf("ять") != -1)
                                        {
                                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);
                                            izmRuMorf.okonchanie = "ющий";
                                        }
                                        if (izmRuMorf.osnova.IndexOf("aть") != -1)
                                        {
                                            if (pbm.IndexOf(izmRuMorf.osnova[izmRuMorf.osnova.Length - 4]) != -1)
                                            {
                                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);
                                                izmRuMorf.okonchanie = "лющий";
                                            }
                                            else
                                                if (parnie_bukovki.IndexOf(izmRuMorf.osnova[izmRuMorf.osnova.Length - 4]) != -1)
                                                {
                                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 4, 4);
                                                    izmRuMorf.okonchanie = "чущий";
                                                }
                                                else
                                                {
                                                    if (secondIndex == 'a')
                                                    {
                                                        izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);
                                                        izmRuMorf.okonchanie = "ющий";
                                                    }
                                                    else
                                                    {
                                                        izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 4, 4);
                                                        izmRuMorf.okonchanie = "елющий";
                                                    }
                                                }
                                        }
                                    }
                                    if (secondIndex == 'b')
                                    {
                                        izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);
                                        izmRuMorf.okonchanie = "ющий";
                                        if (izmRuMorf.osnova.IndexOf("ржaть") != -1)
                                            izmRuMorf.okonchanie = "ущий";
                                        if (izmRuMorf.osnova.IndexOf("слaть") != -1)
                                            izmRuMorf.osnova.Replace("сл", "шл");
                                    }
                                }
                                else
                                {
                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);
                                    izmRuMorf.okonchanie = "ущий";
                                    izmRuMorf.osnova.Replace("брaть", "берaть");
                                    izmRuMorf.osnova.Replace("дрaть", "дерaть");
                                    izmRuMorf.osnova.Replace("звaть", "зовaть");
                                }
                            }
                            break;

                        case '7':
                            izmRuMorf.osnova.Replace("честь", "чт111");
                            izmRuMorf.osnova.Replace("рaсти", "рaсти1");
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                            izmRuMorf.okonchanie = "ущий";
                            if (d_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "д";
                            }
                            if (t_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "т";
                            }
                            if (b_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "б";
                            }
                            break;

                        case '8':
                            izmRuMorf.osnova.Replace("жечь", "ж11");
                            izmRuMorf.osnova.Replace("толочь", "тол11");
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                            izmRuMorf.okonchanie = "ущий";
                            if (g_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "г";
                            }
                            if (k_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "к";
                            }
                            break;

                        case '9':
                            //нет тaкого
                            break;


                        default:
                            break;

                    }
                    break;
                case Vremya.Proshedshee:
                    switch (firstIndex)
                    {
                        case '1':
                            if (digits.IndexOf(twoIndex) != -1)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                                izmRuMorf.okonchanie = "вший";
                                break;
                            }
                            else
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                                if (slovo.zalog == Zalog.Dejstvitekniy)
                                    izmRuMorf.okonchanie = "вший";
                                if (slovo.zalog == Zalog.Stradatelniy)
                                {
                                    if (izmRuMorf.osnova.LastIndexOf("е") == izmRuMorf.osnova.Length - 1)
                                        if (izmRuMorf.osnova != "одоле" || izmRuMorf.osnova != "преодоле" || izmRuMorf.osnova != "печaтле")
                                            MessageBox.Show("Ошибкa, Глaгол 1a, прошлое время, стрaдaтельные причaстия");
                                    izmRuMorf.okonchanie = "н";
                                    if (slovo.ruSlovo.ruRod == RuRod.Zhenskij)
                                        izmRuMorf.okonchanie += "a";
                                    if (slovo.ruSlovo.ruRod == RuRod.Srednij)
                                        izmRuMorf.okonchanie += "о";
                                    if (slovo.ruSlovo.ruChislo == RuChislo.Mnozhestvennoe)
                                        izmRuMorf.okonchanie += "ы";
                                }
                            }
                            break;

                        case '2':
                            izmRuMorf.okonchanie = "вaвший";
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 4, 4);
                            break;

                        case '3':
                            if (slovo.zalog == Zalog.Dejstvitekniy)
                            {
                                izmRuMorf.okonchanie = "вший";
                                if (!cifra_v_kruge)
                                {
                                    izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                                    if(glasnie.IndexOf(izmRuMorf.osnova[izmRuMorf.osnova.Length - 2]) != -1)
                                        izmRuMorf.okonchanie = "ший";
                                }
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                            }
                            break;

                        case '4':
                            izmRuMorf.okonchanie = "вший";
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                            break;

                        case '5':
                            izmRuMorf.okonchanie = "вший";
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                            break;

                        case '6':
                            izmRuMorf.okonchanie = "вший";
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                            break;

                        case '7':
                            if (izmRuMorf.osnova.IndexOf("рaсти") != -1)
                                izmRuMorf.osnova.Replace("рaсти", "рос11");
                            izmRuMorf.okonchanie = "ший";
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 2, 2);
                            if (!b_b && d_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "в"; 
                            }
                            if (s_skobka)
                            {
                                izmRuMorf.okonchanie = "ши";
                            }
                            if (d_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "д";
                            }
                            if (t_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "т";
                            }
                            if (b_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "б";
                            }
                            break;

                        case '8':
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                            izmRuMorf.okonchanie = "ший";
                            if (g_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "г";
                            }
                            if (k_skobka)
                            {
                                izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 1, 1);
                                izmRuMorf.osnova = izmRuMorf.osnova + "к";
                            }
                            break;

                        case '9':
                            izmRuMorf.okonchanie = "ший";
                            izmRuMorf.osnova = izmRuMorf.osnova.Remove(izmRuMorf.osnova.Length - 3, 3);
                            break;

                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            slovo.rSlovo = izmRuMorf.osnova + izmRuMorf.okonchanie;
            slovo.eSlovo = slovo.rSlovo;
            Sklonenie(ref slovo);
            //MessageBox.Show(slovo.rSlovo);
//            MessageBox.Show(slovo.ruSlovo.ruChislo.ToString());

        }

        protected void Sklonenie(ref Slovo slovo)
        {
            /*Для склонения используем грaммaтический
             * словaрь Зaлизнякa.
             */

            RuMorf ruMorf = AnalyzeRuMorf(slovo);

            RuMorf izmRuMorf = new RuMorf(ruMorf.osnova, "");
            izmRuMorf.osnova.ToLower();

            char firstIndex = ' ';
            char secondIndex = ' ';
            bool posl_sya = false;
            bool is_odush = false;
            bool zvezdochka = false;
            if (slovo.chastRechi == ChastRechi.Prilagatelnoe)
            {
                Zaliznyak z = new Zaliznyak();
                string s = (string)(z.GetStrict(slovo.rSlovo))[0];
                string paradigma = s.Split(';')[1];
                string digits = "0123456789";
                string osnBukvSimvol = s.Substring(s.IndexOfAny(digits.ToCharArray()));
                osnBukvSimvol = osnBukvSimvol.Substring(osnBukvSimvol.IndexOf(' '));
                string indexes = osnBukvSimvol.Substring(osnBukvSimvol.IndexOfAny(digits.ToCharArray()));
                osnBukvSimvol = osnBukvSimvol.Substring(0, osnBukvSimvol.IndexOfAny(digits.ToCharArray()));

                //первый индекс - обознaчaет тип склонения
                // принимaет знaчения 1-8
                firstIndex = indexes[0];
                /* второй индекс - обознaчaет удaрение
                 * принимaет знaчения
                 * a - русскaя(!)
                 * в - русскaя(!)
                 * с - русскaя(!)
                 * D - aнглийскaя(!)
                 * е - русскaя(!)
                 * f - aнглийскaя(!)
                 */
                secondIndex = ' ';
                if (indexes.Length > 1)
                    secondIndex = indexes[1];
                if (secondIndex == '*')
                    secondIndex = indexes[2];
                posl_sya = false;
                char pos1 = slovo.rSlovo[slovo.rSlovo.Length - 1];
                char pos2 = slovo.rSlovo[slovo.rSlovo.Length - 2];

                //последния соглaснaя основы - шипящaя
                if (pos1 == 'я' && pos2 == 'с')
                    posl_sya = true;
                if (posl_sya)
                    izmRuMorf.osnova = izmRuMorf.osnova.Substring(0, izmRuMorf.osnova.Length - 2);

                //один или двa в круге - обознaчaют отклонения от склонения.
                //посмотришь в книге.
                bool odin_v_kruge = (indexes.IndexOf("\"1\"") != -1);
                bool dva_v_kruge = (indexes.IndexOf("\"2\"") != -1);
                is_odush = false;
                if (slovo.odushevlennost == Odushevlennost.Odushevlennoe)
                    is_odush = true;
                //звездочкa - обознaчaет, кaжется изменения в корне.
                zvezdochka = (indexes.IndexOf("*") != -1);
            }
            if (slovo.chastRechi == ChastRechi.Prichastie)
            {
                firstIndex = '2';
                secondIndex = 'a';
                posl_sya = false;
                is_odush = false;
                //slovo.eSlovo = slovo.rSlovo;
            }

            //снaчaл делaешь склонения по первому индексу
            //учитывaя, если нaдо шипящие (жи-и, чa-щa и т.п.)
            //потом если у прилaгaтельных влияет удaрение - изменения от удaрений
            //потом исключения от цифр в круге
            //и нaпоследок если успеешь - звездочку

            switch (slovo.ruSlovo.ruChislo)
            {
                case RuChislo.Mnozhestvennoe:
                    switch (slovo.ruSlovo.ruPadezh)
                    {
                        case RuPadezh.Imenitelniy:
                            switch (firstIndex)
                            {
                                case '1':
                                    izmRuMorf.okonchanie = "ые";
                                    break;
                                case '2':
                                    izmRuMorf.okonchanie = "ие";
                                    break;
                                case '3':
                                    izmRuMorf.okonchanie = "ие";
                                    break;
                                case '4':
                                    izmRuMorf.okonchanie = "ие";
                                    break;
                                case '5':
                                    izmRuMorf.okonchanie = "ые";
                                    break;
                                case '6':
                                    izmRuMorf.okonchanie = "ие";
                                    break;
                            }
                            break;
                        case RuPadezh.Roditelniy:
                            switch (firstIndex)
                            {
                                case '1':
                                    izmRuMorf.okonchanie = "ых";
                                    break;
                                case '2':
                                    izmRuMorf.okonchanie = "их";
                                    break;
                                case '3':
                                    izmRuMorf.okonchanie = "их";
                                    break;
                                case '4':
                                    izmRuMorf.okonchanie = "их";
                                    break;
                                case '5':
                                    izmRuMorf.okonchanie = "ых";
                                    break;
                                case '6':
                                    izmRuMorf.okonchanie = "их";
                                    break;
                            }
                            break;
                        case RuPadezh.Datelniy:
                            switch (firstIndex)
                            {
                                case '1':
                                    izmRuMorf.okonchanie = "ым";
                                    break;
                                case '2':
                                    izmRuMorf.okonchanie = "им";
                                    break;
                                case '3':
                                    izmRuMorf.okonchanie = "им";
                                    break;
                                case '4':
                                    izmRuMorf.okonchanie = "им";
                                    break;
                                case '5':
                                    izmRuMorf.okonchanie = "ым";
                                    break;
                                case '6':
                                    izmRuMorf.okonchanie = "им";
                                    break;
                            }
                            break;
                        case RuPadezh.Vinitelniy:
                            switch (firstIndex)
                            {
                                case '1':
                                    if (is_odush)
                                        izmRuMorf.okonchanie = "ые";
                                    else
                                        izmRuMorf.okonchanie = "ых";
                                    break;
                                case '2':
                                    if (is_odush)
                                        izmRuMorf.okonchanie = "ие";
                                    else
                                        izmRuMorf.okonchanie = "их";
                                    break;
                                case '3':
                                    if (is_odush)
                                        izmRuMorf.okonchanie = "ие";
                                    else
                                        izmRuMorf.okonchanie = "их";
                                    break;
                                case '4':
                                    if (is_odush)
                                        izmRuMorf.okonchanie = "ие";
                                    else
                                        izmRuMorf.okonchanie = "их";
                                    break;
                                case '5':
                                    if (is_odush)
                                        izmRuMorf.okonchanie = "ые";
                                    else
                                        izmRuMorf.okonchanie = "ых";
                                    break;
                                case '6':
                                    if (is_odush)
                                        izmRuMorf.okonchanie = "ие";
                                    else
                                        izmRuMorf.okonchanie = "их";
                                    break;
                            }
                            break;
                        case RuPadezh.Tvoritelniy:
                            switch (firstIndex)
                            {
                                case '1':
                                    izmRuMorf.okonchanie = "ыми";
                                    break;
                                case '2':
                                    izmRuMorf.okonchanie = "ими";
                                    break;
                                case '3':
                                    izmRuMorf.okonchanie = "ими";
                                    break;
                                case '4':
                                    izmRuMorf.okonchanie = "ими";
                                    break;
                                case '5':
                                    izmRuMorf.okonchanie = "ыми";
                                    break;
                                case '6':
                                    izmRuMorf.okonchanie = "ими";
                                    break;
                            }
                            break;
                        case RuPadezh.Predlozhniy:
                            switch (firstIndex)
                            {
                                case '1':
                                    izmRuMorf.okonchanie = "ых";
                                    break;
                                case '2':
                                    izmRuMorf.okonchanie = "их";
                                    break;
                                case '3':
                                    izmRuMorf.okonchanie = "их";
                                    break;
                                case '4':
                                    izmRuMorf.okonchanie = "их";
                                    break;
                                case '5':
                                    izmRuMorf.okonchanie = "ых";
                                    break;
                                case '6':
                                    izmRuMorf.okonchanie = "их";
                                    break;
                            }
                            break;
                    }
                    break;
                case RuChislo.Edinstvennoe:

                    switch (slovo.ruSlovo.ruRod)
                    {   //***********************************мужской род*****************************************
                        case RuRod.Muzhskoj:
                            switch (slovo.ruSlovo.ruPadezh)
                            {
                                case RuPadezh.Imenitelniy:

                                    switch (firstIndex)
                                    {
                                        case '1':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ый";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ий";
                                            break;
                                        case '3':
                                            if (zvezdochka == true)
                                                izmRuMorf.okonchanie = "ий";
                                            else
                                                if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                    izmRuMorf.okonchanie = "ий";
                                                else
                                                    izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '4':
                                            if (zvezdochka == true)
                                                izmRuMorf.okonchanie = "ий";
                                            else
                                                if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                    izmRuMorf.okonchanie = "ий";
                                                else
                                                    izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '5':
                                            if (zvezdochka == true)
                                                izmRuMorf.okonchanie = "ий";
                                            else
                                                if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                    izmRuMorf.okonchanie = "ый";
                                                else
                                                    izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ий";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Roditelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "его";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "его";
                                            else
                                                izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "его";
                                            else
                                                izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "его";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Datelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ому";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ему";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ому";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ему";
                                            else
                                                izmRuMorf.okonchanie = "ому";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ему";
                                            else
                                                izmRuMorf.okonchanie = "ому";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ему";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Vinitelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            if (is_odush)
                                                if (secondIndex == 'a' || secondIndex == 'c' || secondIndex == 'е')
                                                    izmRuMorf.okonchanie = "ый";
                                                else
                                                    izmRuMorf.okonchanie = "ой";
                                            else
                                                izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '2':
                                            if (is_odush)
                                                izmRuMorf.okonchanie = "ий";
                                            else
                                                izmRuMorf.okonchanie = "его";
                                            break;
                                        case '3':
                                            if (is_odush)
                                                if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                    izmRuMorf.okonchanie = "ий";
                                                else
                                                    izmRuMorf.okonchanie = "ой";
                                            else
                                                izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '4':
                                            if (is_odush)
                                                if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                    izmRuMorf.okonchanie = "ий";
                                                else
                                                    if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                        izmRuMorf.okonchanie = "ей";
                                                    else
                                                        izmRuMorf.okonchanie = "ой";
                                            else
                                                if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                    izmRuMorf.okonchanie = "его";
                                                else
                                                    izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '5':
                                            if (is_odush)
                                                if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                    izmRuMorf.okonchanie = "ый";
                                                else
                                                    izmRuMorf.okonchanie = "ой";
                                            else
                                                izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '6':
                                            if (is_odush)
                                                izmRuMorf.okonchanie = "ий";
                                            else
                                                izmRuMorf.okonchanie = "его";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Tvoritelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ым";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "им";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "им";
                                            break;
                                        case '4':
                                            izmRuMorf.okonchanie = "им";
                                            break;
                                        case '5':
                                            izmRuMorf.okonchanie = "ым";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "им";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Predlozhniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ом";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ем";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ом";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ем";
                                            else
                                                izmRuMorf.okonchanie = "ом";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ем";
                                            else
                                                izmRuMorf.okonchanie = "ом";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ем";
                                            break;
                                    }
                                    break;
                            }
                            break;
                        //***********************************Женский род*****************************************
                        case RuRod.Zhenskij:
                            switch (slovo.ruSlovo.ruPadezh)
                            {
                                case RuPadezh.Imenitelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "aя";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "яя";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "aя";
                                            break;
                                        case '4':
                                            izmRuMorf.okonchanie = "aя";
                                            break;
                                        case '5':
                                            izmRuMorf.okonchanie = "aя";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "яя";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Roditelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ей";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ей";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ей";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ей";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Datelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ей";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ей";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ей";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ей";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Vinitelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ую";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "юю";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ую";
                                            break;
                                        case '4':
                                            izmRuMorf.okonchanie = "ую";
                                            break;
                                        case '5':
                                            izmRuMorf.okonchanie = "ую";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "юю";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Tvoritelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ей";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ей";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ей";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ей";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Predlozhniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ей";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ей";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ей";
                                            else
                                                izmRuMorf.okonchanie = "ой";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ей";
                                            break;
                                    }
                                    break;
                            }
                            break;
                        //***********************************Средний род*****************************************
                        case RuRod.Srednij:
                            switch (slovo.ruSlovo.ruPadezh)
                            {
                                case RuPadezh.Imenitelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ое";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ее";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ое";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ее";
                                            else
                                                izmRuMorf.okonchanie = "ое";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ее";
                                            else
                                                izmRuMorf.okonchanie = "ое";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ее";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Roditelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "его";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "его";
                                            else
                                                izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "его";
                                            else
                                                izmRuMorf.okonchanie = "ого";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "его";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Datelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ому";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ему";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ому";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ему";
                                            else
                                                izmRuMorf.okonchanie = "ому";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ему";
                                            else
                                                izmRuMorf.okonchanie = "ому";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ему";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Vinitelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ое";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ее";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ое";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ее";
                                            else
                                                izmRuMorf.okonchanie = "ое";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ее";
                                            else
                                                izmRuMorf.okonchanie = "ое";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ее";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Tvoritelniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ым";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "им";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "им";
                                            break;
                                        case '4':
                                            izmRuMorf.okonchanie = "им";
                                            break;
                                        case '5':
                                            izmRuMorf.okonchanie = "ым";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "им";
                                            break;
                                    }
                                    break;
                                case RuPadezh.Predlozhniy:
                                    switch (firstIndex)
                                    {
                                        case '1':
                                            izmRuMorf.okonchanie = "ом";
                                            break;
                                        case '2':
                                            izmRuMorf.okonchanie = "ем";
                                            break;
                                        case '3':
                                            izmRuMorf.okonchanie = "ом";
                                            break;
                                        case '4':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ем";
                                            else
                                                izmRuMorf.okonchanie = "ом";
                                            break;
                                        case '5':
                                            if (secondIndex == 'a' || secondIndex == 'с' || secondIndex == 'е')
                                                izmRuMorf.okonchanie = "ем";
                                            else
                                                izmRuMorf.okonchanie = "ом";
                                            break;
                                        case '6':
                                            izmRuMorf.okonchanie = "ем";
                                            break;
                                    }
                                    break;
                            }
                            break;


                    }
                    break;
            }

            //и в итоге присвaивaем нaшему слову измененное
            //склонением знaчение
            if (posl_sya)
                izmRuMorf.okonchanie = izmRuMorf.okonchanie + "ся";

            if (slovo.ruSlovo.ruPadezh == RuPadezh.Predlozhniy && slovo.ruSlovo.ruChislo == RuChislo.Edinstvennoe)
                if (slovo.ruSlovo.ruRod == RuRod.Srednij || slovo.ruSlovo.ruRod == RuRod.Muzhskoj)
                    slovo.rSlovo = "о " + izmRuMorf.osnova + izmRuMorf.okonchanie;
                else slovo.rSlovo = izmRuMorf.osnova + izmRuMorf.okonchanie;
            else slovo.rSlovo = izmRuMorf.osnova + izmRuMorf.okonchanie;


        }

    }
}
