using KeeltekoolV2.ApplicationServices;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using KeeltekoolV2.UnitTesting.Macros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

using KeeltekoolV2.ApplicationServices.Servises;
namespace KeeltekoolV2.UnitTesting
{
    public abstract class TestBase : IDisposable
    {
        protected IServiceProvider serviceProvider { get; set; }

        protected TestBase()
        {
            var services = new ServiceCollection();
            SetupServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        public virtual void SetupServices(IServiceCollection services)
        {
            services.AddScoped<ILanguageServices, LanguageCoursesServices>();
            services.AddScoped<ILanguageServices, LanguageCoursesServices>();
            services.AddScoped<ILecturersServices, LecturersServices>();

            services.AddDbContext<KeeltekoolV2Context>(x =>
            {
                x.UseInMemoryDatabase("TEST");
                x.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            RegisterMacros(services);
        }

        private void RegisterMacros(IServiceCollection services)
        {
            var macroBaseType = typeof(Imacros);
            var macros = macroBaseType.Assembly.GetTypes()
                .Where(t => macroBaseType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var macro in macros)
                services.AddSingleton(macro);
        }

        protected T Svc<T>() where T : notnull => serviceProvider.GetRequiredService<T>();

        public void Dispose() => (serviceProvider as IDisposable)?.Dispose();
    }
}