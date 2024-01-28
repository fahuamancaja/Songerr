namespace Songerr.Models
{
    namespace Songerr.Models
    {
        public class Artist : IArtist
        {
            public string Name { get; set; }
            // Add other fields as needed
        }

        public class Album : IAlbum
        {
            public string Title { get; set; }
            public string ReleaseDate { get; set; }
            // Add other fields as needed
        }
    }

}
