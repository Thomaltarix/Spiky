using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    public float amplitude = 8f;     // height of floating in pixels
    public float frequency = 1f;      // velocity of movement

    private RectTransform rect;
    private Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        rect.anchoredPosition = startPos + new Vector2(0, yOffset);
    }
}
