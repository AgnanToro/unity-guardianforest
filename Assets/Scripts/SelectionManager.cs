using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{


    public static SelectionManager instance { get; set; }
    
    // Static untuk akses global
    public bool onTarget;


    public GameObject interaction_Info_UI;
    private TextMeshProUGUI interaction_text; // Private karena diisi via Start

    private void Start()
    {
        onTarget = false;
        // Pastikan interaction_Info_UI tidak null sebelum ambil komponen
        if (interaction_Info_UI != null)
        {
            interaction_text = interaction_Info_UI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Awake()
    {
        // Singleton pattern untuk memastikan hanya ada satu instance SelectionManager
        if (instance !=null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
           instance = this;
        }
    }



    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            // Masukkan ke variabel 'interactable' supaya tidak panggil GetComponent berkali-kali
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            // Cek apakah objek punya script InteractableObject DAN pemain dalam jarak aman
            if (interactable != null && interactable.playerInRange)
            {

                onTarget = true;
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);



            }
            else
            {
                onTarget = false;
                interaction_Info_UI.SetActive(false);
            }
        }
        else
        {
            onTarget = false;
            interaction_Info_UI.SetActive(false);
        }
    }
}