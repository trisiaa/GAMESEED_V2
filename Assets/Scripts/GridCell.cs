using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int x, y;
    public bool IsOccupied { get; private set; }

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
        IsOccupied = false;
    }

    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }

    // GIZMOS Debug
    void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.green;
        Vector3 pos = transform.position;
        Gizmos.DrawWireCube(pos, new Vector3(60, 60, 0));
    }
}
