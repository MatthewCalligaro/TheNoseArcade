using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Checker : MonoBehaviour
{ 
    public GameObject instance;
     
    private bool isCheckerSet = false;

    public SlotType playerNumber;

    void Update()
    {
        string keyPress = Input.inputString;
        if (!isCheckerSet) MoveChecker(keyPress);
    }

    void MoveChecker(string keyPress)
    {
        switch (keyPress)
        {
            case "a":
                if (instance.transform.position.x > 1)
                {
                    instance.transform.position += new Vector3(-God.deltaX, 0, 0);
                }
                break;
            case "d":
                if (instance.transform.position.x < God.numColumns)
                {
                    instance.transform.position += new Vector3(God.deltaX, 0, 0);
                }
                break;
            case "\r":
            case "\n":
                isCheckerSet = God.HandleCheckerPlace(this);
                break;
        }
    }
}
