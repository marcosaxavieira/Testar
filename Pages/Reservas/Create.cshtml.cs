using AgenciaTurismo.Data;
using AgenciaTurismo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IOFile = System.IO.File;

namespace AgenciaTurismo.Pages.Reservas
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;

            // Atribuição do delegate para capacidade de reservas
            Reserva.CapacityReached += Logger.LogToConsole;
        }

        [BindProperty]
        public Reserva Reserva { get; set; } = default!;

        // Totaliza o número de reservas do pacote
        public int totalReservas { get; set; }

        // Criação das funções para o Multicast Deletate (02)
        public static class Logger
        {
            public static void LogToConsole(string message)
            {
                Console.WriteLine(message);
            }

            public static void LogToFile(string message)
            {
                string path = "Arquivos/Exercicio02.txt";
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                IOFile.AppendAllText(path, $"{message}{Environment.NewLine}");
            }

            public static List<string> MemoryLog = new();

            public static void LogToMemory(string message)
            {
                MemoryLog.Add(message);
            }

        }
        public IActionResult OnGet()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["PacoteTuristicoId"] = new SelectList(_context.PacoteTuristicos, "Id", "Titulo");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Recarrega os dropdowns em caso de erro
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["PacoteTuristicoId"] = new SelectList(_context.PacoteTuristicos, "Id", "Titulo");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verifica se os IDs estão preenchidos
            if (Reserva.ClienteId == null || Reserva.PacoteTuristicoId == null)
            {
                ModelState.AddModelError(string.Empty, "Cliente e Pacote Turístico são obrigatórios.");
                return Page();
            }

            // Atribuição das funções logs ao Multicast Delegate 
            Action<string> logs = Logger.LogToConsole;
            logs += Logger.LogToFile;
            logs += Logger.LogToMemory;

            // Execuçao das funções logs ao Multicast Delegate 
            Reserva.Cliente = await _context.Clientes.FindAsync(Reserva.ClienteId);
            Reserva.PacoteTuristico = await _context.PacoteTuristicos.FindAsync(Reserva.PacoteTuristicoId);
            string message = $"[RESERVA] Cliente: {Reserva.Cliente?.Nome} Pacote: {Reserva.PacoteTuristico?.Titulo} Data: {Reserva.DataReserva:dd-MM-yyyy}";
            logs.Invoke(message);

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

            // Adiciona e salva
            _context.Reservas.Add(Reserva);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
