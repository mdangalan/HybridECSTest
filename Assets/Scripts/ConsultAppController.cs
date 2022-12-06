using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace KneeSystems
{
    public class ConsultAppController : KneeSystemsController<ConsultAppStructure>
    {
        /// <summary>
        /// This is where you put the desired Generators and optional cleaners. 
        /// Generators are only called once. Think of them as initialisers.
        /// </summary>
        protected override IGenerator[] OnAdditionalGenerators() //Start()
        {
            return new IGenerator[]
            {
                new Initialise(),
                new CleanStructure(),
                new GenerateModels(),
            };
        }

        /// <summary>
        /// This is where you put the desired Runners. Runners are called every frame.
        /// </summary>
        protected override IUpdater[] OnAdditionalRunners() //Update()
        {
            return new IUpdater[]
            {
                new GetSelectables(),
                new SelectAndDeselectObjects(),
            };
        }

        protected override void InitializeStructure()
        {
            base.InitializeStructure();
        }

        public void PostUpdate()
        {
            LateUpdateAll();
        }

        public bool IsObjectInSelectable(GameObject obj, out Selectable selected)
        {
            bool IsSelectable = false;
            selected = null;
            foreach (var selectable in structure.selectables)
            {
                if (selectable.entity.core.gameObject == obj)
                {
                    selected = selectable;
                    return IsSelectable = true;
                }
            }

            return IsSelectable;
        }
    }
}
