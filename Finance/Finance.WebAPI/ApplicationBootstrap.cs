using BestAgroCore.Common.Domain;
using BestAgroCore.CrossCutting;
using BestAgroCore.CrossCutting.Infrastructure;
using Finance.Domain.Aggregate.ApprovalBkuBtu;
using Finance.Domain.Aggregate.ApprovalSPD;
using Finance.Domain.Aggregate.VoucherApproval;
using Finance.Infrastructure;
using Finance.Infrastructure.Repositories;
using Finance.Infrastructure.Repositories.ApprovalBKU;
using Finance.Infrastructure.Repositories.ApprovalSPD;
using Finance.Infrastructure.Repositories.VoucherApproval;
using Finance.WebAPI.Application.Commands;
using Finance.WebAPI.Application.Commands.ApprovalBKU;
using Finance.WebAPI.Application.Commands.ApprovalSPD;
using Finance.WebAPI.Application.Commands.VoucherApproval;
using Finance.WebAPI.Application.Queries;
using Finance.WebAPI.Application.Queries.ApprovalBKU;
using Finance.WebAPI.Application.Queries.ApprovalSPD;
using Finance.WebAPI.Application.Queries.ListHistoryApprovalSPD;
using Finance.WebAPI.Application.Queries.VoucherApproval;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.WebAPI
{
    public static class ApplicationBootstrap
    {
        public static IServiceCollection InitBootstraper(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitBestAgroBootstrap_v2()
                .RegisterEf()
                .AddDbContext<FinanceContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("JVE")))
                .AddDbContext<FinanceScanContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("SCAN_JVE")));

            return services;
        }

        public static IServiceCollection InitAppServices(this IServiceCollection services)
        {
            #region Command
            // add scope for command handler
            services.AddScoped<IDt_ApprovalBkuBtuRepository, Dt_ApprovalBkuBtuRepository>();
            services.AddScoped<IFn_BKU_CountRepository, Fn_BKU_CountRepository>();
            services.AddScoped<IDt_DocStatusRepository, Dt_DocStatusRepository>();
            services.AddScoped<IDt_DocDeliveryStatusRepository, Dt_DocDeliveryStatusRepository>();
            services.AddScoped<IDt_DocProcessStatusRepository, Dt_DocProcessStatusRepository>();
            services.AddScoped<IDt_DocFlowSettingRepository, Dt_DocFlowSettingRepository>();
            services.AddScoped<IDt_NotesRepository, Dt_NotesRepository>();
            services.AddScoped<IFn_SPD_CountRepository, Fn_SPD_CountRepository>();
            services.AddScoped<IFn_SPDRepository, Fn_SPDRepository>();

            services.AddScoped<IFn_RealisasiVoucherRepository, Fn_RealisasiVoucherRepository>();
            services.AddScoped<IFn_RealisasiVoucher_DetailRepository, Fn_RealisasiVoucher_DetailRepository>();
            services.AddScoped<IFn_RealisasiVoucher_ApprovalRepository, Fn_RealisasiVoucher_ApprovalRepository>();

            #endregion

            #region Query
            // add scope for query
            services.AddScoped<IApprovalBKUQueries, ApprovalBKUQueries>();
            services.AddScoped<IApprovalSPDQueries, ApprovalSPDQueries>();

            services.AddScoped<IVoucherApprovalQueries, VoucherApprovalQueries>();

            services.AddScoped<IListHistoryApprovalSPDQueries, ListHistoryApprovalSPDQueries>();
            #endregion

            return services;
        }

        public static IServiceCollection InitEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandler<CreateUpdateApprovalBKUCommand>, CreateUpdateApprovalBKUCommandHandler>();
            services.AddTransient<ICommandHandler<CreateKirimSPDCommand>, CreateKirimSPDCommandHandler>();
            services.AddTransient<ICommandHandler<CreateTerimaSPDCommand>, CreateTerimaSPDCommandHandler>();
            services.AddTransient<ICommandHandler<CreateTolakSPDCommand>, CreateTolakSPDCommandHandler>();

            //services.AddTransient<ICommandHandler<CreateKirimSPDESTCommand>, CreateKirimSPDESTCommandHandler>();
            //services.AddTransient<ICommandHandler<CreateKirimSPDHOCommand>, CreateKirimSPDHOCommandHandler>();
            //services.AddTransient<ICommandHandler<CreateCommentCommand>, CreateCommentCommandHandler>();
            //services.AddTransient<ICommandHandler<UpdateDitujukanCommand>, UpdateDitujukanCommandHandler>();

            services.AddTransient<ICommandHandler<CreateVoucherApprovalCommand>, CreateVoucherApprovalCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateVoucherApprovalCommand>, UpdateVoucherApprovalCommandHandler>();


            return services;
        }

    }
}
