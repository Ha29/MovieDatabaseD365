using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDH.MoviePlugins
{
    [DataContract]
    public partial class OmdbResponseData 
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Year { get; set; }
        [DataMember]
        public string Rated { get; set; }
        [DataMember]
        public string Released { get; set; }
        [DataMember]
        public string Runtime { get; set; }
        [DataMember]
        public string Genre { get; set; }
        [DataMember]
        public string Director { get; set; }
        [DataMember]
        public string Writer { get; set; }
        [DataMember]
        public string Actors { get; set; }
        [DataMember]
        public string Plot { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Awards { get; set; }
        [DataMember]
        public string Poster { get; set; }
        [DataMember]
        public List<Rating> Ratings { get; set; }
        [DataMember]
        public string Metascore { get; set; }
        [DataMember]
        public string imdbRating { get; set; }
        [DataMember]
        public string imdbVotes { get; set; }
        [DataMember]
        public string imdbID { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string DVD { get; set; }
        [DataMember]
        public string BoxOffice { get; set; }
        [DataMember]
        public string Production { get; set; }
        [DataMember]
        public string Website { get; set; }
        [DataMember]
        public string Response { get; set; }
        public string Json { get; set; }

    }
    [DataContract]
    public partial class Rating
    {
        [DataMember]
        public string Source { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
