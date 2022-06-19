using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPoint : MonoBehaviour
{
    public float viewportX;
    public float viewportY;

    public float paddingLeft;
    public float paddingRight;

    public startAnchor starterPoint;

    private Vector3 starterWorldPoint;

    public enum startAnchor
    {
        LEFT_CENTER,
        MIDDLE_CENTER,
        RIGHT_CENTER
    }

    // Start is called before the first frame update
    void Awake()
    {
        float width = Utils.ScreenSize.GetScreenToWorldWidth;

        Vector2 pos = Camera.main.ViewportToWorldPoint(new Vector2(viewportX, viewportY));

        Renderer renderer = GetComponent<Renderer>();

        switch (starterPoint)
        {
            case startAnchor.LEFT_CENTER:
                pos.x += renderer.bounds.size.x / 2f;
                pos.x += paddingLeft;
                break;
            case startAnchor.MIDDLE_CENTER:
                //empty refactor
                break;
            case startAnchor.RIGHT_CENTER:
                pos.x -= renderer.bounds.size.x / 2f;
                pos.x -= paddingRight;
                break;
            default:
                break;
        }

        //pos *= transform

        //transform.position = pos; //testing edit
        starterWorldPoint = pos;
        
    }

    public Vector3 StarterWorldPoint { get => starterWorldPoint; set => starterWorldPoint = value; }
    public Vector3 StarterWorldPoint1 { get => starterWorldPoint; set => starterWorldPoint = value; }
}
