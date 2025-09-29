using AgenciaTurismo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgenciaTurismo.Pages.PacotesTuristicos
{
    public class PacoteDescontoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PacoteDescontoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public delegate double Desconto(double precoOriginalProduto);

        public static double CalculateDelegate(double precoOriginalProduto)
        {
            return precoOriginalProduto - precoOriginalProduto * 0.10;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public double PrecoOriginalProduto { get; set; }

        public double? PrecoComDesconto { get; set; }

        public string? NomePacote { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var pacote = await _context.PacoteTuristicos.FindAsync(Id);

            if (pacote == null || !pacote.Preco.HasValue)
            {
                return NotFound();
            }

            PrecoOriginalProduto = (double)pacote.Preco.Value;
            NomePacote = pacote.Titulo;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Desconto precoComDesconto = CalculateDelegate;
            PrecoComDesconto = precoComDesconto(PrecoOriginalProduto);

            // Recarrega o nome do pacote após o post
            var pacote = await _context.PacoteTuristicos.FindAsync(Id);
            NomePacote = pacote?.Titulo;

            return Page();
        }
    }
}
