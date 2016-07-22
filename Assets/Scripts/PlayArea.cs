using UnityEngine;
using System.Collections.Generic;

public class PlayArea : MonoBehaviour {

    public GameObject CardObject;
    public bool isDirty { get { return transform.childCount != _oldChildCount; } }
    private int _oldChildCount = 0;
    private List<Card> _cards = new List<Card>();
    private bool ascending = false;

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

    public void ToggleSort()
    {
        ascending = !ascending;
        Sort(ascending);
    }

	public void Sort(bool ascending)
    {
        Cards.Sort(
            delegate (Card p1, Card p2)
            {
                return ascending ? p1.Rank.CompareTo(p2.Rank) : p2.Rank.CompareTo(p1.Rank);
            }
        );

        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i].transform.SetSiblingIndex(i);
        }
    }
}
