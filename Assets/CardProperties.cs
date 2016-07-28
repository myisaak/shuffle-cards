using UnityEngine;
using System.Collections;

public class CardProperties : MonoBehaviour
{
    public float CARD_MOVE_SPEED { get; set; }

    private static CardProperties _instance;

    public static CardProperties Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<CardProperties>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        CARD_MOVE_SPEED = 6f;
    }
}
