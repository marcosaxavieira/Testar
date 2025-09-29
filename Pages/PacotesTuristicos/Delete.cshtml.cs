
using AgenciaTurismo.Data;
using AgenciaTurismo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AgenciaTurismo.Pages.PacotesTuristicos
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PacoteTuristico PacoteTuristico { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PacoteTuristico = await _context.PacoteTuristicos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (PacoteTuristico == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PacoteTuristico = await _context.PacoteTuristicos
                .Include(p => p.Destinos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (PacoteTuristico == null)
            {
                return NotFound();
            }

            if (PacoteTuristico.Destinos.Any())
            {
                ModelState.AddModelError(string.Empty, "Precisa excluir os Destinos antes de Excluir o Pacote.");
                return Page();
            }

            _context.PacoteTuristicos.Remove(PacoteTuristico);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
