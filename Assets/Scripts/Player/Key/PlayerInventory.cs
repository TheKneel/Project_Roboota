using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<string> keys = new List<string>();

    public void AddKey(string keyID)
    {
        if (!keys.Contains(keyID))
        {
            keys.Add(keyID);
            Debug.Log("Picked up key: " + keyID);
        }
    }

    public bool HasKey(string keyID)
    {
        return keys.Contains(keyID);
    }
}
