using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanel : MonoBehaviour
{
    private bool ArePrefsLoaded = false;
    virtual public void LoadPrefs()
    {
        if (ArePrefsLoaded) return;
        ArePrefsLoaded = true;
    }
}
