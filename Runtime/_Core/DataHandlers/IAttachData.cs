using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBBox
{

    public interface IAttachDatas
    {
        public Dictionary<string, object> AttachDatas { get; }
    }

}