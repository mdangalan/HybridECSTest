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
        public Selectable selectedObject;

        //holders
        public GameObject cubeHolder;

        public ConsultAppStructure()
        {
            cubeHolder = null;
            floor = null;
            selectables = new List<Selectable>();
            cubeObjects = new List<CubeObject>();
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
}
