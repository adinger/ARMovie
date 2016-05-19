using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// http://api.themoviedb.org/3/search/movie?query=avengers&api_key=29f5a1a60fdd076bb423b169120cd2a8
public class PossibleMovies
{
    public List<PossibleMovie> results;
}

public class PossibleMovie
{
    public int id { get; set; }
}

// 
public class Review
{
    public string author { get; set; }
    public string content { get; set; }
}

public class Reviews
{
    public int id { get; set; }
    public List<Review> results { get; set; }
}

// 
public class Video
{
    public string key { get; set; }
    public string name { get; set; }
    public string type { get; set; }
}

public class Videos
{
    public int id { get; set; }
    public List<Video> results { get; set; }
}
