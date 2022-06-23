using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using WX.DAO.Model;

namespace WX.DAO;

internal class GameContext:DbContext
{
    public DbSet<PlayerAccount> playerAccount { get;set;} = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connStr = "server=localhost; port=3306; user=root; password=Zgc373903632; database=game";
        optionsBuilder.UseMySql(connStr,ServerVersion.AutoDetect(connStr));
        ServerVersion.Create(new Version(), ServerType.MySql);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerAccount>().Property(p => p.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<PlayerAccount>().Property(p => p.UserName).IsRequired().HasMaxLength(20);
        modelBuilder.Entity<PlayerAccount>().Property(p => p.PassWord).HasMaxLength(20);
        modelBuilder.Entity<PlayerAccount>().Property(p => p.PlayName).IsRequired().HasMaxLength(20);
    }
}