using TMPro;
using UnityEngine;

namespace FancyScrollView.Example01WithUnfixedSize
{
    public class ExampleOneUnfixedSizeSubCell : MonoBehaviour
    {
        protected TMP_Text tmpText { get; set; }

        public virtual void Init()
        {
            if (tmpText == null)
            {
                tmpText = gameObject.GetComponentInChildren<TMP_Text>();
            }
        }

        public virtual void SetValue(int value)
        {
            tmpText.text = value.ToString();
        }

        public virtual void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}