// ASP.NET Core MVC kontroller alapjaihoz szükséges névterek importálása
using Microsoft.AspNetCore.Mvc;
// Entity Framework Core az adatbázis műveletekhez
using Microsoft.EntityFrameworkCore;
// Projekt adatbázis kontextus importálása
using pizzaprojekt.Database;
// Projekt modelljei (pl. Tartalmaz) importálása
using pizzaprojekt.Models;
// Validációs attribútumok (pl. [Required]) importálása
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// Namespace definiálása a kontrollerek számára
namespace pizzaprojekt.Controllers
{
    // API route beállítása: pl. api/tartalmaz
    [Route("api/[controller]")]
    // Megadjuk, hogy ez egy API controller, nem sima MVC
    [ApiController]
    public class TartalmazController : ControllerBase
    {
        // DTO a Tartalmaz létrehozásához / frissítéséhez
        public class TartalmazDTO
        {
            [Required]
            public int RendelesId { get; set; } // Rendelés azonosító kötelező mező
            [Required]
            public int PizzaId { get; set; } // Pizza azonosító kötelező mező
        }

        // DTO a Tartalmaz lekérdezéséhez / visszaküldéséhez
        public class TartalmazOutDTO
        {
            public int RendelesId { get; set; } // Rendelés azonosító
            public int PizzaId { get; set; } // Pizza azonosító
        }

        // DTO-k a komplex POST-hoz
        public class VasarloDTO
        {
            [JsonPropertyName("rendeloAzonosito")]
            public int? RendeloAzonosito { get; set; }

            [Required]
            public string? Nev { get; set; }

            [JsonPropertyName("telefonszam")]
            public string? Telefonszam { get; set; }
        }

        public class KiszallitasiHelySaveDTO
        {
            [JsonPropertyName("cimId")]
            public int? CimId { get; set; }

            public string? Varos { get; set; }
            public string? Iranyitoszam { get; set; }
            public string? Utca { get; set; }
            public string? Hazszam { get; set; }
            public string? EmeletAjto { get; set; }
        }

        public class KeriDTO
        {
            // Nincs extra adat, csak kapcsolat
        }

        public class RendelesSaveDTO
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
        }

        public class RendelesTetelDTO
        {
            [Required]
            public int PizzaId { get; set; }

            [Required]
            public int Mennyiseg { get; set; }
        }

        public class ComplexSaveDTO
        {
            [Required]
            public VasarloDTO? Vasarlo { get; set; }

            [Required]
            public KiszallitasiHelySaveDTO? KiszallitasiHely { get; set; }

            [Required]
            public RendelesSaveDTO? Rendeles { get; set; }

            [Required]
            public List<RendelesTetelDTO>? TetelLista { get; set; }
        }

        // Privát adatbázis kontextus mező
        private readonly PizzaContext _ctx; // EF DbContext a műveletekhez

        // Konstruktor: dependency injection az adatbázis kontextushoz
        public TartalmazController(PizzaContext ctx)
        {
            _ctx = ctx; // privát mező inicializálása
        }

        // GET: api/tartalmaz
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TartalmazOutDTO>>> Get()
        {
            // Lekérjük az összes Tartalmaz rekordot, és DTO-ba alakítjuk
            return await _ctx.Tartalmaz
                .Select(t => new TartalmazOutDTO
                {
                    RendelesId = t.RendelesId, // Rendelés azonosító
                    PizzaId = t.PizzaId // Pizza azonosító
                })
                .ToListAsync(); // Aszinkron lista lekérése
        }

        // GET api/tartalmaz/{rendelesId}/{pizzaId}
        [HttpGet("{rendelesId:int}/{pizzaId:int}")]
        public async Task<ActionResult<TartalmazOutDTO>> Get(int rendelesId, int pizzaId)
        {
            // Lekérjük a konkrét rekordot két kulcs alapján és DTO-ba konvertáljuk
            var result = await _ctx.Tartalmaz
                .Where(t => t.RendelesId == rendelesId && t.PizzaId == pizzaId) // Szűrés kulcsokra
                .Select(t => new TartalmazOutDTO
                {
                    RendelesId = t.RendelesId,
                    PizzaId = t.PizzaId
                })
                .FirstOrDefaultAsync(); // Csak az első találat vagy null

            if (result == null)
                return NotFound(); // Ha nincs találat, 404

            return result; // Ha van, visszaadjuk a DTO-t
        }

        // POST api/tartalmaz
        [HttpPost]
        public async Task<ActionResult<Tartalmaz>> Post([FromBody] TartalmazDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Ha DTO invalid, 400-as hiba

            // Új Tartalmaz objektum létrehozása DTO alapján
            var result = new Tartalmaz
            {
                RendelesId = dto.RendelesId,
                PizzaId = dto.PizzaId
            };

            _ctx.Tartalmaz.Add(result); // Hozzáadás az adatbázishoz
            await _ctx.SaveChangesAsync(); // Mentés aszinkron

            // Visszaadjuk a létrehozott rekordot DTO formában, HTTP 201 státusszal
            return CreatedAtAction(
                nameof(Get), // GET metódus hivatkozása az új elem lekéréséhez
                new { rendelesId = result.RendelesId, pizzaId = result.PizzaId }, // Új rekord útvonala
                new TartalmazOutDTO
                {
                    RendelesId = result.RendelesId,
                    PizzaId = result.PizzaId
                });
        }

        // POST api/tartalmaz/savecomplex - Nagy POST request a vásárló, kiszállítási hely, Lead és Kéri mentéséhez
        [HttpPost("savecomplex")]
        public async Task<ActionResult<string>> PostComplex([FromBody] ComplexSaveDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Ha DTO invalid, 400-as hiba

            var vasarloDto = dto.Vasarlo!;
            var kiszallitasiHelyDto = dto.KiszallitasiHely!;
            var rendelesDto = dto.Rendeles!;
            var tetelLista = dto.TetelLista!;

            await using var transaction = await _ctx.Database.BeginTransactionAsync();

            try
            {
                var normalizedNev = vasarloDto.Nev?.Trim() ?? string.Empty;
                var normalizedTelefon = vasarloDto.Telefonszam?.Trim();
                var normalizedVaros = kiszallitasiHelyDto.Varos?.Trim();
                var normalizedIranyitoszam = kiszallitasiHelyDto.Iranyitoszam?.Trim();
                var normalizedUtca = kiszallitasiHelyDto.Utca?.Trim();
                var normalizedHazszam = kiszallitasiHelyDto.Hazszam?.Trim();
                var normalizedEmeletAjto = kiszallitasiHelyDto.EmeletAjto?.Trim();

                // Ha kaptunk vásárló azonosítót, elsődlegesen azt használjuk újra
                Vasarlo? vasarlo = null;

                if (vasarloDto.RendeloAzonosito.HasValue)
                {
                    vasarlo = await _ctx.Vasarlo
                        .FirstOrDefaultAsync(v => v.RendeloAzonosito == vasarloDto.RendeloAzonosito.Value);
                }

                // Ha nincs azonosító vagy nem található, akkor név + telefonszám alapján keresünk
                if (vasarlo == null)
                {
                    vasarlo = await _ctx.Vasarlo
                        .FirstOrDefaultAsync(v =>
                            v.Nev == normalizedNev &&
                            (v.Telefonszam ?? string.Empty) == (normalizedTelefon ?? string.Empty));
                }

                if (vasarlo == null)
                {
                    vasarlo = new Vasarlo
                    {
                        Nev = normalizedNev,
                        Telefonszam = normalizedTelefon
                    };

                    _ctx.Vasarlo.Add(vasarlo);
                    await _ctx.SaveChangesAsync();
                }

                // Ha a kiszállítási helyet kiválasztották azonosító alapján, azt használjuk
                Kiszallitasi_Hely? kiszallitasiHely = null;

                if (kiszallitasiHelyDto.CimId.HasValue)
                {
                    kiszallitasiHely = await _ctx.Kiszallitasi_Hely
                        .FirstOrDefaultAsync(k => k.CimId == kiszallitasiHelyDto.CimId.Value);
                }

                // Ha a kiszállítási hely már létezik, nem mentjük újra
                if (kiszallitasiHely == null)
                {
                    kiszallitasiHely = await _ctx.Kiszallitasi_Hely
                        .FirstOrDefaultAsync(k =>
                            (k.Varos ?? string.Empty) == (normalizedVaros ?? string.Empty) &&
                            (k.Iranyitoszam ?? string.Empty) == (normalizedIranyitoszam ?? string.Empty) &&
                            (k.Utca ?? string.Empty) == (normalizedUtca ?? string.Empty) &&
                            (k.Hazszam ?? string.Empty) == (normalizedHazszam ?? string.Empty) &&
                            (k.EmeletAjto ?? string.Empty) == (normalizedEmeletAjto ?? string.Empty));
                }

                if (kiszallitasiHely == null)
                {
                    kiszallitasiHely = new Kiszallitasi_Hely
                    {
                        Varos = normalizedVaros,
                        Iranyitoszam = normalizedIranyitoszam,
                        Utca = normalizedUtca,
                        Hazszam = normalizedHazszam,
                        EmeletAjto = normalizedEmeletAjto
                    };

                    _ctx.Kiszallitasi_Hely.Add(kiszallitasiHely);
                    await _ctx.SaveChangesAsync();
                }

                // A Keri kapcsolótáblát csak akkor bővítjük, ha még nincs ilyen kapcsolat
                var keri = await _ctx.Keri
                    .FirstOrDefaultAsync(k =>
                        k.CimId == kiszallitasiHely.CimId &&
                        k.RendeloAzonosito == vasarlo.RendeloAzonosito);

                if (keri == null)
                {
                    keri = new Keri
                    {
                        CimId = kiszallitasiHely.CimId,
                        RendeloAzonosito = vasarlo.RendeloAzonosito
                    };

                    _ctx.Keri.Add(keri);
                }

                // Új Rendeles objektum létrehozása DTO alapján
                var rendeles = new Rendeles
                {
                    Datum = rendelesDto.Datum,
                    Statusz = rendelesDto.Statusz,
                    FizetesMod = rendelesDto.Fizetes,
                    Mennyiseg = tetelLista.Sum(t => t.Mennyiseg)
                };

                _ctx.Rendeles.Add(rendeles);
                await _ctx.SaveChangesAsync(); // Mentés, hogy megkapjuk az rendelés azonosítóját

                // Új Lead objektum létrehozása a rendeléshez és vásárlóhoz
                var lead = new Lead
                {
                    RendeloAzonosito = vasarlo.RendeloAzonosito,
                    RendelesiId = rendeles.RendelesId
                };

                _ctx.Lead.Add(lead);
                await _ctx.SaveChangesAsync(); // Mentés, hogy a Lead rekord is bekerüljön

                // Több Tartalmaz rekord létrehozása a rendelés tételeihez
                foreach (var tetel in tetelLista)
                {
                    var tartalmaz = new Tartalmaz
                    {
                        RendelesId = rendeles.RendelesId,
                        PizzaId = tetel.PizzaId
                    };
                    _ctx.Tartalmaz.Add(tartalmaz);
                }

                await _ctx.SaveChangesAsync(); // Összes mentés aszinkron
                await transaction.CommitAsync();

                // Visszaadjuk sikeres üzenetet
                return Ok(new
                {
                    RendelesId = rendeles.RendelesId,
                    VasarloId = vasarlo.RendeloAzonosito,
                    CimId = kiszallitasiHely.CimId,
                    TetelCount = tetelLista.Count
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var inner = ex.InnerException != null ? $" | Inner: {ex.InnerException.Message}" : string.Empty;
                return Problem(detail: $"{ex.Message}{inner}");
            }
        }

        // PUT api/tartalmaz/{rendelesId}/{pizzaId}
        [HttpPut("{rendelesId:int}/{pizzaId:int}")]
        public async Task<IActionResult> Put(
            int rendelesId,
            int pizzaId,
            TartalmazDTO dto)
        {
            // Megkeressük a rekordot a két kulcs alapján
            var result = await _ctx.Tartalmaz
                .FirstOrDefaultAsync(t => t.RendelesId == rendelesId && t.PizzaId == pizzaId);

            if (result == null)
                return NotFound(); // Ha nincs rekord, 404

            // DTO adatokkal frissítjük a rekordot
            result.RendelesId = dto.RendelesId;
            result.PizzaId = dto.PizzaId;

            await _ctx.SaveChangesAsync(); // Mentés aszinkron
            return NoContent(); // 204 No Content vissza
        }

        // DELETE api/tartalmaz/{rendelesId}/{pizzaId}
        [HttpDelete("{rendelesId:int}/{pizzaId:int}")]
        public async Task<ActionResult<Tartalmaz>> Delete(int rendelesId, int pizzaId)
        {
            // Megkeressük a rekordot a két kulcs alapján
            var result = await _ctx.Tartalmaz
                .FirstOrDefaultAsync(t => t.RendelesId == rendelesId && t.PizzaId == pizzaId);

            if (result == null)
                return NotFound(); // Ha nincs rekord, 404

            _ctx.Tartalmaz.Remove(result); // Törlés az adatbázisból
            await _ctx.SaveChangesAsync(); // Mentés aszinkron

            return NoContent(); // 204 No Content vissza
        }
    }
}