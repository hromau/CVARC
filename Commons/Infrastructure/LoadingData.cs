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

        public override string ToString()
        {
            return AssemblyName + " " + Level;
        }

        public static LoadingData Parse(string str)
        {
            var splited = str.Split(' ');
            return new LoadingData {AssemblyName = splited[0], Level = splited[1]};
        }
    }
}
