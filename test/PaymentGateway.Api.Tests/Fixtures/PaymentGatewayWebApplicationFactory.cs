using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PaymentGateway.Application.Data;
using PaymentGateway.Application.Interfaces;

namespace PaymentGateway.Api.Tests.Fixtures;

public class PaymentGatewayWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    public Mock<IPaymentProcessor> PaymentProcessorMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (dbDescriptor is not null)
                services.Remove(dbDescriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            var processorDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IPaymentProcessor));

            if (processorDescriptor is not null)
                services.Remove(processorDescriptor);

            services.AddScoped(_ => PaymentProcessorMock.Object);
        });
    }

    public IServiceScope CreateScope() => Services.CreateScope();
}
