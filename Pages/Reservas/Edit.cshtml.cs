using AgenciaTurismo.Data;
using AgenciaTurismo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgenciaTurismo.Pages.Reservas
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;

            // Atribuição do delegate para capacidade de reservas
            Reserva.CapacityReached += Logger.LogToConsole;
        }

        [BindProperty]
        public Reserva Reserva { get; set; } = default!;

        // Totaliza o número de reservas do pacote
        public int totalReservas { get; set; }

        // Criação da função delegate.
        public static class Logger
        {
            public static void LogToConsole(string message)
            {
                Console.WriteLine(message);
            }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }
            Reserva = reserva;
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["PacoteTuristicoId"] = new SelectList(_context.PacoteTuristicos, "Id", "Titulo");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
                ViewData["PacoteTuristicoId"] = new SelectList(_context.PacoteTuristicos, "Id", "Titulo");
                return Page();
            }

            // Verifica se os IDs estão preenchidos
            if (Reserva.ClienteId == null || Reserva.PacoteTuristicoId == null)
            {
                ModelState.AddModelError(string.Empty, "Cliente e Pacote Turístico são obrigatórios.");
                return Page();
            }

            // Carrega o pacote turístico completo
            var pacote = await _context.PacoteTuristicos
                .FirstOrDefaultAsync(p => p.Id == Reserva.PacoteTuristicoId);

            if (pacote == null)
            {
                ModelState.AddModelError(string.Empty, "Pacote turístico não encontrado.");
                return Page();
            }

            // totaliza o numero de reservas
            totalReservas = await _context.Reservas
                                .CountAsync(r => r.PacoteTuristicoId == Reserva.PacoteTuristicoId) + 1;

            if (Reserva.PacoteTuristico?.CapacidadeMaxima.HasValue == true)
            {
                if (totalReservas > Reserva.PacoteTuristico.CapacidadeMaxima.Value)
                {
                    // Aqui você pode disparar o evento e mostra a mensagem
                    Reserva.DisparaEvento($"Número de reservas chegou ao limite do pacote");

                    ModelState.AddModelError(string.Empty, "Número de reservas já atingiu o limite do pacote.");

                    return Page();
                }
            }

            _context.Attach(Reserva).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(Reserva.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
