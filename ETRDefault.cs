/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 07.05.2009
 * Time: 7:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace ETRTranslator
{
	/// <summary>
	/// Description of ETRDefault.
	/// </summary>
	public class ETRDefault : IModule
	{
		public ETRDefault()
		{
		}
		public Slovo Analyze(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			return analyzed;
		}
		
		public Slovo Translate(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			Dictionary dict = new Dictionary();
			ArrayList al = dict.GetStrict(analyzed.eSlovo);
            if(al.Count > 0)
			{
				analyzed.rSlovo = ((DictSlovo)al[0]).Rus;
			}
			else
            {
				analyzed.rSlovo = analyzed.eSlovo;
			}
			
			return analyzed;
		}	
	}
}
