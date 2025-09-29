using System.ComponentModel.DataAnnotations;

namespace AgenciaTurismo.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cliente é obrigatório.")]
        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [Required(ErrorMessage = "Pacote Turístico é obrigatório.")]
        [Display(Name = "Pacote Turístico")]
        public int? PacoteTuristicoId { get; set; }
        public PacoteTuristico? PacoteTuristico { get; set; }

        [Required(ErrorMessage = "Data da Reserva é obrigatória.")]
        [Display(Name = "Data da Reserva")]
        [DataType(DataType.Date)]
        public DateTime? DataReserva { get; set; }

        public static event Action<string>? CapacityReached;

        public static void DisparaEvento(string massege)
        {
            CapacityReached?.Invoke(massege);
        }
    }
}
