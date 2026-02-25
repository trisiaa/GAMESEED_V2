using UnityEngine;
using UnityEngine.UI;

public class GridDropZonee : MonoBehaviour
{
    public int gridWidth = 4;
    public int gridHeight = 4;
    public GameObject gridCellPrefab;
    public float cellSize = 60f;

    public GameObject fullBanner;  // Banner to indicate grid is full
    public Button actionButton;     // Button to appear when grid is full

    private GridCell[,] gridCells;
    private bool isInitialized = false;

    void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        if (isInitialized) return;

        if (gridCellPrefab == null)
        {
            Debug.LogError("gridCellPrefab not assigned in Inspector!", this);
            return;
        }

        gridCells = new GridCell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject cell = Instantiate(gridCellPrefab, transform);
                cell.name = $"GridCell_{x}_{y}";

                RectTransform rect = cell.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = new Vector2(0, 1);
                    rect.pivot = new Vector2(0, 1);
                    rect.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
                }

                GridCell gridCell = cell.GetComponent<GridCell>();
                if (gridCell != null)
                {
                    gridCell.Init(x, y);
                    gridCells[x, y] = gridCell;
                }
                else
                {
                    Debug.LogError($"GridCell component missing on {cell.name}!", cell);
                }
            }
        }

        isInitialized = true;

        // Ensure banner and button are not active at the start
        if (fullBanner != null)
            fullBanner.SetActive(false);
        if (actionButton != null)
            actionButton.gameObject.SetActive(false);

        Debug.Log("Grid initialization completed", this);
    }

    public void PlacePackage(Vector2Int[] shapeOffsets, int baseX, int baseY)
    {
        if (!IsReady()) return;

        foreach (var offset in shapeOffsets)
        {
            int x = baseX + offset.x;
            int y = baseY + offset.y;

            if (IsValidGridPosition(x, y))
            {
                gridCells[x, y].SetOccupied(true);
            }
        }

        CheckIfGridFull();
    }

    public bool CanPlacePackage(Vector2Int[] shapeOffsets, int baseX, int baseY)
    {
        if (!IsReady()) return false;

        foreach (var offset in shapeOffsets)
        {
            int x = baseX + offset.x;
            int y = baseY + offset.y;

            if (!IsValidGridPosition(x, y) || gridCells[x, y].IsOccupied)
            {
                return false;
            }
        }
        return true;
    }

    public void RemovePackage(Vector2Int[] shapeOffsets, int baseX, int baseY)
    {
        if (!IsReady()) return;

        foreach (var offset in shapeOffsets)
        {
            int x = baseX + offset.x;
            int y = baseY + offset.y;

            if (IsValidGridPosition(x, y))
            {
                gridCells[x, y].SetOccupied(false);
            }
        }

        CheckIfGridFull();
    }

    private void CheckIfGridFull()
    {
        if (fullBanner == null || actionButton == null) return;

        bool allFilled = true;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (!gridCells[x, y].IsOccupied)
                {
                    allFilled = false;
                    break;
                }
            }
            if (!allFilled) break;
        }

        fullBanner.SetActive(allFilled);
        actionButton.gameObject.SetActive(allFilled); // Show the button if the grid is full
    }

    private bool IsReady()
    {
        if (isInitialized) return true;

        Debug.LogWarning("Grid not ready, initializing...", this);
        InitializeGrid();
        return isInitialized;
    }

    private bool IsValidGridPosition(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight && gridCells[x, y] != null;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
