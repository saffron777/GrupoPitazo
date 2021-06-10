using GP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Repository
{
    public class AppContext:DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {

        }

        public virtual DbSet<Transacciones> Transactiones { get; set; }
        public virtual DbSet<Banquedas> Banquedas { get; set; }
        public virtual DbSet<Carreras> Carreras { get; set; }
        public virtual DbSet<Hipodromos> Hipodromos { get; set; }
        public virtual DbSet<Jugadas> Jugadas { get; set; }
        public virtual DbSet<Notificaciones> Notificaciones { get; set; }
        public virtual DbSet<TipoJugadas> TipoJugadas { get; set; }
        public virtual DbSet<Tokens> Tokens { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Aceptaciones> Aceptaciones { get; set; }
        public virtual DbSet<Gradeo> Gradeo { get; set; }
        public virtual DbSet<Parameters> Parameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hipodromos>()
                .HasMany(e => e.Carreras)
                .WithOne(e => e.Hipodromos)
                .HasForeignKey(p => p.HipodromoID);

            modelBuilder.Entity<Carreras>()
               .HasMany(e => e.Jugadas)
               .WithOne(e => e.Carreras)
               .HasForeignKey(p => p.CarreraID);

            modelBuilder.Entity<Jugadas>()
                .Property(p => p.JugadaID)
                .UseIdentityColumn(seed:1,increment:1);

            modelBuilder.Entity<Banquedas>()
               .Property(p => p.BanquedaID)
               .UseIdentityColumn(seed: 1, increment: 1);

            modelBuilder.Entity<Banquedas>()
               .HasMany(e => e.Aceptaciones)
               .WithOne(e => e.Banquedas)
               .HasForeignKey(p => p.BanqueadaID);

            modelBuilder.Entity<Jugadas>()
                .HasMany(e => e.Banquedas)
                .WithOne(e => e.Jugadas)
                .HasForeignKey(p => p.JugadaID);

            modelBuilder.Entity<Jugadas>()
                .HasMany(e => e.Aceptaciones)
                .WithOne(e => e.Jugadas)
                .HasForeignKey(p => p.JugadaID);

            modelBuilder.Entity<TipoJugadas>()
                .HasMany(e => e.Banquedas)
                .WithOne(e => e.TipoJugadas)
                .HasForeignKey(p => p.TipoJugadaID);

            modelBuilder.Entity<TipoJugadas>()
                .HasMany(e => e.Jugadas)
                .WithOne(e => e.TipoJugadas)
                .HasForeignKey(p => p.TipoJugadaID);

            
            
        }
    }
}
