// ASP.NET Core MVC kontroller alapjaihoz szükséges névterek importálása
using Microsoft.AspNetCore.Mvc;
// Entity Framework Core a relációs adatbázis műveletekhez
using Microsoft.EntityFrameworkCore;
// Projekt-specifikus adatbázis kontextus importálása
using pizzaprojekt.Database;
// Projekt modelljeinek importálása, például PizzaFeltetek
using pizzaprojekt.Models;
// Validációs attribútumok (pl. [Required]) importálása
using System.ComponentModel.DataAnnotations;

namespace pizzaprojekt.Controllers
{
    // DTO a PizzaFeltetek létrehozásához vagy frissítéséhez
    public class PizzaFeltetekDTO
    {
        [Required]
        public int PizzaId { get; set; } // A Pizza azonosítója kötelező mező, ami a kapcsolódó pizzát jelöli

        [Required]
        public int FeltetId { get; set; } // A Feltét azonosítója kötelező mező, ami a pizza feltétjét jelöli
    }

    // DTO a PizzaFeltetek lekérdezéséhez vagy visszaküldéséhez
    public class PizzaFeltetekOutDTO
    {
        public int PizzaId { get; set; } // Visszaadott Pizza azonosító
        public int FeltetId { get; set; } // Visszaadott Feltét azonosító
    }

    // API controller, amely a pizza feltétek kezeléséért felelős
    [Route("api/[controller]")] // Az útvonal automatikusan a controller neve alapján generálódik (pl. api/pizzafeltetek)
    [ApiController] // Megadjuk, hogy ez egy API controller, amely automatikusan kezeli a model validációkat és hibákat
    public class PizzaFeltetekController : ControllerBase
    {
        // Privát mező az adatbázis kontextus tárolásához
        private readonly PizzaContext _ctx; // A DbContext példány, amely lehetővé teszi a PizzaFeltetek tábla CRUD műveleteit

        // Konstruktor, ami az adatbázis kontextust injektálja a controllerbe
        public PizzaFeltetekController(PizzaContext ctx)
        {
            _ctx = ctx; // Inicializáljuk a privát mezőt a konstruktor paraméterével
        }

        // GET: api/pizzafeltetek
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaFeltetekOutDTO>>> Get()
        {
            // Lekérjük az összes pizza-feltét kapcsolatot az adatbázisból
            // Minden rekordot DTO formátumba alakítunk, hogy csak a szükséges adatokat küldjük vissza
            return await _ctx.Pizza_Feltetek
                .Select(pf => new PizzaFeltetekOutDTO
                {
                    PizzaId = pf.PizzaId, // Pizza azonosító másolása DTO-ba
                    FeltetId = pf.FeltetId // Feltét azonosító másolása DTO-ba
                })
                .ToListAsync(); // Aszinkron lekérdezés, amely listát ad vissza
        }

        // GET api/pizzafeltetek/{pizzaId}/{feltetId}
        [HttpGet("{pizzaId:int}/{feltetId:int}")]
        public async Task<ActionResult<PizzaFeltetekOutDTO>> Get(int pizzaId, int feltetId)
        {
            // Lekérjük a konkrét rekordot a pizza és feltét azonosítója alapján
            var result = await _ctx.Pizza_Feltetek
                .Where(pf => pf.PizzaId == pizzaId && pf.FeltetId == feltetId) // Szűrés mindkét kulcs alapján
                .Select(pf => new PizzaFeltetekOutDTO
                {
                    PizzaId = pf.PizzaId, // DTO-ba másoljuk a PizzaId-t
                    FeltetId = pf.FeltetId // DTO-ba másoljuk a FeltetId-t
                })
                .FirstOrDefaultAsync(); // Csak az első találatot adjuk vissza, vagy null-t, ha nincs találat

            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404-et adunk vissza

            return result; // Ha van találat, visszaadjuk a DTO-t
        }

        // POST api/pizzafeltetek
        [HttpPost]
        public async Task<ActionResult<PizzaFeltetekOutDTO>> Post([FromBody] PizzaFeltetekDTO dto)
        {
            // Ellenőrizzük a model validációt, ha nem érvényes, 400 Bad Request-et adunk vissza
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Új PizzaFeltetek objektum létrehozása a DTO alapján
            var result = new Pizza_Feltetek
            {
                PizzaId = dto.PizzaId, // DTO PizzaId másolása az új entitásba
                FeltetId = dto.FeltetId // DTO FeltetId másolása az új entitásba
            };

            // Hozzáadjuk az új rekordot az adatbázishoz
            _ctx.Pizza_Feltetek.Add(result);
            // Mentjük az adatbázis módosításokat aszinkron módon
            await _ctx.SaveChangesAsync();

            // Visszaadjuk az újonnan létrehozott rekordot DTO formátumban és 201 Created státusszal
            return CreatedAtAction(
                nameof(Get), // Hivatkozás a GET metódusra az új rekord lekéréséhez
                new { pizzaId = result.PizzaId, feltetId = result.FeltetId }, // Útvonal paraméterek az új rekordhoz
                new PizzaFeltetekOutDTO
                {
                    PizzaId = result.PizzaId, // DTO PizzaId
                    FeltetId = result.FeltetId // DTO FeltetId
                });
        }

        // PUT api/pizzafeltetek/{pizzaId}/{feltetId}
        [HttpPut("{pizzaId:int}/{feltetId:int}")]
        public async Task<IActionResult> Put(int pizzaId, int feltetId, [FromBody] PizzaFeltetekDTO dto)
        {
            // Ellenőrizzük a model validációt, ha hibás, 400-at adunk vissza
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lekérjük a frissítendő rekordot a két kulcs alapján
            var result = await _ctx.Pizza_Feltetek
                .FirstOrDefaultAsync(pf => pf.PizzaId == pizzaId && pf.FeltetId == feltetId);

            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404

            // DTO adatokkal frissítjük a rekordot
            result.PizzaId = dto.PizzaId;
            result.FeltetId = dto.FeltetId;

            // Mentjük a módosításokat az adatbázisba
            await _ctx.SaveChangesAsync();

            // Nincs visszaadott tartalom, csak a 204 No Content státusz
            return NoContent();
        }

        // DELETE api/pizzafeltetek/{pizzaId}/{feltetId}
        [HttpDelete("{pizzaId:int}/{feltetId:int}")]
        public async Task<IActionResult> Delete(int pizzaId, int feltetId)
        {
            // Lekérjük a törlendő rekordot a két kulcs alapján
            var result = await _ctx.Pizza_Feltetek
                .FirstOrDefaultAsync(pf => pf.PizzaId == pizzaId && pf.FeltetId == feltetId);

            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404

            // Töröljük a rekordot az adatbázisból
            _ctx.Pizza_Feltetek.Remove(result);
            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat

            // Nincs visszaadott tartalom, csak 204 No Content státusz
            return NoContent();
        }
    }
}