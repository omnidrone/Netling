using System;
namespace Netling.Core
{
    public interface IRequestSequence
    {
        Request Next();
    }
}
