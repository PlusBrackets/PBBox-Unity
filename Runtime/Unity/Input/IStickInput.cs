using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox.Unity.Input
{
    public interface IStickInput
    {
        Vector2 InputVector { get; }
        bool IsInputting { get; }
        void Interrupt();
    }
}