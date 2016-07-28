using UnityEngine;
using UnityEngine.UI;

public class PlayArea : CardArea {

    public override void RefreshCards(GameObject cardToBeDestroyed = null)
    {
        if (transform.childCount > 0)
        {
            var cards = Cards;

            Vector3[] oldCardPositions = new Vector3[cards.Count];

            for (int i = 0; i < cards.Count; i++)
            {
                oldCardPositions[i] = cards[i].transform.position;
            }

            if (transform.ChildCountActive() < 7)
            {
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 50f + transform.childCount * 100f);
            }
            else
            {
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 650f);
            }

            GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
            GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).position = oldCardPositions[i];
            }
        }
    }
}
