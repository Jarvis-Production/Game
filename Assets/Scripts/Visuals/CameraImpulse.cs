using UnityEngine;

namespace TrainSurvival.Visuals
{
    public class CameraImpulse : MonoBehaviour
    {
        [SerializeField] private Transform cameraRig;
        [SerializeField] private float swayAmplitude = 0.06f;
        [SerializeField] private float swayFrequency = 1.5f;
        [SerializeField] private float recoilRecoverSpeed = 18f;

        private Vector3 recoilOffset;

        private void Update()
        {
            Vector3 sway = new(
                Mathf.Sin(Time.time * swayFrequency) * swayAmplitude,
                Mathf.Cos(Time.time * swayFrequency * 0.8f) * swayAmplitude * 0.6f,
                0f);

            recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, recoilRecoverSpeed * Time.deltaTime);
            cameraRig.localPosition = sway + recoilOffset;
        }

        public void AddRecoil(float force)
        {
            recoilOffset += new Vector3(Random.Range(-0.02f, 0.02f), 0f, -force);
        }
    }
}
