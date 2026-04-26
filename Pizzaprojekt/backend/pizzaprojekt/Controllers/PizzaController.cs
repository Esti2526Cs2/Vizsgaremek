// ASP.NET Core MVC kontroller alapjaihoz szükséges névterek importálása
using Microsoft.AspNetCore.Mvc;
// Entity Framework Core a relációs adatbázis műveletekhez
using Microsoft.EntityFrameworkCore;
// Projekt-specifikus adatbázis kontextus importálása
using pizzaprojekt.Database;
// Projekt modelljeinek importálása, például Pizza
using pizzaprojekt.Models;
// Validációs attribútumok (pl. [Required], [Range]) importálása
using System.ComponentModel.DataAnnotations;

namespace pizzaprojekt.Controllers
{
    // DTO a Pizza létrehozásához vagy frissítéséhez
    public class PizzaDTO
    {
        [Range(1, 100000)]
        public int Ar { get; set; } // Az ár kötelező és 1–100000 között kell legyen

        [Required]
        public string Nev { get; set; } = string.Empty; // A pizza neve kötelező mező

        [Required]
        public string Alap { get; set; } = string.Empty; // A pizza alapja kötelező mező
    }

    // DTO a Pizza lekérdezéséhez vagy visszaküldéséhez
    public class PizzaOutDTO
    {
        public int Id { get; set; } // Pizza azonosító visszaküldése
        public string? Nev { get; set; } // Pizza neve visszaküldve
        public int Ar { get; set; } // Pizza ára visszaküldve
        public string? Alap { get; set; } // Pizza alapja visszaküldve
        public string? Image { get; set; } //Pizza képe visszaküldve
    }

    // API controller, amely a pizza táblát kezeli
    [Route("api/[controller]")] // Az útvonal automatikusan a controller neve alapján generálódik (pl. api/pizza)
    [ApiController] // Megadjuk, hogy ez egy API controller, automatikusan kezeli a model validációt
    public class PizzaController : ControllerBase
    {
        // Privát mező az adatbázis kontextus tárolásához
        private readonly PizzaContext _ctx;

        // Konstruktor az adatbázis kontextus injektálásához
        public PizzaController(PizzaContext ctx)
        {
            _ctx = ctx; // Inicializáljuk a privát mezőt a konstruktor paraméterével
        }
        [HttpGet("debug")]
        public async Task<IActionResult> Debug()
        {
           using var conn= _ctx.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd= conn.CreateCommand();
            cmd.CommandText = "SHOW COLUMnS FROM pizza";
            using var reader= await cmd.ExecuteReaderAsync();
            var colums= new List<string>();
            while(await reader.ReadAsync())
            {
                colums.Add(reader.GetString(0));//Oszlop neve
            }
            return Ok(colums);
        }

        // GET: api/pizza
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaOutDTO>>> Get()
        {
            // Lekérjük az összes pizzát az adatbázisból és DTO-ba konvertáljuk
            return await _ctx.Pizza
                .Select(p => new PizzaOutDTO
                {
                    Id = p.PizzaId, // DTO Id mezőbe másoljuk az adatbázisbeli PizzaId-t
                    Nev = p.Nev, // DTO Nev mezőbe másoljuk a pizza nevét
                    Ar = p.pizzaar, // DTO Ar mezőbe másoljuk az árat
                    Alap = p.alap, // DTO Alap mezőbe másoljuk az alapot
                    Image=p.Image
                })
                .ToListAsync(); // Aszinkron lekérdezés listába gyűjtve
        }

        // GET api/pizza/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaOutDTO>> Get(int id)
        {
            // Lekérjük a konkrét pizzát az azonosító alapján
            var result = await _ctx.Pizza
                .Where(p => p.PizzaId == id) // Szűrés PizzaId alapján
                .Select(p => new PizzaOutDTO
                {
                    Id = p.PizzaId,
                    Nev = p.Nev,
                    Ar = p.pizzaar,
                    Alap = p.alap,
                    Image =p.Image
                })
                .FirstOrDefaultAsync(); // Csak az első találatot adjuk vissza, vagy null-t, ha nincs találat

            if (result == null)
                return NotFound(); // Ha nincs ilyen pizza, 404-et adunk vissza

            return result; // Ha van, visszaküldjük DTO formátumban
        }

        // POST api/pizza
        [HttpPost]
        public async Task<ActionResult<PizzaOutDTO>> Post([FromBody] PizzaDTO pizzaDto)
        {
            // Validáció ellenőrzése, ha nem érvényes, 400 Bad Request-et adunk vissza
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Új Pizza entitás létrehozása DTO alapján
            var result = new Pizza
            {
                pizzaar = pizzaDto.Ar, // Ár beállítása
                Nev = pizzaDto.Nev, // Név beállítása
                alap = pizzaDto.Alap // Alap beállítása

            };

            // Hozzáadjuk az új rekordot az adatbázishoz
            _ctx.Pizza.Add(result);
            // Mentjük az adatbázis módosításokat aszinkron módon
            await _ctx.SaveChangesAsync();

            // Visszaadjuk az új rekordot DTO formátumban és 201 Created státusszal
            return CreatedAtAction(
                nameof(Get), // Hivatkozás a GET metódusra az új rekord lekéréséhez
                new { id = result.PizzaId }, // Útvonal paraméter az új rekordhoz
                new PizzaOutDTO
                {
                    Id = result.PizzaId,
                    Nev = result.Nev,
                    Ar = result.pizzaar,
                    Alap = result.alap,
                    Image=result.Image
                });
        }

        // PUT api/pizza/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] PizzaDTO pizzaDto)
        {
            // Validáció ellenőrzése, ha hibás, 400-at adunk vissza
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lekérjük a frissítendő pizzát az adatbázisból
            var result = await _ctx.Pizza.FindAsync(id);
            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404

            // DTO adatokkal frissítjük az entitást
            result.pizzaar = pizzaDto.Ar;
            result.alap = pizzaDto.Alap;
            result.Nev = pizzaDto.Nev;

            // Mentjük a módosításokat az adatbázisba
            await _ctx.SaveChangesAsync();

            // Nincs visszaadott tartalom, csak a 204 No Content státusz
            return NoContent();
        }

        // DELETE api/pizza/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            // Lekérjük a törlendő rekordot
            var result = await _ctx.Pizza.FindAsync(id);
            if (result == null)
                return NotFound(); // Ha nincs ilyen pizza, 404

            // Töröljük a rekordot az adatbázisból
            _ctx.Pizza.Remove(result);
            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat

            // Nincs visszaadott tartalom, csak 204 No Content státusz
            return NoContent();
        }
    }
}