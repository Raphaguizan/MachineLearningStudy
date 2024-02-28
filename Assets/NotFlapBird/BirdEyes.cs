using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace MLS.Bird
{
    public enum BirdVision
    {
        seeUpWall = 0,
        seeDownWall = 1,
        seeTop = 2,
        seeBottom = 3,
        none = 4
    }
    public class BirdEyes : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        private List<Transform> eyesDirections;
        [SerializeField]
        private float visionDist = 1f;

        [SerializeField]
        private bool showDebug = false;

        private BirdVision currentVision = BirdVision.none;

        public BirdVision CurrentVision => currentVision;

        // Update is called once per frame
        void Update()
        {
            GameObject closiestObj = null;
            float closiestDist = float.MaxValue;
            for (int i = 0; i < eyesDirections.Count; i++)
            {
                if (showDebug)
                {
                    Debug.DrawRay(eyesDirections[i].transform.position, eyesDirections[i].transform.forward * visionDist, Color.red);
                }

                RaycastHit2D hit = Physics2D.Raycast(eyesDirections[i].transform.position, eyesDirections[i].transform.forward, visionDist, ~layerMask);

                if (hit.collider != null)
                {
                    float dist = Vector2.Distance(hit.point, transform.position);
                    if (dist < closiestDist)
                    {
                        closiestObj = hit.collider.gameObject;
                        closiestDist = dist;
                    }
                }
            }
            SetVision(closiestObj);
        }

        private void SetVision(GameObject obj)
        {
            if(obj == null)
            {
                currentVision = BirdVision.none;
                return;
            }
            currentVision = obj.tag switch
            {
                "top" => BirdVision.seeTop,
                "bottom" => BirdVision.seeBottom,
                "upwall" => BirdVision.seeUpWall,
                "downwall" => BirdVision.seeDownWall,
                _ => BirdVision.none,
            };
        }
    }
}