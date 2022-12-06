using UnityEngine;

namespace KneeSystems
{
    /// <summary>
    /// Checks if something is selected in the scene. Return a selectable gameObject if
    /// the selection is valid.
    /// </summary>
    internal class GetSelectables : IUpdater
    {
        ConsultAppStructure _structure;
        public void Update(KneeSystemsStructure structure)
        {
            _structure = structure as ConsultAppStructure;
            if (_structure == null)
                return;

            if (Input.GetMouseButtonDown(0))
                GetSelectableOnClick();
                      
        }

        void GetSelectableOnClick()
        {
            RaycastHit hit;
            ConsultAppController controller = _structure.controller as ConsultAppController;
            Ray ray = _structure.mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (controller.IsObjectInSelectable(hit.transform.gameObject, 
                    out Selectable selected))
                {
                    _structure.selectedObject = selected;
                    Debug.Log("An object was selected");
                }
            }

        }
    }
}