using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

namespace PBBox
{

    public static partial class PBExtensions
    {
        public static void SetGenericBinding(this PlayableDirector director, string bindingStreamName, Object value)
        {
            if (director.playableAsset == null)
            {
                Log.Error($"PlayableDirector {director.name} has no playableAsset, please use SetGenericBinding(Object, Object) instead.");
                return;
            }
            Object _sourceObj = director.playableAsset.outputs.FirstOrDefault(x => x.streamName == bindingStreamName).sourceObject;
            if (_sourceObj == null)
            {
                Log.Error($"PlayableDirector {director.name} has no output named {bindingStreamName}");
                return;
            }
            director.SetGenericBinding(_sourceObj, value);
        }

        public static Object GetGenericBinding(this PlayableDirector director, string bindingStreamName)
        {
            if (director.playableAsset == null)
            {
                Log.Error($"PlayableDirector {director.name} has no playableAsset, please use GetGenericBinding(Object) instead.");
                return null;
            }
            Object _sourceObj = director.playableAsset.outputs.FirstOrDefault(x => x.streamName == bindingStreamName).sourceObject;
            if (_sourceObj == null)
            {
                return null;
            }
            return director.GetGenericBinding(_sourceObj);
        }
    }
}