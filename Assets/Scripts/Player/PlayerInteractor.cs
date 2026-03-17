using UnityEngine;
using TrainSurvival.Core;
using TrainSurvival.UI;

namespace TrainSurvival.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float range = 4f;
        [SerializeField] private LayerMask interactionMask;

        private IInteractable currentInteractable;

        private void Update()
        {
            ScanForInteraction();

            if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
                currentInteractable.Interact();
        }

        private void ScanForInteraction()
        {
            currentInteractable = null;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, range, interactionMask))
            {
                currentInteractable = hit.collider.GetComponentInParent<IInteractable>();
            }

            if (UIManager.Instance != null)
                UIManager.Instance.SetInteractionPrompt(currentInteractable?.GetPrompt() ?? string.Empty);
        }
    }
}
