using RocketWorks.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityVisualizer
{
    void Init(Entity entity);
    void DeInit(Entity entity);
}
