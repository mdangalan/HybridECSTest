using System.Collections.Generic;
using UnityEngine;

namespace KneeSystems
{
    /// <summary>
    /// Generators which KneeSystemsController/s uses. Ideally, the only objects/references the generators use are the KneeSystemsStructure members.
    /// The methods inside this interface will be utilized usually when excecuting/starting/loading/initializing.
    /// </summary>
    public interface IGenerator
    {
        void Generate(KneeSystemsStructure structure);
    }
    /// <summary>
    /// Cleaners which KneeSystemsController/s uses. Optional partner for IGenerator as this will be utilized/called when cleaning the Generators.
    /// Will be called before IGenerator::Generate
    /// </summary>
    public interface ICleaner
    {
        void Clean(KneeSystemsStructure structure);
    }
    /// <summary>
    /// Runners. Called every frame for KneeSystemsController/s
    /// Ideally, the only objects/references the runners use are the KneeSystemsStructure members.
    /// </summary>
    public interface IUpdater
    {
        void Update(KneeSystemsStructure structure);
    }

    /// <summary>
    /// NOTE: To make use of this, a class must also use IUpdater, even if it
    /// has an empty Update() function, and added to OnAdditionalRunners()
    /// </summary>
    public interface ILateUpdater
    {
        void LateUpdate(KneeSystemsStructure structure);
    }

    /// <summary>
    /// Think of BaseEntity as entity as Data only! 
    /// Data components are attached to acquire related data 
    /// NOTE: 
    ///  Some Entities will be able to create themselves and this should be okay.
    ///  If we want to finalize a working Entity system, look for Entitas.
    /// WARNING: 
    ///  - Do NOT give Entities the logic to interact with other entities.
    ///  - Entities just need reference with other entities. DONT change state inside entities
    /// </summary>
    public sealed class BaseEntity
    {
        static List<BaseEntity> allEntity = new List<BaseEntity>();
        public CoreComponent core { get; private set; }
        List<BaseComponent> components = new List<BaseComponent>();

        BaseEntity()
        {
            allEntity.Add(this);
        }

        public static BaseEntity Create()
        {
            BaseEntity entity = new BaseEntity();
            entity.core = new CoreComponent();
            return entity;
        }


        public delegate bool EntityCondition(BaseEntity entity);
        public static List<BaseEntity> FindEntities(EntityCondition predicate)
        {
            List<BaseEntity> views = new List<BaseEntity>();
            int len = allEntity.Count;
            for (int index = 0; index < len; index++)
            {
                BaseEntity entity = allEntity[index];

                //Checks if the entity is the target content
                if (predicate(entity))
                    views.Add(entity);
            }
            return views;
        }

        public static List<T> FindEntitiesWithComponent<T>(IEnumerable<BaseEntity> entities)
            where T : BaseComponent
        {
            List<T> components = new List<T>();
            foreach (var entity in entities)
            {
                T acquiredComponent = GetComponent<T>(entity);
                if (acquiredComponent != null)
                    components.Add(acquiredComponent);
            }
            return components;
        }

        public static List<T> FindEntitiesWithComponent<T>() where T : BaseComponent
        {
            return FindEntitiesWithComponent<T>(allEntity);
        }

        public static T GetComponent<T>(BaseEntity entity) where T : BaseComponent
        {
            foreach (var component in entity.components)
            {
                if (component is T)
                    return component as T;
            }
            return null;
        }

        public T GetOrAddComponent<T>() where T : BaseComponent, new()
        {
            return GetComponent<T>() ?? AddComponent<T>();
        }

        public T GetComponent<T>() where T : BaseComponent
        {
            return BaseEntity.GetComponent<T>(this);
        }

        public T AddComponent<T>() where T : BaseComponent, new()
        {
            return BaseEntity.AddComponent<T>(this);
        }

        /// <summary>
        /// Add a component to an entity for the systems to check and change the state
        /// NOTE: This will not allow adding of existing components
        /// </summary>
        public static T AddComponent<T>(BaseEntity entity) where T : BaseComponent, new()
        {
            foreach (var component in entity.components)
            {
                if (component is T)
                    return null;
            }

            T newComponent = BaseComponent.Create<T>(entity);
            entity.components.Add(newComponent);
            return newComponent;
        }

        /// <summary>
        /// Removes the component FROM owner to a THIS entity. 
        /// True for success on transfer and remove
        /// </summary>
        public bool OwnComponent<T>(BaseEntity from) where T : BaseComponent
        {
            var fromComponent = from.GetComponent<T>();

            //No component
            if (fromComponent == null)
                return false;

            //Check if we already have the component
            foreach (var component in components)
            {
                if (component is T)
                    return false;
            }

            //Success
            components.Add(fromComponent);
            BaseEntity.RemoveComponent<T>(from);

            return true;
        }
        /// <summary>
        /// In theory, this will remove a capability of the entity.
        /// NOTE: This is not yet tested to work
        /// </summary>
        public static void RemoveComponent<T>(BaseEntity entity) where T : BaseComponent
        {
            int targetComponent = -1;
            for (int index = 0; index < entity.components.Count; index++)
            {
                if (entity.components[index] is T)
                {
                    targetComponent = index;
                    break;
                }
            }

            if (targetComponent != -1)
            {
                BaseComponent component = entity.components[targetComponent];
                entity.components.RemoveAt(targetComponent);
            }
        }

        /// <summary>
        /// Removes the entity from list of all entity
        /// NOTE: This will not assume destruction of the Gameobject
        /// </summary>
        public static void DestroyEntity(BaseEntity entity)
        {
            if (entity != null)
                allEntity.Remove(entity);
        }

        /// <summary> 
        /// Wipeout all existing entities
        /// THOUGHT: Maybe we can better automate the purge 
        /// process from to avoid leaking of entities
        /// </summary>
        public static void Purge()
        {
            int len = allEntity.Count;
            while (len > 0)
            {
                DestroyEntity(allEntity[0]);
                len--;
            }
        }
    }

    public interface IComponentTracker
    {
        bool HasNewItem();
        bool HasRemovedItem();
        void Clean();
        void ClearState();
    }

    /// <summary>
    /// Handle the list of component state for new items or deleted items
    /// The context of "add" and "remove" is simple an event to track for changes
    /// 
    /// NOTE: Clear() must ALWAYS be called by the end of the Update controller 
    /// Clearing enforces the idea that if something is added on the frame, 
    /// then the item must be ready before it leaves the frame
    /// </summary>
    public sealed class ComponentTracker<T> : IComponentTracker
        where T : BaseComponent
    {
        HashSet<T> _added = new HashSet<T>();
        public HashSet<T> addedItems { get { return _added; } }

        HashSet<T> _alive = new HashSet<T>();
        public HashSet<T> alive { get { return _alive; } }

        HashSet<T> _removed = new HashSet<T>();
        public HashSet<T> removeItem { get { return _removed; } }

        /// <summary> 
        /// WARNING: Add is NOT alway an instantiated gameobject 
        /// </summary>
        public void AddComponent(T item)
        {
            _added.Add(item);
            alive.Add(item);
        }
        public bool HasNewItem() { return _added.Count > 0; }

        /// <summary> 
        /// WARNING: Removed is NOT always a delete gameobject
        /// </summary>
        public void RemovedComponent(T item)
        {
            _removed.Add(item);
            alive.Remove(item);
        }

        public bool HasRemovedItem() { return _removed.Count > 0; }

        /// <summary> 
        /// Adding and removing should only be 
        /// called at the end of update frames 
        /// </summary>
        public void ClearState()
        {
            _added.Clear();
            _removed.Clear();
        }

        /// <summary> 
        /// This will be normally called on the end of 
        /// modes to remove any previous tracked item 
        /// </summary>
        public void Clean()
        {
            alive.Clear();
            ClearState();
        }
    }

    // Just have an abstract component, 
    // we might need to have a general component
    public abstract class BaseComponent
    {
        public BaseEntity entity { get; private set; }
        public static T Create<T>(BaseEntity enitiy) where T : BaseComponent, new()
        {
            T component = new T();
            component.entity = enitiy;
            return component;
        }
    }

    /// <summary> Every BaseEntity will have this component </summary>
    public sealed class CoreComponent : BaseComponent
    {
        public Transform transform { get { return gameObject.transform; } }
        public GameObject gameObject;
    }
}
