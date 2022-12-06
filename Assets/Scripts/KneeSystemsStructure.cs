using System.Collections.Generic;
using UnityEngine;

namespace KneeSystems
{
    public class KneeSystemsStructure
    {
        public KneeSystemsController controller;

        public Camera mainCamera;
        public List<GameObject> holders;

        public bool isModelStateDirty = false;

        /// <summary>
        /// We use a set of events instead of boolean flags to 
        /// support multiple update events
        /// </summary>
        public HashSet<string> events;
        internal bool isActive;

        public KneeSystemsStructure()
        {
            mainCamera = null;

            //Initialise all declared variables.
            events = new HashSet<string>();
            holders = new List<GameObject>();
        }
    }
}
