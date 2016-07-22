using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public GameObject CardObject;
    public PlayerHand Hand;

    public const int MAX_CARDS_DECK = 48;

    public List<int> cardIndexList = new List<int>(MAX_CARDS_DECK);

    private void Awake ()
    {
        for (int i = 0; i < MAX_CARDS_DECK; i++)
        {
            cardIndexList.Add(i % 13);
        }

        Shuffle();
    }

    public void Shuffle()
    {
        cardIndexList.Shuffle();

        StartCoroutine(AddCardsToDeck());
    }

    private IEnumerator AddCardsToDeck ()
    {
        ClearDeck();

        for (int i = 0; i < cardIndexList.Count; i++)
        {
            var card = Instantiate(CardObject);
            card.transform.SetParent(transform, false);
            card.GetComponent<Card>().Rank = cardIndexList[i];
            card.GetComponent<Card>().State = Card.FlipState.Hidden;
            yield return true;
        }
    }

    public void DrawCard ()
    {
        if (transform.childCount == 0) return;

        var cardIndex = transform.childCount-1;
        var card = transform.GetChild(cardIndex).GetComponent<Card>();

        Hand.DropCard(ref card);
    
        cardIndexList.RemoveAt(cardIndex);
    }

    public void ClearDeck()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

public static class ThreadSafeRandom
{
    [System.ThreadStatic]
    private static System.Random Local;

    public static System.Random ThisThreadsRandom
    {
        get { return Local ?? (Local = new System.Random(unchecked(System.Environment.TickCount * 31 + System.Threading.Thread.CurrentThread.ManagedThreadId))); }
    }
}

static class CardExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
