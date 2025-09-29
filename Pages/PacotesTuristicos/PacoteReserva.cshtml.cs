using AgenciaTurismo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AgenciaTurismo.Pages.PacotesTuristicos
{
    public class PacoteReservaModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public PacoteReservaModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public int quantidadeReserva { get; set; } = 0;
        public decimal totalReserva { get; set; } = decimal.Zero;

        public PacoteTuristico Pacote { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Pacote = await _context.PacoteTuristicos
                .Include(p => p.Reservas)
                .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Pacote == null)
            {
                return NotFound();

            }
            else
            {
                quantidadeReserva = Pacote.Reservas.Count;

                Func<int, decimal, decimal> calcularTotal = (quantidade, preco) => quantidade * preco;

                totalReserva = Pacote.Preco.HasValue ?
                               calcularTotal(quantidadeReserva, Pacote.Preco.Value) : 0;
            }

            return Page();
        }
    }
}
