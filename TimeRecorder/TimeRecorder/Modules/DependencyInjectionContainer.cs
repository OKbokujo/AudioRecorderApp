using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TimeRecorder.Services;
using TimeRecorder.ViewModels;

namespace TimeRecorder.Modules
{
    public static class DependencyInjectionContainer
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<MainPageViewModel>();
            services.AddTransient<SettingsPageViewModel>();
            services.AddTransient<NotesViewModel>();
            services.AddTransient<ItemMenuViewModel>();

            services.AddSingleton<LocalCache>();
            services.AddSingleton<ManageData>();

            return services;
        }
    }
}
