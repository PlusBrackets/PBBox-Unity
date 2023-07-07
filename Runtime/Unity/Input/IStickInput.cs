using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Unity.Input
{
    public interface IStickInput
    {
        event Action<IStickInput> OnInputBegin;
        event Action<IStickInput> OnInputEnd;
        Vector2 InputVector { get; }
        bool IsInputting { get; }
        void Interrupt();
    }
}