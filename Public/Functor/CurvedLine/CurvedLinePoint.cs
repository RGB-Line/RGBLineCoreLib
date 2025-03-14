﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
	public class CurvedLinePoint : MonoBehaviour
	{
		[HideInInspector] public bool showGizmo = true;
		[HideInInspector] public float gizmoSize = 0.1f;
		[HideInInspector] public Color gizmoColor = new Color(1, 0, 0, 0.5f);

		void OnDrawGizmos()
		{
			if (showGizmo == true)
			{
				Gizmos.color = gizmoColor;

				Gizmos.DrawSphere(this.transform.position, gizmoSize);
			}
		}

		//update parent line when this point moved
		void OnDrawGizmosSelected()
		{
			CurvedLineRenderer curvedLine = this.transform.parent.GetComponent<CurvedLineRenderer>();

			if (curvedLine != null)
			{
				curvedLine.Update();
			}
		}
	}
}