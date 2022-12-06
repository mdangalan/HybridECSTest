using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace KneeSystems
{
    /// <summary>
    /// Extends the Base KneeSystemsController.
    /// </summary>
    public abstract class KneeSystemsController<T> : KneeSystemsController
        where T : KneeSystemsStructure, new()
    {
        public new T structure { set { base.structure = value; } get { return base.structure as T; } }
        protected override void InitializeStructure() { structure = new T(); }
    }

    /// <summary>
    /// Base class for additional projects: ConsultApp, Wrapper, HackIPS etc...
    /// The fields, the methods, the ECS contents specifically built for them to be used easier.
    /// Has KneeSystemsStructure for holding the data.
    /// Contains Generators and Runners as Systems in ECS.
    /// </summary>
    public abstract class KneeSystemsController : MonoBehaviour
    {
        private static List<KneeSystemsController> controller = new List<KneeSystemsController>();
        private KneeSystemsStructure _structure = null;
        public KneeSystemsStructure structure { set { _structure = value; } get { return _structure; } }

        private List<IGenerator> _generators = new List<IGenerator>();
        private List<IUpdater> _runners = new List<IUpdater>();

        protected bool _isGenerated = false;
        public bool isGenerated { get { return _isGenerated; } }
        public bool isActive { get { return _structure.isActive; } }

        private IGenerator[] _originalGeneratorsRef;
        private IUpdater[] _originalRunnersRef;


        #region Abstract To be Inherited
        /// <summary>
        /// This will be called on Awake. No need for the inheritors to call this.
        /// This is where you put All the Generators.
        /// </summary>
        protected abstract IGenerator[] OnAdditionalGenerators();

        /// <summary>
        /// This will be called on Awake. No need for the inheritors to call this.
        /// This is where you put All the Runners.
        /// </summary>
        protected abstract IUpdater[] OnAdditionalRunners();

        #endregion

        #region virtual methods
        /// <summary>0
        /// Initialze KneeSystemsStructure Here.
        /// e.g. _structure = new KneeSystemsStructure();
        /// </summary>
        protected virtual void InitializeStructure() { _structure = new KneeSystemsStructure(); }

        #endregion

        private void Awake()
        {
            //General Generators and Updaters
            InitializeStructure();

            _structure.controller = this;
            //Include the addtional Generators and Updaters
            SetOriginalSystems();
        }


        private void Start()
        {
            controller.Add(this);
        }

        public static void LateUpdateAll()
        {
            for (int i = 0; i < controller.Count; i++)
                controller[i].LateUpdateThis();
        }

#if false  // New way             
        // TODO: Used to be Update(), for MonoBehaviour to call, but this was getting
		// called early, before CameraController.LateUpdate() had updated things, so
		// sketch mode often ended up a frame late, making some things lag with respect
		// to others.  This does quite a lot though.  Is it safe for all this to be done
		// in LateUpdate() rather than Update()?  It is still called first in
        private void LateUpdateThis()
#else      // Old way                 
        private void LateUpdateThis()
        {
            // TODO: Some runners would be better run here?
            int len = _runners.Count;
            for (int i = 0; i < len; i++)
            {
                var lateUpdater = _runners[i] as ILateUpdater;
                if (lateUpdater != null)
                    lateUpdater.LateUpdate(structure);
            }
        }

        private void Update()
#endif
        {
            UpdateRunners();
            //Clear up the event before the late update
            //We assume that all evaluation will be done
            //inside the UpdateRunners()           
            _structure.events.Clear();
            _structure.holders.Clear();
        }

        private void OnApplicationQuit()
        {
            BaseEntity.Purge();
        }

        private void OnDestroy()
        {
            controller.Remove(this);
        }


        #region Private/Protected/internal methods
        /// <summary>
        /// Convenience method for setting the system to Original system for this KneeSystemsController.
        /// </summary>
        internal void SetOriginalSystems()
        {
            SetGenerators(_originalGeneratorsRef ?? (_originalGeneratorsRef = OnAdditionalGenerators()));
            SetRunners(_originalRunnersRef ?? (_originalRunnersRef = OnAdditionalRunners()));
        }

        /// <summary>
        /// Clears the current Runners then set the new runners
        /// </summary>
        protected void SetRunners(params IUpdater[] runnerArray)
        {
            _runners.Clear();
            _runners.AddRange(runnerArray);
        }

        /// <summary>
        /// Clears the current Generators then will set new generators
        /// </summary>
        protected void SetGenerators(params IGenerator[] generatorArray)
        {
            _generators.Clear();
            _generators.AddRange(generatorArray);
        }

        /// <summary>
        /// Call all initializer to create entities and components.
        /// </summary>
        private void ExecuteGenerators()
        {
            int len = _generators.Count;
            for (int i = 0; i < len; i++)
            {
                var gen = _generators[i];
                gen.Generate(structure);
            }
            _isGenerated = true;
        }
        private void CleanGenerators()
        {
            _isGenerated = false;
            BaseEntity.Purge();
            int len = _generators.Count;
            for (int i = 0; i < len; i++)
            {
                var gen = _generators[i];
                if (gen is ICleaner)
                {
                    (gen as ICleaner).Clean(structure);
                }
            }
        }

        /// <summary>
        /// Handles the state changes
        /// </summary>
        private void UpdateRunners()
        {
            if (!_isGenerated || _runners == null)
                return;

            int len = _runners.Count;
            for (int i = 0; i < len; i++)
            {
                _runners[i].Update(structure);
            }
        }


        /// <summary>
        /// After Awake, This starts the whole process of ECS.
        /// Executes all generators and then updates the runners.
        /// </summary>
        protected void Generate(bool autoClean = true)
        {
            if (autoClean)
                CleanGenerators();

            _isGenerated = false;
            ExecuteGenerators();
        }
        #endregion

        public void GenerateConsultApp()
        {
            Generate(false);
        }
    }

}
