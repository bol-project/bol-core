using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Core.Model
{
	public class CodenamePerson : BasePerson 
	{
		public string FirstNameCharacter { get; set; }
		public DateTime YearOfBirth { get; set; }
		public string ShortHash { get; set; }
		public string CheckSum { get; set; }
	}
}
