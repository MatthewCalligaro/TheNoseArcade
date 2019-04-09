using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuItem
{
    void HandleLeft();
    void HandleRight();
    void HandleEnter();
    void HandleExit();
}
