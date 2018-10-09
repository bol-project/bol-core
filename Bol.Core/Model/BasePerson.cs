using System;
using System.Collections.Generic;
using System.Text;

namespace Bol.Core.Model
{
	public class BasePerson
	{
		public string CountryCode { get; set; }
		public string Surname { get; set; }
		public string MiddleName { get; set; }
		public string ThirdName { get; set; }
		public Gender Gender { get; set; }
		public string Combination { get; set; }
	}

	public enum Gender
	{
		Male,
		Female,
		Unspecified
	}
}
