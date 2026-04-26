// ASP.NET Core MVC kontroller alapjaihoz szükséges névterek importálása
using Microsoft.AspNetCore.Mvc;
// Entity Framework Core az adatbázis műveletekhez
using Microsoft.EntityFrameworkCore;
// Projekt adatbázis kontextus importálása
using pizzaprojekt.Database;
// Projekt modelljei (pl. Keri) importálása
using pizzaprojekt.Models;
// Validációs attribútumok importálása
using System.ComponentModel.DataAnnotations;

namespace pizzaprojekt.Controllers
{
    // DTO a Keri létrehozásához / frissítéséhez
    public class KeriDTO
    {
        [Required]
        public int CimId { get; set; } // Cím azonosító kötelező
        [Required]
        public int RendeloAzonosito { get; set; } // Vásárló azonosító kötelező
    }

    // DTO a Keri lekérdezéséhez / visszaküldéséhez
    public class KeriOutDTO
    {
        public int CimId { get; set; } // Cím azonosító
        public int RendeloAzonosito { get; set; } // Vásárló azonosító
    }

    // API route beállítása: pl. api/keri
    [Route("api/[controller]")]
    // Megadjuk, hogy ez egy API controller
    [ApiController]
    public class KeriController : ControllerBase
    {
        // Privát adatbázis kontextus mező
        private readonly PizzaContext _ctx; // EF DbContext a műveletekhez

        // Konstruktor: dependency injection az adatbázis kontextushoz
        public KeriController(PizzaContext ctx)
        {
            _ctx = ctx; // privát mező inicializálása
        }

        // GET: api/keri
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KeriOutDTO>>> Get()
        {
            // Lekérjük az összes Keri rekordot, DTO-ba alakítjuk
            return await _ctx.Keri
                .Select(k => new KeriOutDTO
                {
                    CimId = k.CimId, // Cím azonosító
                    RendeloAzonosito = k.RendeloAzonosito // Vásárló azonosító
                })
                .ToListAsync(); // Aszinkron lista lekérése
        }

        // GET api/keri/{cimId}/{rendeloAzonosito}
        [HttpGet("{cimId:int}/{rendeloAzonosito:int}")]
        public async Task<ActionResult<KeriOutDTO>> Get(int cimId, int rendeloAzonosito)
        {
            // Lekérjük a konkrét rekordot a két kulcs alapján
            var result = await _ctx.Keri
                .Where(k => k.CimId == cimId && k.RendeloAzonosito == rendeloAzonosito)
                .Select(k => new KeriOutDTO
                {
                    CimId = k.CimId,
                    RendeloAzonosito = k.RendeloAzonosito
                })
                .FirstOrDefaultAsync(); // Csak az első találat vagy null

            if (result == null)
                return NotFound(); // Ha nincs, 404

            return result; // Ha van, DTO visszaküldése
        }

        // POST api/keri
        [HttpPost]
        public async Task<ActionResult<KeriOutDTO>> Post([FromBody] KeriDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Ha DTO invalid, 400

            // Új Keri objektum létrehozása DTO alapján
            var result = new Keri
            {
                CimId = dto.CimId,
                RendeloAzonosito = dto.RendeloAzonosito
            };

            _ctx.Keri.Add(result); // Hozzáadás az adatbázishoz
            await _ctx.SaveChangesAsync(); // Mentés aszinkron

            // Visszaadjuk az új rekordot DTO formában, HTTP 201 státusszal
            return CreatedAtAction(
                nameof(Get), // GET metódus hivatkozása az új elem lekéréséhez
                new { cimId = result.CimId, rendeloAzonosito = result.RendeloAzonosito }, // Új rekord útvonala
                new KeriOutDTO
                {
                    CimId = result.CimId,
                    RendeloAzonosito = result.RendeloAzonosito
                });
        }

        // PUT api/keri/{cimId}/{rendeloAzonosito}
        [HttpPut("{cimId:int}/{rendeloAzonosito:int}")]
        public async Task<IActionResult> Put(
            int cimId,
            int rendeloAzonosito,
            [FromBody] KeriDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Ha DTO invalid, 400

            // Megkeressük a rekordot a két kulcs alapján
            var result = await _ctx.Keri
                .FirstOrDefaultAsync(k =>
                    k.CimId == cimId &&
                    k.RendeloAzonosito == rendeloAzonosito);

            if (result == null)
                return NotFound(); // Ha nincs, 404

            // DTO adatokkal frissítjük a rekordot
            result.CimId = dto.CimId;
            result.RendeloAzonosito = dto.RendeloAzonosito;

            await _ctx.SaveChangesAsync(); // Mentés aszinkron
            return NoContent(); // 204 No Content vissza
        }

        // DELETE api/keri/{cimId}/{rendeloAzonosito}
        [HttpDelete("{cimId:int}/{rendeloAzonosito:int}")]
        public async Task<IActionResult> Delete(int cimId, int rendeloAzonosito)
        {
            // Megkeressük a rekordot a két kulcs alapján
            var result = await _ctx.Keri
                .FirstOrDefaultAsync(k => k.CimId == cimId && k.RendeloAzonosito == rendeloAzonosito);

            if (result == null)
                return NotFound(); // Ha nincs, 404

            _ctx.Keri.Remove(result); // Törlés az adatbázisból
            await _ctx.SaveChangesAsync(); // Mentés aszinkron

            return NoContent(); // 204 No Content vissza
        }
    }
}