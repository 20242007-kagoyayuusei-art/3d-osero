using UnityEngine;

public class PutStoneByMouse : MonoBehaviour
{
    public GameObject stonePrefab;   // コマのPrefab

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell == null) return;

                if (cell.IsEmpty())
                {
                    PutStone(cell);
                }
            }
        }
    }

    void PutStone(Cell cell)
    {
        Instantiate(
            stonePrefab,
            cell.transform.position,
            Quaternion.identity
        );

        cell.hasStone = true;
    }
}