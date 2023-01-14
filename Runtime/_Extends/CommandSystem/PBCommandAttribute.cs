/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.10.14
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PBBox
{
    /// <summary>
    /// PB指令引导class，设置在静态方法所属的class上
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PBCommandClassAttribute : Attribute
    {
        
    }

    /// <summary>
    /// PB指令,仅对静态方法有效，参数现支持int,float,bool和string，执行样例:PBCommands.Excute("cmdName:param1,param2,param3",extraParam);
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PBCommandAttribute : Attribute
    {
        public string cmdName { get; private set; }
        public bool keepFullParamStr { get; set; } = false;
        public bool passExtraParam { get; set; } = false;

        /// <summary>
        /// PB指令,仅对静态方法有效，参数现支持int,float,bool和string，执行样例:PBCommands.Excute("cmdName:param1,param2,param3",extraParam);
        /// </summary>
        /// <param name="cmdName">指令名</param>
        /// <param name="keepFullParamStr">是否直接传入完整的参数字符串，用于自定义解析</param>
        /// <param name="passExtraParam">是否传入额外的参数，需要在方法最后定义一个额外的object参数</param>
        public PBCommandAttribute(string cmdName, bool keepFullParamStr = false, bool passExtraParam = false)
        {
            this.cmdName = cmdName;
            this.keepFullParamStr = keepFullParamStr;
            this.passExtraParam = passExtraParam;
        }
    }
}