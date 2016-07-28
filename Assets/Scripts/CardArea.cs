using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardArea : MonoBehaviour
{
    private const float FADE_OUT_TIME = 0.5f;

    /// /// <summary>
    /// Use this to access all the selected cards. Use if to check for combos.
    /// </summary>
    public List<Card> Cards
    {
        get
        {
            var _cards = new List<Card>();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetChild(0).GetComponent<Card>() != null)
                {
                    _cards.Add(transform.GetChild(i).GetChild(0).GetComponent<Card>());
                }
            }

            return _cards;
        }
    }

    public virtual void RefreshCards(GameObject cardToBeDestroyed = null)
    {
        if (transform.childCount > 0)
        {
            var cards = Cards;

            Vector3[] oldCardPositions = new Vector3[cards.Count];

            for (int i = 0; i < cards.Count; i++)
            {
                oldCardPositions[i] = cards[i].transform.position;
            }

            GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
            GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).position = oldCardPositions[i];
            }
        }
    }

    public void ClearArea()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(0).GetComponent<Image>().CrossFadeAlpha(0, FADE_OUT_TIME, false);
            transform.GetChild(i).GetComponent<LayoutElement>().ignoreLayout = true;
            StartCoroutine(DestroyTimer(transform.GetChild(i).gameObject));
        }
        
        RefreshCards();

        //var cardsToBeDestroyed = new GameObject("ToBeDestroyed", typeof(RectTransform));
        //cardsToBeDestroyed.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>().root);

        //var childrenTobedestroyed = new List<RectTransform>();

        //foreach (RectTransform child in GetComponent<RectTransform>())
        //{
        //    childrenTobedestroyed.Add(child);
        //    if (child == GetComponent<RectTransform>())
        //    {
        //        Debug.Log("blsdlfhal");
        //        continue;
        //    }
        //    child.GetChild(0).GetComponent<Shadow>().enabled = false;
        //    child.GetChild(0).GetComponent<Image>().CrossFadeAlpha(0, FADE_OUT_TIME, false);
        //    child.SetParent(cardsToBeDestroyed.GetComponent<RectTransform>(), true);
        //    Debug.Log("Setting " + child.name + "to cbe destroyed");
        //}

        //Debug.Log(childrenTobedestroyed.Count + "or " + transform.childCount);

        //Destroy(cardsToBeDestroyed, FADE_OUT_TIME);
    }

    private IEnumerator DestroyTimer(GameObject obj)
    {
        yield return null;
        obj.GetComponent<RectTransform>().SetParent(transform.parent, true);
        obj.transform.SetAsFirstSibling();
        yield return new WaitForSeconds(FADE_OUT_TIME);
        DestroyImmediate(obj);
        RefreshCards();
    }
}

public static class ExtensionMethods
{
    public static int ChildCountActive(this Transform t)
    {
        int k = 0;
        foreach (Transform c in t)
        {
            if (!c.GetComponent<LayoutElement>().ignoreLayout)
                k++;
        }
        return k;
    }

    static public float GetDistPointToLine(this Transform t, Vector3 direction, Vector3 point)
    {
        Vector3 point2origin = t.position - point;
        Vector3 point2closestPointOnLine = point2origin - Vector3.Dot(point2origin, direction) * direction;
        return point2closestPointOnLine.magnitude;
    }
}