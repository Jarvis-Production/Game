using UnityEngine;

namespace TrainSurvival.Train
{
    public class TrainMotionVisual : MonoBehaviour
    {
        [SerializeField] private Renderer[] scrollingRenderers;
        [SerializeField] private Vector2 scrollSpeed = new(0.8f, 0f);
        [SerializeField] private Transform[] loopingProps;
        [SerializeField] private float loopLength = 220f;
        [SerializeField] private float propSpeed = 35f;

        private Vector2[] offsets;

        private void Awake()
        {
            offsets = new Vector2[scrollingRenderers.Length];
        }

        private void Update()
        {
            for (int i = 0; i < scrollingRenderers.Length; i++)
            {
                if (scrollingRenderers[i] == null) continue;
                offsets[i] += scrollSpeed * Time.deltaTime;
                scrollingRenderers[i].material.SetTextureOffset("_BaseMap", offsets[i]);
            }

            for (int i = 0; i < loopingProps.Length; i++)
            {
                if (loopingProps[i] == null) continue;
                loopingProps[i].Translate(Vector3.back * (propSpeed * Time.deltaTime), Space.World);
                if (loopingProps[i].position.z < -loopLength)
                    loopingProps[i].position += Vector3.forward * loopLength * 2f;
            }
        }
    }
}
