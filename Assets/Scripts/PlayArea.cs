using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayArea : MonoBehaviour {

    public GameObject CardObject;
    public bool isDirty { get { return transform.childCount != _oldChildCount; } }
    private int _oldChildCount = 0;

    public List<Card> Cards
    {
        get
        {
            if (isDirty)
            {
                _cards = new List<Card>();

                for (int i = 0; i < transform.childCount; i++)
                {
                    _cards.Add(transform.GetChild(i).GetComponent<Card>());
                }

                _oldChildCount = transform.childCount;

                return _cards;
            }
            else
            {
                return _cards;
            }
        }
    }
    private List<Card> _cards = new List<Card>();

	public void Sort()
    {
        if (!isDirty) return;

        Cards.Sort(
            delegate (Card p1, Card p2)
            {
                return p1.Rank.CompareTo(p2.Rank);
            }
        );

        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i].transform.SetSiblingIndex(i);
            Cards[i].Position = new Vector2(i * 50, 0); 
        }
    }
}
