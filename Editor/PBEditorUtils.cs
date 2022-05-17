/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections.Generic;

namespace PBBox.CEditor
{
    public static class PBEditorUtils
    {
        public static Dictionary<string, object> KeyCache = new Dictionary<string, object>();  

        public static bool GetBool(string key, bool defaultValue = false)
        {
            object value = defaultValue;
            KeyCache.TryGetValue(key, out value);
            return System.Convert.ToBoolean(value);
        }

        public static void SetBool(string key, bool value)
        {
            CommonUtils.AddToDictionary(KeyCache, key, value);
        }
    }
}