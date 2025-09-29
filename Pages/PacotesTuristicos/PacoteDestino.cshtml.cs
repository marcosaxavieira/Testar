using AgenciaTurismo.Data;
using AgenciaTurismo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgenciaTurismo.Pages.PacotesTuristicos
{
    public class PacoteDestinoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PacoteDestinoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int DestinoSelecionadoId { get; set; }

        [BindProperty]
        public bool MostrarSelecao { get; set; }

        public SelectList DestinosSelectList { get; set; }

        public PacoteTuristico Pacote { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await CarregarDados(id);
            return Page();
        }

        public async Task<IActionResult> OnPostMostrarSelecaoAsync(int id)
        {
            MostrarSelecao = true;
            await CarregarDados(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAdicionarDestinoAsync(int id)
        {
            var pacote = await _context.PacoteTuristicos
                .Include(p => p.Destinos)
                .FirstOrDefaultAsync(p => p.Id == id);

            var destino = await _context.Destinos.FindAsync(DestinoSelecionadoId);

            if (pacote != null && destino != null && !pacote.Destinos.Any(d => d.Id == destino.Id))
            {
                pacote.Destinos.Add(destino);
                await _context.SaveChangesAsync();
            }

            MostrarSelecao = false;
            await CarregarDados(id);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, int destinoId)
        {
            var pacote = await _context.PacoteTuristicos
                .Include(p => p.Destinos)
                .FirstOrDefaultAsync(p => p.Id == id);

            var destino = pacote.Destinos.FirstOrDefault(d => d.Id == destinoId);
            if (destino != null)
            {
                pacote.Destinos.Remove(destino);
                await _context.SaveChangesAsync();
            }

            await CarregarDados(id);
            return Page();
        }

        private async Task CarregarDados(int id)
        {
            Pacote = await _context.PacoteTuristicos
                .Include(p => p.Destinos)
                .FirstOrDefaultAsync(p => p.Id == id);

            var destinos = await _context.Destinos.ToListAsync();
            DestinosSelectList = new SelectList(destinos.Select(d => new
            {
                d.Id,
                CidadePais = $"{d.Cidade} - {d.Pais}"
            }), "Id", "CidadePais");
        }
    }
}
