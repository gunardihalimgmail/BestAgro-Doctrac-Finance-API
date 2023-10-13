using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.Aggregate.ApprovalSPD;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure
{
    public partial class FinanceScanContext : DbContext
    {
        public FinanceScanContext()
        {

        }

        public FinanceScanContext(DbContextOptions<FinanceScanContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public virtual DbSet<Fn_BKU_Count> Fn_BKU_Count { get; set; }
        public virtual DbSet<Fn_SPD_Count> Fn_SPD_Count { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
