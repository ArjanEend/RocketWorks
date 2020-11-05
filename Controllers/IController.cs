using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    void Init();
    void DeInit();
}

public interface IController<T> : IController
{
    void Init(T data);
}
