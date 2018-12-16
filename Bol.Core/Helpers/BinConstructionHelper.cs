using System;
using System.Collections.Generic;
using System.Text;
using Bol.Core.Abstractions.Helpers;

namespace Bol.Core.Helpers
{
    internal class BinConstructionHelper : IBinConstructionHelper
    {
        public string Construct(char gender, int dayOfBirth, string firstName, string surName, string postalCode)
        {
            var addToBirthDay = new int();
            switch (gender)
            {
                case 'M':
                    {
                        addToBirthDay = dayOfBirth;
                        break;
                    }

                case 'F':
                {
                    addToBirthDay = dayOfBirth + 40;
                    break;
                }

                case 'U':
                {
                    addToBirthDay = dayOfBirth + 75;
                    break;
                }

                default: break;
            }




            throw new NotImplementedException();
        }
    }
}
