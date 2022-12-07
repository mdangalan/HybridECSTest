using UnityEngine;

namespace KneeSystems
{
    internal class Initialise : IGenerator
    {
        ConsultAppStructure _structure;

        public void Generate(KneeSystemsStructure structure)
        {
            _structure = structure as ConsultAppStructure;

            //Create Camera
            _structure.mainCamera = CreateCamera("MainCamera");
            _structure.mainCamera.transform.position = new Vector3(-4f, 4f, -3f);
            _structure.mainCamera.transform.localEulerAngles = new Vector3(36f, 56f, 0f);

            //Create floor that objects can be placed on.
            _structure.floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _structure.floor.transform.position = Vector3.zero;

            //Create Holders
            _structure.cubeHolder = new GameObject("Cubes");
            _structure.sphereHolder = new GameObject("Spheres");
        }
      
        Camera CreateCamera(string name)
        {
            GameObject go = new GameObject(name);
            Camera cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            return cam;            
        }
    }
}