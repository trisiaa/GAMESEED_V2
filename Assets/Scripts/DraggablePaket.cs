using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DraggablePaket : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startAnchoredPosition;
    private Vector3 startWorldPosition;
    private Transform originalParent;
    private Canvas canvas;
    private PaketSpawnerTroli spawner;

    public Vector2Int[] shapeOffsets;
    private GridDropZonee currentDropZone;
    private int currentBaseX, currentBaseY;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        ExtractShapeOffsets();
    }

    void ExtractShapeOffsets()
    {
        List<Vector2Int> offsets = new List<Vector2Int>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("CellAnchor"))
            {
                Vector2 local = child.localPosition;
                int x = Mathf.RoundToInt(local.x / 60f);
                int y = Mathf.RoundToInt(-local.y / 60f);
                offsets.Add(new Vector2Int(x, y));
            }
        }

        if (offsets.Count == 0)
        {
            Debug.LogWarning($"{name} has no shapeOffsets extracted! Are CellAnchor children missing?");
        }

        shapeOffsets = offsets.ToArray();
    }

    public void SetSpawner(PaketSpawnerTroli troli)
    {
        spawner = troli;
    }

    public void ForceSetInitialData(Transform parent, Vector2 anchoredPos)
    {
        originalParent = parent;
        startAnchoredPosition = anchoredPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (spawner != null)
        {
            spawner.HentikanGerakPaket(gameObject);
        }
        else
        {
            Debug.LogWarning($"{name}: spawner belum di-set!");
        }

        originalParent = transform.parent;

        // 🛠️ FIX: Inisialisasi canvas jika null
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError($"{name}: Tidak menemukan Canvas di parent saat drag!");
                return;
            }
        }

        transform.SetParent(canvas.transform, true);

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        startAnchoredPosition = rectTransform.anchoredPosition;
        startWorldPosition = rectTransform.position;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        transform.SetAsLastSibling();

        if (currentDropZone != null)
        {
            currentDropZone.RemovePackage(shapeOffsets, currentBaseX, currentBaseY);
            currentDropZone = null;
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        GridDropZonee[] dropZones = Object.FindObjectsByType<GridDropZonee>(FindObjectsSortMode.None);
        GridDropZonee bestZone = null;
        float minDistance = float.MaxValue;
        int bestBaseX = 0, bestBaseY = 0;

        foreach (var dropZone in dropZones)
        {
            RectTransform rt = dropZone.GetComponent<RectTransform>();
            Vector2 screenPoint = rectTransform.position;
            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPoint, null, out localPoint);

            int baseX = Mathf.FloorToInt(localPoint.x / 60f);
            int baseY = Mathf.FloorToInt(-localPoint.y / 60f);

            if (dropZone.CanPlacePackage(shapeOffsets, baseX, baseY))
            {
                float distance = Vector2.Distance(screenPoint, rt.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestZone = dropZone;
                    bestBaseX = baseX;
                    bestBaseY = baseY;
                }
            }
        }

        if (bestZone != null)
        {
            // sukses
            bestZone.PlacePackage(shapeOffsets, bestBaseX, bestBaseY);
            currentDropZone = bestZone;
            currentBaseX = bestBaseX;
            currentBaseY = bestBaseY;

            rectTransform.SetParent(originalParent, false);
            rectTransform.anchoredPosition = startAnchoredPosition;

            rectTransform.SetParent(bestZone.transform, false);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(bestBaseX * 60f, -bestBaseY * 60f);

            // Simpan posisi drop terakhir (untuk fallback jika gagal next time)
            startAnchoredPosition = rectTransform.anchoredPosition;
            startWorldPosition = rectTransform.position;

            return;
        }

        // DROP GAGAL → kembalikan ke posisi terakhir valid
        transform.SetParent(originalParent, true);
        rectTransform.SetPositionAndRotation(startWorldPosition, Quaternion.identity);

        // Pastikan tidak digerakkan oleh spawner lagi
        if (spawner != null)
        {
            spawner.HentikanGerakPaket(gameObject); // ← ini yang penting!
            if (!spawner.IsPaketTerdaftar(gameObject))
            {
                spawner.TambahkanKembali(gameObject);
            }
        }

        Debug.Log("Drop failed — kembali ke posisi terakhir valid");

    }

}
