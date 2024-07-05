using UnityEngine;

namespace View
{
  public class ClickChecker : MonoBehaviour
  {
    [SerializeField]
    private Camera _camera;
    private void Update()
    {
      if (!Input.GetMouseButtonDown(0)) return;
      Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

      if (!Physics.Raycast(ray, out RaycastHit hit)) return;
      if (hit.collider == null) return;
      
      GameObject clickedObject = hit.collider.gameObject;
      if (!hit.transform.CompareTag("Clickable Item")) return;
      
      clickedObject.TryGetComponent(out Item item);
      item.Clicked();
    }
  }
}