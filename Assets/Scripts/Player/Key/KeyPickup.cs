using UnityEngine;
using TMPro;

public class KeyPickup : Interactable
{
    public string keyID = "RedKey"; // unique key identifier
    public TextMeshProUGUI keyPickUpText;

    private void Start()
    {
        keyPickUpText.text = "";
    }
    protected override void Interact()
    {
        PlayerInventory inventory = Object.FindFirstObjectByType<PlayerInventory>(); // assumes single player
        if (inventory != null)
        {
            inventory.AddKey(keyID);

            if (keyPickUpText != null)
                keyPickUpText.text += "Key Picked";

            Debug.Log("Picked up key: " + keyID);
            Destroy(gameObject); // remove key from scene
        }
    }
}
