using System.Net.Mime;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PBBox;

namespace PBBox.UI
{
    [AddComponentMenu("PBBox/UI/Components/Lerp Num Text")]
    /// <summary>
    /// 缓动数字
    /// </summary>
    public class LerpNumText : MonoBehaviour
    {
        [Space()]
        [Header("LerpSetting")]
        [Space()]
        public float lerpTime = 0.5f;
        public AnimationCurve lerpCurve;
        [SerializeField]
        private bool m_UseUnscaleTime = false;
        public bool useUnscaleTime
        {
            get
            {
                return timer != null ? timer.useUnscaleTime : m_UseUnscaleTime;
            }
            set
            {
                if (timer != null)
                    timer.useUnscaleTime = value;
                m_UseUnscaleTime = value;
            }
        }
        private GameDualityTimer timer;

        public string prefix;
        public string suffix;
        public bool intNumber = true;
        public string numFormat;
        public bool isLerping => timer == null ? false : !timer.isStoped;
        private Coroutine m_ShowingAnim;

        private Text m_Text;

        public string text
        {
            get => m_Text.text;
            set
            {
                OnTextChanged(value);
            }
        }

        private float m_Num = 0;

        public float num
        {
            get => m_Num;
            set
            {
                OnNumChanged(value);
            }
        }
        private float showingNum;
        private float fromNum;

        private void Awake()
        {
            m_Text = GetComponent<Text>();
        }

        private void OnDisable()
        {
            if (m_ShowingAnim != null)
            {
                StopCoroutine(m_ShowingAnim);
                m_ShowingAnim = null;
            }
            if (isLerping)
            {
                SetNumber(m_Num);
            }
        }

        private void OnTextChanged(string text)
        {
            float num;
            if (float.TryParse(text, out num))
            {
                this.num = num;
            }
        }

        private void OnNumChanged(float value)
        {
            if (!enabled || !gameObject.activeInHierarchy)
            {
                SetNumber(value);
                return;
            }
            if (!isLerping)
            {
                fromNum = m_Num;
                m_Num = value;
                if (timer == null)
                    timer = new GameDualityTimer(useUnscaleTime);
                m_ShowingAnim = StartCoroutine(WaitForLerpNum());
            }
            else
            {
                fromNum = showingNum;
                m_Num = value;
                timer.Start(lerpTime);
            }
        }

        private IEnumerator WaitForLerpNum()
        {
            timer.Start(lerpTime);
            while (timer.state == GameTimer.State.Started)
            {
                float t = lerpCurve.Evaluate(timer.progress);
                showingNum = Mathf.Lerp(fromNum, num, t);
                m_Text.text = CombineShowText(showingNum);
                if (timer.IsOver())
                {
                    timer.Stop();
                    break;
                }
                yield return null;
            }
            m_ShowingAnim = null;
        }

        private string CombineShowText(float showNum)
        {
            return this.prefix + (intNumber ? ((int)showNum).ToString() : showNum.ToString(numFormat)) + this.suffix;
        }

        /// <summary>
        /// 立刻设置
        /// </summary>
        /// <param name="number"></param>
        public void SetNumber(float number)
        {
            m_Num = number;
            if (isLerping)
            {
                timer.Stop();
            }
            m_Text.text = CombineShowText(number);
        }

    }
}