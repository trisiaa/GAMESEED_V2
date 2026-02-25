using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Dialog
{
    public string text;
    public Sprite image;
    public Sprite backgroundImage;  // The background image
}
public class DialogManager : MonoBehaviour
{
    public Dialog[] dialogs;
    private int currentIndex = 0;

    public Dialog GetCurrentDialog()
    {
        if (currentIndex >= 0 && currentIndex < dialogs.Length)
        {
            return dialogs[currentIndex];
        }
        return null;
    }

    public bool MoveNext()
    {
        if (currentIndex < dialogs.Length - 1)
        {
            currentIndex++;
            return true;
        }
        return false;
    }

    public bool CanMoveNext()
    {
        return currentIndex < dialogs.Length - 1;
    }
}