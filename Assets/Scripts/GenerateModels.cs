using UnityEngine;

namespace KneeSystems
{

    /// <summary>
    /// Generate the necessarry models to be used in the scene.
    /// </summary>
    internal class GenerateModels : IGenerator
    {
        ConsultAppStructure _consultAppStructure;
        void IGenerator.Generate(KneeSystemsStructure structure)
        {
            _consultAppStructure = structure as ConsultAppStructure;
            CreateCubeObjects(5);
        }

        void CreateCubeObjects(int count)
        {
            for(int i = 0; i < count; i++)
            {
                CubeObject cube = CubeObject.Create(_consultAppStructure.cubeHolder.transform);                
                float x = Random.Range(-5f, 5f);
                float z = Random.Range(-5f, 5f);
                cube.entity.core.transform.position = new Vector3(x, 0.5f, z);
                Selectable selectable = cube.entity.GetComponent<Selectable>();
                selectable.select =  new SelectObject(cube.entity.core.gameObject.GetComponent<MeshRenderer>());

                _consultAppStructure.cubeObjects.Add(cube);
                _consultAppStructure.selectables.Add(selectable);
            }
        }
    }
}