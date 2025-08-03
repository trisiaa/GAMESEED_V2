using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaketSpawnerTroli : MonoBehaviour
{
    public GameObject[] paketPrefabs;
    public RectTransform spawnPoint;
    public RectTransform targetPoint;
    public float spawnInterval = 3f;
    public float speedTurun = 100f;
    public float jarakTumpuk = 180f;

    private List<GameObject> semuaPaket = new List<GameObject>();
    private GridDropZonee dropZone;
    private bool isSpawning = false;

    void Start()
    {
        dropZone = Object.FindFirstObjectByType<GridDropZonee>();

        if (dropZone == null)
        {
            Debug.LogError("GridDropZonee tidak ditemukan di scene!", this);
            return;
        }

        StartCoroutine(WaitAndStartSpawning());
    }

    IEnumerator WaitAndStartSpawning()
    {
        yield return new WaitUntil(() => dropZone.IsInitialized());

        if (spawnPoint == null || targetPoint == null)
        {
            Debug.LogError("SpawnPoint atau TargetPoint belum diatur!", this);
            yield break;
        }

        if (paketPrefabs == null || paketPrefabs.Length == 0)
        {
            Debug.LogError("Tidak ada paketPrefabs yang diatur!", this);
            yield break;
        }

        isSpawning = true;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            yield return new WaitUntil(() => IsSafeToSpawn());
            SpawnPaketBaru();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    bool IsSafeToSpawn()
    {
        foreach (GameObject paket in semuaPaket)
        {
            if (paket == null) continue;
            RectTransform rt = paket.GetComponent<RectTransform>();
            if (Mathf.Abs(rt.anchoredPosition.y - spawnPoint.anchoredPosition.y) < jarakTumpuk)
            {
                return false;
            }
        }
        return true;
    }

    void SpawnPaketBaru()
    {
        int index = Random.Range(0, paketPrefabs.Length);
        GameObject paket = Instantiate(paketPrefabs[index]);
        paket.transform.SetParent(spawnPoint, false);

        RectTransform rt = paket.GetComponent<RectTransform>();
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;

        semuaPaket.Add(paket);

        DraggablePaket dp = paket.GetComponent<DraggablePaket>();
        if (dp != null)
        {
            dp.SetSpawner(this);
            dp.ForceSetInitialData(spawnPoint, rt.anchoredPosition);
        }
        else
        {
            Debug.LogError("Komponen DraggablePaket tidak ditemukan pada prefab paket!", paket);
        }

        if (dropZone != null)
        {
           
        }
    }

    void Update()
    {
        for (int i = semuaPaket.Count - 1; i >= 0; i--)
        {
            if (semuaPaket[i] == null)
            {
                semuaPaket.RemoveAt(i);
                continue;
            }

            RectTransform rt = semuaPaket[i].GetComponent<RectTransform>();
            Vector2 pos = rt.anchoredPosition;
            pos.y -= speedTurun * Time.deltaTime;
            rt.anchoredPosition = pos;

            if (pos.y < targetPoint.anchoredPosition.y)
            {
                Destroy(semuaPaket[i]);
                semuaPaket.RemoveAt(i);
            }
        }
    }

    public bool IsPaketTerdaftar(GameObject paket)
    {
        return semuaPaket.Contains(paket);
    }

    public void TambahkanKembali(GameObject paket)
    {
        if (!semuaPaket.Contains(paket))
        {
            semuaPaket.Add(paket);
        }
    }

    public void HentikanGerakPaket(GameObject paket)
    {
        semuaPaket.Remove(paket);
    }

    void OnDestroy()
    {
        isSpawning = false;
    }
}
