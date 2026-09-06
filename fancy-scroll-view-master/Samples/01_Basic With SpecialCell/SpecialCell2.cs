using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example01Special
{
    public class SpecialCell2 : MonoBehaviour
    {
        private Text m_TMPText;

        private Coroutine m_Coroutine;

        private void Awake()
        {
            m_TMPText = GetComponent<Text>();
        }

        private void OnEnable()
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }
            m_Coroutine = StartCoroutine(TimeCounter());
        }

        private void OnDisable()
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }
        }

        private IEnumerator TimeCounter()
        {
            var end = 99;
            var time = 5f;
            var timer = 0f;

            var add = true;
            while (true)
            {
                if (timer < time && add)
                {
                    timer += Time.deltaTime;
                    if (timer>=time)
                    {
                        add = false;
                    }
                }

                if (timer > 0 && !add)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        add = true;
                    }
                }
                
                var offset = (int)((timer / time) * end);
                m_TMPText.text = offset.ToString();

                yield return null;
            }
        }
    }
}