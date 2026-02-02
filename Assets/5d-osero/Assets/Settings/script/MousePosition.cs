using UnityEngine;
using UnityEngine.InputSystem;

public class PutStoneByMouse : MonoBehaviour
{
    public GameObject stonePrefab;   // コマのPrefab

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(
                Mouse.current.position.ReadValue()
            );

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
