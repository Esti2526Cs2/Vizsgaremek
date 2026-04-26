// ASP.NET Core MVC és EF Core névterek importálása
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizzaprojekt.Database; // Az adatbázis kontextus
using pizzaprojekt.Models; // A modellek (KiszalitasiHely)
using System.ComponentModel.DataAnnotations; // Validációs attribútumok (Required, Range stb.)

namespace pizzaprojekt.Controllers
{
    // DTO a kiszállítási hely létrehozásához vagy frissítéséhez
    public class KiszallitasiHelyDTO
    {
        [Required] // A cím azonosítója kötelező
        public int Cim { get; set; }

        [Required] // A város mező kötelező
        public string Varos { get; set; } = string.Empty;

        [Required] // Az irányítószám kötelező
        public string Iranyito { get; set; } = string.Empty;

        [Required] // Az utca kötelező
        public string Utca { get; set; } = string.Empty;

        [Required] // A házszám kötelező
        public string Hazszam { get; set; } = string.Empty;

        // Az emelet/ajtó mező opcionális
        public string? Emeletajto { get; set; }
    }

    // DTO a kiszállítási hely lekérdezéséhez vagy visszaküldéséhez
    public class KiszallitasiHelyOutDTO
    {
        public int Cim { get; set; } // A cím azonosítója
        public string? Varos { get; set; } // Város
        public string? Iranyito { get; set; } // Irányítószám
        public string? Utca { get; set; } // Utca
        public string? Hazszam { get; set; } // Házszám
        public string? Emeletajto { get; set; } // Emelet/ajtó, opcionális
    }

    // Controller a KiszalitasiHely táblához
    [Route("api/[controller]")] // Az alapútvonal a controller neve: api/kiszalitasihely
    [ApiController] // Automatikusan kezeli a model validációt és JSON formátumú válaszokat
    public class KiszalitasiHelyController : ControllerBase
    {
        private readonly PizzaContext _ctx; // Privát mező az adatbázis kontextushoz

        // Konstruktor az adatbázis kontextus injektálásához
        public KiszalitasiHelyController(PizzaContext ctx)
        {
            _ctx = ctx; // Inicializáljuk a privát mezőt
        }

        // GET: api/kiszalitasihely
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KiszallitasiHelyOutDTO>>> Get()
        {
            // Az összes kiszállítási hely lekérése az adatbázisból DTO formátumban
            return await _ctx.Kiszallitasi_Hely
                .Select(k => new KiszallitasiHelyOutDTO
                {
                    Cim = k.CimId,
                    Varos = k.Varos,
                    Iranyito = k.Iranyitoszam,
                    Utca = k.Utca,
                    Hazszam = k.Hazszam,
                    Emeletajto = k.EmeletAjto
                })
                .ToListAsync(); // Aszinkron listába gyűjtve
        }

        // GET api/kiszalitasihely/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<KiszallitasiHelyOutDTO>> Get(int id)
        {
            // Lekérjük az adott ID-jú kiszállítási hely rekordot
            var result = await _ctx.Kiszallitasi_Hely
                .Where(k => k.CimId == id)
                .Select(k => new KiszallitasiHelyOutDTO
                {
                    Cim = k.CimId,
                    Varos = k.Varos,
                    Iranyito = k.Iranyitoszam,
                    Utca = k.Utca,
                    Hazszam = k.Hazszam,
                    Emeletajto = k.EmeletAjto
                })
                .FirstOrDefaultAsync(); // Csak az első találatot adjuk vissza

            if (result == null)
                return NotFound(); // Ha nincs ilyen rekord, 404-et adunk

            return result; // Ha van, DTO formátumban visszaküldjük
        }

        // POST api/kiszalitasihely
        [HttpPost]
        public async Task<ActionResult<KiszallitasiHelyOutDTO>> Post([FromBody] KiszallitasiHelyDTO dto)
        {
            // Ellenőrizzük a model validációt
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Új KiszalitasiHely entitás létrehozása a DTO adatokkal
            var result = new Kiszallitasi_Hely
            {
                CimId = dto.Cim,
                Varos = dto.Varos,
                Iranyitoszam = dto.Iranyito,
                Utca = dto.Utca,
                Hazszam = dto.Hazszam,
                EmeletAjto = dto.Emeletajto
            };

            // Hozzáadjuk az adatbázishoz és mentjük
            _ctx.Kiszallitasi_Hely.Add(result);
            await _ctx.SaveChangesAsync();

            // Visszaadjuk a létrehozott rekordot DTO formátumban 201 Created státusszal
            return CreatedAtAction(
                nameof(Get),
                new { id = result.CimId }, // Útvonal paraméter
                new KiszallitasiHelyOutDTO
                {
                    Cim = result.CimId,
                    Varos = result.Varos,
                    Iranyito = result.Iranyitoszam,
                    Utca = result.Utca,
                    Hazszam = result.Hazszam,
                    Emeletajto = result.EmeletAjto
                });
        }

        // PUT api/kiszalitasihely/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] KiszallitasiHelyDTO dto)
        {
            // Ellenőrizzük a model validációt
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lekérjük a frissítendő rekordot
            var result = await _ctx.Kiszallitasi_Hely.FindAsync(id);
            if (result == null)
                return NotFound(); // Ha nincs rekord, 404

            // DTO alapján frissítjük az entitást
            result.CimId = dto.Cim;
            result.Varos = dto.Varos;
            result.Iranyitoszam = dto.Iranyito;
            result.Utca = dto.Utca;
            result.Hazszam = dto.Hazszam;
            result.EmeletAjto = dto.Emeletajto;

            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat
            return NoContent(); // 204 No Content státusz
        }

        // DELETE api/kiszalitasihely/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<KiszallitasiHelyOutDTO>> Delete(int id)
        {
            // Lekérjük a törlendő rekordot
            var result = await _ctx.Kiszallitasi_Hely.FindAsync(id);
            if (result == null)
                return NotFound(); // Ha nincs rekord, 404

            // Töröljük az entitást
            _ctx.Kiszallitasi_Hely.Remove(result);
            await _ctx.SaveChangesAsync(); // Mentjük a módosításokat
            return NoContent(); // 204 No Content státusz visszaadása
        }
    }
}