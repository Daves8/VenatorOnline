using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Canvas inventoryCanvas;

    // Start is called before the first frame update
    void Start()
    {
        inventoryCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}