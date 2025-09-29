using AgenciaTurismo.Data;
using AgenciaTurismo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AgenciaTurismo.Pages.PacotesTuristicos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PacoteTuristico> PacoteTuristico { get; set; } = default!;

        public async Task OnGetAsync()
        {
            PacoteTuristico = await _context.PacoteTuristicos.ToListAsync();
        }
    }
}
