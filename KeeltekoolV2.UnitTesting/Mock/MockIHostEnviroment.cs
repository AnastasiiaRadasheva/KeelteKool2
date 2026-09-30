using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.UnitTesting.M
{
    internal class MockIHostEnviroment : IHostEnvironment
    {
        string IHostEnvironment.ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IFileProvider IHostEnvironment.ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IHostEnvironment.ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IHostEnvironment.EnvironmentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
