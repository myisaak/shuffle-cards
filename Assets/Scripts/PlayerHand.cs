using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class PlayerHand : MonoBehaviour
{
    public Transform PlayArea;

    public readonly float MAX_HAND_CARDS = 9;
    private const float THROW_RANDOM_THRESHOLD = 25;

    private void Awake ()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Card>().Active = false;
        }
    }

    private void Start()
    {   
        RefreshHandCards();
    }

	public void RefreshHandCards()
    {
        for (int i = 0; i < transform.childCount ; i++)
        {
            int j = i;
            var card = transform.GetChild(j);

            if (card.GetComponent<Button>() != null)
            {
                card.GetComponent<Button>().onClick.RemoveAllListeners();
                card.GetComponent<Button>().onClick.AddListener(() => { card.GetComponent<Card>().ToggleSelection(); });
            }
        }

        //GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    public bool DropCard(ref Card deckCard)
    {
        GetComponent<HorizontalLayoutGroup>().enabled = false;
        bool canDrop = false;
        string value = deckCard.Value;

        for (int i = 0; i < transform.childCount; i++)
        {
            int j = i;
            var card = transform.GetChild(j);
            if (card.name == "Empty")
            {    
                deckCard.transform.SetParent(transform, true);
                deckCard.Position = card.GetComponent<Card>().Position;
                deckCard.transform.SetSiblingIndex(card.GetSiblingIndex());
                deckCard.GetComponent<Card>().State = Card.FlipState.Shown;
                Destroy(card.gameObject);
                canDrop = true;
                break;
            }
        }

        RefreshHandCards();

        return canDrop;
    }

    public void PlayCards()
    {
        GetComponent<HorizontalLayoutGroup>().enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            var card = transform.GetChild(i);
            if (!card.GetComponent<Card>().Selected) continue;

            var newCard = Instantiate(card.gameObject);
            newCard.transform.SetParent(PlayArea, false);
            newCard.transform.localPosition = PlayArea.InverseTransformPoint(card.position);
            newCard.GetComponent<Card>().Value = card.GetComponent<Card>().Value;
            newCard.GetComponent<Card>().Position = Random.insideUnitCircle * THROW_RANDOM_THRESHOLD;
            newCard.GetComponent<Button>().onClick.RemoveAllListeners();

            card.GetComponent<Card>().Selected = false;
            card.GetComponent<Card>().Active = false;
        }

        RefreshHandCards();
    }
}