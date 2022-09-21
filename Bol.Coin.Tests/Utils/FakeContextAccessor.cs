using Bol.Core.Abstractions;
using Bol.Core.Model;

namespace Bol.Coin.Tests.Utils;

public class FakeContextAccessor : IContextAccessor
{
    private readonly BolContext _context;

    public FakeContextAccessor(BolContext context)
    {
        _context = context;
    }

    public BolContext GetContext()
    {
        return _context;
    }
}
