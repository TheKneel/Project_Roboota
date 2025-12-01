using UnityEngine;

public class Door : Interactable
{
    public string requiredKeyID = "RedKey"; // key needed to open door
    public Animator doorAnimator; // optional for open animation
    private bool isOpen = false;
    [SerializeField] GameObject GameWinPanel;

    private void Start()
    {
        GameWinPanel.SetActive(false);
    }
    protected override void Interact()
    {
        if (isOpen) return;

        PlayerInventory inventory = Object.FindFirstObjectByType<PlayerInventory>();
        if (inventory != null && inventory.HasKey(requiredKeyID))
        {
            OpenDoor();
        }
        else
        {
            promptMessage = "need Key";
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Door opened!");
        GameWinPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");
        else
            gameObject.SetActive(false); // fallback if no animation
    }
}
