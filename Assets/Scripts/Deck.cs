using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : CardArea
{
    public GameObject CardObject;
    public RectTransform Hand;

    public List<Sprite> Hearts;
    public List<Sprite> Diamonds;
    public List<Sprite> Spades;
    public List<Sprite> Clubs;

    public const int MAX_CARDS_DECK = 39;

    private List<int> cardIndexList = new List<int>(MAX_CARDS_DECK);
    private bool isReady;

    private void Awake ()
    {
        for (int i = 0; i < MAX_CARDS_DECK; i++)
        {
            cardIndexList.Add(i % 53);
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
        isReady = false;

        ClearArea();

        List<Sprite> allCards = new List<Sprite>();

        allCards.AddRange(Hearts);
        allCards.AddRange(Diamonds);
        allCards.AddRange(Spades);
        allCards.AddRange(Clubs);

        for (int i = 0; i < cardIndexList.Count; i++)
        {
            var card = Instantiate(CardObject);

            card.GetComponent<Card>().ForegroundSprite = allCards[cardIndexList[i]];
            card.GetComponent<Card>().Rank = cardIndexList[i]%13;
            card.GetComponent<Card>().State = Card.FlipState.Hidden;
            card.GetComponent<Card>().MoveToArea(GetComponent<RectTransform>());
            yield return true;
        }

        isReady = true;
    }

    public void DrawCard ()
    {
        if (transform.childCount == 0 || !isReady) return;

        var cardIndex = transform.childCount-1;
        var card = transform.GetChild(cardIndex).GetChild(0).GetComponent<Card>();
        card.MoveToArea(Hand, true);
        cardIndexList.RemoveAt(cardIndex);
        //Hand.GetComponent<CardArea>().RefreshCards();
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
