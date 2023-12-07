using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;

    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    public virtual void AdjustPosition()
    {

        Vector2 mousePosition = Input.mousePosition;

        float newXOffset;
        float newYOffset;

        if (mousePosition.x > xLimit)
            newXOffset = -xOffset;
        else
            newXOffset = xOffset;

        if (mousePosition.y > yLimit)
            newYOffset = -yOffset;
        else
            newYOffset = yOffset;

        transform.position = new Vector2(mousePosition.x + newXOffset, mousePosition.y + newYOffset);
    }
}
