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
        get
        {
            return _state;
        }
        set
        {
            _state = value;
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

            gameObject.name = value == "0" ? "" : value;
            _cardText.text = value == "0" ? "" : value;
        }
    }

    public bool Selected { get; set; }

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
                    return int.Parse(Value) - 1;
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

    public bool isDraggable
    {
        set
        {
            GetComponent<EventTrigger>().enabled = value;
        }
    }

    private const float FLIP_SPEED = 5f;
    private const float MOVE_SPEED = 10f;
    private const float CARD_SELECTION_HEIGHT = 20f;
    private const float DRAG_SPEED = 10f;

    private Text _cardText;
    private RectTransform _canvasRect;
    private Canvas _canvas;
    private FlipState _state;

    private void Awake ()
    {
        _canvas = FindObjectOfType<Canvas>();
        _canvasRect = _canvas.GetComponent<RectTransform>();

        Selected = false;
    }

    public void ToggleSelection()
    {
        Selected = !Selected;
    }

    public void OnDragEnter()
    {
        StopAllCoroutines();
    }

    public void OnDrag(BaseEventData eventData)
    {
        var pointerData = eventData as PointerEventData;
        if (pointerData == null) return;

        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvasRect, pointerData.position, Camera.main, out worldPoint);
        transform.SetAsLastSibling();
        transform.position = Vector3.Lerp(transform.position, new Vector3(worldPoint.x, transform.position.y, transform.position.z), Time.deltaTime * DRAG_SPEED);
    }

    public void OnDragExit(BaseEventData eventData)
    {
        var pointerData = eventData as PointerEventData;
        var raycastResults = new List<RaycastResult>();
        FindObjectOfType<GraphicRaycaster>().Raycast(pointerData, raycastResults);
        foreach (var hit in raycastResults)
        {
            if (hit.gameObject != gameObject && hit.gameObject.name.Length == 1)
            {
                int targetIndex = hit.gameObject.transform.parent.GetSiblingIndex();
                transform.parent.SetSiblingIndex(targetIndex + 1);
                return;
            }
        }
    }

    private void Update()
    {
        if (transform.parent != null)
        {
            transform.position = Vector3.Lerp(transform.position, transform.parent.position + (Selected ? Vector3.up * CARD_SELECTION_HEIGHT : Vector3.zero), Time.deltaTime * MOVE_SPEED);
        }

        _cardText.enabled = transform.localEulerAngles.y < 90;
        GetComponent<Image>().sprite = transform.localEulerAngles.y < 90 ? ForegroundSprite : BackgroundSprite;

        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0, _state == FlipState.Hidden ? 180 : 0, 0), Time.deltaTime * FLIP_SPEED);
    }
}
