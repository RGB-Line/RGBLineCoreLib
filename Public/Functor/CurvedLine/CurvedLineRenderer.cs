﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace RGBLineCoreLib.Functor
{
	/// <summary>
	/// Curved Line을 그리기 위한 Renderer
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
	public class CurvedLineRenderer : MonoBehaviour
	{
		//PUBLIC
		public float lineSegmentSize = 0.15f;
		public float lineWidth = 0.1f;
		[Header("Gizmos")]
		public bool showGizmos = true;
		public float gizmoSize = 0.1f;
		public Color gizmoColor = new Color(1, 0, 0, 0.5f);
		//PRIVATE
		private CurvedLinePoint[] linePoints = new CurvedLinePoint[0];
		private Vector3[] linePositions = new Vector3[0];
		private Vector3[] linePositionsOld = new Vector3[0];

		// Update is called once per frame
		public void Update()
		{
			GetPoints();
			SetPointsToLine();
		}

		private void GetPoints()
		{
			//find curved points in children
			linePoints = this.GetComponentsInChildren<CurvedLinePoint>();

			//add positions
			linePositions = new Vector3[linePoints.Length];
			for (int i = 0; i < linePoints.Length; i++)
			{
				linePositions[i] = linePoints[i].transform.position;
			}
		}

		private void SetPointsToLine()
		{
			//create old positions if they dont match
			if (linePositionsOld.Length != linePositions.Length)
			{
				linePositionsOld = new Vector3[linePositions.Length];
			}

			//check if line points have moved
			bool moved = false;
			for (int i = 0; i < linePositions.Length; i++)
			{
				//compare
				if (linePositions[i] != linePositionsOld[i])
				{
					moved = true;
				}
			}

			//update if moved
			if (moved == true)
			{
				LineRenderer line = this.GetComponent<LineRenderer>();

				//get smoothed values
				Vector3[] smoothedPoints = LineSmoother.SmoothLine(linePositions, lineSegmentSize);

				//set line settings
				line.positionCount = smoothedPoints.Length;
				line.SetPositions(smoothedPoints);
				line.startWidth = lineWidth;
				line.endWidth = lineWidth;
			}
		}

		private void OnDrawGizmosSelected()
		{
			Update();
		}

		private void OnDrawGizmos()
		{
			if (linePoints.Length == 0)
			{
				GetPoints();
			}

			//settings for gizmos
			foreach (CurvedLinePoint linePoint in linePoints)
			{
				linePoint.showGizmo = showGizmos;
				linePoint.gizmoSize = gizmoSize;
				linePoint.gizmoColor = gizmoColor;
			}
		}
	}
}