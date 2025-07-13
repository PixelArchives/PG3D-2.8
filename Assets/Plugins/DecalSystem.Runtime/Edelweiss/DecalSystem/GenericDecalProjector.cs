namespace Edelweiss.DecalSystem
{
	public abstract class GenericDecalProjector<D, P, DM> : GenericDecalProjectorBase where D : GenericDecals<D, P, DM> where P : GenericDecalProjector<D, P, DM> where DM : GenericDecalsMesh<D, P, DM>
	{
		private DM m_DecalsMesh;
	}
}
