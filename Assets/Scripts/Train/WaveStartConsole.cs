using UnityEngine;
using TrainSurvival.Core;

namespace TrainSurvival.Train
{
    public class WaveStartConsole : MonoBehaviour, IInteractable
    {
        public string GetPrompt() => "Press [E] to start next wave";

        public void Interact()
        {
            GameManager.Instance.WaveManager.StartNextWave();
        }
    }
}
