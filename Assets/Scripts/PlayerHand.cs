using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class PlayerHand : MonoBehaviour
{
    public Transform PlayArea;
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

    public readonly float MAX_HAND_CARDS = 9;
    private const float THROW_RANDOM_THRESHOLD = 25;

    public void DropCard(ref Card deckCard)
    {
        deckCard.transform.SetParent(transform, true);
        deckCard.transform.SetAsLastSibling();
        deckCard.GetComponent<Card>().State = Card.FlipState.Shown;
    }

    public void PlayCards ()
    {
        StartCoroutine(IPlayCards());
    }

    private IEnumerator IPlayCards()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var card = transform.GetChild(i);
            if (!card.GetComponent<Card>().Selected) continue;

            var newCard = Instantiate(card.gameObject);
            newCard.transform.SetParent(PlayArea, true);
            newCard.GetComponent<Card>().Value = card.GetComponent<Card>().Value;
            newCard.GetComponent<Button>().onClick.RemoveAllListeners();

            card.GetComponent<Card>().Selected = false;
            yield return true;
        }
    }
}