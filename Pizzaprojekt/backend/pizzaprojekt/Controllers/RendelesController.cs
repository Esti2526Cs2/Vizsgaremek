// ASP.NET Core MVC kontroller alapjaihoz szükséges névterek importálása
using Microsoft.AspNetCore.Mvc;
// Entity Framework Core az adatbázis műveletekhez
using Microsoft.EntityFrameworkCore;
// Projekt adatbázis kontextus importálása
using pizzaprojekt.Database;
// Projekt modelljei (pl. Rendeles) importálása
using pizzaprojekt.Models;
// Validációs attribútumok (pl. [Required], [Range]) importálása
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pizzaprojekt.Controllers
{
    // API route beállítása: pl. api/rendeles
    [Route("api/[controller]")]
    // Megadjuk, hogy ez egy API controller
    [ApiController]
    public class RendelesController : ControllerBase
    {
        // DTO a Rendeles létrehozásához / frissítéséhez
        public class RendelesDTO
        {
            [Required]
            [JsonPropertyName("datum")]
            public DateTime Datum { get; set; }

            [Required]
            [JsonPropertyName("statusz")]
            public string Statusz { get; set; } = string.Empty;

            [Required]
            [JsonPropertyName("fizetesiMod")]
            public string Fizetes { get; set; } = string.Empty;

            [Required]
            [JsonPropertyName("vasarloId")]
            public int VasarloId { get; set; }
        }

        // DTO a Rendeles lekérdezéséhez / visszaküldéséhez
        public class RendelesOutDTO
        {
            public int Id { get; set; }
            public DateTime Datum { get; set; }
            [JsonPropertyName("fizetesiMod")]
            public string? Fizetes { get; set; }
            public string? Statusz { get; set; }
            public int VasarloId { get; set; }
        }

        // Privát adatbázis kontextus mező
        private readonly PizzaContext _ctx; // EF DbContext a műveletekhez

        // Konstruktor: dependency injection az adatbázis kontextushoz
        public RendelesController(PizzaContext ctx)
        {
            _ctx = ctx; // privát mező inicializálása
        }

        // GET: api/rendeles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RendelesOutDTO>>> Get()
        {
            // Lekérjük az összes rendelést, DTO-ba alakítjuk
            return await _ctx.Rendeles
                .Select(r => new RendelesOutDTO
                {
                    Id = r.RendelesId, // Adatbázis ID -> DTO
                    Datum = r.Datum, // Dátum
                    Fizetes = r.FizetesMod, // Fizetés módja
                    Statusz = r.Statusz, // Státusz

                })
                .ToListAsync(); // Aszinkron lista lekérése
        }

        // GET api/rendeles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RendelesOutDTO>> Get(int id)
        {
            // Lekérjük a konkrét rendelést ID alapján
            var result = await _ctx.Rendeles
                .Where(r => r.RendelesId == id) // Szűrés ID alapján
                .Select(r => new RendelesOutDTO
                {
                    Id = r.RendelesId,
                    Datum = r.Datum,
                    Fizetes = r.FizetesMod,
                    Statusz = r.Statusz,

                })
                .FirstOrDefaultAsync(); // Csak az első találat vagy null

            if (result == null)
                return NotFound(); // Ha nincs találat, 404

            return result; // Ha van, DTO visszaküldése
        }

        // POST api/rendeles
        [HttpPost]
        public async Task<ActionResult<RendelesOutDTO>> Post([FromBody] RendelesDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vasarlo = await _ctx.Vasarlo.FindAsync(dto.VasarloId);
            if (vasarlo == null)
                return BadRequest("Nincs ilyen vásárló");

            var result = new Rendeles
            {
                FizetesMod = dto.Fizetes,
                Statusz = dto.Statusz,

                Datum= dto.Datum,
            };

            _ctx.Rendeles.Add(result);
            await _ctx.SaveChangesAsync();
            return CreatedAtAction(
                nameof(Get),
                new { id = result.RendelesId },
                new RendelesOutDTO
                {
                    Id = result.RendelesId,
                    Fizetes = result.FizetesMod,
                    Statusz = result.Statusz,

                    Datum = result.Datum,
                });
        }



            // Visszaadjuk az új rekordot DTO formában, HTTP 201 státusszal
    

        // PUT api/rendeles/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] RendelesDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Ha DTO invalid, 400

            // Megkeressük a rendelést ID alapján
            var result = await _ctx.Rendeles.FindAsync(id);
            if (result == null)
                return NotFound(); // Ha nincs, 404

            // DTO adatokkal frissítjük a rekordot
            result.Datum = dto.Datum;
            result.FizetesMod = dto.Fizetes;
            result.Statusz = dto.Statusz;
            result.Datum = dto.Datum;

            await _ctx.SaveChangesAsync(); // Mentés aszinkron
            return NoContent(); // 204 No Content vissza
        }

        // DELETE api/rendeles/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rendeles>> Delete(int id)
        {
            // Megkeressük a rendelést ID alapján
            var result = await _ctx.Rendeles.FindAsync(id);
            if (result == null)
                return NotFound(); // Ha nincs, 404

            _ctx.Rendeles.Remove(result); // Törlés az adatbázisból
            await _ctx.SaveChangesAsync(); // Mentés aszinkron

            return NoContent(); // 204 No Content vissza
        }
    }
}