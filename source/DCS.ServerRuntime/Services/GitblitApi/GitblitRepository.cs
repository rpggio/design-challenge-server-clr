namespace DCS.ServerRuntime.Services.GitblitApi
{
    /// <summary>
    /// https://github.com/gitblit/gitblit/blob/3e0c6ca8a65bd4b076cac1451c9cdfde4be1d4b8/src/main/java/com/gitblit/models/RepositoryModel.java
    /// </summary>
    public class GitblitRepository
    {
        public string name { get; set; }
        public string description { get; set; }
        public string owner { get; set; }
        public string accessRestriction { get; set; }
    }
}