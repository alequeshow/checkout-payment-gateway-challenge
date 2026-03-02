using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Models;

namespace PaymentGateway.Application.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();
}
