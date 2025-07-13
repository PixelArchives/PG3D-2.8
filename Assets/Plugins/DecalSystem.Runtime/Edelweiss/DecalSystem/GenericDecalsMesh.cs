using System.Collections.Generic;
using UnityEngine;

namespace Edelweiss.DecalSystem
{
	public abstract class GenericDecalsMesh<D, P, DM> : GenericDecalsMeshBase where D : GenericDecals<D, P, DM> where P : GenericDecalProjector<D, P, DM> where DM : GenericDecalsMesh<D, P, DM>
	{
		protected D m_Decals;

		private List<P> m_Projectors;

		internal RemoveRangeDelegate m_RemoveRangeDelegate;

		internal List<P> Projectors
		{
			get
			{
				return m_Projectors;
			}
		}

		protected override void RecalculateUVs()
		{
			if (!((Object)m_Decals != (Object)null) || m_Decals.CurrentUVMode != 0)
			{
				return;
			}
			foreach (P projector in m_Projectors)
			{
				P current = projector;
				if (!current.IsUV1ProjectionCalculated)
				{
					CalculateProjectedUV1(current);
				}
			}
		}

		protected override bool HasUV2LightmappingMode()
		{
			bool result = true;
			if ((Object)m_Decals != (Object)null && m_Decals.CurrentUV2Mode != UV2Mode.Lightmapping)
			{
				result = false;
			}
			return result;
		}

		protected override void RecalculateUV2s()
		{
			if (!((Object)m_Decals != (Object)null) || m_Decals.CurrentUV2Mode != UV2Mode.Project)
			{
				return;
			}
			foreach (P projector in m_Projectors)
			{
				if (!projector.IsUV2ProjectionCalculated)
				{
					CalculateProjectedUV2(projector);
				}
			}
		}

		protected override void RecalculateTangents()
		{
			if (!((Object)m_Decals != (Object)null) || m_Decals.CurrentTangentsMode != TangentsMode.Project)
			{
				return;
			}
			foreach (P projector in m_Projectors)
			{
				if (!projector.IsTangentProjectionCalculated)
				{
					CalculateProjectedTangents(projector);
				}
			}
		}

		private void CalculateProjectedUV1(GenericDecalProjectorBase a_Projector)
		{
			Matrix4x4 a_DecalsToProjectorMatrix = a_Projector.WorldToProjectorMatrix * m_Decals.CachedTransform.localToWorldMatrix;
			UVRectangle a_UVRectangle = m_Decals.uvRectangles[a_Projector.UV1RectangleIndex];
			List<Vector2> uVs = m_UVs;
			int decalsMeshLowerVertexIndex = a_Projector.DecalsMeshLowerVertexIndex;
			int decalsMeshUpperVertexIndex = a_Projector.DecalsMeshUpperVertexIndex;
			CalculateProjectedUV(a_DecalsToProjectorMatrix, a_UVRectangle, uVs, decalsMeshLowerVertexIndex, decalsMeshUpperVertexIndex);
			a_Projector.IsUV1ProjectionCalculated = true;
		}

		private void CalculateProjectedUV2(GenericDecalProjectorBase a_Projector)
		{
			Matrix4x4 a_DecalsToProjectorMatrix = a_Projector.WorldToProjectorMatrix * m_Decals.CachedTransform.localToWorldMatrix;
			UVRectangle a_UVRectangle = m_Decals.uv2Rectangles[a_Projector.UV2RectangleIndex];
			List<Vector2> uV2s = m_UV2s;
			int decalsMeshLowerVertexIndex = a_Projector.DecalsMeshLowerVertexIndex;
			int decalsMeshUpperVertexIndex = a_Projector.DecalsMeshUpperVertexIndex;
			CalculateProjectedUV(a_DecalsToProjectorMatrix, a_UVRectangle, uV2s, decalsMeshLowerVertexIndex, decalsMeshUpperVertexIndex);
			a_Projector.IsUV2ProjectionCalculated = true;
		}

		private void CalculateProjectedUV(Matrix4x4 a_DecalsToProjectorMatrix, UVRectangle a_UVRectangle, List<Vector2> a_UVs, int a_LowerIndex, int a_UpperIndex)
		{
			Vector2 lowerLeftUV = a_UVRectangle.lowerLeftUV;
			Vector2 size = a_UVRectangle.Size;
			while (a_UVs.Count < a_LowerIndex)
			{
				a_UVs.Add(Vector2.zero);
			}
			for (int i = a_LowerIndex; i <= a_UpperIndex; i++)
			{
				Vector3 v = base.Vertices[i];
				v = a_DecalsToProjectorMatrix.MultiplyPoint3x4(v);
				float x = lowerLeftUV.x + (v.x + 0.5f) * size.x;
				float y = lowerLeftUV.y + (v.z + 0.5f) * size.y;
				Vector2 vector = new Vector2(x, y);
				if (i < a_UVs.Count)
				{
					a_UVs[i] = vector;
				}
				else
				{
					a_UVs.Add(vector);
				}
			}
		}

		private void CalculateProjectedTangents(GenericDecalProjectorBase a_Projector)
		{
			while (m_Tangents.Count < a_Projector.DecalsMeshLowerVertexIndex)
			{
				m_Tangents.Add(Vector4.zero);
			}
			Matrix4x4 transpose = (m_Decals.CachedTransform.localToWorldMatrix * a_Projector.WorldToProjectorMatrix).inverse.transpose;
			Matrix4x4 transpose2 = (a_Projector.ProjectorToWorldMatrix * m_Decals.CachedTransform.worldToLocalMatrix).inverse.transpose;
			for (int i = a_Projector.DecalsMeshLowerVertexIndex; i <= a_Projector.DecalsMeshUpperVertexIndex; i++)
			{
				Vector3 v = base.Normals[i];
				v = transpose.MultiplyVector(v);
				v.z = 0f;
				if (Mathf.Approximately(v.x, 0f) && Mathf.Approximately(v.y, 0f))
				{
					v = new Vector3(0f, 1f, 0f);
				}
				v = new Vector3(v.y, 0f - v.x, v.z);
				v = transpose2.MultiplyVector(v);
				v.Normalize();
				Vector4 vector = new Vector4(v.x, v.y, v.z, -1f);
				if (i < m_Tangents.Count)
				{
					m_Tangents[i] = vector;
				}
				else
				{
					m_Tangents.Add(vector);
				}
			}
		}
	}
}
