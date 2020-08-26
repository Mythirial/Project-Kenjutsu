using UnityEngine;

namespace Assets.Scripts
{
    public class DrawCut : MonoBehaviour
    {
        Vector3 _pointA;
        Vector3 _pointB;
    
        Camera _cam;
        public GameObject obj;

        private void Start() {
            _cam = FindObjectOfType<Camera>();
        }

        private void Update()
        {
            Vector3 mouse = Input.mousePosition;
            mouse.z = -_cam.transform.position.z;

            if (Input.GetMouseButtonDown(0)) {
                _pointA = _cam.ScreenToWorldPoint(mouse);
            
            }
            if (Input.GetMouseButtonUp(0)) {
                _pointB = _cam.ScreenToWorldPoint(mouse);
                CreateSlicePlane();
            }
        }

        void CreateSlicePlane() {
            Vector3 centre = (_pointA+_pointB)/2;
            Vector3 up = Vector3.Cross((_pointA-_pointB),(_pointA-_cam.transform.position)).normalized;
        
        
            Cutter.Cut(obj, centre, up,null,true,true);
        }
    }
}
