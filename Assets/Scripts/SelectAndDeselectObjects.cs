using UnityEngine;
using UnityEngine.EventSystems;

namespace KneeSystems
{
    /// <summary>
    /// Updates the state of selection of entities. Getting the selected 
    /// object will be the GetSelectables class responsiblity.
    /// </summary>
    internal class SelectAndDeselectObjects : IUpdater
    {
        ConsultAppStructure _structure;
        public void Update(KneeSystemsStructure structure)
        {
            _structure = structure as ConsultAppStructure;
            if (_structure == null)
                return;

            if(_structure.selectedObject != null)
            {
                if(_structure.selectedObject.select.isSelected)
                    _structure.selectedObject.select.Hide();
                else
                    _structure.selectedObject.select.Show();
            }

            _structure.selectedObject = null;
        }


        // Since we always track ALL objects/entities that are created in the system. 
        // we can easily reference them without having to find our gameobject lowering
        // using of system resources. Selecting and Deselecting all of them is easier.

        void SelectAllSelectables()
        {
            foreach(var selectable in _structure.selectables)
                selectable.select.Show();           
        }

        void UnselectAllSelectables()
        {
            foreach (var selectable in _structure.selectables)
                selectable.select.Hide();
        }
    }


    #region SELECTION_COMPONENTS
    public interface ISelect
    {
        bool isSelected { get; }
        void Show();
        void Hide();
    }

    public class Selectable : BaseComponent
    {
        static readonly ISelect defaultSelect = new UnSelectable();
        sealed class UnSelectable : ISelect
        {
            public bool isSelected { get; private set; }
            public void Show() { }
            public void Hide() { }
        }
        
        public ISelect select;
        public bool isSelected { get { return select.isSelected; } }
        public Selectable()
        {
            select = defaultSelect;
        }
    }

    public class SelectObject : ISelect
    {
        MeshRenderer _meshRenderer;
        public SelectObject(MeshRenderer meshRenderer)
        {
            _meshRenderer = meshRenderer;
            _isSelected = false;
        }

        bool _isSelected = false;
        public bool isSelected { get { return _isSelected; } }

        public void Hide()
        {
            _meshRenderer.material.color = Color.red;
            _isSelected = false;
            Debug.Log("Unselected!");
        }

        public void Show()
        {
            _meshRenderer.material.color = Color.green;
            _isSelected = true;
            Debug.Log("Selected!");
        }

        #endregion

    }
}