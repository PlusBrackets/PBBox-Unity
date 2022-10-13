/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.26
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PBBox.UI
{
    [AddComponentMenu("PBBox/UI/Components/Radar Chart")]
    [RequireComponent(typeof(CanvasRenderer)), DisallowMultipleComponent]
    public class PBRadarChart : MaskableGraphic
    {

        [Header("Properties")]
        [SerializeField, Min(0f), Tooltip("雷达图的半径")]
        float m_FixedRadius = 100f;
        [SerializeField, Tooltip("使用RectTransform的宽高作为Radius")]
        bool m_UseRectSizeAsRadius = false;
        float m_RectAsRadius = 0f;
        [SerializeField]
        Color m_VertexColor = Color.white;
        [SerializeField, Range(0f, 1f)]
        float m_VertexColorBlendWeight = 0f;
        [SerializeField]
        bool m_DrawOutline = false;
        [SerializeField]
        bool m_DrawOutlineInner = false;
        [SerializeField]
        float m_OutlineWidth = 3f;
        [SerializeField]
        Color m_OutlineColor = Color.black;

        [SerializeField, Range(0f, 1f), Tooltip("基础Ratios")]
        float m_RatioOffset = 0.1f;
        [SerializeField, Range(0f, 1f), Tooltip("各个顶点的相对于Radius的比例")]
        List<float> m_Ratios = new List<float> { 0.5f, 0.5f, 0.5f, 0.5f };

        /// <summary>
        /// 各个顶点相对于radius的比例
        /// 若直接更改，请在更改完成后调用SetSetVerticesDirty()来更新图像
        /// </summary>
        public List<float> ratios => m_Ratios;

        public float ratioOffset
        {
            get => m_RatioOffset;
            set
            {
                if (m_RatioOffset != value)
                {
                    m_RatioOffset = value;
                    SetVerticesDirty();
                }
            }
        }
        /// <summary>
        /// 设置radius时会设置FixedRadius，当useRectSizeAsRadius==false时才会反映在图像上。
        /// 当useRectSizeAsRadius == true 时可以设置rectTransform的size来改变radius
        /// </summary>
        /// <value></value>
        public float radius
        {
            get
            {
                return Mathf.Max(m_UseRectSizeAsRadius ? m_RectAsRadius : m_FixedRadius, 0);
            }
            set
            {
                if (m_FixedRadius != value)
                {
                    m_FixedRadius = Mathf.Max(value, 0);
                    if (m_UseRectSizeAsRadius)
                    {
                        SetVerticesDirty();
                    }
                }
            }
        }
        /// <summary>
        /// 是否使用RectTransform的size作为radius
        /// </summary>
        /// <value></value>
        public bool useRectSizeAsRadius
        {
            get => m_UseRectSizeAsRadius;
            set
            {
                if (m_UseRectSizeAsRadius != value)
                {
                    m_UseRectSizeAsRadius = value;
                    SetVerticesDirty();
                }
            }
        }

        public Color vertexColor
        {
            get => m_VertexColor;
            set
            {
                if (m_VertexColor != value)
                {
                    m_VertexColor = value;
                    SetVerticesDirty();
                }
            }
        }

        public float vertexColorBlendWeight
        {
            get => m_VertexColorBlendWeight;
            set
            {
                if (m_VertexColorBlendWeight != value)
                {
                    m_VertexColorBlendWeight = value;
                    SetVerticesDirty();
                }
            }
        }

        public bool drawOutline
        {
            get => m_DrawOutline;
            set
            {
                if (m_DrawOutline != value)
                {
                    m_DrawOutline = value;
                    SetVerticesDirty();
                }
            }
        }

        public bool drawOutlineInner
        {
            get => m_DrawOutlineInner;
            set
            {
                if (m_DrawOutlineInner != value)
                {
                    m_DrawOutlineInner = value;
                    SetVerticesDirty();
                }
            }
        }

        public float outlineWidth
        {
            get => m_OutlineWidth;
            set
            {
                if (m_OutlineWidth != value)
                {
                    m_OutlineWidth = value;
                    SetVerticesDirty();
                }
            }
        }

        public Color outlineColor
        {
            get => m_OutlineColor;
            set
            {
                if (m_OutlineColor != value)
                {
                    m_OutlineColor = value;
                    SetVerticesDirty();
                }
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (m_RectAsRadius == 0f)
            {
                m_RectAsRadius = (rectTransform.rect.width > rectTransform.rect.height
                            ? rectTransform.rect.height
                            : rectTransform.rect.width) / 2f;
            }
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            m_RectAsRadius = (rectTransform.rect.width > rectTransform.rect.height
                        ? rectTransform.rect.height
                        : rectTransform.rect.width) / 2f;
            SetVerticesDirty();
        }

        /// <summary>
        /// 设置比例，
        /// </summary>
        /// <param name="values"></param>
        public void SetRatios(IEnumerable<float> values)
        {
            m_Ratios.Clear();
            m_Ratios.AddRange(values);
            SetVerticesDirty();
        }

        public void SetRatios(int index, float value)
        {
            if (index >= ratios.Count)
                return;
            ratios[index] = Mathf.Max(0f, value);
            SetVerticesDirty();
        }

        public void SetRatiosSame(int count, float value)
        {
            m_Ratios.Clear();
            for (int i = 0; i < count; i++)
            {
                m_Ratios.Add(value);
            }
            SetVerticesDirty();
        }

        /// <summary>
        /// 获取雷达图相对于自身坐标的中心位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRadarCenter()
        {
            var diameter = 2 * radius;
            Vector3 posCenter = new Vector3(
                (0.5f - rectTransform.pivot.x) * diameter,
                (0.5f - rectTransform.pivot.y) * diameter
            );
            return posCenter;
        }

        /// <summary>
        /// 获取雷达图相对于RadarCenter的顶点位置
        /// </summary>
        /// <param name="i"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool TryGetRadarVertexPos(int i, out Vector3 pos)
        {
            pos = Vector3.zero;

            if (i < m_Ratios.Count)
            {
                float rad = 2f * Mathf.PI / m_Ratios.Count * i;
                pos += new Vector3(Mathf.Sin(rad), Mathf.Cos(rad)) * radius * m_Ratios[i];
            }
            return false;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            m_RectAsRadius = (rectTransform.rect.width > rectTransform.rect.height
                            ? rectTransform.rect.height
                            : rectTransform.rect.width) / 2f;

            base.OnRectTransformDimensionsChange();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            GenRadarVertex(vh);
            if (m_DrawOutline && m_OutlineWidth >= 0.00001f)
            {
                GenRadarOutline(vh);
            }
        }

        UIVertex GetUIVertex(Vector3 pos, Color color,float uvx = 0, float uvy = 0)
        {
            UIVertex vertex = new UIVertex();
            vertex.position = pos;
            vertex.color = color;
            vertex.uv0 = new Vector4(uvx, uvy);
            return vertex;
        }

        void GenRadarVertex(VertexHelper vh)
        {
            //计算中心
            var posCenter = GetRadarCenter();
            vh.AddVert(GetUIVertex(posCenter, color,0.5f,0.5f));

            //计算各顶点,顺序为从最上方，按顺时针方向
            int splitCount = m_Ratios.Count;
            float deltaRad = 2 * Mathf.PI / splitCount;
            float curRad = 0f;
            var radius = this.radius;
            Color _vcolor = Color.Lerp(color, m_VertexColor, m_VertexColorBlendWeight);
            for (int i = 0; i < splitCount; i++)
            {
                float r = (ratioOffset + (m_Ratios[i] * (1 - ratioOffset))) * radius;
                Vector3 posOffset = r * new Vector3(Mathf.Sin(curRad), Mathf.Cos(curRad));
                vh.AddVert(GetUIVertex(posCenter + posOffset, _vcolor,1f,0.5f));
                curRad += deltaRad;
            }

            //设置三角型
            for (int i = 0; i < splitCount; i++)
            {
                vh.AddTriangle(0, i + 1, i + 2 > splitCount ? 1 : i + 2);
            }
        }

        Lazy<List<Vector2>> m_OuterVertexPosCaches1 = new Lazy<List<Vector2>>();
        Lazy<List<Vector2>> m_OuterVertexPosCaches2 = new Lazy<List<Vector2>>();
        static Lazy<UIVertex[]> m_VertexQuadArray = new Lazy<UIVertex[]>(() => { return new UIVertex[4]; });


        void GenRadarOutline(VertexHelper vh)
        {
            UIVertex[] vertexQuad = m_VertexQuadArray.Value;
            List<Vector2> outerPos = m_OuterVertexPosCaches1.Value;
            List<Vector2> outerVertexPos = m_OuterVertexPosCaches2.Value;
            outerPos.Clear();
            outerVertexPos.Clear();

            int vertCount = vh.currentVertCount - 1;
            for (int i = 1; i <= vertCount; i++)
            {
                UIVertex a = new UIVertex();
                UIVertex b = new UIVertex();
                vh.PopulateUIVertex(ref a, i);
                vh.PopulateUIVertex(ref b, i + 1 > vertCount ? 1 : i + 1);

                Vector3 dirOffset = m_OutlineWidth * (Vector2.Perpendicular((m_DrawOutlineInner ? -1 : 1) * (b.position - a.position)).normalized);

                outerPos.Add(a.position + dirOffset);
                outerPos.Add(b.position + dirOffset);
            }
            for (int i = 0; i < outerPos.Count; i += 2)
            {
                Vector2 ps1 = outerPos[i];
                Vector2 pe1 = outerPos[i + 1];
                Vector2 ps2 = outerPos[i - 2 < 0 ? outerPos.Count - 2 : i - 2];
                Vector2 pe2 = outerPos[i - 1 < 0 ? outerPos.Count - 1 : i - 1];
                Vector2 newVertexPos;
                if (!PBMath.LineIntersection(ps1, pe1, ps2, pe2, out newVertexPos))
                {
                    newVertexPos = ps1;
                }
                outerVertexPos.Add(newVertexPos);
            }
            for (int i = 0; i < outerVertexPos.Count; i++)
            {
                int next_i = i + 1 >= outerVertexPos.Count ? 0 : i + 1;

                UIVertex a = new UIVertex();
                UIVertex b = new UIVertex();
                vh.PopulateUIVertex(ref a, i + 1);
                vh.PopulateUIVertex(ref b, next_i + 1);
                a.color = b.color = m_OutlineColor;

                vertexQuad[0] = a;
                vertexQuad[1] = GetUIVertex(outerVertexPos[i], m_OutlineColor);
                vertexQuad[2] = GetUIVertex(outerVertexPos[next_i], m_OutlineColor);
                vertexQuad[3] = b;
                vh.AddUIVertexQuad(vertexQuad);
            }
        }

    }

}
