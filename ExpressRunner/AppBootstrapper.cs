using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ExpressRunner
{
    public class AppBootstrapper : Bootstrapper<ShellViewModel>
    {
        private readonly CompositionContainer container;

        public AppBootstrapper()
        {
            var appCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var pluginsCatalog = new AssemblyCatalog(typeof(XunitPlugin.XunitFramework).Assembly);
            var catalog = new AggregateCatalog(appCatalog, pluginsCatalog);

            this.container = new CompositionContainer(catalog);
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            var contractName = AttributedModelServices.GetContractName(service);
            return container.GetExportedValues<object>(contractName);
        }

        protected override object GetInstance(Type service, string key)
        {
            var contractName = string.IsNullOrEmpty(key)
                ? AttributedModelServices.GetContractName(service)
                : key;

            return container.GetExportedValue<object>(contractName);
        }
    }
}
