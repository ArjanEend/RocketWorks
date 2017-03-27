using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RocketWorks.Networking
{
    public interface IPingResolver<T>
    {
        void Resolve(T item, float ping);
    }
}
