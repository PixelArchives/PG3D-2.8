using System.Collections.Generic;
using UnityEngine;

namespace Edelweiss.DecalSystem
{
	public class SkinnedDecalsMesh : GenericDecalsMesh<SkinnedDecals, SkinnedDecalProjectorBase, SkinnedDecalsMesh>
	{
		private AddSkinnedMeshDelegate m_AddSkinnedMeshDelegate;

		private List<Vector3> m_OriginalVertices;

		private List<BoneWeight> m_BoneWeights;

		private List<Transform> l_Bones;

		private List<Matrix4x4> m_BindPoses;

		public List<Vector3> OriginalVertices
		{
			get
			{
				return m_OriginalVertices;
			}
		}

		public List<BoneWeight> BoneWeights
		{
			get
			{
				return m_BoneWeights;
			}
		}

		public List<Transform> Bones
		{
			get
			{
				return l_Bones;
			}
		}

		public List<Matrix4x4> BindPoses
		{
			get
			{
				return m_BindPoses;
			}
		}
	}
}
