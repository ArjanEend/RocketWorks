using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RocketWorks.Networking
{
    public interface ICommand<T>
    {
        void Execute(T target);
    }
}
