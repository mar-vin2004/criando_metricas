using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adicionar serviços ao contêiner.
        builder.Services.AddSingleton<HatCoMetrics>();

        var app = builder.Build();

        // Configurar o pipeline de solicitações HTTP.
        app.MapGet("/", () => "Hello World!");

        app.MapPost("/complete-sale", ([FromBody] SaleModel model, HatCoMetrics metrics) =>
        {
            // ... Lógica de negócios para salvar a venda em um banco de dados ...

            metrics.HatsSold(model.QuantitySold);

            return Results.Ok();
        });

        app.Run();
    }
}

public class SaleModel
{
    public int QuantitySold { get; set; }
}

public class HatCoMetrics
{
    private readonly Counter<int> _hatsSold;

    public HatCoMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("HatCo.Store");
        _hatsSold = meter.CreateCounter<int>("hatco.store.hats_sold");
    }

    public void HatsSold(int quantity)
    {
        _hatsSold.Add(quantity);
    }
}