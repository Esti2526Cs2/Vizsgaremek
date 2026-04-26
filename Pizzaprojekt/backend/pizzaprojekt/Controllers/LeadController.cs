// ASP.NET Core MVC kontrollerhez szükséges névterek importálása
using Microsoft.AspNetCore.Mvc;
// Entity Framework Core az adatbázis műveletekhez
using Microsoft.EntityFrameworkCore;
// Projekt-specifikus adatbázis kontextus importálása
using pizzaprojekt.Database;
// Projekt modelljeinek importálása, például Lead
using pizzaprojekt.Models;
// Validációs attribútumok importálása (pl. [Required])
using System.ComponentModel.DataAnnotations;

namespace pizzaprojekt.Controllers
{
    // DTO a Lead létrehozásához vagy frissítéséhez
    public class LeadDTO
    {
        [Required]
        public int RendeloAzonosito { get; set; } // Kötelező mező, a vásárló azonosítója

        [Required]
        public int RendelesiId { get; set; } // Kötelező mező, a rendelés azonosítója
    }

    // DTO a Lead lekérdezéséhez vagy visszaküldéséhez
    public class LeadOutDTO
    {
        public int RendeloAzonosito { get; set; } // Vásárló azonosító visszaküldése
        public int RendelesiId { get; set; } // Rendelés azonosító visszaküldése
    }

    // API controller a Lead tábla kezeléséhez
    [Route("api/[controller]")] // Automatikusan a controller neve alapján generált útvonal (pl. api/lead)
    [ApiController] // Ez egy API controller, automatikusan kezeli a model validációt és a válaszformátumot
    public class LeadController : ControllerBase
    {
        private readonly PizzaContext _ctx; // Privát mező az adatbázis kontextus tárolásához

        // Konstruktor az adatbázis kontextus injektálásához
        public LeadController(PizzaContext ctx)
        {
            _ctx = ctx; // Inicializáljuk a privát mezőt a konstruktor paraméterével
        }

        // GET: api/lead
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeadOutDTO>>> Get()
        {
            // Lekérjük az összes Lead rekordot az adatbázisból és DTO-ba konvertáljuk
            return await _ctx.Lead
                .Select(l => new LeadOutDTO
                {
                    RendeloAzonosito = l.RendeloAzonosito, // DTO mezőbe másoljuk a vásárló azonosítóját
                    RendelesiId = l.RendelesiId // DTO mezőbe másoljuk a rendelés azonosítóját
                })
                .ToListAsync(); // Aszinkron lekérdezés listába gyűjtve
        }

        // GET api/lead/{rendeloAzonosito}/{rendelesiId}
        [HttpGet("{rendeloAzonosito:int}/{rendelesiId:int}")]
        public async Task<ActionResult<LeadOutDTO>> Get(int rendeloAzonosito, int rendelesiId)
        {
            // Lekérjük a konkrét Lead rekordot a vásárló és rendelés azonosító alapján
            var result = await _ctx.Lead
                .Where(l => l.RendeloAzonosito == rendeloAzonosito && l.RendelesiId == rendelesiId) // Szűrés két mező alapján
                .Select(l => new LeadOutDTO
                {
                    RendeloAzonosito = l.RendeloAzonosito,
                    RendelesiId = l.RendelesiId
                })
                .FirstOrDefaultAsync(); // Csak az első találatot adjuk vissza, vagy null-t ha nincs találat

            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404-et adunk vissza

            return result; // Ha van, visszaküldjük DTO formátumban
        }

        // POST api/lead
        [HttpPost]
        public async Task<ActionResult<LeadOutDTO>> Post([FromBody] LeadDTO dto)
        {
            // Ellenőrizzük a model validációt, ha hibás, 400 Bad Request-et adunk vissza
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Új Lead entitás létrehozása DTO alapján
            var result = new Lead
            {
                RendeloAzonosito = dto.RendeloAzonosito, // Vásárló azonosító beállítása
                RendelesiId = dto.RendelesiId // Rendelés azonosító beállítása
            };

            // Hozzáadjuk az új rekordot az adatbázishoz
            _ctx.Lead.Add(result);
            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat

            // Visszaadjuk az új rekordot DTO formátumban és 201 Created státusszal
            return CreatedAtAction(
                nameof(Get), // Hivatkozás a GET metódusra az új rekord lekéréséhez
                new { rendeloAzonosito = result.RendeloAzonosito, rendelesiId = result.RendelesiId }, // Útvonal paraméterek
                new LeadOutDTO
                {
                    RendeloAzonosito = result.RendeloAzonosito,
                    RendelesiId = result.RendelesiId
                });
        }

        // PUT api/lead/{rendeloAzonosito}/{rendelesiId}
        [HttpPut("{rendeloAzonosito:int}/{rendelesiId:int}")]
        public async Task<IActionResult> Put(int rendeloAzonosito, int rendelesiId, [FromBody] LeadDTO dto)
        {
            // Validáció ellenőrzése
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lekérjük a frissítendő Lead rekordot az adatbázisból
            var result = await _ctx.Lead
                .FirstOrDefaultAsync(l => l.RendeloAzonosito == rendeloAzonosito && l.RendelesiId == rendelesiId);

            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404

            // DTO adatokkal frissítjük az entitást
            result.RendeloAzonosito = dto.RendeloAzonosito;
            result.RendelesiId = dto.RendelesiId;

            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat
            return NoContent(); // Nincs visszaadott tartalom, csak 204 No Content státusz
        }

        // DELETE api/lead/{rendeloAzonosito}/{rendelesiId}
        [HttpDelete("{rendeloAzonosito:int}/{rendelesiId:int}")]
        public async Task<IActionResult> Delete(int rendeloAzonosito, int rendelesiId)
        {
            // Lekérjük a törlendő rekordot
            var result = await _ctx.Lead
                .FirstOrDefaultAsync(l => l.RendeloAzonosito == rendeloAzonosito && l.RendelesiId == rendelesiId);

            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404

            // Töröljük a rekordot az adatbázisból
            _ctx.Lead.Remove(result);
            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat
            return NoContent(); // 204 No Content visszaadása
        }
    }
}