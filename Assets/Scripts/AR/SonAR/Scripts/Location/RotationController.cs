using System.Collections;
using UnityEngine;

namespace SonAR.Location
{
    public class RotationController : MonoBehaviour
    {
        [SerializeField]
        private GPSLocationService locationService;

        #region Exponential Moving Average

        private const float smoothingFactor = 2f;
        private const int smoothingDays = 9;
        private float exponentialSmoothingConstant = smoothingFactor / (1 + smoothingDays);

        #endregion Exponential Moving Average

        private bool locationServiceStarted = false;
        private bool shouldUpdate = false;

        //Debug
        public TMPro.TextMeshProUGUI rotationDebug;

        private void Awake()
        {
            Input.compass.enabled = true; //Enable the compass

            locationService.OnServiceStart.AddListener(handleLocationServiceStart);
            locationService.OnServiceStop.AddListener(handleLocationServiceStop);
        }

        private void handleLocationServiceStart()
        {
            locationServiceStarted = true;
            shouldUpdate = true;
            StartCoroutine(updateRotation());
        }

        private void handleLocationServiceStop()
        {
            shouldUpdate = false;
            locationServiceStarted = false;
        }

        private IEnumerator updateRotation()
        {
            float previousSignedAngleToRotate = 0f;

            while (shouldUpdate)
            {
                float rotationAngle = Input.compass.trueHeading;

                float currentRotationAngle = transform.eulerAngles.y;
                float rawSignedAngleToRotate = signedDeltaAngle(currentRotationAngle, rotationAngle);

                /*
                 Low pass filter, Exponential Moving Average formula                 
                 Found here, page 4: https://pdf.sciencedirectassets.com/280203/1-s2.0-S1877050913X00110/1-s2.0-S187705091301260X/main.pdf?X-Amz-Security-Token=IQoJb3JpZ2luX2VjENH%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaCXVzLWVhc3QtMSJHMEUCIBiPGU4c2uWema%2BCyR1MXp9Y8YvjlztXaEMKrJJmhNX7AiEA3TjA2cXeEXRQSrBap52ml1K2SfWUVUk31XTof%2BF1lNYqtAMISRADGgwwNTkwMDM1NDY4NjUiDK72fBlg0qdovItCLiqRAxKP%2BWv%2BuIA27P7kh71g2XfwXCCD0l%2FoOkhnHHAHzO92jBPRW0GDPRGGXVUGjZog%2FJYAF3977BRCj%2BpI3cTs31HlUF9fpNpcODc9yXHYaWN7DXCkEvGIrBmTpwNSvM1RabT8Q4I0m%2BEhgH%2BhxH9x9pC6ioHBT0MC%2FTp9epjP5B5%2B2fD5sPtBByc7dNpdpkRvIes8wwLB73jGGhqXqXfNIsm7epGjAL8e1tzd2tTwkrNvomEN6AOKbTPj2qCAzCI1djY1ud8%2FBP%2FtRz7AuvUOXdzypIAxnyFKuilttgmR7MphjsKNse105hW2ZGpmfXNfUPwcvRZizLC8FJjSpDrRXr%2Fd29FRjKuR%2BMAfB%2FX65vPXnlQyaR8PtfMTGs%2F8YDR0%2BuxlRcARGCFW5GNGC7vb5wfR6HQQVloCYhzoKxBfUCQI5qwG%2FcQT%2BWAm1pQ6wdFUYRGixbKVxo3eEvUSIzUrUpKlrsIzDp9GIQbFBmfmzL2Ufb%2Fk3x6Wx5Dof%2BLJLyQnmNRy6woI5alY0Fp6sBJCs6fUML6tifcFOusB9vGG7Z5lGdYpN8Z%2FYSjhHlLvlZW37f9QJrmFImzq924viM5lhflrfJfDSNQq1WWRHA9Sy8lXklD%2FqPgHMvQBo6BsikRvQERdHSKOrBUSu5ZzQUb86EM9hHLnhXXoIcos1DGB05pxqB7BHulcmMAOwhvo7Suejrw%2BGrnsvRQTgs9%2BdoiIBWdXi1afyHlQT6JUNvJ4Xb%2BZuAHtHLO5iGLspJVeURxT7qhhIgZXk%2FomUdZNccXfpbdIsw6UAtDc9A%2F5ii2YUeQYBNklTDv9bfZgi2tFMSZ2AcxHEEI3RXltiZ0Xt9wDZgRd4A6rUg%3D%3D&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Date=20200611T163343Z&X-Amz-SignedHeaders=host&X-Amz-Expires=300&X-Amz-Credential=ASIAQ3PHCVTYTFIUKTNZ%2F20200611%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Signature=49010d2611c1e6ffba3dbf00715bab5741408f42a5cad3def8204a99b2f36202&hash=a6726e44a43fb1478ef078212b26211a6bbc508e81e9c4b9183df746951f33fd&host=68042c943591013ac2b2430a89b270f6af2c76d8dfd086a07176afe7c76c2c61&pii=S187705091301260X&tid=spdf-6e031ddc-5d66-4ee3-b87f-95c789e8d092&sid=8212902d61aa0240d369249-6e38b875e404gxrqa&type=client

                 **Note: '_' denotes subscript
                 
                 Calculates angle to rotate around y-axis in degrees based on previous values, S_n
                 S_n = a * Y_n-1 + (1 - a) * S_n-1

                Terms:
                 - a: Exponential smoothing constant = smoothing / (1 + days)
                 - Y_n-1: Current, raw rotation data
                 - S_n-1: Previous rotation angle
                */

                float aComplement = 1 - exponentialSmoothingConstant;
                float signedAngleToRotate = (exponentialSmoothingConstant * rawSignedAngleToRotate) +
                    (aComplement * previousSignedAngleToRotate);

                previousSignedAngleToRotate = signedAngleToRotate; //Record rotation angle for use in next iteration

                Quaternion targetRotation = transform.rotation * Quaternion.AngleAxis(signedAngleToRotate, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);

                rotationDebug.text = "[Rotation] " + rotationAngle + ", " + signedAngleToRotate;

                yield return null;
            }
        }

        public static float signedDeltaAngle(float a, float b)
        {
            float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(a, b));
            float sign = 1;
            
            if (b > a && b - a > 180) //Negative
                sign = -1;
            else if (a > b && a - b <= 180) //Negative
                sign = -1;

            return sign * deltaAngle;
        }

        private void OnEnable()
        {
            if (locationServiceStarted && !shouldUpdate)
            {
                shouldUpdate = true;
                StartCoroutine(updateRotation());
            }
        }

        private void OnDisable()
        {
            if (locationServiceStarted && shouldUpdate)
                shouldUpdate = false;
        }
    }
}