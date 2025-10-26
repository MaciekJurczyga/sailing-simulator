using UnityEngine;

namespace Statistics
{
    public class BoatIconController : MonoBehaviour
    {
        public RectTransform boatIconTransform;
        public RectTransform apparentWindArrowTransform;
        public RectTransform trueWindArrowTransform;
        private float apparentWindAngle;
        private float trueWindAngle;

        private float boatHeadingAngle;

        public void Initialize()
        {
        }
        void Update()
        {

            boatIconTransform.localEulerAngles = new Vector3(0, 0, -boatHeadingAngle);
            apparentWindArrowTransform.localEulerAngles = new Vector3(0, 0, apparentWindAngle);
            trueWindArrowTransform.localEulerAngles = new Vector3(0, 0, trueWindAngle);
        }
        
        public void SetBoatHeading(float headingDegrees)
        {
            boatHeadingAngle = headingDegrees;
        }
        
        public void SetWindAngle(BoatData boatData)
        {
            float wDeg = boatData.wDeg;
            float vDeg = boatData.vDeg;
            if (vDeg > 180 && vDeg < 360)
            {
                vDeg -= 360;
            }

            if (wDeg > 180 && wDeg < 360)
            {
                wDeg -= 360;
            }

            trueWindAngle = wDeg;
            apparentWindAngle = vDeg;
        }
    }
}