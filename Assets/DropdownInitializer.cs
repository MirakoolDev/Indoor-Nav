using UnityEngine;
using TMPro;  // Ensure TMPro namespace is used for TextMesh Pro components

public class DropdownInitializer : MonoBehaviour
{
    public TMP_Dropdown locationDropdown; // Reference to your dropdown component
    public Transform[] destinations; // Array to hold your destination points

    // Start is called before the first frame update
    void Start()
    {
        locationDropdown.options.Clear(); // Clear existing dropdown options
        foreach (Transform destination in destinations)
        {
            locationDropdown.options.Add(new TMP_Dropdown.OptionData(destination.name)); // Add destinations to dropdown
        }
    }
}
