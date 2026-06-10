using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForestManager : MonoBehaviour
{
    public static ForestManager Instance;

    public int totalTrees;
    public int currentTrees;
    public GameObject forestHealthUI;
    public Image forestBar;
    public TMP_Text percentText;

    private RectTransform barRect;
    private float maxWidth;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        forestHealthUI.SetActive(true);
        totalTrees = FindObjectsOfType<TreeHealth>().Length;
        currentTrees = totalTrees;

        barRect = forestBar.GetComponent<RectTransform>();
        maxWidth = barRect.sizeDelta.x;

        UpdateUI();
    }

    public void TreeDestroyed()
    {
        currentTrees--;
        currentTrees = Mathf.Max(currentTrees, 0);

        UpdateUI();
    }

    void UpdateUI()
    {
        float percent = (float)currentTrees / totalTrees;

        // Update text
        percentText.text = Mathf.RoundToInt(percent * 100) + "%";

        // Update ukuran bar
        barRect.sizeDelta = new Vector2(
            maxWidth * percent,
            barRect.sizeDelta.y
        );

        // Update warna
        if (percent > 0.6f)
        {
            forestBar.color = new Color32(54, 140, 0, 255); // hijau
        }
        else if (percent > 0.3f)
        {
            forestBar.color = new Color32(255, 193, 7, 255); // kuning
        }
        else
        {
            forestBar.color = new Color32(169, 0, 16, 255); // merah
        }
    }
}