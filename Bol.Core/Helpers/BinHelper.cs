using System;
using System.Collections.Generic;
using System.Text;
using Bol.Core.Abstractions.Helpers;
using Bol.Core.Model;

namespace Bol.Core.Helpers
{
    class BinHelper : IBinHelper
    {
        private readonly IBinConstructionHelper _binConstructionHelper;

        public BinHelper(IBinConstructionHelper binConstructionHelper)
        {
            _binConstructionHelper = binConstructionHelper ?? throw new ArgumentNullException(nameof(binConstructionHelper));
        }

        public string Generate(NaturalPerson person)
        {
            throw new NotImplementedException();
        }
    }
}
