using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField]
    private int sortingOrderBase=5000;

    [SerializeField]
    private int offset = 0;
    private Renderer thisRenderer;

    private void Awake()
    {
        thisRenderer = GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        thisRenderer.sortingOrder = (int)(sortingOrderBase - (transform.position.y- offset)/2);
    }
}
