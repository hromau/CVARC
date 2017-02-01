namespace HoMM.Generators
{
    sealed class ModifiedMapWrapper<TCell> : ImmutableSigmaMap<TCell>
    {
        public ISigmaMap<TCell> ParentMaze { get; private set; }
        public Location ModifiedLocation { get; private set; }
        public TCell ModifiedCell { get; private set; }

        public override MapSize Size { get { return ParentMaze.Size; } }

        public override TCell this[Location location]
        {
            get { return ModifiedLocation.Equals(location) ? ModifiedCell : ParentMaze[location]; }
        }

        public ModifiedMapWrapper(ISigmaMap<TCell> parent, Location modLocation, TCell modCell)
        {
            ParentMaze = parent;
            ModifiedLocation = modLocation;
            ModifiedCell = modCell;
        } 
    }
}
