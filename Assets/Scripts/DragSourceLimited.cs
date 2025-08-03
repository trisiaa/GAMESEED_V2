using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSourceLimited : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject paketPrefab;
    public Transform parentCanvas;
    public int maxUse = 5;

    private int currentUse = 0;
    private GameObject draggingObject;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentUse >= maxUse)
        {
            Debug.Log("Batas penggunaan sudah habis.");
            return;
        }

        draggingObject = Instantiate(paketPrefab, parentCanvas);
        draggingObject.transform.position = eventData.position;

        var dragHandler = draggingObject.GetComponent<DraggablePaket>();
        if (dragHandler != null)
        {
            // Gunakan versi terbaru dan cepat
            PaketSpawnerTroli spawner = Object.FindFirstObjectByType<PaketSpawnerTroli>();
            if (spawner != null)
            {
                dragHandler.SetSpawner(spawner);
            }

            ExecuteEvents.Execute(draggingObject, eventData, ExecuteEvents.beginDragHandler);
        }

        currentUse++;
        Debug.Log("Paket dipakai ke-" + currentUse);

        if (currentUse >= maxUse)
        {
            GetComponent<Image>().color = Color.gray;
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (draggingObject != null)
        {
            ExecuteEvents.Execute(draggingObject, eventData, ExecuteEvents.dragHandler);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingObject != null)
        {
            ExecuteEvents.Execute(draggingObject, eventData, ExecuteEvents.endDragHandler);
            draggingObject = null;
        }
    }
}
