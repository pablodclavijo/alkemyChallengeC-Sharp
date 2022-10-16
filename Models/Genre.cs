namespace AlkemyChallenge.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }= string.Empty;
        public string Img { get; set;} = string.Empty;
        public virtual List<Movie> Movies { get; set; }

    }
}
