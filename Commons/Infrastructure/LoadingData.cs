namespace Infrastructure
{
    public class LoadingData
    {
        public string AssemblyName { get; set; }
        public string Level { get; set; }

        public override bool Equals(object obj)
        {
            var loadingData = obj as LoadingData;
            if (loadingData == null)
                return false;

            return string.Equals(AssemblyName, loadingData.AssemblyName) && 
                string.Equals(Level, loadingData.Level);
        }

        public override int GetHashCode()
        {
            return AssemblyName.GetHashCode()/2 + Level.GetHashCode()/2;
        }
    }
}
