using System;
using System.Collections;
using ETRTranslator;

namespace ETEnTranslator
{
	public class ETEnCore
	{
        IModule NounModule;
        IModule AdjectiveModule;
        IModule VerbModule;
        IModule PredlogModule;
        IModule DefaultModule;
        IModule OtherModule;

        public event CoreTickEventHandler Tick;
        public event CoreEndEventHandler End;

        double Count = 0;
        double Scet = 0;

        public void OnTick()
        {
            if (this.Tick != null)
                Tick(this, new CoreTickEventArgs(Count != 0 ? Scet / Count : Count));
        }

		public ETEnCore()
		{
            NounModule = new ETEnNoun();
            /**
             * Сюда будете добавлять
             * свои модули для анализа,
             * когда сделаете
             */
            AdjectiveModule = new ETEnAdjective();
            VerbModule = new ETEnGlagol();
            PredlogModule = new ETEnPredlog();
            DefaultModule = new ETEnEmpty(); //new ETEnDefault();
            OtherModule = new ETEnOther();
		}

        public ArrayList Analyze(string inText)
		{
			Grafemat gr = new Grafemat();
			ArrayList prText = gr.AnalyzeTextEl(inText);
			
			for(int i=0;i<prText.Count;i++)
			{
				Predlozhenie curPred = (Predlozhenie)prText[i];
				
				//
				//	Первый этап - анализ
				//
				
				//Нулевой проход - определение частей речи
				
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					if(curSlovo.chastRechi != ChastRechi.Znak)
					{
						string eSlovo = curSlovo.eSlovo;
                        if (eSlovo.Length == 1 || eSlovo.Length == 2 && eSlovo[1] == '-')
                        {
                            curSlovo.chastRechi = ChastRechi.Mestoimenie;
                            curSlovo = OtherModule.Analyze(curPred, j);
                        }
						else
						switch(eSlovo[0])
						{
							case 'Q':
							case 'W':
								curSlovo.chastRechi = ChastRechi.Suschestvitelnoe;
								break;
							case 'F':
								curSlovo.chastRechi = ChastRechi.Predlog;
								break;
							case 'E':
								curSlovo.chastRechi = ChastRechi.Prilagatelnoe;
								break;
							case 'I':
								curSlovo.chastRechi = ChastRechi.Prichastie;
								break;
							case 'A':
                                curSlovo.chastRechi = ChastRechi.Mestoimenie;
								break;
							case 'R':
							case 'T':
							case 'Y':
							case 'U':
							case 'O':
								curSlovo.chastRechi = ChastRechi.Glagol;
								break;
							//etc...
							default:
								curSlovo.chastRechi = ChastRechi.Mezhdometie;
								break;
						}
					}
					
					curPred.SetSlovo(curSlovo,j);
				}
				
				// Первый проход - существительные
				
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					if(curSlovo.chastRechi != ChastRechi.Znak)
					{
						string eSlovo = curSlovo.eSlovo;
						if(eSlovo[0]=='Q' || eSlovo[0]=='W')
						{
							curSlovo = NounModule.Analyze(curPred,j);	
						}
					}
					
					curPred.SetSlovo(curSlovo,j);
				}
				
				// Полуторный проход - глаголы
				
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
                    if (curSlovo.chastRechi != ChastRechi.Znak && curSlovo.chastRechi != ChastRechi.Mestoimenie)
					{
						string eSlovo = curSlovo.eSlovo;
						if(eSlovo[0]=='R' || eSlovo[0]=='T' || eSlovo[0]=='Y' || eSlovo[0]=='U')
						{
							curSlovo = VerbModule.Analyze(curPred,j);	
						}
					}
					
					curPred.SetSlovo(curSlovo,j);
				}
				
				//Второй проход - все остальное.
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					if(curSlovo.chastRechi != ChastRechi.Znak)
					{
						string eSlovo = curSlovo.eSlovo;
						
						//
						// Заменить на соответствующее для данной части речи.
						//
						if(eSlovo[0]=='E' || eSlovo[0]=='I')
						{
							curSlovo = AdjectiveModule.Analyze(curPred,j);
						}
						if(eSlovo[0]=='F')
						{
							curSlovo = PredlogModule.Analyze(curPred,j);
						}
						
						if(eSlovo[0]=='A')
						{
							curSlovo = OtherModule.Analyze(curPred,j);
						}
					}
					
					curPred.SetSlovo(curSlovo,j);
				}
                prText[i] = curPred;
			}
            return prText;
		}

        /**
         * Тоже самое что и Analyze, только не лезем в БД.
         * Тупо получаем список и порядок частей речи
         */
        public ArrayList GetMorphology(string inText)
        {
            Grafemat gr = new Grafemat();
            ArrayList prText = gr.AnalyzeTextEl(inText);

            for (int i = 0; i < prText.Count; i++)
            {
                Predlozhenie curPred = (Predlozhenie)prText[i];

                //
                //	Первый этап - анализ
                //

                //Нулевой проход - определение частей речи

                for (int j = 0; j < curPred.Count; j++)
                {
                    Slovo curSlovo = curPred[j];
                    if (curSlovo.chastRechi != ChastRechi.Znak)
                    {
                        string eSlovo = curSlovo.eSlovo;
                        if (eSlovo.Length == 1 || eSlovo.Length == 2 && eSlovo[1] == '-')
                        {
                            curSlovo.chastRechi = ChastRechi.Mestoimenie;
                            curSlovo = OtherModule.Analyze(curPred, j);
                        }
                        else
                            switch (eSlovo[0])
                            {
                                case 'Q':
                                case 'W':
                                    curSlovo.chastRechi = ChastRechi.Suschestvitelnoe;
                                    break;
                                case 'F':
                                    curSlovo.chastRechi = ChastRechi.Predlog;
                                    break;
                                case 'E':
                                    curSlovo.chastRechi = ChastRechi.Prilagatelnoe;
                                    break;
                                case 'I':
                                    curSlovo.chastRechi = ChastRechi.Prichastie;
                                    break;
                                case 'A':
                                    curSlovo.chastRechi = ChastRechi.Mestoimenie;
                                    break;
                                case 'R':
                                case 'T':
                                case 'Y':
                                case 'U':
                                case 'O':
                                    curSlovo.chastRechi = ChastRechi.Glagol;
                                    break;
                                //etc...
                                default:
                                    curSlovo.chastRechi = ChastRechi.Mezhdometie;
                                    break;
                            }
                    }

                    curPred.SetSlovo(curSlovo, j);
                }

                prText[i] = curPred;
            }
            return prText;
        }

        protected String NormalizeText(string inText)
        {
            ArrayList wordList = GetMorphology(inText);
            string resultText = "";
            for (int i = 0; i < wordList.Count; i++)
            {
                Predlozhenie curPred = (Predlozhenie)wordList[i];
                /**
                 * Что считать нормальным порядком слов в предложении?
                 * Подлежащее сказуемое а дальше.. хз что дальше..
                 * Хотя для английского наиболее важны только подлежаще и сказуемое.
                 * Лан. Забиваем на остальные части речи пока
                 */
                /**
                 * Ищем подлежащее и сказуемое
                 * Что считать подлежащим?
                 * Первое найденное существительное или местоимение, перед которым не стоит предлог
                 * Если перед существительным стоит прилагательное (определяющее слово), то переносим
                 * его вместе с подлежащим в начало
                 * Что считать сказуемым?
                 * Ближайший к подлежащему глагол
                 */
                Slovo predSlovo = null;
                Slovo podlejashee = null;
                int podlejashee_index = -1;
                Slovo skazuemoe = null;
                int skazuemoe_index = -1;
                Slovo adjective = null;
                int adjective_index = -1;

                string[] личныеместоимения = new string[] { "Q-", "W", "E-", "T-", "R-", "U", "I-", "Y" };

                for (int j = 0; j < curPred.Count; j++)
                {
                    if (podlejashee_index < 0 && j > 5)
                        break;

                    Slovo curSlovo = curPred[j];
                    if (podlejashee_index < 0 && (curSlovo.chastRechi == ChastRechi.Suschestvitelnoe || Array.Exists<String>(личныеместоимения, new Predicate<String>(delegate(String str)
                    {
                        return str == curSlovo.eSlovo;
                    }))) &&
                        (predSlovo == null || predSlovo.chastRechi != ChastRechi.Predlog))
                    {
                        podlejashee = curSlovo;
                        podlejashee_index = j;
                        if (predSlovo != null)
                        {
                            if (predSlovo.chastRechi == ChastRechi.Prilagatelnoe)
                            {
                                adjective = predSlovo;
                                adjective_index = j - 1;
                            }
                        }
                    }
                    if (curSlovo.chastRechi == ChastRechi.Glagol)
                    {
                        skazuemoe = curSlovo;
                        skazuemoe_index = j;
                        if (podlejashee_index >= 0)
                            break; //мы все нашли
                    }
                    predSlovo = curSlovo;
                }
                int hasAdjective = adjective_index >= 0 ? 1 : 0;
                if (adjective_index >= 0 && curPred.Count > 1)
                {
                    for (int t = adjective_index; t > 0; t--)
                        curPred[t] = curPred[t - 1];
                    curPred[0] = adjective;
                }
                if (podlejashee_index >= 0 && curPred.Count > 1 + hasAdjective)
                {
                    for (int t = podlejashee_index; t > hasAdjective; t--)
                        curPred[t] = curPred[t - 1];
                    curPred[hasAdjective] = podlejashee;
                }
                if (skazuemoe_index >= 0 && curPred.Count > 2 + hasAdjective)
                {
                    if (skazuemoe_index < podlejashee_index)
                        skazuemoe_index += 1 + hasAdjective;
                    for (int t = skazuemoe_index; t > 1 + hasAdjective; t--)
                        curPred[t] = curPred[t - 1];
                    curPred[hasAdjective + 1] = skazuemoe;
                }
                resultText += curPred.ToEString() + " ";
            }
            return resultText.Trim();
        }

        /**
         * Собственно, перевод..
         */
        public string Translate(string inText)
        {
            //Перед тем как переводить, приведем текст в нормальную форму
            inText = NormalizeText(inText);

            Grafemat gr = new Grafemat();
            ArrayList prText = gr.AnalyzeTextEl(inText);

            Count = 0;
            Scet = 0;

            //Считаем слова (для тиков)
            foreach (Predlozhenie curPred in prText)
                for (int i = 0; i < curPred.Count; i++)
                    Count++;

            Count *= 2;

            for (int i = 0; i < prText.Count; i++)
            {
                Predlozhenie curPred = (Predlozhenie)prText[i];

                //
                //	Первый этап - анализ
                //

                //Нулевой проход - определение частей речи

                for (int j = 0; j < curPred.Count; j++)
                {
                    Slovo curSlovo = curPred[j];
                    if (curSlovo.chastRechi != ChastRechi.Znak)
                    {
                        string eSlovo = curSlovo.eSlovo;
                        if (eSlovo.Length == 1)
                            curSlovo.chastRechi = ChastRechi.Mestoimenie;
                        else
                            switch (eSlovo[0])
                            {
                                case 'Q':
                                case 'W':
                                    curSlovo.chastRechi = ChastRechi.Suschestvitelnoe;
                                    break;
                                case 'F':
                                case 'P':
                                    curSlovo.chastRechi = ChastRechi.Predlog;
                                    break;
                                case 'E':
                                    curSlovo.chastRechi = ChastRechi.Prilagatelnoe;
                                    break;
                                case 'I':
                                    curSlovo.chastRechi = ChastRechi.Prichastie;
                                    break;
                                case 'A':
                                    curSlovo.chastRechi = ChastRechi.Mestoimenie;
                                    break;
                                case 'R':
                                case 'T':
                                case 'Y':
                                case 'U':
                                case 'O':
                                    curSlovo.chastRechi = ChastRechi.Glagol;
                                    break;
                                //etc...
                                default:
                                    curSlovo.chastRechi = ChastRechi.Mezhdometie;
                                    break;
                            }
                    }

                    curPred.SetSlovo(curSlovo, j);
                }

                // Первый проход - существительные

                for (int j = 0; j < curPred.Count; j++)
                {
                    Slovo curSlovo = curPred[j];
                    if (curSlovo.chastRechi != ChastRechi.Znak)
                    {
                        string eSlovo = curSlovo.eSlovo;
                        if (eSlovo[0] == 'Q' || eSlovo[0] == 'W')
                        {
                            curSlovo = NounModule.Analyze(curPred, j);
                            Scet++;
                            OnTick();
                        }
                    }

                    curPred.SetSlovo(curSlovo, j);
                }

                // Полуторный проход - глаголы

                for (int j = 0; j < curPred.Count; j++)
                {
                    Slovo curSlovo = curPred[j];
                    if (curSlovo.chastRechi != ChastRechi.Znak)
                    {
                        string eSlovo = curSlovo.eSlovo;
                        if (eSlovo[0] == 'R' || eSlovo[0] == 'T' || eSlovo[0] == 'Y' || eSlovo[0] == 'U' || eSlovo[0] == 'I')
                        {
                            curSlovo = VerbModule.Analyze(curPred, j);
                            Scet++;
                            OnTick();
                        }
                    }

                    curPred.SetSlovo(curSlovo, j);
                }

                //Второй проход - все остальное.
                for (int j = 0; j < curPred.Count; j++)
                {
                    Slovo curSlovo = curPred[j];
                    if (curSlovo.chastRechi != ChastRechi.Znak)
                    {
                        string eSlovo = curSlovo.eSlovo;

                        //
                        // Заменить на соответствующее для данной части речи.
                        //
                        if (eSlovo[0] == 'E')
                        {
                            curSlovo = AdjectiveModule.Analyze(curPred, j);
                            Scet++;
                            OnTick();
                        }
                        if (eSlovo[0] == 'F')
                        {
                            curSlovo = PredlogModule.Analyze(curPred, j);
                            Scet++;
                            OnTick();
                        }

                        if (eSlovo[0] == 'A')
                        {
                            curSlovo = OtherModule.Analyze(curPred, j);
                            Scet++;
                            OnTick();
                        }
                    }

                    curPred.SetSlovo(curSlovo, j);
                }

                //
                //	Второй этап - перевод
                //

                for (int j = 0; j < curPred.Count; j++)
                {
                    Slovo curSlovo = curPred[j];
                    try
                    {

                        switch (curSlovo.chastRechi)
                        {
                            case ChastRechi.Suschestvitelnoe:
                                curSlovo = NounModule.Translate(curPred, j);
                                break;
                            case ChastRechi.Prilagatelnoe:
                                curSlovo = AdjectiveModule.Translate(curPred, j);
                                break;
                            case ChastRechi.Prichastie:
                            case ChastRechi.Glagol:
                                curSlovo = VerbModule.Translate(curPred, j);
                                break;
                            case ChastRechi.Predlog:
                                curSlovo = PredlogModule.Translate(curPred, j);
                                break;
                            case ChastRechi.Mestoimenie:
                            case ChastRechi.Mezhdometie:
                            case ChastRechi.Narechie:
                                curSlovo = OtherModule.Translate(curPred, j);
                                break;
                            default:
                                curSlovo = DefaultModule.Translate(curPred, j);
                                break;
                        }
                    }
                    catch
                    { }

                    curPred.SetSlovo(curSlovo, j);
                    Scet++;
                    OnTick();
                }

                prText[i] = curPred;
            }

            string rezult = "";

            Scet = Count;
            OnTick();

            for (int i = 0; i < prText.Count; i++)
            {
                rezult += ((Predlozhenie)prText[i]).ToEnString();
            }

            if (this.End != null)
                End(this, new CoreEndEventArgs(rezult));

            return rezult;
        }
    }
}
