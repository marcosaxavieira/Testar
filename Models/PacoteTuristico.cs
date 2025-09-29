using System.ComponentModel.DataAnnotations;

namespace AgenciaTurismo.Models
{
    public class PacoteTuristico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título do pacote é obrigatório.")]
        [MinLength(3, ErrorMessage = "O título deve ter pelo menos 3 caracteres.")]
        public string? Titulo { get; set; }
        public DateTime? DataInicio { get; set; }
        public int? CapacidadeMaxima { get; set; }
        public decimal? Preco { get; set; }
        public List<Destino>? Destinos { get; set; }

        public List<Reserva>? Reservas { get; set; }
    }
}
