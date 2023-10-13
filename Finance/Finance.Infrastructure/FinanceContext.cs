using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.Aggregate.ApprovalSPD;
using Finance.Domain.Aggregate.VoucherApproval;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.Infrastructure
{
    public partial class FinanceContext : DbContext
    {
        public FinanceContext()
        {

        }

        public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public virtual DbSet<Dt_ApprovalBkuBtu> Dt_ApprovalBkuBtu { get; set; }
        public virtual DbSet<Fn_BKU_Count> Fn_BKU_Count { get; set; }
        public virtual DbSet<Dt_DocStatus> Dt_DocStatus { get; set; }
        public virtual DbSet<Dt_DocDeliveryStatus> Dt_DocDeliveryStatus { get; set; }
        public virtual DbSet<Dt_DocProcessStatus> Dt_DocProcessStatus { get; set; }
        public virtual DbSet<Dt_DocFlowSetting> Dt_DocFlowSetting { get; set; }
        public virtual DbSet<Dt_Notes> Dt_Notes { get; set; }
        public virtual DbSet<Fn_SPD> Fn_SPD { get; set; }


        // Db Set untuk Transaksi CRUD
        public virtual DbSet<Fn_RealisasiVoucher> Fn_RealisasiVoucher { get; set; }
        public virtual DbSet<Fn_RealisasiVoucher_Approval> Fn_RealisasiVoucher_Approval { get; set; }
        public virtual DbSet<Fn_RealisasiVoucher_Detail> Fn_RealisasiVoucher_Detail { get; set; }
        public virtual DbSet<Ms_Keuangan_RptRealVoucher> Ms_Keuangan_RptRealVoucher { get; set; }


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
