
using Microsoft.EntityFrameworkCore;
using ZenCash.Api.Models;

namespace ZenCash.Api.Data;

public class ZenCashContext : DbContext
{
    public ZenCashContext(
        DbContextOptions<ZenCashContext> options
    ) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Transacao> Transacoes => Set<Transacao>();

    public DbSet<Meta> Metas => Set<Meta>();

    public DbSet<Conta> Contas => Set<Conta>();
}
