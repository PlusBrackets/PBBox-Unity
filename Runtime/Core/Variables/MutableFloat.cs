/*--------------------------------------------------------
 *Copyright (c) 2016-2023 PlusBrackets
 *@update: 2023.03.09
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.Variables
{
    /// <summary>
    /// 可变值类, Value = Constant || ∏(Multiply) * ( base + ∑(Flat) + base * ∑(Percent) )
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public sealed partial class MutableFloat : MutableValue<float>
    {
        protected override float AddValue(float a, float b)
        {
            return a + b;
        }

        protected override float MulValue(float a, float b)
        {
            return a * b;
        }
    }
}