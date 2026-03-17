using UnityEngine;

namespace TrainSurvival.UI
{
    public class CloseUpgradePanelButton : MonoBehaviour
    {
        public void Close()
        {
            UIManager.Instance?.CloseUpgradePanel();
        }
    }
}
