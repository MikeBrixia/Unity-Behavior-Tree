using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


namespace Core
{
    [Serializable]
    public class SubclassOf<T>
    {
        [SerializeField]
        private string SubclassTypeQualifiedName = "None";
        [SerializeField]
        private string SubclassTypeName = "None";
        [SerializeField]
        private string ParentTypeQualifiedName = "None";
        private Type SelectedTypeInternal;
     
        public Type SelectedType
        {
            get 
            {
                Type selectedType = Type.GetType(SubclassTypeQualifiedName);
                if (selectedType == null || !selectedType.IsSubclassOf(typeof(T)) && selectedType != typeof(T))
                {
                    // fallback on the null type
                    ParentTypeQualifiedName = typeof(T).AssemblyQualifiedName;
                    SubclassTypeQualifiedName = "Null";
                    SubclassTypeName = "None";
                    return null;
                }
                return selectedType; 
            }
            set
            {
                if(value != null)
                {
                   if(value.IsSubclassOf(typeof(T)))
                   {
                       SelectedTypeInternal = value.GetType();
                       SubclassTypeQualifiedName = SelectedTypeInternal.AssemblyQualifiedName;
                       SubclassTypeName = SelectedTypeInternal.ToString();
                   }
                   else
                   {
                       SubclassTypeQualifiedName = "Null";
                       SubclassTypeName = "None";
                       Debug.LogError("Value is not subclass of " + typeof(T));
                   }
                }
                else 
                {
                   SubclassTypeQualifiedName = "Null";
                   SubclassTypeName = "None";
                   Debug.LogAssertion("Invalide value");
                }
            }
        }
    
        public string ParentTypeName { 
        
            get
            {
               return ParentTypeQualifiedName;
            }
        }

        public SubclassOf()
        {
            Type GenericType = typeof(T);
            ParentTypeQualifiedName = GenericType.AssemblyQualifiedName;
            SubclassTypeQualifiedName = "Null";
            if (SelectedType != null)
            {
                SubclassTypeName = SelectedType.ToString();
            }
            else
            {
                SubclassTypeName = "None";
            }
        }

        public override string ToString()
        {
           return SelectedType.Name;
       }
    }
}



