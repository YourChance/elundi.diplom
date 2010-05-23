/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 07.05.2009
 * Time: 8:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace ETRTranslator
{
	/// <summary>
	/// Description of RTEVerb.
	/// </summary>
	public class RTEVerb : IModule
	{
		public RTEVerb()
		{
		}
		public Slovo Analyze(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			return analyzed;
		}
		
		Slovo c;
		
		 public Slovo Translate(Predlozhenie a, int b)//для перевода русских глаголов на эльюнди
        {
		 	bool glag = true;
            c = new Slovo();
            c = a[b];
            if (a == null || a[b] == null) return c;
            if (c.rSlovo == "") return c;
            //Dictionary dict = a.Dict;
            
            //c.eSlovo = c.eSlovo.ToUpper(); - он вообще в словарь заглядывал?? /pas/
            
            string ruInf = a[b].eSlovo;
            if(a[b].eSlovo.Substring(a[b].eSlovo.Length-2)=="ся")
            {
            	ruInf = a[b].eSlovo.Substring(0,a[b].eSlovo.Length-2);
            	c.zalog = Zalog.Vozvratniy;
            }
            //throw new Exception(ruInf);
            ArrayList ar = a.Dict.GetStrictRusEx(ruInf);
            c.rSlovo = a[b].eSlovo;
            c.eSlovo = ar.Count>0?((DictSlovo)ar[0]).El:"";
            if(c.naklonenie == Naklonenie.Infinitiv)
            {
            	if(c.zalog == Zalog.Vozvratniy)
            		c.eSlovo = c.eSlovo+"-A";
            	return c;
            }
            Regex reg = new Regex(@"(?<часть1>.{1,})(?<часть2>((я|а|и)сь)|(вши))(?<часть3>(сь)?)");
            Match mreg = reg.Match(c.rSlovo);
            if (mreg.Success && (mreg.Groups["часть1"].Value + mreg.Groups["часть2"].Value + mreg.Groups["часть3"].Value == c.rSlovo)) glag = false;
            else
            {
                char s = c.rSlovo[c.rSlovo.Length - 1];
                if (s == 'а' || /*s == 'я' ||*/ s == 'в' || s == 'и') glag = false;
            }
            if (c.eSlovo != "")
            {
                if (c.eSlovo[0] == 'R' || c.eSlovo[0] == 'T' || c.eSlovo[0] == 'Y' || c.eSlovo[0] == 'U' || c.eSlovo[0] == 'O')
                {
                   
                	c = HaractEl(c);
                    if (!glag) c.eSlovo = GlagModule.ZamLit(c.eSlovo, 'O', 0);
                }
            }
            else
            {
            	c.eSlovo = ruInf;
            	c.eSlovo = a.Dict.CreateNewSlovoEx(c);

            	/*
            	//ересь
                //на входе c.rSlovo
                //.......
                //код по постановке русского глагола или деепричастия в инифинитив глагола без приставок
                //.......
                //на выходе в c.rSlovo должен быть инфинитив русского глагола 
                c.eSlovo = dict.TranslateRus(a[b].rSlovo);
                if (c.eSlovo != "")
                {
                    if (c.eSlovo[0] == 'R' || c.eSlovo[0] == 'T' || c.eSlovo[0] == 'Y' || c.eSlovo[0] == 'U' || c.eSlovo[0] == 'O')
                    {
                        c = HaractEl(c);
                        if (!glag) c.eSlovo = GlagModule.ZamLit(c.eSlovo, 'O', 0);
                    }
                }
                else
                {
                    //переводим как любой перевод (стр.160-161)
                    //требуется глобальная переменная (например int COUNT в головном модуле)
                    int COUNT = 0;
                    if (!glag) c.eSlovo = 'O' + "-N-" + (++COUNT).ToString();
                    else
                    {
                        switch (c.vremya)
                        {
                            case Vremya.Buduschee: c.eSlovo = 'U' + "-N-" + (++COUNT).ToString(); break;
                            case Vremya.Nastoyaschee: c.eSlovo = 'T' + "-N-" + (++COUNT).ToString(); break;
                            case Vremya.Proshedshee: c.eSlovo = 'Y' + "-N-" + (++COUNT).ToString(); break;
                            default: c.eSlovo = 'R' + "-N-" + (++COUNT).ToString(); break;
                        }
                    }
                }
                */

            }
            return c;
        }
        public Slovo HaractEl(Slovo c)
        {
            string slovo = GlagModule.OsnovaEl(c.eSlovo);
            GlagModule.ZaliznyakMy t = new GlagModule.ZaliznyakMy();
            if (t.VidGlag(c.rSlovo) == Vid.Zavershennost) switch (c.vid)
                {
                    default: /*Vid.Zavershennost:*/ slovo = slovo[0].ToString() + "S-" + slovo.Substring(1); break;
                    case Vid.NevozvratnayaZavershennost: slovo = slovo[0].ToString() + "S-" + slovo.Substring(1) + "-S"; break;
                    case Vid.Mgnovennost: slovo = slovo[0].ToString() + "-RBY-" + slovo.Substring(1); break;
                    case Vid.NachaloDejstviya: slovo = slovo[0].ToString() + "-SCY-" + slovo.Substring(1); break;
                    case Vid.OgranichenieDlitelnosti: slovo = slovo[0].ToString() + "-PVI-" + slovo.Substring(1); break;
                }
            else switch (c.vid)
                {
                    default: /*Vid.NeopredelennayaDlitelnost:*/ slovo = slovo[0].ToString() + "-PVS-" + slovo.Substring(1); break;
                    case Vid.PostoyannayaDlitelnost: slovo = slovo[0].ToString() + "-RBA-" + slovo.Substring(1); break;
                    case Vid.NezavershennostDejstviya: slovo = slovo[0].ToString() + "-UCS-" + slovo.Substring(1); break;
                }
            if (c.sostoynie == Sostoynie.Безличное) slovo = slovo[0].ToString() + "-TVY-" + slovo.Substring(1);
            bool tire = true;
            while (tire)
            {
                tire = false;
                Regex ra = new Regex(@"(?<часть1>.*)(?<часть2>\-{2,})(?<часть3>.*)");
                Match mb = ra.Match(slovo);                
                if (mb.Success)
                {
                    tire = true;
                    slovo = mb.Groups["часть1"].Value + "-" + mb.Groups["часть3"].Value;
                }
            }
            if (slovo[slovo.Length - 1] == '-') slovo = slovo.Substring(0, slovo.Length - 1);
            switch (c.zalog)
            {
                case Zalog.Vozvratniy: slovo += "-A"; break;
                case Zalog.Vzaimniy: slovo += "-SBA"; break;
                case Zalog.Stradatelniy: slovo += "-F"; break;
            }
            switch (c.vremya)
            {
                case Vremya.Buduschee:
                    slovo = GlagModule.ZamLit(slovo, 'U', 0);
                    break;
                case Vremya.Nastoyaschee:
                    slovo = GlagModule.ZamLit(slovo, 'T', 0);
                    break;
                case Vremya.Proshedshee:
                    slovo = GlagModule.ZamLit(slovo, 'Y', 0);
                    break;
                case Vremya.BuduscheeDalekoe:
                    slovo = GlagModule.ZamLit(slovo, 'U', 0);
                    slovo += "-UBA";
                    break;
                case Vremya.BuduscheeSProshedshim:
                    slovo = GlagModule.ZamLit(slovo, 'U', 0);
                    slovo += "-YBA";
                    break;
                case Vremya.BuduscheeVNastoyaschem:
                    slovo = GlagModule.ZamLit(slovo, 'U', 0);
                    slovo += "-TBA";
                    break;
                case Vremya.BuduscheeDlitelnoe:
                    slovo = GlagModule.ZamLit(slovo, 'U', 0);
                    slovo += "-RBA";
                    break;
                case Vremya.ProshedsheeBuduscheeBezNastoyaschego:
                    slovo = GlagModule.ZamLit(slovo, 'Y', 0);
                    slovo += "-UBA";
                    break;
                case Vremya.Davnoproshedshee:
                    slovo = GlagModule.ZamLit(slovo, 'Y', 0);
                    slovo += "-YBA";
                    break;
                case Vremya.ProshedsheeSNastoyaschim:
                    slovo = GlagModule.ZamLit(slovo, 'Y', 0);
                    slovo += "-TBA";
                    break;
                case Vremya.ProshedsheeDlitelnoe:
                    slovo = GlagModule.ZamLit(slovo, 'Y', 0);
                    slovo += "-RBA";
                    break;
                case Vremya.NastoyascheeSBuduschim:
                    slovo = GlagModule.ZamLit(slovo, 'T', 0);
                    slovo += "-UBA";
                    break;
                case Vremya.NastoyascheeSProshedshim:
                    slovo = GlagModule.ZamLit(slovo, 'T', 0);
                    slovo += "-YBA";
                    break;
                case Vremya.NastoyascheeVNastoyaschem:
                    slovo = GlagModule.ZamLit(slovo, 'T', 0);
                    slovo += "-TBA";
                    break;
                case Vremya.NastoyascheeDlitelnoe:
                    slovo = GlagModule.ZamLit(slovo, 'T', 0);
                    slovo += "-RBA";
                    break;
            }
            switch (c.naklonenie)
            {
                case Naklonenie.Povelitelnoe: slovo += "-J"; break;
                case Naklonenie.Soslagatelnoe: slovo = "J-OBP " + slovo; break;
            }
            c.eSlovo = slovo;
            return c;
        }
		
		/*public Slovo Translate(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			TranslateOsnova(ref pr,ref analyzed);
			
			return analyzed;
		}	*/
		
		public void TranslateOsnova(ref Predlozhenie pr,ref Slovo analyzed)
		{
			ArrayList al = pr.Dict.GetStrictRusEx(analyzed.eSlovo);
            if(al.Count > 0)
			{
				analyzed.eSlovo = ((DictSlovo)al[0]).El;
			}
			else
            {
				analyzed.eSlovo = pr.Dict.CreateNewSlovoEx(analyzed);
			}
		}
	}
}
