/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.03.29
 *@author: PlusBrackets
 --------------------------------------------------------*/
#if UNITY_EDITOR
using UnityEngine;
using PBBox;

namespace PBBox
{
    public static class GizmosUtils
    {
        #region arrow
        static Vector3 arrow1 = new Vector3(.1f, .0f, -.3f);
        static Vector3 arrow2 = new Vector3(-.1f, .0f, -.3f);
        static Vector3 arrow3 = new Vector3(.0f, .1f, -.3f);
        static Vector3 arrow4 = new Vector3(.0f, -.1f, -.3f);

        public static void DrawWireArrow(Vector3 from, Vector3 to)
        {
            Gizmos.DrawLine(from, to);

            Quaternion rot = Quaternion.LookRotation(to - from, Vector3.up);//Quaternion.FromToRotation(Vector3.forward, to - from);
            Vector3 a1 = rot * arrow1 + to;
            Vector3 a2 = rot * arrow2 + to;
            Vector3 a3 = rot * arrow3 + to;
            Vector3 a4 = rot * arrow4 + to;
            Gizmos.DrawLine(to, a1);
            Gizmos.DrawLine(to, a2);
            Gizmos.DrawLine(to, a3);
            Gizmos.DrawLine(to, a4);
            Gizmos.DrawLine(a1, a2);
            Gizmos.DrawLine(a3, a4);
        }
        #endregion

        #region grid
        public static void DrawGrid(Vector3 origin, Vector3 girdCount, Vector3 girdSize, Quaternion rot)
        {
            Vector3 _from;
            Vector3 _to;
            //x
            for (int y = 0; y <= girdCount.y; y++)
            {
                _from = _to = origin;
                _to.x = _to.x + (girdCount.x * girdSize.x);
                _from.y += y * girdSize.y;
                _to.y += y * girdSize.y;
                for (int i = 0; i <= girdCount.z; i++)
                {
                    Gizmos.DrawLine(rot * _from, rot * _to);
                    _from.z += girdSize.z;
                    _to.z += girdSize.z;
                }
            }
            //y
            for (int z = 0; z <= girdCount.z; z++)
            {
                _from = _to = origin;
                _to.y = _to.y + (girdCount.y * girdSize.y);
                _from.z += z * girdSize.z;
                _to.z += z * girdSize.z;
                for (int i = 0; i <= girdCount.x; i++)
                {
                    Gizmos.DrawLine(rot * _from, rot * _to);
                    _from.x += girdSize.x;
                    _to.x += girdSize.x;
                }
            }
            //z
            for (int y = 0; y <= girdCount.y; y++)
            {
                _from = _to = origin;
                _to.z = _to.z + (girdCount.z * girdSize.z);
                _from.y += y * girdSize.y;
                _to.y += y * girdSize.y;
                for (int i = 0; i <= girdCount.x; i++)
                {
                    Gizmos.DrawLine(rot * _from, rot * _to);
                    _from.x += girdSize.x;
                    _to.x += girdSize.x;
                }
            }
            ////z
            //_from = origin;
            //_to = _from;
            //_to.x += (girdCount.x * girdSize.x);
            //for (int i = 0; i <= girdCount.z; i++)
            //{
            //    Gizmos.DrawLine(rot * _from, rot * _to);
            //    _from.z += girdSize.z;
            //    _to.z += girdSize.z;
            //}
            ////y
            //_from = origin;
            //_to = _from;
            //_to.y += (girdCount.y * girdSize.y);
            //for (int i = 0; i <= girdCount.y; i++)
            //{
            //    Gizmos.DrawLine(rot * _from, rot * _to);
            //    _from.y += girdSize.y;
            //    _to.y += girdSize.y;
            //}


            //float w = range.width;
            //float h = range.height;
            //float x = range.x;
            //float y = range.y;
            //Vector3 from = new Vector3(0 + offsetW - UNIT / 2, 0, 0 + offsetH - UNIT / 2);
            //Vector3 to = new Vector3(0 + offsetW - UNIT / 2, 0, UNIT * h + offsetH - UNIT / 2);
            //for (int i = 0; i <= w; i++)
            //{
            //    from.x += UNIT;
            //    to.x += UNIT;
            //    Gizmos.DrawLine(from, to);
            //}
            //from = new Vector3(offsetW - UNIT / 2, 0, offsetH - UNIT / 2);
            //to = new Vector3(UNIT * w + offsetW - UNIT / 2, 0, offsetH - UNIT / 2);
            //for (int i = 0; i <= h; i++)
            //{
            //    from.z += UNIT;
            //    to.z += UNIT;
            //    Gizmos.DrawLine(from, to);
            //}
        }
        #endregion

        public static void DrawRect(Rect rect)
        {
            Vector2 bottomLeft = rect.position;
            Vector2 leftTop = rect.position;
            leftTop.y += rect.size.y;
            Vector2 topRight = rect.position + rect.size;
            Vector2 rightBottom = rect.position;
            rightBottom.x += rect.size.x;
            DrawLines(bottomLeft, leftTop, topRight, rightBottom,bottomLeft);
        }

        public static void DrawRectInXZ(Rect rect)
        {
            Vector2 bottomLeft = rect.position;
            Vector2 leftTop = rect.position;
            leftTop.y += rect.size.y;
            Vector2 topRight = rect.position + rect.size;
            Vector2 rightBottom = rect.position;
            rightBottom.x += rect.size.x;
            DrawLines(CommonUtils.PraseXYToXZ(bottomLeft), CommonUtils.PraseXYToXZ(leftTop), CommonUtils.PraseXYToXZ(topRight), CommonUtils.PraseXYToXZ(rightBottom), CommonUtils.PraseXYToXZ(bottomLeft));
        }

        public static void DrawLines(params Vector3[] positions)
        {
            for(int i= 0; i < positions.Length - 1; i++)
            {
                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }
        }

        public static void DrawCircle(Vector3 center, float radius, int segment = 36)
        {
            Vector3[] pos = new Vector3[segment + 1];
            float delta = 2 * Mathf.PI / segment;
            for (int i = 0; i < segment; i++)
            {
                float x = radius * Mathf.Cos(delta * i);
                float z = radius * Mathf.Sin(delta * i);
                pos[i] = new Vector3(x, 0, z);
            }
            pos[segment] = pos[0];
            DrawLines(pos);
        }
    }

}
#endif