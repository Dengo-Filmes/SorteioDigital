//////////////////////////////////////////////////
/// Simple class to rotate an object around the z axis for +/- 90 degrees at set intervals + some randomness
///
/// © vipercode corp
/// 2022
/// Please use this asset according to the attached license
/// Attributions, mentions and reviews are always welcomed
///
///////////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace viperOSK_Examples
{

    public class viperOSK_Rotator : MonoBehaviour
    {

        float nextRotateTime;

        /// <summary>
        /// how often the object rotates in seconds
        /// </summary>
        public float rotationFrequency;

        public bool randomizeDirection;

        Quaternion current;
        Quaternion target;

        float t = -1f;

        public void ShowHide(bool show)
        {
            this.gameObject.SetActive(show);
            
        }

        // Start is called before the first frame update
        void Start()
        {
            nextRotateTime = Time.time + UnityEngine.Random.Range(rotationFrequency-.2f, rotationFrequency+.2f);
        }

        // Update is called once per frame
        void Update()
        {
            if(Time.time > nextRotateTime)
            {
                current = this.transform.rotation;

                float r = UnityEngine.Random.Range(0f, 100f);
                if(r >= 50f)
                {
                    r = 90f;
                } else
                {
                    r = -90f;
                }

                target = Quaternion.Euler(0f, 0f, r);
                t = 0f;
                nextRotateTime = Time.time + UnityEngine.Random.Range(rotationFrequency - .2f, rotationFrequency + .2f);
            }

            if (t >= 0f)
            {
                this.transform.rotation = Quaternion.Slerp(current, target, t);
                t += Time.deltaTime;
                if(t >= 1f)
                {
                    t = -1f;
                }
            }
        }
    }

}
