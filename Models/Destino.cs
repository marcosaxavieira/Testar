namespace AgenciaTurismo.Models
{
    public class Destino
    {
        public int Id { get; set; }

        public string? Cidade { get; set; }
        public string? Pais { get; set; }
        public bool? IsDeleted { get; set; } = false;  
    }
}
