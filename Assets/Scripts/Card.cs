using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    public enum FlipState { Shown, Hidden }

    public Sprite BackgroundSprite, ForegroundSprite;

    public FlipState State
    {
        set
        {
            FlipCard(value);
        }
    }

    public string Value
    {
        get
        {
            if (_cardText == null) _cardText = GetComponentInChildren<Text>();

            return _cardText.text;
        }
        set
        {
            if (_cardText == null) _cardText = GetComponentInChildren<Text>();

            gameObject.name = value;
            _cardText.text = value;
        }
    }

    public bool Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            if (value == _selected) return;

            _selected = value;
            MoveCard(new Vector3(transform.localPosition.x, (value ? -40 + CARD_SELECTION_HEIGHT : -50), transform.localPosition.z));
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
        set
        {
            MoveCard(value);
        }
    }

    public bool Active
    {
        get { return _active; }
        set
        {
            _active = value;
            Value = value ? "A" : string.Empty;
            GetComponent<CanvasRenderer>().SetAlpha(value ? 1 : 0);
            GetComponent<Image>().raycastTarget = value;
            GetComponent<Button>().enabled = value;
            gameObject.name = value ? Value : "Empty";
        }
    }

    public int Rank
    {
        get
        {
            if (string.IsNullOrEmpty(Value)) return 1;

            switch (Value)
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
                    return int.Parse(Value);
            }
        }
        set
        {
            switch (value)
            {
                case 12:
                    Value = "Q";
                    return;
                case 11:
                    Value = "K";
                    return;
                case 10:
                    Value = "J";
                    return;
                case 9:
                    Value = "A";
                    return;
                default:
                    Value = (value + 1).ToString();
                    return;
            }
        }
    }

    private const float FLIP_SPEED = 5f;
    private const float MOVE_SPEED = 5f;
    private const float CARD_SELECTION_HEIGHT = 35f;
    private const float DRAG_SPEED = 10f;

    private Text _cardText;
    private bool _active = true;
    private bool _flipping = false;
    private bool _selected = false;
    private Vector3 _cachePosition;
    private RectTransform _canvasRect;

    private void Awake ()
    {
        _canvasRect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
    }

    public void ToggleSelection()
    {
        Selected = !Selected;
    }

    public void OnDragEnter()
    {
        StopAllCoroutines();
        _cachePosition = transform.localPosition;
    }

    public void OnDrag(BaseEventData eventData)
    {
        var pointerData = eventData as PointerEventData;
        if (pointerData == null) return;

        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvasRect, pointerData.position, Camera.main, out worldPoint);

        transform.position = Vector3.Lerp(transform.position, new Vector3(worldPoint.x, transform.position.y, transform.position.z), Time.deltaTime * DRAG_SPEED);
        //transform.position += new Vector3(0.3f*pointerData.delta.x, 0, 0);
    }

    public void OnDragExit(BaseEventData eventData)
    {
        // if Mathf.Abs(transform.localPosition.x - _cachePosition.x) > 100f
        var pointerData = eventData as PointerEventData;
        var raycastResults = new List<RaycastResult>();
        FindObjectOfType<GraphicRaycaster>().Raycast(pointerData, raycastResults);
        foreach (var hit in raycastResults)
        {
            if (hit.gameObject != gameObject && hit.gameObject.name.Length == 1)
            {
                MoveCard(hit.gameObject.transform.localPosition);
                hit.gameObject.GetComponent<Card>().MoveCard(_cachePosition);
                int targetIndex = hit.gameObject.transform.GetSiblingIndex();
                hit.gameObject.transform.SetSiblingIndex(transform.GetSiblingIndex());
                transform.SetSiblingIndex(targetIndex);
                return;
            }
        }
        
        MoveCard(_cachePosition);
    }

    private void FlipCard(FlipState state)
    {
        if (_flipping)
        {
             StopCoroutine(IFlipCard(FlipState.Hidden));
             StopCoroutine(IFlipCard(FlipState.Shown));
             _flipping = false;
        }

        StartCoroutine(IFlipCard(state));
    }

    private IEnumerator IFlipCard(FlipState state)
    {
        _flipping = true;
        while (state == FlipState.Hidden ? transform.eulerAngles.y < 180 : transform.eulerAngles.y > 0)
        {
            _cardText.enabled = transform.eulerAngles.y < 90;
            GetComponent<Image>().sprite = transform.eulerAngles.y < 90 ? ForegroundSprite : BackgroundSprite;

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, state == FlipState.Hidden ? 180 : 0, 0), Time.deltaTime * FLIP_SPEED);
            yield return true;
        }
        _flipping = false;
    }

    private void MoveCard(Vector3 position)
    {
        StopAllCoroutines();
        
        StartCoroutine(IMoveCard(position));
    }

    private IEnumerator IMoveCard(Vector3 position)
    {
        while (transform.localPosition != position)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, position, Time.deltaTime * MOVE_SPEED);

            if (Mathf.Abs(transform.localPosition.x - position.x) < 0.1f && Mathf.Abs(transform.localPosition.y - position.y) < 0.1f)
            {
                transform.localPosition = position;
            }

            yield return true;
        }
    }
}
