using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonAR.Location
{
    public class GyroscopeService : MonoBehaviour
    {
        private Gyroscope gyro;

        private void Start()
        {
            enableGyro();
        }

        /// <summary>
        /// Attempts to get the current rotation of the gyroscope.
        /// </summary>
        /// <param name="rotation">Rotation in left handed Unity coordinate space.</param>
        /// <returns>True rotation was retrieved, false otherwise.</returns>
        public bool tryGetRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;

            if (!gyro.enabled)
                return false;

            rotation = gyroToUnity(Input.gyro.attitude);
            return true;
        }

        //Enables the gyroscope if the device supports a gyroscope
        private bool enableGyro()
        {
            if (SystemInfo.supportsGyroscope) //Gyroscope available
            {
                gyro = Input.gyro;
                gyro.enabled = true;
                return true;
            }
            else //Gyroscope not available
            {
                Debug.LogError("Gyroscope not available");
                return false;
            }
        }

        /// <summary>
        /// The Gyroscope is right-handed. Unity is left handed. 
        /// Perform neccessary conversion.
        /// </summary>
        private static Quaternion gyroToUnity(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }
    }


}