// ASP.NET Core MVC és EF Core névterek importálása
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizzaprojekt.Database; // Adatbázis kontextus
using pizzaprojekt.Models; // Modellek (Feltetek)
using System.ComponentModel.DataAnnotations; // Validációs attribútumok (Required, Range, stb.)

namespace pizzaprojekt.Controllers
{
    // DTO a Feltét létrehozásához vagy frissítéséhez
    public class FeltetekDTO
    {
        [Required] // A név mező kötelező, nem lehet null vagy üres
        public string Nev { get; set; } = string.Empty;

        [Range(1, 10000)] // Az ár 1 és 10 000 közötti érték lehet
        public int Ar { get; set; }
    }

    // DTO a Feltét lekérdezéséhez vagy visszaküldéséhez
    public class FeltetekOutDTO
    {
        public int Id { get; set; } // A feltét azonosítója
        public string? Nev { get; set; } // A feltét neve
        public int Ar { get; set; } // A feltét ára
    }

    // Controller a Feltetek táblához
    [Route("api/[controller]")] // Útvonal alapja a controller neve (api/feltetek)
    [ApiController] // Automatikusan kezeli a model validációt és JSON formátumú válaszokat
    public class FeltetekController : ControllerBase
    {
        private readonly PizzaContext _ctx; // Privát mező az adatbázis kontextushoz

        // Konstruktor az adatbázis kontextus injektálásához
        public FeltetekController(PizzaContext ctx)
        {
            _ctx = ctx; // Inicializáljuk a privát mezőt
        }

        // GET: api/feltetek
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeltetekOutDTO>>> Get()
        {
            // Az összes Feltét lekérdezése az adatbázisból DTO formátumban
            return await _ctx.Feltetek
                .Select(f => new FeltetekOutDTO
                {
                    Id = f.FeltetId,
                    Nev = f.Nev,
                    Ar = f.Feltetar
                })
                .ToListAsync(); // Aszinkron listába gyűjtve
        }

        // GET api/feltetek/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FeltetekOutDTO>> Get(int id)
        {
            // Lekérjük az adott ID-jú Feltét rekordot
            var result = await _ctx.Feltetek
                .Where(f => f.FeltetId == id)
                .Select(f => new FeltetekOutDTO
                {
                    Id = f.FeltetId,
                    Nev = f.Nev,
                    Ar = f.Feltetar
                })
                .FirstOrDefaultAsync(); // Csak az első találatot adjuk vissza

            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404-et adunk

            return result; // Ha van, DTO formátumban visszaküldjük
        }

        // POST api/feltetek
        [HttpPost]
        public async Task<ActionResult<FeltetekOutDTO>> Post([FromBody] FeltetekDTO feltetekDto)
        {
            // Validáció ellenőrzése
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Új Feltét entitás létrehozása a DTO adatokkal
            var result = new Feltetek
            {
                Nev = feltetekDto.Nev,
                Feltetar = feltetekDto.Ar
            };

            // Hozzáadjuk az adatbázishoz és mentjük
            _ctx.Feltetek.Add(result);
            await _ctx.SaveChangesAsync();

            // Visszaadjuk a létrehozott rekordot DTO formátumban 201 Created státusszal
            return CreatedAtAction(
                nameof(Get), // Hivatkozás a GET metódusra
                new { id = result.FeltetId }, // Útvonal paraméterek
                new FeltetekOutDTO
                {
                    Id = result.FeltetId,
                    Nev = result.Nev,
                    Ar = result.Feltetar
                });
        }

        // PUT api/feltetek/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<FeltetekOutDTO>> Put(int id, [FromBody] FeltetekDTO feltetekDTO)
        {
            // Validáció ellenőrzése
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lekérjük a frissítendő rekordot
            var result = await _ctx.Feltetek.FindAsync(id);
            if (result == null)
                return NotFound(); // Ha nincs rekord, 404

            // DTO alapján frissítjük az entitást
            result.Nev = feltetekDTO.Nev;
            result.Feltetar = feltetekDTO.Ar;

            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat
            return NoContent(); // 204 No Content státusz, nincs visszatérési érték
        }

        // DELETE api/feltetek/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<FeltetekOutDTO>> Delete(int id)
        {
            // Lekérjük a törlendő rekordot
            var result = await _ctx.Feltetek.FindAsync(id);
            if (result == null)
                return NotFound(); // Ha nincs rekord, 404

            // Töröljük az entitást
            _ctx.Feltetek.Remove(result);
            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat
            return NoContent(); // 204 No Content státusz visszaadása
        }
    }
}