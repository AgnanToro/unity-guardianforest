using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool playerInRange;
    public string ItemName;

    public string GetItemName()
    {
        return ItemName;
    }


 void Update()
    {
      if (Input.GetKeyDown(KeyCode.Mouse0)&& playerInRange && SelectionManager.instance.onTarget)
        {
            // Logika interaksi dengan objek
            Debug.Log("item added to inventory");

           Destroy(gameObject);


        }



    }





    // Perbaikan: Collider (double 'l')
    private void OnTriggerEnter(Collider other)
    {
        // Perbaikan: Tambah titik (.) sebelum CompareTag
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Perbaikan: Tambah titik (.) sebelum CompareTag
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}