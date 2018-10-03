using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlayerHand : CardArea
{  
    /// <summary>
    /// Use this to access all the cards the player is holding.
    /// </summary>
    public List<Card> SelectedCards
    {
        get
        {
            var selectedCards = new List<Card>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var card = transform.GetChild(i).GetChild(0).GetComponent<Card>();

                if (!card.Selected) continue;

                selectedCards.Add(card);
            }

            return selectedCards;
        }
    }

    public RectTransform PlayArea;
    public readonly float MAX_HAND_CARDS = 9;
    
    private bool _ascending;

    public virtual void PlayCards ()
    {
        if (SelectedCards.Count > 0)
        {
            PlayArea.GetComponent<PlayArea>().ClearArea();

            StartCoroutine(IPlayCards());

            PlayArea.GetComponent<PlayArea>().RefreshCards();
        }
    }

    private IEnumerator IPlayCards()
    {
        var selectedCards = SelectedCards;

        for (int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].MoveToArea(PlayArea);
            selectedCards[i].transform.RotateAround(selectedCards[i].transform.position - Vector3.up*10, -Vector3.forward, 3*(i-(selectedCards.Count/2)));
            yield return null;
            RefreshCards();
        }

        yield return null;

        RefreshCards();
    }

    public virtual void DrawCard()
    {
        Deck.Instance().DrawCard(this);
    }

    public void ToggleSort()
    {
        _ascending = !_ascending;
        Sort(_ascending);
    }

    public void Sort(bool ascending)
    {
        var cards = Cards;

        cards.Sort(
            delegate (Card p1, Card p2)
            {
                return ascending ? p1.Rank.CompareTo(p2.Rank) : p2.Rank.CompareTo(p1.Rank);
            }
        );

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.parent.SetSiblingIndex(i);
        }

        RefreshCards();
    }

    public void MoveBesideOther(Card one, Card two)
    {
        int startIndex = one.transform.parent.GetSiblingIndex();
        int targetIndex = two.transform.parent.GetSiblingIndex();

        one.transform.parent.SetSiblingIndex(targetIndex + (startIndex > targetIndex ? 1 : 0));

        RefreshCards();
    }

    public void SwapCards(Card one, Card two)
    {
        int targetIndex = two.transform.parent.GetSiblingIndex();
        
        two.gameObject.transform.parent.SetSiblingIndex(one.transform.parent.GetSiblingIndex());
        one.transform.parent.SetSiblingIndex(targetIndex);

        RefreshCards();
    }

    public override void RefreshCards(GameObject cardToBeDestroyed = null)
    {
        if (transform.childCount > 0)
        {
            var cards = Cards;
            int indexToSkip = -1;
            
            Vector3[] oldCardPositions = new Vector3[cards.Count];
            if (cardToBeDestroyed != null)
            {
                indexToSkip = cardToBeDestroyed.transform.GetSiblingIndex();
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (i == indexToSkip)
                {
                    continue;
                }
                oldCardPositions[i] = cards[i].transform.position;
            }

            if (transform.childCount < 6)
            {
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f + transform.childCount * 150f);
            }
            else
            {
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000f);
            }

            GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
            GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (i == indexToSkip)
                {
                    continue;
                }
                transform.GetChild(i).GetChild(0).position = oldCardPositions[i];
            }
        }
    }

    public void ReplaceOther(Card one, Card two)
    {
        StartCoroutine(IReplaceOther(one, two));
    }

    private IEnumerator IReplaceOther(Card one, Card two)
    {
        int targetIndex = two.transform.parent.GetSiblingIndex();
              
        one.transform.parent.SetSiblingIndex(targetIndex);

        RefreshCards(two.transform.parent.gameObject);
        DestroyImmediate(two.transform.parent.gameObject);
        yield return true;
    }
}