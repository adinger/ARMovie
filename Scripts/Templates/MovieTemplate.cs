// Json template class for movie returned by TMDB
public class TMDBMovie
{
    // http://api.themoviedb.org/3/movie/500?api_key=29f5a1a60fdd076bb423b169120cd2a8

    //public int id { get; set; }
    public string imdb_id { get; set; }
    public string title { get; set; }
    public string release_date { get; set; }
    public double vote_average { get; set; }
    public string overview { get; set; }
    public string poster_path { get; set; }
}

// Json template class for movie returned by OMDB
public class OMDBMovie
{
    // http://www.omdbapi.com/?t=inception&y=&plot=short&r=json
    public string Title { get; set; }
    public string Year { get; set; }
    public string Rated { get; set; }
    public string Released { get; set; }
    public string Runtime { get; set; }
    public string Genre { get; set; }
    public string Director { get; set; }
    public string Writer { get; set; }
    public string Actors { get; set; }
    public string Plot { get; set; }
    public string Language { get; set; }
    public string Country { get; set; }
    public string Awards { get; set; }
    public string Poster { get; set; }
    public string Metascore { get; set; }
    public string imdbRating { get; set; }
    public string imdbID { get; set; }
}

public class TMDBandOMDB
{
    public OMDBMovie om;
    public TMDBMovie tm;

    public TMDBandOMDB(OMDBMovie omm, TMDBMovie tmm)
    {
        om = omm;
        tm = tmm;
    }
}
