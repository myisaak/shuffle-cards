using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class testplayerhand : MonoBehaviour {

    public GameObject CardObject;

    private HorizontalLayoutGroup _hor;
    private Vector3[] newCardPositions = new Vector3[0];

    private void Awake ()
    {
        _hor = GetComponent<HorizontalLayoutGroup>();

        StartCoroutine(MoveCards());
    }

    public IEnumerator RefreshCards()
    {
        var oldCardPositions = new Vector3[transform.childCount];
        newCardPositions = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            oldCardPositions[i] = transform.GetChild(i).position;
        }

        yield return true;

        _hor.CalculateLayoutInputHorizontal();
        _hor.SetLayoutHorizontal();

        for (int i = 0; i < transform.childCount; i++)
        {
            newCardPositions[i] = transform.GetChild(i).position;
        }

        yield return true;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = oldCardPositions[i];
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine( RefreshCards());
        }
    }

    private IEnumerator MoveCards()
    {
        while (true)
        {
            if (newCardPositions.Length == 0) yield break;

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position, newCardPositions[i], 0.01f);
            }

            yield return true;
        }
    }
}
