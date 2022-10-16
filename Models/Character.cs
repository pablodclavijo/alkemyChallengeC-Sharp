namespace AlkemyChallenge.Models
{
    public class Character
    {
        public int Id { get; set; }

        public string Img { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Story { get; set; } = string.Empty;
        public int Age { get; set; }

        public double Weight { get; set; }
        public virtual List<Movie> Movies { get; set; }


    }
}