
using Microsoft.EntityFrameworkCore;
using pizzaprojekt.Database;

namespace pizzaprojekt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options => // Elinditom a CORS-t
            {
                options.AddDefaultPolicy( //Beallitom az alap�rtelmezett szab�lyzatot a k�vetkez�k�ppen
                    policy => // Defini�lom a szab�lyzatot ny�l f�ggv�nnyel
                    {
                        policy.WithOrigins("*"); //Enged�lyezem a k�r�seket.

                    });
            });
            builder.Services.AddControllers(); //Hozz�adom a vez�rl�ket a szolg�ltat�sokhoz a app konfigur�ci�j�ban.
            builder.Services.AddDbContext<PizzaContext>(opt => opt.UseMySQL("server=localhost;port=3306;database=pizzaprojekt;user=root;")); //Hozz�adom az adatb�zis kontextust a szolg�ltat�sokhoz, �s konfigur�lom a MySQL adatb�zis kapcsolatot.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer(); //megcsin�lom az API v�gpontok felt�rk�pez�s�t
            builder.Services.AddSwaggerGen(c => c.CustomSchemaIds(type => (type.FullName ?? type.Name).Replace("+", "."))); //Hozz�adom a Swagger gener�tort és biztosítom az egyedi schemaID-ket.
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build(); //Alkalmaz�s �p�t�se a konfigur�lt szolg�ltat�sokkal.

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) // ellen�rz�m, hogy fejleszt�i k�rnyezetben fut-e az alkalmaz�s
            {
                app.UseSwagger(); //Ez enged�lyezi a Swagger middleware-t, amely automatikusan gener�lja az API dokument�ci�t a k�dod alapj�n (endpointok, modellek, attrib�tumok)
                app.UseSwaggerUI(); //Ez bekapcsolja a Swagger felhaszn�l�i fel�letet (UI), ahol b�ng�sz�ben interakt�van kipr�b�lhatod az API v�gpontokat.
            }

            app.UseHttpsRedirection(); //�tir�ny�tom a HTTP k�r�seket HTTPS-re a biztons�g �rdek�ben.

            app.UseCors();

            app.UseAuthorization(); //Enged�lyezem a hiteles�t�st �s jogosults�gkezel�st az alkalmaz�sban.

            app.MapControllers(); //Lek�pezem a vez�rl�ket az alkalmaz�s �tvonalaira, hogy kezelj�k a bej�v� HTTP k�r�seket.

            app.Run(); //Elind�tom az alkalmaz�st �s elkezdem hallgatni a bej�v� HTTP k�r�seket.
        }
    }
}
