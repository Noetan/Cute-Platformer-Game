using System;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryManagment
{
    #region Custom
    // Used to hold gameobjects which are to be instantiated
    // Require user to reset each gameobject manually
    public class GameObjectPool
    {
        Stack<GameObject> m_objectStack;
        GameObject m_prefab;
        GameObject m_parent;

        // Constructor
        public GameObjectPool(int initialBufferSize, GameObject preFab, GameObject parent = null
            , bool lazyInstance = false)
        {
            m_objectStack = new Stack<GameObject>(initialBufferSize);
            m_prefab = preFab;
            m_parent = parent;

            // Pre-instantiate all the gameobjects unless stated otherwise
            if (!lazyInstance)
            {
                for (int i = 0; i < initialBufferSize; i++)
                {
                    GameObject newObj = SpawnGameObject();
                    m_objectStack.Push(newObj);
                }
            }
        }

        // Return an inactive gameobject for use
        // Creates a new one if none are available
        public GameObject New()
        {
            GameObject newObj;

            // Retrieve an unused gameobject from the stack
            if (m_objectStack.Count > 0)
            {
                newObj = m_objectStack.Pop();
            }
            // Create a new one if the stack is empty
            else
            {
                newObj = SpawnGameObject();
            }

            return newObj;
        }

        // Store the newObj for later re-use
        public void Store(GameObject newObj)
        {
            newObj.SetActive(false);
            m_objectStack.Push(newObj);
        }

        // Instantiate and set up a new gameobject
        GameObject SpawnGameObject()
        {
            GameObject newObj = GameObject.Instantiate(m_prefab) as GameObject;

            // Parent this gameobject if a parent is provided
            if (m_parent != null)
            {
                newObj.transform.parent = m_parent.transform;
            }

            newObj.SetActive(false);

            return newObj;
        }
    }
    #endregion    
}