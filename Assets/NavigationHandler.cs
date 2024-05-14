using UnityEngine;
using TMPro;

public class NavigationHandler : MonoBehaviour
{
    public TMP_Dropdown locationDropdown;

    public void NavigateToSelectedLocation()
    {
        string selectedLocation = locationDropdown.options[locationDropdown.value].text;
        Debug.Log("Navigate to: " + selectedLocation);
        // Implement your navigation logic here based on selectedLocation
    }
}
