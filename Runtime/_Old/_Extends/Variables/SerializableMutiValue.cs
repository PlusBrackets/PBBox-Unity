// /**
//  * 
//  * @plusBrackets
//  * create 2019.11.20
//  * update 2019.11.23
//  * 
//  **/

// using System;
// using UnityEngine;

// namespace PBBox.Properties
// {

//     [Serializable]
//     public abstract class SerializableMutiValue<T>
//     {
//         protected T[] values;

//         protected T GetValue(int index)
//         {
//             return index < values.Length ? values[index] : default(T);
//         }
//     }

//     /// <summary>
//     /// 可序列化的多个Float合集，用来代替vector3等不能序列化的unity内部变量类型
//     /// </summary>
//     [Serializable]
//     public class SMutiFloat : SerializableMutiValue<float>
//     {
//         public SMutiFloat(Vector2 v) { values = new float[2] { v.x, v.y }; }

//         public SMutiFloat(Vector3 v) { values = new float[3] { v.x, v.y, v.z }; }

//         public SMutiFloat(Vector4 v) { values = new float[4] { v.x, v.y, v.z, v.w }; }

//         public SMutiFloat(Quaternion qua) { values = new float[4] { qua.x, qua.y, qua.z, qua.w }; }

//         public SMutiFloat(Rect rect) { values = new float[4] { rect.x, rect.y, rect.width, rect.height }; }

//         public SMutiFloat(Color color) { values = new float[4] { color.r, color.g, color.b, color.a }; }

//         public static implicit operator Vector2(SMutiFloat value)
//         {
//             return value==null?default: new Vector2(value.GetValue(0), value.GetValue(1));
//         }

//         public static implicit operator Vector3(SMutiFloat value)
//         {
//             return value == null ? default : new Vector3(value.GetValue(0), value.GetValue(1), value.GetValue(2));
//         }

//         public static implicit operator Vector4(SMutiFloat value)
//         {
//             return value == null ? default : new Vector4(value.GetValue(0), value.GetValue(1), value.GetValue(2), value.GetValue(3));
//         }

//         public static implicit operator Quaternion(SMutiFloat value)
//         {
//             return value == null ? default : new Quaternion(value.GetValue(0), value.GetValue(1), value.GetValue(2), value.GetValue(3));
//         }

//         public static implicit operator Rect(SMutiFloat value)
//         {
//             return value == null ? default : new Rect(value.GetValue(0), value.GetValue(1), value.GetValue(2), value.GetValue(3));
//         }

//         public static implicit operator Color(SMutiFloat value)
//         {
//             return value == null ? default : new Color(value.GetValue(0), value.GetValue(1), value.GetValue(2), value.GetValue(3));
//         }

//         public static implicit operator SMutiFloat(Vector2 value)
//         {
//             return new SMutiFloat(value);
//         }

//         public static implicit operator SMutiFloat(Vector3 value)
//         {
//             return new SMutiFloat(value);
//         }

//         public static implicit operator SMutiFloat(Vector4 value)
//         {
//             return new SMutiFloat(value);
//         }

//         public static implicit operator SMutiFloat(Quaternion value)
//         {
//             return new SMutiFloat(value);
//         }

//         public static implicit operator SMutiFloat(Rect value)
//         {
//             return new SMutiFloat(value);
//         }

//         public static implicit operator SMutiFloat(Color value)
//         {
//             return new SMutiFloat(value);
//         }
//     }

//     /// <summary>
//     /// 可序列化的多个Int合集，用来代替VectorInt等不能序列化的unity内部变量类型
//     /// </summary>
//     [Serializable]
//     public class SMutiInt : SerializableMutiValue<int>
//     {
//         public SMutiInt(Vector2Int v) { values = new int[2] { v.x, v.y }; }

//         public SMutiInt(Vector3Int v) { values = new int[3] { v.x, v.y, v.z }; }

//         public SMutiInt(RectInt rect) { values = new int[4] { rect.x, rect.y, rect.width, rect.height }; }

//         public static implicit operator Vector2Int(SMutiInt value)
//         {
//             return value == null ? default : new Vector2Int(value.GetValue(0), value.GetValue(1));
//         }

//         public static implicit operator Vector3Int(SMutiInt value)
//         {
//             return value == null ? default : new Vector3Int(value.GetValue(0), value.GetValue(1), value.GetValue(2));
//         }

//         public static implicit operator RectInt(SMutiInt value)
//         {
//             return value == null ? default : new RectInt(value.GetValue(0), value.GetValue(1), value.GetValue(2), value.GetValue(3));
//         }

//         public static implicit operator SMutiInt(Vector2Int value)
//         {
//             return new SMutiInt(value);
//         }

//         public static implicit operator SMutiInt(Vector3Int value)
//         {
//             return new SMutiInt(value);
//         }

//         public static implicit operator SMutiInt(RectInt value)
//         {
//             return new SMutiInt(value);
//         }
//     }
// }