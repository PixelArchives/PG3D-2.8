using UnityEngine;

namespace Edelweiss.DecalSystem
{
	public abstract class GenericDecalProjectorBase
	{
		private bool m_IsActiveProjector;

		private int m_DecalsMeshLowerVertexIndex;

		private int m_DecalsMeshUpperVertexIndex;

		private int m_DecalsMeshLowerTriangleIndex;

		private int m_DecalsMeshUpperTriangleIndex;

		private bool m_IsUV1ProjectionCalculated;

		private bool m_IsUV2ProjectionCalculated;

		private bool m_IsTangentProjectionCalculated;

		public int DecalsMeshLowerVertexIndex
		{
			get
			{
				return m_DecalsMeshLowerVertexIndex;
			}
		}

		public int DecalsMeshUpperVertexIndex
		{
			get
			{
				return m_DecalsMeshUpperVertexIndex;
			}
		}

		public int DecalsMeshVertexCount
		{
			get
			{
				return DecalsMeshUpperVertexIndex - DecalsMeshLowerVertexIndex + 1;
			}
		}

		public int DecalsMeshLowerTriangleIndex
		{
			get
			{
				return m_DecalsMeshLowerTriangleIndex;
			}
		}

		public int DecalsMeshUpperTriangleIndex
		{
			get
			{
				return m_DecalsMeshUpperTriangleIndex;
			}
		}

		public bool IsUV1ProjectionCalculated
		{
			get
			{
				return m_IsUV1ProjectionCalculated;
			}
			internal set
			{
				m_IsUV1ProjectionCalculated = value;
			}
		}

		public bool IsUV2ProjectionCalculated
		{
			get
			{
				return m_IsUV2ProjectionCalculated;
			}
			internal set
			{
				m_IsUV2ProjectionCalculated = value;
			}
		}

		public bool IsTangentProjectionCalculated
		{
			get
			{
				return m_IsTangentProjectionCalculated;
			}
		}

		public abstract Vector3 Position { get; }

		public abstract Quaternion Rotation { get; }

		public abstract Vector3 Scale { get; }

		public abstract int UV1RectangleIndex { get; }

		public abstract int UV2RectangleIndex { get; }

		public Matrix4x4 ProjectorToWorldMatrix
		{
			get
			{
				return Matrix4x4.TRS(Position, Rotation, Scale);
			}
		}

		public Matrix4x4 WorldToProjectorMatrix
		{
			get
			{
				return ProjectorToWorldMatrix.inverse;
			}
		}
	}
}
