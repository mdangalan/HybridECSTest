using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KneeSystems
{
    public class ConsultAppStructure : KneeSystemsStructure
    {
        // These variables can also be under KneeSystemsStructure if it will 
        // be shared by multiple systems. But for now, we assume that it's strictly
        // for the ConsultApp.
        public GameObject floor;
        public List<Selectable> selectables;
        public List<CubeObject> cubeObjects;
        public List<SphereObject> sphereObjects;
        public Selectable selectedObject;

        //holders
        public GameObject cubeHolder;
        internal GameObject sphereHolder;

        public ConsultAppStructure()
        {
            cubeHolder = null;
            sphereHolder = null;
            floor = null;
            selectables = new List<Selectable>();
            cubeObjects = new List<CubeObject>();
            sphereObjects = new List<SphereObject>();

        }
    }

    // Entities
    public class CubeObject : BaseComponent
    {
        public static CubeObject Create(Transform parent = null)
        {
            BaseEntity baseEntity = BaseEntity.Create();
            CubeObject cube = baseEntity.AddComponent<CubeObject>();
            cube.entity.AddComponent<Selectable>();
            cube.entity.core.gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            if (parent != null)
                cube.entity.core.gameObject.transform.SetParent(parent);
                
            return cube;
        }
    }

    public class SphereObject : BaseComponent
    {
        public static SphereObject Create(Transform parent = null)
        {
            BaseEntity baseEntity = BaseEntity.Create();
            SphereObject cube = baseEntity.AddComponent<SphereObject>();
            cube.entity.AddComponent<Selectable>();
            cube.entity.core.gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            if (parent != null)
                cube.entity.core.gameObject.transform.SetParent(parent);

            return cube;
        }
    }
}
