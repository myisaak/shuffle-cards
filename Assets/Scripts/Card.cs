using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

public class Card : MonoBehaviour
{
    public enum FlipState { Shown, Hidden }

    public Sprite BackgroundSprite;
        
    public Sprite ForegroundSprite
    {
        get
        {
            return _foregroundSprite;
        }
        set
        {
            _foregroundSprite = value;
        }
    }

    /// <summary>
    /// Use this to flip the card.
    /// </summary>
    public FlipState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
        }
    }

    /// <summary>
    /// The card id used to identify each card.
    /// </summary>
    public string ID
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
            gameObject.name = value == "0" ? "" : value;
        }
    }

    public bool Selected { get; set; }

    /// <summary>
    /// Used for sorting the cards.
    /// </summary>
    public int Rank
    {
        get
        {
            if (string.IsNullOrEmpty(ID)) return 1;

            switch (ID)
            {
                case "Q":
                    return 12;
                case "K":
                    return 11;
                case "J":
                    return 10;
                case "A":
                    return 9;
                default:
                    return int.Parse(ID) - 1;
            }
        }
        set
        {
            switch (value)
            {
                case 12:
                    ID = "Q";
                    return;
                case 11:
                    ID = "K";
                    return;
                case 10:
                    ID = "J";
                    return;
                case 9:
                    ID = "A";
                    return;
                default:
                    ID = (value + 1).ToString();
                    return;
            }
        }
    }

    /// <summary>
    /// Is the player allowed to move this card.
    /// </summary>
    public bool isDraggable
    {
        set
        {
            GetComponent<EventTrigger>().enabled = value;
        }
    }

    private const float FLIP_SPEED = 5f;
    private const float CARD_SELECTION_HEIGHT = 1f;
    private const float DRAG_DELAY = 0.3f;
    
    private RectTransform _canvasRect;
    private Canvas _canvas;
    private FlipState _state;
    private Vector3 offset = Vector3.zero;
    private Sprite _foregroundSprite;
    private int _lastSiblingIndex;
    private float _dragTimer;
    private string _id;
    private bool once;
    private GameObject dummyCard;
    private GameObject dummyLayout;

    private void Awake ()
    {
        _canvas = FindObjectOfType<Canvas>();
        _canvasRect = _canvas.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Move this card to a specific area. Handles all the smoothing interpolations.
    /// </summary>
    /// <param name="parent">Card area to be moved into. e.g Play Area</param>
    /// <param name="isPlayerDeck">Enables the card to be draggable</param>
    public void MoveToArea(RectTransform parent, bool isPlayerDeck = false)
    {
        var cardLayout = new GameObject(ID, typeof(RectTransform), typeof(LayoutElement));
        var oldCardLayout = transform.parent != null ? transform.parent.gameObject : null;
        Vector3 oldCardPos = transform.position;

        cardLayout.GetComponent<RectTransform>().SetParent(parent, false);
        cardLayout.GetComponent<RectTransform>().localScale = Vector3.one;

        GetComponent<RectTransform>().SetParent(cardLayout.transform, true);
        GetComponent<RectTransform>().localScale = Vector3.one;
        isDraggable = isPlayerDeck;
        State = isPlayerDeck ? FlipState.Shown : State;

        Selected = false;

        StartCoroutine(IMoveToArea(oldCardPos));
        
        if (oldCardLayout != null && oldCardLayout.name == ID)
        {
            DestroyImmediate(oldCardLayout);
        }

        parent.GetComponent<CardArea>().RefreshCards();
    }

    private IEnumerator IMoveToArea(Vector3 oldCardPos)
    {
        yield return true;
        transform.position = oldCardPos;
    }

    public void ToggleSelection()
    {
        Selected = !Selected;
    }

    public void OnDragEnter()
    {
        _lastSiblingIndex = transform.parent.GetSiblingIndex();
        _dragTimer = 0;
        once = false;
    }

    public void OnDrag(BaseEventData eventData)
    {
        var pointerData = eventData as PointerEventData;
        if (pointerData == null) return;

        _dragTimer += Time.deltaTime;
        if (_dragTimer < DRAG_DELAY) return;

        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvasRect, pointerData.position, Camera.main, out worldPoint);

        if(!once)
        {
            StartCoroutine(DragClone());
            StartCoroutine(DragLoop());
            once = true;
        }

        offset = new Vector3(worldPoint.x, worldPoint.y, 0);

    }

    private IEnumerator DragLoop()
    {
        while (dummyLayout != null)
        {
            int index = dummyLayout.transform.GetSiblingIndex();

            float smallestDistance = Mathf.Infinity;
            int targetindex = 0;

            for (int i = 0; i < FindObjectOfType<PlayerHand>().transform.childCount; i++)
            {
                float distance = transform.GetDistPointToLine(Vector3.up, FindObjectOfType<PlayerHand>().transform.GetChild(i).position);

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    targetindex = i;
                }
            }

            dummyLayout.transform.SetSiblingIndex(targetindex);

            yield return true;
            yield return true;
            yield return true;
            FindObjectOfType<PlayerHand>().RefreshCards();
        }
    }

    private IEnumerator DragClone()
    {
        dummyLayout = new GameObject("DummyLayout", typeof(RectTransform));
        dummyLayout.GetComponent<RectTransform>().SetParent(transform.parent.parent, false);
        dummyLayout.transform.localScale = Vector3.one;

        Vector3 oldpos = transform.position;

        transform.parent.GetComponent<RectTransform>().SetParent(transform.parent.parent.parent, true);
        yield return true;
        transform.parent.SetAsLastSibling();
        yield return true;
        transform.position = oldpos;     

        dummyCard = (GameObject)Instantiate(gameObject, transform.position, Quaternion.identity);
        dummyCard.GetComponent<RectTransform>().SetParent(dummyLayout.transform, true);
        dummyCard.GetComponent<Card>().BackgroundSprite = BackgroundSprite;
        dummyCard.GetComponent<Card>().ForegroundSprite = ForegroundSprite;
        dummyCard.GetComponent<Image>().sprite = ForegroundSprite;
        dummyCard.GetComponent<Image>().CrossFadeAlpha(0.6f, 0.5f, true);
        dummyCard.GetComponent<EventTrigger>().enabled = false;
        dummyCard.transform.localScale = Vector3.one;
        yield return true;
    }

    public void OnDragExit(BaseEventData eventData)
    {
        if (_dragTimer < DRAG_DELAY) return;

        var pointerData = eventData as PointerEventData;
        var raycastResults = new List<RaycastResult>();

        _dragTimer = 0;
        offset = Vector3.zero;
        Selected = !Selected;

        transform.parent.SetSiblingIndex(_lastSiblingIndex);
        transform.parent.GetComponent<RectTransform>().SetParent(FindObjectOfType<PlayerHand>().transform, true);
        transform.localScale = Vector3.one;
        //FindObjectOfType<PlayerHand>().MoveBesideOther(this, dummyCard.GetComponent<Card>());
        FindObjectOfType<PlayerHand>().ReplaceOther(this, dummyCard.GetComponent<Card>());
    }

    private void Update()
    {
        if (transform.parent != null)
        {
            transform.position = Vector3.Lerp(transform.position, offset == Vector3.zero ? transform.parent.position + (Selected ? Vector3.up * CARD_SELECTION_HEIGHT : Vector3.zero) : offset, Time.deltaTime * CardProperties.Instance.CARD_MOVE_SPEED);
        }
        
        GetComponent<Image>().sprite = transform.localEulerAngles.y < 90 ? ForegroundSprite : BackgroundSprite;

        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(transform.localEulerAngles.x, _state == FlipState.Hidden ? 180 : 0, transform.localEulerAngles.z), Time.deltaTime * FLIP_SPEED);
    }
}
