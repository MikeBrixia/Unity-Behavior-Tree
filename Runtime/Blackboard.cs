using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Runtime
{
    /// <summary>
    /// The blackboard is the "brain" of the Behavior Tree, responsible of storing
    /// relevant data, used to make it's own decision.
    /// The blackboard supports a limited number of types which can be used by the tree,
    /// see BlackboardSupportedTypes in BlackboardPropertyBase for more info.
    /// </summary>
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "AI/Blackboard")]
    public class Blackboard : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Dictionary which contains all serialized blackboard key-value pairs.
        /// </summary>
        [SerializeField] private Dictionary<string, BlackboardPropertyBase> blackboardDict = new Dictionary<string, BlackboardPropertyBase>();
        
        ///<summary>
        /// All the properties stored in this blackboard.
        ///</summary>
        [SerializeField] private List<BlackboardPropertyBase> blackboardProperties = new List<BlackboardPropertyBase>();
        
        ///<summary>
        /// Get a blackboard value of type T by it's key.
        ///</summary>
        ///<param name="key"> the blackboard key used to search for the value</param>
        ///<returns> The value of type T associated with the given blackboard key.</returns>
        public T GetBlackboardValueByKey<T>(string key)
        {
            BlackboardProperty<T> property = (BlackboardProperty<T>) blackboardDict[key];
            return property.value;
        }
        
        ///<summary>
        /// Set the blackboard value of type T associated with the given key
        ///</summary>
        ///<param name="key"> the blackboard key used to search and set the blackboard property.</param>
        ///<param name="newValue"> the new property value of type T</param>
        public void SetBlackbordValue<T>(string key, T newValue)
        {
            BlackboardProperty<T> property = (BlackboardProperty<T>) blackboardDict[key];
            property.value = newValue;
        }
        
        public void OnBeforeSerialize()
        {
        }
        
        public void OnAfterDeserialize()
        {
            blackboardDict = new Dictionary<string, BlackboardPropertyBase>();
            foreach(BlackboardPropertyBase property in blackboardProperties)
            {
                BlackboardPropertyBase newProperty = property.InitializeProperty();
                blackboardDict.TryAdd(property.name, newProperty);
            }
        }
        
        ///<summary>
        ///Clone this blackboard asset.
        ///</summary>
        ///<returns> A copy of this blackboard asset.</returns>
        public Blackboard Clone()
        {
            return Instantiate(this);
        }
    }
}

