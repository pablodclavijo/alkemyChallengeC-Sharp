namespace AlkemyChallenge.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty ;
        public float Rating { get; set; }
        public string Date { get; set; } = string.Empty;

        public virtual List<Character> Characters { get; set; } 
        public virtual List<Genre> Genres { get; set; } 

    }
}
