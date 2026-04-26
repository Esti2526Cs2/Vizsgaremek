using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pizzaprojekt.Database;
using pizzaprojekt.Models;
using System.ComponentModel.DataAnnotations;


namespace pizzaprojekt.Controllers
{
    public class RendelesDto
    {
        public int? Id { get; set; }
        public string? Datum { get; set; }
        public string? Cim { get; set; }
        public string? RendeloNev { get; set; }
        public string? Telefonszam { get; set; }
        public string? FizetesiMod { get; set; }
        public string? Statusz { get; set; }
        public int? Osszeg { get; set; }
        public int? SutesiIdo { get; set; }
        
        public List<TetelDTO>? Tetelek { get; set; }
    }
    public class TetelDTO
    {
        public string? PizzaNev { get; set; }
        public int? Mennyiseg { get; set; }
        public string? Fizetes { get; set; }
        public int? TetelOsszeg { get; set; }
        public List<FeltetDto>? Feltetek { get; set; }
    }
    public class FeltetDto
    {
        public string? Nev { get; set; }
        public int? Ar { get; set; }
    }

    public class StatusUpdateDto
    {
        public string? Statusz { get; set; }
    }

    public class BakeTimeDto
    {
        public int SutesiIdo { get; set; }
    }

    public static class BakeTimeStore
    {
        public static Dictionary<int, DateTime> BakeEndTimes = new();

        public static int? GetRemainingSeconds(int orderId)
        {
            if (!BakeEndTimes.TryGetValue(orderId, out var endTime))
            {
                return null;
            }

            var remainingSeconds = (int)Math.Ceiling((endTime - DateTime.UtcNow).TotalSeconds);
            if (remainingSeconds <= 0)
            {
                BakeEndTimes.Remove(orderId);
                return 0;
            }

            return remainingSeconds;
        }
    }

    // API controller, amely a rendeléseket kezeli
    [Route("api/rendelesek")] // Az útvonal api/rendelesek
    [ApiController] // Megadjuk, hogy ez egy API controller, automatikusan kezeli a model validációt
    public class RendelesekController : ControllerBase
    {
        // Privát mező az adatbázis kontextus tárolásához
        private readonly PizzaContext _ctx;

        // Konstruktor az adatbázis kontextus injektálásához
        public RendelesekController(PizzaContext ctx)
        {
            _ctx = ctx; // Inicializáljuk a privát mezőt a konstruktor paraméterével
        }

                // ============================
                //  RENDELÉSEK LEKÉRÉSE
                // ============================
            [HttpGet]
            public IActionResult GetOrders()
            {
                try
                {
                    var today = DateTime.Today;
                    var staleOrders = _ctx.Rendeles
                        .Where(r =>
                            r.Datum.Date < today &&
                            (r.Statusz == "folyamatban" ||
                             r.Statusz == "függő" ||
                             r.Statusz == "sütés alatt"))
                        .ToList();

                    if (staleOrders.Count > 0)
                    {
                        foreach (var order in staleOrders)
                        {
                            order.Statusz = "kész";
                            BakeTimeStore.BakeEndTimes.Remove(order.RendelesId);
                        }

                        _ctx.SaveChanges();
                    }

                    var orders = _ctx.Rendeles
                        .AsNoTracking()
                        .OrderByDescending(r => r.Datum)
                        .ToList()
                        .Select(r =>
                        {
                            var rendeloAzonosito = _ctx.Lead
                                .AsNoTracking()
                                .Where(l => l.RendelesiId == r.RendelesId)
                                .Select(l => (int?)l.RendeloAzonosito)
                                .FirstOrDefault();

                            var rendelo = rendeloAzonosito.HasValue
                                ? _ctx.Vasarlo
                                    .AsNoTracking()
                                    .Where(v => v.RendeloAzonosito == rendeloAzonosito.Value)
                                    .Select(v => new { v.Nev, v.Telefonszam })
                                    .FirstOrDefault()
                                : null;

                            var cimId = rendeloAzonosito.HasValue
                                ? _ctx.Keri
                                    .AsNoTracking()
                                    .Where(k => k.RendeloAzonosito == rendeloAzonosito.Value)
                                    .Select(k => (int?)k.CimId)
                                    .FirstOrDefault()
                                : null;

                            var cim = cimId.HasValue
                                ? _ctx.Kiszallitasi_Hely
                                    .AsNoTracking()
                                    .Where(k => k.CimId == cimId.Value)
                                    .Select(k => (k.Varos ?? "") + ", " + (k.Utca ?? "") + " " + (k.Hazszam ?? ""))
                                    .FirstOrDefault()
                                : null;

                            var tetelek = _ctx.Tartalmaz
                                .AsNoTracking()
                                .Where(t => t.RendelesId == r.RendelesId)
                                .Select(t => new TetelDTO
                                {
                                    PizzaNev = t.Pizza != null ? t.Pizza.Nev : null,
                                    Mennyiseg = r.Mennyiseg,
                                    Fizetes = r.FizetesMod,
                                    Feltetek = _ctx.Pizza_Feltetek
                                        .AsNoTracking()
                                        .Where(pf => pf.PizzaId == t.PizzaId)
                                        .Select(pf => new FeltetDto
                                        {
                                            Nev = pf.feltet != null ? pf.feltet.Nev : null,
                                            Ar = pf.feltet != null ? pf.feltet.Feltetar : 0
                                        })
                                        .ToList(),
                                    TetelOsszeg =
                                        (t.Pizza != null ? t.Pizza.pizzaar : 0) +
                                        (_ctx.Pizza_Feltetek
                                            .AsNoTracking()
                                            .Where(pf => pf.PizzaId == t.PizzaId)
                                            .Select(pf => (int?)(pf.feltet != null ? pf.feltet.Feltetar : 0))
                                            .Sum() ?? 0)
                                })
                                .ToList();

                            return new RendelesDto
                            {
                                Id = r.RendelesId,
                                Datum = r.Datum.ToString("yyyy-MM-dd HH:mm"),
                                RendeloNev = string.IsNullOrWhiteSpace(rendelo?.Nev) ? "Ismeretlen rendelő" : rendelo.Nev,
                                Telefonszam = string.IsNullOrWhiteSpace(rendelo?.Telefonszam) ? "Nincs telefonszám" : rendelo.Telefonszam,
                                FizetesiMod = r.FizetesMod,
                                Statusz = r.Statusz,
                                Cim = string.IsNullOrWhiteSpace(cim) ? "Nincs megadva" : cim.Trim(),
                                SutesiIdo = BakeTimeStore.GetRemainingSeconds(r.RendelesId),
                                Tetelek = tetelek,
                                Osszeg = tetelek.Sum(t => (t.TetelOsszeg ?? 0) * (t.Mennyiseg ?? 0))
                            };
                        })
                        .ToList();

                    return Ok(orders);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Hiba a rendelések lekérésekor.", detail = ex.Message });
                }
            }

            // ============================
            //  STÁTUSZ MÓDOSÍTÁSA
            // ============================
            [HttpPut("{id}/status")]
                public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateDto body)
                {
                    if (body == null || string.IsNullOrWhiteSpace(body.Statusz))
                        return BadRequest("A státusz megadása kötelező.");

                    var order = _ctx.Rendeles.FirstOrDefault(r => r.RendelesId == id);
                    if (order == null) return NotFound();

                    order.Statusz = body.Statusz;

                    if (!string.Equals(body.Statusz, "sütés alatt", StringComparison.OrdinalIgnoreCase))
                    {
                        BakeTimeStore.BakeEndTimes.Remove(id);
                    }

                    _ctx.SaveChanges();

                    return Ok();
                }

                // ============================
                //  SÜTÉSI IDŐ BEÁLLÍTÁSA
                // ============================
                [HttpPut("{id}/sutesiido")]
                public IActionResult SetBakeTime(int id, [FromBody] BakeTimeDto body)
                {
                    if (body == null || body.SutesiIdo < 0)
                        return BadRequest("A sütési idő nem lehet negatív.");

                    if (!_ctx.Rendeles.Any(r => r.RendelesId == id))
                        return NotFound();

                    BakeTimeStore.BakeEndTimes[id] = DateTime.UtcNow.AddMinutes(body.SutesiIdo);

                    return Ok();
                }
            }
    }
