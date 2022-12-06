using System.Collections.Generic;
using UnityEngine;

namespace KneeSystems
{
    public class CleanStructure : IGenerator, ICleaner
    {
        ConsultAppStructure _structure;
        public void Generate(KneeSystemsStructure structure)
        {
            GeneralClean(structure);
        }

        public void Clean(KneeSystemsStructure structure)
        {
            GeneralClean(structure);
        }

        public void GeneralClean(KneeSystemsStructure structure)
        {
            _structure = structure as ConsultAppStructure;
            DeleteSelectables(_structure.selectables);
            DeleteCubes(_structure.cubeObjects);             
        }

        // a very straightforward cleaner. in the event an entity loses
        // it's gameObject (which we assume entities always have a gameobject, not not always
        // the case) we untrack them. That object might've been deleted prematurely so
        // clean it to prevent errors when interacting with the object
        void DeleteSelectables(List<Selectable> selectables)
        {
            foreach (var selectable in _structure.selectables)
            {
                if(selectable.entity.core.gameObject == null)
                    _structure.selectables.Remove(selectable);               
            }          
        }

        void DeleteCubes(List<CubeObject> cubes)
        {
            foreach (var cube in _structure.cubeObjects)
            {
                if (cube.entity.core.gameObject == null)
                    _structure.cubeObjects.Remove(cube);
            }
        }
    }
}
