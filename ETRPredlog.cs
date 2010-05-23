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
	public class ETRPredlog : IModule
	{
		public ETRPredlog()
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
			
			bool translated = false;
			
			if(analyzed.eSlovo == "FQV" || analyzed.eSlovo == "FZJ" || analyzed.eSlovo == "FZP")
			{
				analyzed.rSlovo = "";
				translated = true;
			}
			else if(analyzed.eSlovo == "FT")
			{
				if(place-1>0 && pr[place-1].eSlovo[0] != 'Q')
				{
					analyzed.rSlovo = "";
					translated = true;
				}
			}
			if(!translated)
			{
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
			}
			
			return analyzed;
		}	
	}
}
