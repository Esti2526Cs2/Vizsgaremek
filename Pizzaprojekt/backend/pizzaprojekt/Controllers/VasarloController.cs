// ASP.NET Core MVC kontroller alapjaihoz szükséges névterek importálása
using Microsoft.AspNetCore.Mvc;
// Entity Framework Core, az adatbázis műveletekhez
using Microsoft.EntityFrameworkCore;
// Projekt adatbázis kontextusát importáljuk
using pizzaprojekt.Database;
// Projekt modelljei (pl. Vasarlo) importálása
using pizzaprojekt.Models;
// Validációs attribútumok (pl. [Required], [Phone]) importálása
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// Namespace definiálása a kontrollerek számára
namespace pizzaprojekt.Controllers
{
    // A controller route-ját definiáljuk: pl. api/vasarlo
    [Route("api/[controller]")]
    // Megadjuk, hogy ez egy API controller, nem sima MVC
    [ApiController]
    public class VasarloController : ControllerBase
    {
        // DTO (Data Transfer Object) a vásárló létrehozásához / frissítéséhez
        public class VasarloDTO
        {
            // Név kötelező mezőként jelölve
            [Required]
            [JsonPropertyName("nev")]
            public string Nev { get; set; } = string.Empty; // alapértelmezett üres stringgel
            // Telefon opcionális, de ha meg van adva, ellenőrzés: telefonszám formátum
            [Phone]
            [JsonPropertyName("telefonszam")]
            public string? Telefonszam { get; set; }=string.Empty;
        }

        public class CimOutDTO
        {
            [JsonPropertyName("cimId")]
            public int CimId { get; set; }

            [JsonPropertyName("varos")]
            public string? Varos { get; set; }

            [JsonPropertyName("iranyitoszam")]
            public string? Iranyitoszam { get; set; }

            [JsonPropertyName("utca")]
            public string? Utca { get; set; }

            [JsonPropertyName("hazszam")]
            public string? Hazszam { get; set; }

            [JsonPropertyName("emeletAjto")]
            public string? EmeletAjto { get; set; }
        }

        // DTO a vásárló lekérdezéséhez / visszaküldéséhez
        public class VasarloOutDTO
        {
            [JsonPropertyName("rendeloAzonosito")]
            public int Id { get; set; } // Egyedi azonosító
            [JsonPropertyName("nev")]
            public string? Nev { get; set; } // Név (nullable)
            [JsonPropertyName("telefonszam")]
            public string? Telefonszam { get; set; } // Telefon (nullable)
            [JsonPropertyName("cim")]
            public CimOutDTO? Cim { get; set; } // Legutóbbi cím
            [JsonPropertyName("cimek")]
            public List<CimOutDTO> Cimek { get; set; } = new(); // Összes mentett cím
        }

        // Adatbázis kontextus privát mezőként
        private readonly PizzaContext _ctx; // Adatbázis műveletekhez szükséges EF DbContext

        // Konstruktor: dependency injection segítségével adjuk át az adatbázis kontextust
        public VasarloController(PizzaContext ctx)
        {
            _ctx = ctx; // privát mező inicializálása
        }

        // GET: api/vasarlo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VasarloOutDTO>>> Get()
        {
            var vasarlok = await _ctx.Vasarlo
                .AsNoTracking()
                .Select(v => new
                {
                    v.RendeloAzonosito,
                    v.Nev,
                    v.Telefonszam
                })
                .ToListAsync();

            var cimKapcsolatok = await _ctx.Keri
                .AsNoTracking()
                .Join(_ctx.Kiszallitasi_Hely.AsNoTracking(),
                    k => k.CimId,
                    c => c.CimId,
                    (k, c) => new
                    {
                        k.RendeloAzonosito,
                        Cim = new CimOutDTO
                        {
                            CimId = c.CimId,
                            Varos = c.Varos,
                            Iranyitoszam = c.Iranyitoszam,
                            Utca = c.Utca,
                            Hazszam = c.Hazszam,
                            EmeletAjto = c.EmeletAjto
                        }
                    })
                .ToListAsync();

            var eredmeny = vasarlok
                .GroupBy(v => new
                {
                    Nev = (v.Nev ?? string.Empty).Trim(),
                    Telefonszam = (v.Telefonszam ?? string.Empty).Trim()
                })
                .Select(group =>
                {
                    var vevoAzonositok = group
                        .Select(v => v.RendeloAzonosito)
                        .ToHashSet();

                    var cimek = cimKapcsolatok
                        .Where(k => vevoAzonositok.Contains(k.RendeloAzonosito))
                        .Select(k => k.Cim)
                        .GroupBy(c => c.CimId)
                        .Select(csoport => csoport.First())
                        .OrderByDescending(c => c.CimId)
                        .ToList();

                    var elso = group.OrderBy(v => v.RendeloAzonosito).First();

                    return new VasarloOutDTO
                    {
                        Id = elso.RendeloAzonosito,
                        Nev = elso.Nev,
                        Telefonszam = elso.Telefonszam,
                        Cim = cimek.FirstOrDefault(),
                        Cimek = cimek
                    };
                })
                .OrderBy(v => v.Nev)
                .ToList();

            return eredmeny;
        }

        // GET api/vasarlo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VasarloOutDTO>> Get(int id)
        {
            // Lekérjük a konkrét vásárlót ID alapján és DTO-ba konvertáljuk
            var result = await _ctx.Vasarlo
                .Where(v => v.RendeloAzonosito == id) // Szűrés az ID alapján
                .Select(v => new VasarloOutDTO
                {
                    Id = v.RendeloAzonosito,
                    Nev = v.Nev,
                    Telefonszam = v.Telefonszam,
                    Cim = _ctx.Keri
                        .Where(k => k.RendeloAzonosito == v.RendeloAzonosito)
                        .Join(_ctx.Kiszallitasi_Hely,
                            k => k.CimId,
                            c => c.CimId,
                            (k, c) => new CimOutDTO
                            {
                                CimId = c.CimId,
                                Varos = c.Varos,
                                Iranyitoszam = c.Iranyitoszam,
                                Utca = c.Utca,
                                Hazszam = c.Hazszam,
                                EmeletAjto = c.EmeletAjto
                            })
                        .OrderByDescending(c => c.CimId)
                        .FirstOrDefault(),
                    Cimek = _ctx.Keri
                        .Where(k => k.RendeloAzonosito == v.RendeloAzonosito)
                        .Join(_ctx.Kiszallitasi_Hely,
                            k => k.CimId,
                            c => c.CimId,
                            (k, c) => new CimOutDTO
                            {
                                CimId = c.CimId,
                                Varos = c.Varos,
                                Iranyitoszam = c.Iranyitoszam,
                                Utca = c.Utca,
                                Hazszam = c.Hazszam,
                                EmeletAjto = c.EmeletAjto
                            })
                        .OrderByDescending(c => c.CimId)
                        .ToList()
                })
                .FirstOrDefaultAsync(); // Csak az első találat vagy null

            if (result == null)
                return NotFound(); // Ha nincs találat, 404-et küldünk

            return result; // Ha van, visszaadjuk a DTO-t
        }

        // POST api/vasarlo
        [HttpPost]
        public async Task<ActionResult<VasarloOutDTO>> Post([FromBody] VasarloDTO vasarloDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Ha a DTO invalid, 400-as hibát küldünk

            // Új Vásárló objektum létrehozása a DTO alapján
            var result = new Vasarlo
            {
                Nev = vasarloDto.Nev, // Név beállítása
                Telefonszam = vasarloDto.Telefonszam, // Telefon beállítása
            };

            _ctx.Vasarlo.Add(result); // Hozzáadjuk az adatbázishoz
            await _ctx.SaveChangesAsync();
   

            // Visszaadjuk a létrehozott objektumot DTO formában, HTTP 201 státusszal
            return CreatedAtAction(
                nameof(Get), // A GET metódus hivatkozása az új elem lekéréséhez
                new { id = result.RendeloAzonosito }, // Új ID útvonala
                new VasarloOutDTO
                {
                    Id = result.RendeloAzonosito,
                    Nev = result.Nev,
                    Telefonszam = result.Telefonszam
                });
        }

        // PUT api/vasarlo/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Vasarlo>> Put(int id, [FromBody] VasarloDTO vasarloDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Ha a DTO invalid, 400-as hibát küldünk

            var result = await _ctx.Vasarlo.FindAsync(id); // Megkeressük a vásárlót ID alapján
            if (result == null)
                return NotFound(); // Ha nincs, 404

            // DTO adatokkal frissítjük az objektumot
            result.Nev = vasarloDto.Nev;
            result.Telefonszam = vasarloDto.Telefonszam;

            await _ctx.SaveChangesAsync(); // Mentjük a változtatásokat

            return NoContent(); // 204 visszaküldése, nincs tartalom
        }

        // DELETE api/vasarlo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Vasarlo>> Delete(int id)
        {
            var result = await _ctx.Vasarlo.FindAsync(id); // Megkeressük az ID alapján
            if (result == null)
                return NotFound(); // Ha nincs, 404

            _ctx.Vasarlo.Remove(result); // Töröljük az adatbázisból
            await _ctx.SaveChangesAsync(); // Mentjük a változtatásokat

            return NoContent(); // 204 No Content visszaküldése
        }
    }
}