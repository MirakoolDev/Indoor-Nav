using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class NavigationController : MonoBehaviour
{
    public TMP_Dropdown locationDropdown;
    public Button navigateButton;
    public Button toggleButton;
    public NavMeshAgent pathfinder;
    public Transform[] destinations;
    public LineRenderer lineRenderer;
    public GameObject arrowPrefab;
    public Transform userTransform; // This should be your ARCamera's transform
    private List<GameObject> arrows = new List<GameObject>();
    private bool isMenuVisible = true;
    private float closeDistanceThreshold = 2.0f;

    void Start()
    {
        locationDropdown.onValueChanged.AddListener(delegate { NavigateToSelectedLocation(); });
        toggleButton.onClick.AddListener(ToggleMenuVisibility);
        ToggleMenuVisibility();
    }

    void ToggleMenuVisibility()
    {
        isMenuVisible = !isMenuVisible;
        locationDropdown.gameObject.SetActive(isMenuVisible);
        navigateButton.gameObject.SetActive(isMenuVisible);
    }

    public void NavigateToSelectedLocation()
    {
        int selectedIndex = locationDropdown.value;
        if (!pathfinder.isOnNavMesh)
        {
            Debug.LogError("Pathfinder agent is not on the NavMesh.");
            return;
        }
        pathfinder.SetDestination(destinations[selectedIndex].position);
        UpdatePathVisual();
        if (isMenuVisible) ToggleMenuVisibility();
    }

    public void NavigateToLocation(string locationName)
    {
        int selectedIndex = -1;
        for (int i = 0; i < destinations.Length; i++)
        {
            if (destinations[i].name == locationName)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex == -1)
        {
            Debug.LogError("Location not found: " + locationName);
            return;
        }

        if (!pathfinder.isOnNavMesh)
        {
            Debug.LogError("Pathfinder agent is not on the NavMesh.");
            return;
        }
        pathfinder.SetDestination(destinations[selectedIndex].position);
        UpdatePathVisual();
        if (isMenuVisible) ToggleMenuVisibility();
    }

    void Update()
    {
        // Update the NavMeshAgent's position to the ARCamera's position
        pathfinder.transform.position = userTransform.position;

        // Check if the NavMeshAgent has reached its destination
        CheckUserDistance();

        // Update the path visualization based on the current position
        UpdatePathVisual();
    }

    void UpdatePathVisual()
    {
        if (pathfinder.pathStatus == NavMeshPathStatus.PathComplete && pathfinder.hasPath)
        {
            lineRenderer.positionCount = pathfinder.path.corners.Length;
            lineRenderer.SetPositions(pathfinder.path.corners);
            PlaceArrowsAlongPath();
        }
    }

    void PlaceArrowsAlongPath()
    {
        ClearArrows();
        Vector3[] corners = pathfinder.path.corners;
        if (corners.Length < 2) return;

        float arrowSpacing = 3.0f; // Distance between arrows
        Vector3 previousPoint = corners[0];

        for (int i = 1; i < corners.Length; i++)
        {
            Vector3 segmentStart = previousPoint;
            Vector3 segmentEnd = corners[i];
            Vector3 direction = (segmentEnd - segmentStart).normalized;
            float segmentLength = Vector3.Distance(segmentStart, segmentEnd);

            for (float distance = 0; distance < segmentLength; distance += arrowSpacing)
            {
                Vector3 point = segmentStart + direction * distance;
                Quaternion rotation = Quaternion.LookRotation(direction);

                // Adjust rotation if the arrow points in the wrong direction
                // Rotate 90 degrees around the Y-axis
                rotation *= Quaternion.Euler(90, 90, 0);

                GameObject arrow = Instantiate(arrowPrefab, point + Vector3.up * 0.1f, rotation);
                arrows.Add(arrow); // Corrected the method name to Add
            }
            previousPoint = segmentEnd;
        }
    }

    void ClearArrows()
    {
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
        arrows.Clear();
    }

    void CheckUserDistance()
    {
        if (Vector3.Distance(userTransform.position, pathfinder.destination) < closeDistanceThreshold)
        {
            ClearArrows();
            lineRenderer.positionCount = 0;
        }
    }
}
