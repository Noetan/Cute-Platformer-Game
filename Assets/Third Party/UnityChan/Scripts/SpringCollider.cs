﻿//
//SpringCollider for unity-chan!
//
//Original Script is here:
//ricopin / SpringCollider.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
using UnityEngine;

namespace UnityChan
{
	public class SpringCollider : MonoBehaviour
	{
		//半径
		public float radius = 0.5f;

        [SerializeField]
        bool m_showDebug = true;

		private void OnDrawGizmos()
		{
            if (m_showDebug)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, ScaledRadius);
            }
		}

        public float ScaledRadius
        {
            get
            {
                return radius * transform.lossyScale.x;
            }
        }
	}
}