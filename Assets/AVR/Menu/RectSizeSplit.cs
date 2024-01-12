using UnityEngine;
namespace AVR
{
    namespace SDK
    {
        public class RectSizeSplit : MonoBehaviour
        {
            public int collumns = 2;
            public int rows = 2;
            public int index = 0;
            public int spanx = 1;
            public int spany = 1;

            private Vector2 lastParentSize;

            // detect on start or data update

            void Start() => Split();

            void OnValidate() => Split();

            void Update()
            {
                Split();
            }

            public void Split()
            {
                if (collumns <= 0)
                    collumns = 1;
                if (rows <= 0)
                    rows = 1;
                RectTransform rectTransform = GetComponent<RectTransform>();
                RectTransform parentRectTransform = transform.parent?.GetComponent<RectTransform>();
                if (rectTransform == null || parentRectTransform == null)
                    return;
                float parentWidth = parentRectTransform.rect.width;
                float parentHeight = parentRectTransform.rect.height;
                float width = parentWidth / collumns;
                float height = parentHeight / rows;
                rectTransform.sizeDelta = new Vector2(width / rectTransform.localScale.x * spanx, height / rectTransform.localScale.y * spany);
                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.anchoredPosition = new Vector2(index % collumns * width, -index / collumns * height);
            }
        }
    }
}
